using BankTransferSystemApi.Domain.Models;

namespace BankTransferSystemApi.DTOs;

/// <summary>
/// Контракт відповіді для транзакції переказу.
/// </summary>
public class TransactionResponse
{
    public int Id { get; set; }

    public string TransactionNumber { get; set; } = string.Empty;

    public int FromAccountId { get; set; }

    public int ToAccountId { get; set; }

    public decimal Amount { get; set; }

    public decimal FeeAmount { get; set; }

    public decimal TotalDebited { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }

    public string? FailureReason { get; set; }

    public static TransactionResponse FromDomain(Transaction transaction) => new()
    {
        Id = transaction.Id,
        TransactionNumber = transaction.TransactionNumber,
        FromAccountId = transaction.FromAccountId,
        ToAccountId = transaction.ToAccountId,
        Amount = transaction.Amount,
        FeeAmount = transaction.FeeAmount,
        TotalDebited = transaction.TotalDebited,
        Status = transaction.Status.ToString(),
        CreatedAtUtc = transaction.CreatedAtUtc,
        FailureReason = transaction.FailureReason
    };
}
