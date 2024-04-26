namespace RinhaBackend._2024.Q1.Core.Entities;

public class ClientExtract
{
    public int ClientId { get; set; }
    public int CreditLimit { get; set; }
    public int Balance { get; set; }
    public ISet<Transaction> LastTransaction { get; set; }
}