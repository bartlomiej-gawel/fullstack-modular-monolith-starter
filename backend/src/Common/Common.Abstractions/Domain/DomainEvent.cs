namespace Common.Abstractions.Domain;

public abstract record DomainEvent : IDomainEvent
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}