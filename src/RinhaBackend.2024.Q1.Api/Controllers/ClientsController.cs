using Microsoft.AspNetCore.Mvc;
using RinhaBackend._2024.Q1.Core.Models;

namespace RinhaBackend._2024.Q1.Api.Controllers;

[ApiController]
[Produces("application/json")]
public class ClientsController : Controller
{
    [HttpGet]
    [Route("clientes/{id}/extrato")]
    public IActionResult GetExtract([FromRoute] int id)
    {
        var extract = new ExtractResponse()
        {
            Balance = new ExtractBalance()
            {
                Total = -9098,
                Date = DateTime.UtcNow,
                CreditLimit = 100000
            },
            Transactions = new List<ExtractTransaction>()
            {
                new ExtractTransaction()
                {
                    Value = 10,
                    TransactionDate = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc),
                    Type = 'c',
                    Description = "descricao"
                },
                new ExtractTransaction()
                {
                    Value = 90000,
                    TransactionDate = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc),
                    Type = 'd',
                    Description = "descricao"
                }
            }
        };
        
        return Ok(extract);
    }
    
    [HttpPost]
    [Route("clientes/{id}/transacoes")]
    public IActionResult PostTransaction([FromRoute] int id, [FromBody] TransactionRequest payload)
    {
        return Ok(new TransactionResponse()
        {
            CreditLimit = 100000,
            Balance = -9098,
        });
    }
}

