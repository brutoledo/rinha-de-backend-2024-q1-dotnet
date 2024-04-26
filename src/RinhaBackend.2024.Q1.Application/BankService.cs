﻿using OneOf;
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

        if (request.Type == 'd')
            request.Value *= -1;

        var balanceForecast = client.Balance + request.Value;
        if ((client.CreditLimit * -1) > balanceForecast)
            return new TransactionOutOfLimitAllowedFound();

        await _clientRepository.CreateTransaction(clientId, request);
        
        return new TransactionResponse()
        {
            CreditLimit = client.CreditLimit,
            Balance = balanceForecast,
        };
    }

    public async Task<OneOf<ExtractResponse, NoClientFound>> GetClientExtract(int clientId)
    {
        var client = await _clientRepository.GetClientExtract(clientId);
        if (client is null)
            return new NoClientFound();

        return new ExtractResponse()
        {
            Balance = new ExtractResponseBalance()
            {
                Total = client.Balance,
                Date = DateTime.UtcNow,
                CreditLimit = client.CreditLimit
            },
            Transactions = client.LastTransaction.Select(t =>
                new ExtractResponseTransaction()
                {
                    Value = t.Value,
                    TransactionDate = DateTime.SpecifyKind(t.CreatedDate, DateTimeKind.Utc),
                    Type = t.Type,
                    Description = t.Description,
                }).ToList()
        };
    }
}