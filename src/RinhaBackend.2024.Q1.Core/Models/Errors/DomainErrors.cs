using OneOf;

namespace RinhaBackend._2024.Q1.Core.Models.Errors;

public class DomainErrors : OneOfBase<NoClientFound,TransactionOutOfLimitAllowedFound>
{
    protected DomainErrors(OneOf<NoClientFound, TransactionOutOfLimitAllowedFound> input) : base(input)
    {
    }

    public static implicit operator DomainErrors(NoClientFound _) => new(_);
    public static implicit operator DomainErrors(TransactionOutOfLimitAllowedFound _) => new(_);
}

public record struct NoClientFound();
public record struct TransactionOutOfLimitAllowedFound();
