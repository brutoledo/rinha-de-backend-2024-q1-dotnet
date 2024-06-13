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
                try
                {
                    // this row level lock prevent another row FOR UPDATE to RUN
                    //await connection.ExecuteAsync(@"LOCK TABLE CLIENTS IN ROW EXCLUSIVE MODE", transaction);
                    // Lock the row with FOR UPDATE
                    // await connection.ExecuteAsync(@"SELECT * FROM CLIENTS WHERE ID = @clientId FOR UPDATE",
                    //     new { clientId }, transaction);

                    await connection.ExecuteAsync(@"LOCK TABLE CLIENTS IN ACCESS EXCLUSIVE MODE", transaction);

                    var updateBalance = @"UPDATE CLIENTS SET BALANCE = BALANCE + @value WHERE ID = @clientId";
                    await connection.ExecuteScalarAsync<int>(updateBalance, new { value = request.Value, clientId },
                        transaction);

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
                catch (Exception e)
                {
                    Console.WriteLine(e);

                    // Rollback the transaction in case of error
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        return transactionId;
    }

    public async Task<(int, int, int)> CreateAtomicTransaction(int clientId, TransactionRequest request)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            await using (var transaction = await connection.BeginTransactionAsync())
            {
                try
                {
                    // this row level lock prevent another row FOR UPDATE to RUN
                    await connection.ExecuteAsync(@"LOCK TABLE CLIENTS IN ROW EXCLUSIVE MODE", transaction);

                    // Lock the row with FOR UPDATE
                    var rows = await connection.ExecuteScalarAsync<int>(
                        @"SELECT * FROM CLIENTS WHERE ID = @clientId FOR UPDATE",
                        new { clientId }, transaction);

                    if (rows == 0)
                    {
                        await transaction.CommitAsync();
                        return new(-1, 0, 0); // not found user
                    }

                    var updateBalance =
                        @"UPDATE CLIENTS SET BALANCE = BALANCE + @value WHERE ID = @clientId 
                                                AND (@value > 0 OR BALANCE + @value >= CREDIT_LIMIT * -1)";
                    var updatedRows = await connection.ExecuteAsync(updateBalance,
                        new { value = request.Value, clientId },
                        transaction);

                    if (updatedRows == 0)
                    {
                        await transaction.CommitAsync();
                        return (-2, 0, 0); // then means user has no limit
                    }

                    var insertTransaction = @"INSERT INTO CLIENT_TRANSACTIONS (
                                    CLIENT_ID, VALUE, TYPE, DESCRIPTION)
                                VALUES (
                                        @clientId,@value,@type, @description)
                             ";
                    var payload = new
                        { clientId, value = request.Value, type = request.Type, description = request.Description };
                    await connection.ExecuteScalarAsync(insertTransaction, payload, transaction);

                    var query = "SELECT ID, NAME, CREDIT_LIMIT as CreditLimit, BALANCE FROM CLIENTS where ID=@id";
                    var client =
                        await connection.QuerySingleOrDefaultAsync<Client>(query, new { id = clientId }, transaction);

                    await transaction.CommitAsync();

                    return (1, client.CreditLimit, client.Balance);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);

                    // Rollback the transaction in case of error
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
    }

    public async Task<(int, int, int)> CreateTransactionByPostgresFunc(int clientId, TransactionRequest request)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            try
            {
                var result = await connection.QueryFirstOrDefaultAsync<object[]>(
                    "select createTransaction(@clientId, @transactionValue, @transactionType, @description);",
                    new
                    {
                        clientId,
                        transactionValue = request.Value,
                        transactionType = request.Type,
                        description = request.Description,
                    });

                if (result is null)
                    throw new InvalidOperationException("Unexpected null response from create transaction function");

                if (result.Length == 1)
                {
                    var failureCode = (int)result[0];
                    return (failureCode, 0, 0);
                }

                var balance = (int)result[0];
                var limit = (int)result[1];
                return (1, limit, balance);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}