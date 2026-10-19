using System.ComponentModel.DataAnnotations;
using BankTransferSystemApi.Domain.Enums;

namespace BankTransferSystemApi.DTOs;

/// <summary>
/// Контракт запиту на відкриття нового рахунку.
/// </summary>
public class CreateAccountRequest
{
    [Required(ErrorMessage = "Ім'я власника рахунку є обов'язковим.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Ім'я власника має містити від 2 до 100 символів.")]
    public string OwnerName { get; set; } = string.Empty;

    [Range(0, 100_000_000, ErrorMessage = "Початковий баланс не може бути від'ємним.")]
    public decimal InitialBalance { get; set; }

    [EnumDataType(typeof(AccountType), ErrorMessage = "Вказано невідомий тип рахунку.")]
    public AccountType AccountType { get; set; } = AccountType.Standard;
}
