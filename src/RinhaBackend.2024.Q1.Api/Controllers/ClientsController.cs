using Microsoft.AspNetCore.Mvc;
using RinhaBackend._2024.Q1.Core.Models.Requests;
using RinhaBackend._2024.Q1.Core.Models.Responses;
using RinhaBackend._2024.Q1.Core.Services;

namespace RinhaBackend._2024.Q1.Api.Controllers;

[ApiController]
[Produces("application/json")]
public class ClientsController : Controller
{
    private readonly IBankService _bankService;

    public ClientsController(IBankService bankService)
    {
        _bankService = bankService;
    }
    
    [HttpGet]
    [Route("clientes/{id}/extrato")]
    public async Task<IActionResult> GetExtract([FromRoute] int id)
    {
        var result = await _bankService.GetClientExtract(id);
        
        return result.Match<IActionResult>(
            extract => Ok(extract),
            noClientFound => NotFound());
    }
    
    [HttpPost]
    [Route("clientes/{id}/transacoes")]
    public async Task<IActionResult> PostTransaction([FromRoute] int id, [FromBody] TransactionRequest payload)
    {
        var result = await _bankService.CreateTransaction(id, payload);
        return result.Match<IActionResult>(
            transaction => Ok(transaction),
            noClientFound => NotFound(),
            transactionOutOfLimit => UnprocessableEntity());
    }
}

