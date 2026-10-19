using BankTransferSystemApi.Domain.Models;

namespace BankTransferSystemApi.Services.Interfaces;

/// <summary>
/// Аудит кожної транзакції. Реалізації (Console / File) взаємозамінні
/// і обираються конфігурацією в Program.cs.
/// </summary>
public interface IAuditLogger
{
    Task LogTransactionAsync(Transaction transaction);
}
