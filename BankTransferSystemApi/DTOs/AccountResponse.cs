using BankTransferSystemApi.Domain.Models;

namespace BankTransferSystemApi.DTOs;

/// <summary>
/// Контракт відповіді для банківського рахунку.
/// </summary>
public class AccountResponse
{
    public int Id { get; set; }

    public string AccountNumber { get; set; } = string.Empty;

    public string OwnerName { get; set; } = string.Empty;

    public decimal Balance { get; set; }

    public string AccountType { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }

    public static AccountResponse FromDomain(BankAccount account) => new()
    {
        Id = account.Id,
        AccountNumber = account.AccountNumber,
        OwnerName = account.OwnerName,
        Balance = account.Balance,
        AccountType = account.AccountType.ToString(),
        CreatedAtUtc = account.CreatedAtUtc
    };
}
