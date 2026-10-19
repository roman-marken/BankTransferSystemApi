using BankTransferSystemApi.DTOs;
using BankTransferSystemApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankTransferSystemApi.Controllers;

/// <summary>
/// Presentation Layer: робота з рахунками. Контролер знає лише про IAccountService
/// і мапить доменні моделі у DTO — жодного доступу до збереження даних.
/// </summary>
[ApiController]
[Route("api/accounts")]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    /// <summary>GET /api/accounts — список усіх рахунків.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AccountResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AccountResponse>>> GetAll()
    {
        var accounts = await _accountService.GetAllAsync();
        return Ok(accounts.Select(AccountResponse.FromDomain).ToList());
    }

    /// <summary>GET /api/accounts/{id} — рахунок за ідентифікатором.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AccountResponse>> GetById(int id)
    {
        var account = await _accountService.GetByIdAsync(id);
        if (account is null)
        {
            return NotFound(new { error = $"Рахунок з Id={id} не знайдено." });
        }

        return Ok(AccountResponse.FromDomain(account));
    }

    /// <summary>POST /api/accounts — відкриття нового рахунку.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AccountResponse>> Create([FromBody] CreateAccountRequest request)
    {
        var account = await _accountService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = account.Id }, AccountResponse.FromDomain(account));
    }
}
