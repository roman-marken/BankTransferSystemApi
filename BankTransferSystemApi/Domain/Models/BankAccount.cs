using BankTransferSystemApi.Domain.Enums;

namespace BankTransferSystemApi.Domain.Models;

/// <summary>
/// Банківський рахунок клієнта (чиста доменна сутність без залежностей від інших шарів).
/// </summary>
public class BankAccount
{
    public int Id { get; set; }

    public string AccountNumber { get; set; } = string.Empty;

    public string OwnerName { get; set; } = string.Empty;

    public decimal Balance { get; set; }

    public AccountType AccountType { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}
