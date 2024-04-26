using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using RinhaBackend._2024.Q1.Core.Entities;
using RinhaBackend._2024.Q1.Core.Models.Requests;
using RinhaBackend._2024.Q1.Core.Repositories;

namespace RinhaBackend._2024.Q1.Infrastructure;

public class ClientRepository : IClientRepository
{
    private readonly string? _connectionString;

    public ClientRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("ClientsDb");
    }

    public async Task<Client?> GetById(int id)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var query = "SELECT ID, NAME, CREDIT_LIMIT as CreditLimit, BALANCE FROM CLIENTS where ID=@id";
            var client = await connection.QuerySingleOrDefaultAsync<Client>(query, new { id });
            return client;
        }
    }

    public async Task<ClientExtract?> GetClientExtract(int id)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var transactions = new HashSet<Transaction>();
            ClientExtract? clientExtract = null;

            var query = @"SELECT c.ID as ClientId, c.NAME, c.CREDIT_LIMIT as CreditLimit, c.BALANCE, 
                                 t.ID as TransactionId, t.VALUE, t.TYPE, t.DESCRIPTION, t.CREATED_DATE AT TIME ZONE 'utc' as CreatedDate
                            FROM CLIENTS c
                            LEFT JOIN CLIENT_TRANSACTIONS t ON c.ID = t.CLIENT_ID
                          WHERE c.ID = @id 
                            ORDER BY t.CREATED_DATE DESC
                            LIMIT 10
            ";

            var command = new CommandDefinition(query, new { id });
            await connection.QueryAsync<ClientExtract, Transaction, ClientExtract>(command, map: (c, t) =>
            {
                if (clientExtract == null)                
                    clientExtract = c;

                if (t is not null)
                    transactions.Add(t);

                return c;
            }, splitOn: "TransactionId");

            if (clientExtract is not null)
                clientExtract.LastTransaction = transactions;

            return clientExtract;
        }
    }

    public async Task<int> CreateTransaction(int clientId, TransactionRequest request)
    {
        int transactionId;
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            await using (var transaction = await connection.BeginTransactionAsync())
            {
                var updateBalance = @"UPDATE CLIENTS SET BALANCE = BALANCE + @value WHERE ID = @clientId";
                await connection.ExecuteScalarAsync<int>(updateBalance, new { value = request.Value, clientId }, transaction);
                
                var insertTransaction = @"INSERT INTO CLIENT_TRANSACTIONS (
                                    CLIENT_ID, VALUE, TYPE, DESCRIPTION)
                                VALUES (
                                        @clientId,@value,@type, @description)
                             ";
                var payload = new
                    { clientId, value = request.Value, type = request.Type, description = request.Description };
                transactionId = await connection.ExecuteScalarAsync<int>(insertTransaction, payload, transaction);
                await transaction.CommitAsync();
            }
        }
        
        return transactionId;
    }
}