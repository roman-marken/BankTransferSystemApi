using BankTransferSystemApi.Domain.Enums;
using BankTransferSystemApi.Domain.Models;
using BankTransferSystemApi.DTOs;
using BankTransferSystemApi.Infrastructure.Repositories;
using BankTransferSystemApi.Services.Interfaces;

namespace BankTransferSystemApi.Services.Implementations;

/// <summary>
/// Бізнес-сервіс переказів: валідація бізнес-правил, атомарне зміщення балансів,
/// фіксація транзакцій та аудит. Зареєстрований у DI як Scoped.
/// </summary>
public class TransferService : ITransferService
{
    // Сервіс Scoped (новий екземпляр на запит), тому семафор має бути static:
    // саме він гарантує атомарність паралельних переказів на рівні застосунку.
    private static readonly SemaphoreSlim TransferLock = new(1, 1);

    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IFeeCalculator _feeCalculator;
    private readonly IAuditLogger _auditLogger;
    private readonly ILogger<TransferService> _logger;

    public TransferService(
        IAccountRepository accountRepository,
        ITransactionRepository transactionRepository,
        IFeeCalculator feeCalculator,
        IAuditLogger auditLogger,
        ILogger<TransferService> logger)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
        _feeCalculator = feeCalculator;
        _auditLogger = auditLogger;
        _logger = logger;
    }

    public async Task<TransferResult> ExecuteTransferAsync(TransferFundsRequest request)
    {
        if (request.Amount <= 0)
        {
            return TransferResult.Failed(TransferError.InvalidAmount, "Сума переказу має бути більшою за нуль.");
        }

        var fromAccount = await _accountRepository.GetByIdAsync(request.FromAccountId);
        var toAccount = await _accountRepository.GetByIdAsync(request.ToAccountId);

        // Правило 1: обидва рахунки мають існувати.
        if (fromAccount is null || toAccount is null)
        {
            int missingAccountId = fromAccount is null ? request.FromAccountId : request.ToAccountId;
            var transaction = BuildTransaction(request, feeAmount: 0m, TransactionStatus.Failed, "Рахунок не знайдено");
            await _transactionRepository.AddAsync(transaction);
            await _auditLogger.LogTransactionAsync(transaction);

            return TransferResult.Failed(
                TransferError.AccountNotFound,
                $"Рахунок з Id={missingAccountId} не знайдено.",
                transaction);
        }

        // Правило 2: заборона переказу самому собі.
        if (fromAccount.Id == toAccount.Id)
        {
            var transaction = BuildTransaction(request, feeAmount: 0m, TransactionStatus.Rejected, "Заборонено переказ на той самий рахунок");
            await _transactionRepository.AddAsync(transaction);
            await _auditLogger.LogTransactionAsync(transaction);

            return TransferResult.Failed(
                TransferError.SameAccount,
                "Не можна переказати кошти на той самий рахунок.",
                transaction);
        }

        // Правило 3: комісія залежить від типу рахунку відправника.
        decimal fee = _feeCalculator.CalculateFee(request.Amount, fromAccount.AccountType);
        decimal totalDebited = request.Amount + fee;

        // Правила 4–6: перевірка платоспроможності та атомарне списання/зарахування.
        await TransferLock.WaitAsync();
        try
        {
            if (totalDebited > fromAccount.Balance)
            {
                var transaction = BuildTransaction(
                    request,
                    fee,
                    TransactionStatus.Failed,
                    $"Недостатньо коштів: потрібно {totalDebited:0.##} грн, доступно {fromAccount.Balance:0.##} грн");
                await _transactionRepository.AddAsync(transaction);
                await _auditLogger.LogTransactionAsync(transaction);

                return TransferResult.Failed(
                    TransferError.InsufficientFunds,
                    $"Недостатньо коштів на рахунку: потрібно {totalDebited:0.##} грн (сума + комісія), доступно {fromAccount.Balance:0.##} грн.",
                    transaction);
            }

            fromAccount.Balance -= totalDebited;
            toAccount.Balance += request.Amount;
            await _accountRepository.UpdateAsync(fromAccount);
            await _accountRepository.UpdateAsync(toAccount);
        }
        finally
        {
            TransferLock.Release();
        }

        // Правило 7: фіксація успішної транзакції.
        var completed = BuildTransaction(request, fee, TransactionStatus.Completed, failureReason: null);
        await _transactionRepository.AddAsync(completed);
        await _auditLogger.LogTransactionAsync(completed);

        _logger.LogInformation(
            "Переказ {TransactionNumber} виконано: рахунок {FromId} → {ToId}, {Amount} грн, комісія {Fee} грн",
            completed.TransactionNumber, fromAccount.Id, toAccount.Id, request.Amount, fee);

        return TransferResult.Succeeded(completed);
    }

    public async Task<Transaction?> GetTransactionByIdAsync(int id)
    {
        return await _transactionRepository.GetByIdAsync(id);
    }

    public async Task<IReadOnlyList<Transaction>> GetAccountHistoryAsync(int accountId)
    {
        return await _transactionRepository.GetByAccountIdAsync(accountId);
    }

    private static Transaction BuildTransaction(
        TransferFundsRequest request,
        decimal feeAmount,
        TransactionStatus status,
        string? failureReason) =>
        new()
        {
            FromAccountId = request.FromAccountId,
            ToAccountId = request.ToAccountId,
            Amount = request.Amount,
            FeeAmount = feeAmount,
            TotalDebited = request.Amount + feeAmount,
            Status = status,
            CreatedAtUtc = DateTime.UtcNow,
            FailureReason = failureReason
        };
}
