using BankTransferSystemApi.Domain.Enums;
using BankTransferSystemApi.Services.Interfaces;

namespace BankTransferSystemApi.Services.Implementations;

/// <summary>
/// Багаторівневий калькулятор комісій:
/// Standard — 1.5% (мінімум 5 грн); Premium — 0.5% (без мінімуму); Business — фіксовано 10 грн.
/// </summary>
public class TieredFeeCalculator : IFeeCalculator
{
    private const decimal StandardRate = 0.015m;
    private const decimal StandardMinimumFee = 5m;
    private const decimal PremiumRate = 0.005m;
    private const decimal BusinessFixedFee = 10m;

    public decimal CalculateFee(decimal amount, AccountType senderAccountType)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Сума переказу має бути більшою за нуль.");
        }

        decimal fee = senderAccountType switch
        {
            AccountType.Standard => Math.Max(amount * StandardRate, StandardMinimumFee),
            AccountType.Premium => amount * PremiumRate,
            AccountType.Business => BusinessFixedFee,
            _ => throw new ArgumentOutOfRangeException(nameof(senderAccountType), senderAccountType, "Невідомий тип рахунку.")
        };

        return Math.Round(fee, 2, MidpointRounding.AwayFromZero);
    }
}
