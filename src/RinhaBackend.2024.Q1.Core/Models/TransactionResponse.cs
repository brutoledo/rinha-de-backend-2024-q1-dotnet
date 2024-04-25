using System.Text.Json.Serialization;

namespace RinhaBackend._2024.Q1.Core.Models;

public class TransactionResponse
{
    [JsonPropertyName("limite")]
    public int CreditLimit { get; set; }
    
    [JsonPropertyName("saldo")]
    public int Balance { get; set; }
}