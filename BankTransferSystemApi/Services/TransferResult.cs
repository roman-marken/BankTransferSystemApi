using BankTransferSystemApi.Domain.Models;

namespace BankTransferSystemApi.Services;

/// <summary>
/// Категорія відмови виконання переказу. Контролер мапить її у HTTP статус-код.
/// </summary>
public enum TransferError
{
    None = 0,
    InvalidAmount,
    SameAccount,
    AccountNotFound,
    InsufficientFunds
}

/// <summary>
/// Результат виконання переказу: сервіс не кидає винятків для очікуваних
/// бізнес-відмов, а повертає типізований результат.
/// </summary>
public class TransferResult
{
    public bool Success { get; private init; }

    public TransferError Error { get; private init; }

    public string? ErrorMessage { get; private init; }

    /// <summary>Зафіксована транзакція (у т.ч. Failed/Rejected для відмов).</summary>
    public Transaction? Transaction { get; private init; }

    public static TransferResult Succeeded(Transaction transaction) =>
        new() { Success = true, Error = TransferError.None, Transaction = transaction };

    public static TransferResult Failed(TransferError error, string message, Transaction? transaction = null) =>
        new() { Success = false, Error = error, ErrorMessage = message, Transaction = transaction };
}
