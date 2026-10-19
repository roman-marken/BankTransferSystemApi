using BankTransferSystemApi.Domain.Models;
using BankTransferSystemApi.Services.Interfaces;

namespace BankTransferSystemApi.Services.Implementations;

/// <summary>
/// Аудит транзакцій у консоль з міткою часу (UTC).
/// </summary>
public class ConsoleAuditLogger : IAuditLogger
{
    public Task LogTransactionAsync(Transaction transaction)
    {
        Console.WriteLine(
            $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC | AUDIT | {transaction.TransactionNumber} | {transaction.Status,9} | " +
            $"{transaction.Amount:0.00} грн (комісія {transaction.FeeAmount:0.00}, списано {transaction.TotalDebited:0.00}) | " +
            $"рахунок {transaction.FromAccountId} → {transaction.ToAccountId}" +
            (transaction.FailureReason is null ? string.Empty : $" | Причина: {transaction.FailureReason}"));

        return Task.CompletedTask;
    }
}
