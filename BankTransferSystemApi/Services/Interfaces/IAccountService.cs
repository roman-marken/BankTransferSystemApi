using BankTransferSystemApi.Domain.Models;
using BankTransferSystemApi.DTOs;

namespace BankTransferSystemApi.Services.Interfaces;

/// <summary>
/// Бізнес-сервіс роботи з рахунками (контролери не звертаються до репозиторіїв напряму).
/// </summary>
public interface IAccountService
{
    Task<IReadOnlyList<BankAccount>> GetAllAsync();

    Task<BankAccount?> GetByIdAsync(int id);

    Task<BankAccount> CreateAsync(CreateAccountRequest request);
}
