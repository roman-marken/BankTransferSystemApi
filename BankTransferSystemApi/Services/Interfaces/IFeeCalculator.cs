using BankTransferSystemApi.Domain.Enums;

namespace BankTransferSystemApi.Services.Interfaces;

/// <summary>
/// Стратегія розрахунку комісії за переказ. Реалізації мають бути чистими (без стану).
/// </summary>
public interface IFeeCalculator
{
    /// <summary>Обчислює комісію залежно від суми та типу рахунку відправника.</summary>
    decimal CalculateFee(decimal amount, AccountType senderAccountType);
}
