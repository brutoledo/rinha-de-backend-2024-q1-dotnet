using System.Diagnostics;
using OneOf;
using RinhaBackend._2024.Q1.Core.Models.Errors;
using RinhaBackend._2024.Q1.Core.Models.Requests;
using RinhaBackend._2024.Q1.Core.Models.Responses;
using RinhaBackend._2024.Q1.Core.Repositories;
using RinhaBackend._2024.Q1.Core.Services;

namespace RinhaBackend._2024.Q1.Application;

public class BankService : IBankService
{
    private readonly IClientRepository _clientRepository;

    public BankService(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }
    
    public async Task<OneOf<TransactionResponse, NoClientFound, TransactionOutOfLimitAllowedFound>> CreateTransaction(int clientId, TransactionRequest request)
    {
        var client = await _clientRepository.GetById(clientId);
        if (client is null)
            return new NoClientFound();

        var newBalance = client.Balance;
        switch (request.Type)
        {
            case 'c':
                newBalance += request.Value;
                break;
            
            case 'd':
                newBalance -= request.Value;
                break;
        }

        if ((client.CreditLimit * -1) > newBalance) 
            return new TransactionOutOfLimitAllowedFound();

        await _clientRepository.CreateTransaction(clientId, request);
        
        return new TransactionResponse()
        {
            CreditLimit = client.CreditLimit,
            Balance = newBalance,
        };
    }

    public async Task<OneOf<ExtractResponse, NoClientFound>> GetClientExtract(int clientId)
    {
        var client = await _clientRepository.GetById(clientId);
        if (client is null)
            return new NoClientFound();

        return new ExtractResponse()
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
    }
}