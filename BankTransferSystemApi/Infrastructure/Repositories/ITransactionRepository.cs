using BankTransferSystemApi.Domain.Models;

namespace BankTransferSystemApi.Infrastructure.Repositories;

/// <summary>
/// Абстракція доступу до журналу транзакцій.
/// </summary>
public interface ITransactionRepository
{
    Task<IReadOnlyList<Transaction>> GetAllAsync();

    Task<Transaction?> GetByIdAsync(int id);

    /// <summary>Повертає всі транзакції, у яких рахунок виступав відправником або отримувачем.</summary>
    Task<IReadOnlyList<Transaction>> GetByAccountIdAsync(int accountId);

    /// <summary>Додає транзакцію і заповнює <see cref="Transaction.Id"/> та <see cref="Transaction.TransactionNumber"/>.</summary>
    Task AddAsync(Transaction transaction);
}
