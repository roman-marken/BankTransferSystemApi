namespace BankTransferSystemApi.Domain.Enums;

/// <summary>
/// Тип банківського рахунку. Визначає тариф комісії за переказ
/// (див. <c>TieredFeeCalculator</c>).
/// </summary>
public enum AccountType
{
    Standard = 1,
    Premium = 2,
    Business = 3
}
