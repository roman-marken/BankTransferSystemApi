using BankTransferSystemApi.Domain.Models;

namespace BankTransferSystemApi.Infrastructure.Repositories;

/// <summary>
/// Абстракція доступу до рахунків. Сервіси залежать лише від інтерфейсу,
/// тому In-Memory реалізацію завтра можна замінити на EF Core без зміни бізнес-логіки.
/// </summary>
public interface IAccountRepository
{
    Task<IReadOnlyList<BankAccount>> GetAllAsync();

    Task<BankAccount?> GetByIdAsync(int id);

    Task<BankAccount?> GetByAccountNumberAsync(string accountNumber);

    /// <summary>Додає рахунок і заповнює <see cref="BankAccount.Id"/>, <see cref="BankAccount.AccountNumber"/> та <see cref="BankAccount.CreatedAtUtc"/>.</summary>
    Task AddAsync(BankAccount account);

    Task UpdateAsync(BankAccount account);
}
