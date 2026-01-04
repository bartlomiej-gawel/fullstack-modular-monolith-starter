namespace Common.Abstractions.Domain;

public interface IDomainEvent
{
    Guid Id { get; }
    DateTime OccurredOn { get; }
}
