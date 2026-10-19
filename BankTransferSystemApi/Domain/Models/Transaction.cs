using BankTransferSystemApi.Domain.Enums;

namespace BankTransferSystemApi.Domain.Models;

/// <summary>
/// Фінансова транзакція переказу коштів між рахунками.
/// </summary>
public class Transaction
{
    public int Id { get; set; }

    public string TransactionNumber { get; set; } = string.Empty;

    public int FromAccountId { get; set; }

    public int ToAccountId { get; set; }

    public decimal Amount { get; set; }

    public decimal FeeAmount { get; set; }

    /// <summary>Сума, фактично списана з рахунку відправника: <see cref="Amount"/> + <see cref="FeeAmount"/>.</summary>
    public decimal TotalDebited { get; set; }

    public TransactionStatus Status { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    /// <summary>Причина відмови, якщо транзакція не була виконана.</summary>
    public string? FailureReason { get; set; }
}
