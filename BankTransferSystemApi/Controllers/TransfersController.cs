using BankTransferSystemApi.DTOs;
using BankTransferSystemApi.Services;
using BankTransferSystemApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankTransferSystemApi.Controllers;

/// <summary>
/// Presentation Layer: перекази. HTTP статус-коди мапляться з типізованого
/// TransferResult, який повертає бізнес-сервіс.
/// </summary>
[ApiController]
[Route("api/transfers")]
public class TransfersController : ControllerBase
{
    private readonly ITransferService _transferService;

    public TransfersController(ITransferService transferService)
    {
        _transferService = transferService;
    }

    /// <summary>POST /api/transfers — виконати переказ коштів.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ExecuteTransfer([FromBody] TransferFundsRequest request)
    {
        var result = await _transferService.ExecuteTransferAsync(request);

        if (result.Success)
        {
            return CreatedAtAction(
                nameof(GetTransactionById),
                new { id = result.Transaction!.Id },
                TransactionResponse.FromDomain(result.Transaction));
        }

        return result.Error switch
        {
            TransferError.AccountNotFound => NotFound(new { error = result.ErrorMessage }),
            TransferError.InsufficientFunds => UnprocessableEntity(new { error = result.ErrorMessage }),
            _ => BadRequest(new { error = result.ErrorMessage })
        };
    }

    /// <summary>GET /api/transfers/{id} — транзакція за ідентифікатором.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTransactionById(int id)
    {
        var transaction = await _transferService.GetTransactionByIdAsync(id);
        return transaction is null
            ? NotFound(new { error = $"Транзакцію з Id={id} не знайдено." })
            : Ok(TransactionResponse.FromDomain(transaction));
    }

    /// <summary>GET /api/transfers/account/{accountId} — історія переказів рахунку.</summary>
    [HttpGet("account/{accountId:int}")]
    [ProducesResponseType(typeof(IReadOnlyList<TransactionResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAccountHistory(int accountId)
    {
        var history = await _transferService.GetAccountHistoryAsync(accountId);
        return Ok(history.Select(TransactionResponse.FromDomain).ToList());
    }
}
