using BankTransferSystemApi.Domain.Models;

namespace BankTransferSystemApi.Infrastructure.Repositories;

/// <summary>
/// Потокобезпечна In-Memory реалізація журналу транзакцій (Singleton).
/// </summary>
public class InMemoryTransactionRepository : ITransactionRepository
{
    private readonly List<Transaction> _transactions = new();
    private readonly object _sync = new();
    private int _nextId = 1;

    public Task<IReadOnlyList<Transaction>> GetAllAsync()
    {
        lock (_sync)
        {
            IReadOnlyList<Transaction> snapshot = _transactions
                .OrderBy(transaction => transaction.Id)
                .ToList();

            return Task.FromResult(snapshot);
        }
    }

    public Task<Transaction?> GetByIdAsync(int id)
    {
        lock (_sync)
        {
            return Task.FromResult(_transactions.FirstOrDefault(transaction => transaction.Id == id));
        }
    }

    public Task<IReadOnlyList<Transaction>> GetByAccountIdAsync(int accountId)
    {
        lock (_sync)
        {
            IReadOnlyList<Transaction> history = _transactions
                .Where(transaction => transaction.FromAccountId == accountId || transaction.ToAccountId == accountId)
                .OrderByDescending(transaction => transaction.CreatedAtUtc)
                .ToList();

            return Task.FromResult(history);
        }
    }

    public Task AddAsync(Transaction transaction)
    {
        lock (_sync)
        {
            transaction.Id = _nextId++;
            transaction.TransactionNumber = $"TX-{DateTime.UtcNow:yyyyMMdd}-{transaction.Id:D6}";
            _transactions.Add(transaction);
        }

        return Task.CompletedTask;
    }
}
