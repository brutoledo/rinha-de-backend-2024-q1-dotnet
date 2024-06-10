using OneOf;
using RinhaBackend._2024.Q1.Core.Models.Errors;
using RinhaBackend._2024.Q1.Core.Models.Requests;
using RinhaBackend._2024.Q1.Core.Models.Responses;

namespace RinhaBackend._2024.Q1.Core.Services;

public interface IBankService
{
    Task<OneOf<TransactionResponse, NoClientFound, TransactionOutOfLimitAllowedFound>> CreateTransaction(int clientId,
        TransactionRequest request);

    Task<OneOf<TransactionResponse, NoClientFound, TransactionOutOfLimitAllowedFound>> CreateAtomicTransaction(
        int clientId,
        TransactionRequest request);
    Task<OneOf<ExtractResponse, NoClientFound>> GetClientExtract(int clientId);
}