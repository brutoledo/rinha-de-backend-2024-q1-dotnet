namespace RinhaBackend._2024.Q1.Core.Entities;

public class ClientTransaction
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int Value { get; set; }
    public string Name { get; set; }
    public int CreditLimit { get; set; }
    public int Balance { get; set; }
}