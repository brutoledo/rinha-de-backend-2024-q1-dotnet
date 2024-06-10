using RinhaBackend._2024.Q1.Core.Entities;
using RinhaBackend._2024.Q1.Core.Models.Requests;

namespace RinhaBackend._2024.Q1.Core.Repositories;

public interface IClientRepository
{
    Task<Client?> GetById(int id);
    Task<ClientExtract?> GetClientExtract(int id);
    Task<int> CreateTransaction(int clientId, TransactionRequest request);
    Task<(int, int, int)> CreateAtomicTransaction(int clientId, TransactionRequest request);
}