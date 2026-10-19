using BankTransferSystemApi.Domain.Enums;
using BankTransferSystemApi.Domain.Models;

namespace BankTransferSystemApi.Infrastructure.Repositories;

/// <summary>
/// Потокобезпечна In-Memory реалізація сховища рахунків.
/// Зареєстрована в DI як Singleton, тому стан спільний для всього застосунку.
/// </summary>
public class InMemoryAccountRepository : IAccountRepository
{
    private readonly Dictionary<int, BankAccount> _accounts = new();
    private readonly object _sync = new();
    private int _nextId = 1;

    public InMemoryAccountRepository()
    {
        var seedAccounts = new[]
        {
            new BankAccount { OwnerName = "Іван Петренко", Balance = 5_000m, AccountType = AccountType.Standard },
            new BankAccount { OwnerName = "Марія Коваленко", Balance = 25_000m, AccountType = AccountType.Premium },
            new BankAccount { OwnerName = "ТОВ «Сонячні технології»", Balance = 120_000m, AccountType = AccountType.Business }
        };

        foreach (var account in seedAccounts)
        {
            AddCore(account);
        }
    }

    public Task<IReadOnlyList<BankAccount>> GetAllAsync()
    {
        lock (_sync)
        {
            // Повертаємо копії, щоб перелік не змінився під час серіалізації відповіді.
            IReadOnlyList<BankAccount> snapshot = _accounts.Values
                .OrderBy(account => account.Id)
                .Select(Clone)
                .ToList();

            return Task.FromResult(snapshot);
        }
    }

    public Task<BankAccount?> GetByIdAsync(int id)
    {
        lock (_sync)
        {
            return Task.FromResult(_accounts.TryGetValue(id, out var account) ? account : null);
        }
    }

    public Task<BankAccount?> GetByAccountNumberAsync(string accountNumber)
    {
        lock (_sync)
        {
            var account = _accounts.Values.FirstOrDefault(a => a.AccountNumber == accountNumber);
            return Task.FromResult(account);
        }
    }

    public Task AddAsync(BankAccount account)
    {
        lock (_sync)
        {
            AddCore(account);
        }

        return Task.CompletedTask;
    }

    public Task UpdateAsync(BankAccount account)
    {
        lock (_sync)
        {
            if (!_accounts.ContainsKey(account.Id))
            {
                throw new InvalidOperationException($"Рахунок з Id={account.Id} не знайдено.");
            }

            _accounts[account.Id] = account;
        }

        return Task.CompletedTask;
    }

    private void AddCore(BankAccount account)
    {
        account.Id = _nextId++;
        account.AccountNumber = $"UA{account.Id:D10}";
        account.CreatedAtUtc = DateTime.UtcNow;
        _accounts[account.Id] = account;
    }

    private static BankAccount Clone(BankAccount account) => new()
    {
        Id = account.Id,
        AccountNumber = account.AccountNumber,
        OwnerName = account.OwnerName,
        Balance = account.Balance,
        AccountType = account.AccountType,
        CreatedAtUtc = account.CreatedAtUtc
    };
}
