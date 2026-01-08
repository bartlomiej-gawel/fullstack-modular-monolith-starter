using Common.Abstractions.Correlation;

namespace Common.Abstractions.IntegrationEvents;

public abstract record IntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public Guid CorrelationId { get; init; } = EventCorrelationContext.CorrelationId ?? Guid.CreateVersion7();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
