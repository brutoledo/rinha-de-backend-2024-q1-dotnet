using System.Text.Json.Serialization;

namespace RinhaBackend._2024.Q1.Core.Models.Responses;

public class ExtractResponse
{
    [JsonPropertyName("saldo")]
    public ExtractResponseBalance Balance { get; set; }
    
    [JsonPropertyName("ultimas_transacoes")]
    public IList<ExtractResponseTransaction> Transactions { get; set; }
}

public class ExtractResponseBalance
{
    [JsonPropertyName("total")]
    public int Total { get; set; }
    [JsonPropertyName("data_extrato")]
    public DateTime Date { get; set; }
    [JsonPropertyName("limite")]
    public int CreditLimit { get; set; }
}

public class ExtractResponseTransaction
{
    [JsonPropertyName("valor")]
    public int Value { get; set; }
    
    [JsonPropertyName("tipo")]
    public char Type { get; set; }
    
    [JsonPropertyName("descricao")]
    public string Description { get; set; }
    
    [JsonPropertyName("realizada_em")]
    public DateTime TransactionDate { get; set; }
}