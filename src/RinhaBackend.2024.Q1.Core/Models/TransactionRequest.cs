using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RinhaBackend._2024.Q1.Core.Models;

public class TransactionRequest : IValidatableObject
{
    
    [Required]
    [JsonPropertyName("valor")]
    [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
    public int Value { get; set; }
    
    [Required]
    [JsonPropertyName("tipo")]
    public char Type { get; set; }
    
    [Required]
    [JsonPropertyName("descricao")]
    [StringLength(10)]
    public string Description { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Type != 'c' && Type != 'd')
        {
            yield return new ValidationResult(
                $"Tipo must be either 'c' for 'credit' or 'd' for 'debit'",
                new[] { nameof(Type) });
        }
    }
}