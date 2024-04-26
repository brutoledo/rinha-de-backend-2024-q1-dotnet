namespace RinhaBackend._2024.Q1.Core.Entities;

public class Transaction
{
    public int TransactionId { get; set; }
    public int Value { get; set; }
    public char Type { get; set; }
    public string Description { get; set; }
    public DateTime CreatedDate { get; set; }
}