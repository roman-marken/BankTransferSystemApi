using BankTransferSystemApi.Domain.Models;
using BankTransferSystemApi.DTOs;

namespace BankTransferSystemApi.Services.Interfaces;

/// <summary>
/// Бізнес-сервіс виконання переказів між рахунками.
/// </summary>
public interface ITransferService
{
    Task<TransferResult> ExecuteTransferAsync(TransferFundsRequest request);

    Task<Transaction?> GetTransactionByIdAsync(int id);

    Task<IReadOnlyList<Transaction>> GetAccountHistoryAsync(int accountId);
}
