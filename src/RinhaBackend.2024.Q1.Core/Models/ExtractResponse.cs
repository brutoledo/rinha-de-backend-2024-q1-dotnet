using System.Text.Json.Serialization;

namespace RinhaBackend._2024.Q1.Core.Models;

public class ExtractResponse
{
    [JsonPropertyName("saldo")]
    public ExtractBalance Balance { get; set; }
    
    [JsonPropertyName("ultimas_transacoes")]
    public IList<ExtractTransaction> Transactions { get; set; }
}

public class ExtractBalance
{
    [JsonPropertyName("total")]
    public int Total { get; set; }
    [JsonPropertyName("data_extrato")]
    public DateTime Date { get; set; }
    [JsonPropertyName("limite")]
    public int CreditLimit { get; set; }
}

public class ExtractTransaction
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