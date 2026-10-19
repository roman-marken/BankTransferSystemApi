using System.Text;
using BankTransferSystemApi.Domain.Models;
using BankTransferSystemApi.Services.Interfaces;

namespace BankTransferSystemApi.Services.Implementations;

/// <summary>
/// Аудит транзакцій у локальний файл audit.log у корені проєкту.
/// Записи серіалізуються семафором, щоб паралельні запити не «перемішали» рядки.
/// </summary>
public class FileAuditLogger : IAuditLogger
{
    private readonly string _filePath;
    private readonly SemaphoreSlim _fileLock = new(1, 1);

    public FileAuditLogger(IHostEnvironment environment)
    {
        _filePath = Path.Combine(environment.ContentRootPath, "audit.log");
    }

    public async Task LogTransactionAsync(Transaction transaction)
    {
        var line =
            $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} UTC] {transaction.TransactionNumber} | {transaction.Status} | " +
            $"{transaction.Amount:0.00} грн (комісія {transaction.FeeAmount:0.00}, списано {transaction.TotalDebited:0.00}) | " +
            $"рахунок {transaction.FromAccountId} → {transaction.ToAccountId}" +
            (transaction.FailureReason is null ? string.Empty : $" | Причина: {transaction.FailureReason}") +
            Environment.NewLine;

        await _fileLock.WaitAsync();
        try
        {
            await File.AppendAllTextAsync(_filePath, line, Encoding.UTF8);
        }
        finally
        {
            _fileLock.Release();
        }
    }
}
