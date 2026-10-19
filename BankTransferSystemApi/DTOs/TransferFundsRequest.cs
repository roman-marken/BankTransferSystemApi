using System.ComponentModel.DataAnnotations;

namespace BankTransferSystemApi.DTOs;

/// <summary>
/// Контракт запиту на переказ коштів між рахунками.
/// </summary>
public class TransferFundsRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Ідентифікатор рахунку відправника має бути додатним числом.")]
    public int FromAccountId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Ідентифікатор рахунку отримувача має бути додатним числом.")]
    public int ToAccountId { get; set; }

    [Range(0.01, 100_000_000, ErrorMessage = "Сума переказу має бути більшою за нуль.")]
    public decimal Amount { get; set; }
}
