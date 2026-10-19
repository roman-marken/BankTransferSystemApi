namespace BankTransferSystemApi.Domain.Enums;

/// <summary>
/// Життєвий цикл транзакції переказу.
/// </summary>
public enum TransactionStatus
{
    Pending = 1,
    Completed = 2,
    Failed = 3,
    Rejected = 4
}
