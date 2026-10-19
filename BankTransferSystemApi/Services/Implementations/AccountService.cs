using BankTransferSystemApi.Domain.Models;
using BankTransferSystemApi.DTOs;
using BankTransferSystemApi.Infrastructure.Repositories;
using BankTransferSystemApi.Services.Interfaces;

namespace BankTransferSystemApi.Services.Implementations;

/// <summary>
/// Бізнес-сервіс роботи з рахунками. Зареєстрований у DI як Scoped.
/// </summary>
public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ILogger<AccountService> _logger;

    public AccountService(IAccountRepository accountRepository, ILogger<AccountService> logger)
    {
        _accountRepository = accountRepository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<BankAccount>> GetAllAsync()
    {
        return await _accountRepository.GetAllAsync();
    }

    public async Task<BankAccount?> GetByIdAsync(int id)
    {
        return await _accountRepository.GetByIdAsync(id);
    }

    public async Task<BankAccount> CreateAsync(CreateAccountRequest request)
    {
        var account = new BankAccount
        {
            OwnerName = request.OwnerName.Trim(),
            Balance = request.InitialBalance,
            AccountType = request.AccountType
        };

        await _accountRepository.AddAsync(account);
        _logger.LogInformation(
            "Відкрито рахунок {AccountNumber} ({OwnerName}, {AccountType}), початковий баланс {Balance} грн",
            account.AccountNumber, account.OwnerName, account.AccountType, account.Balance);

        return account;
    }
}
