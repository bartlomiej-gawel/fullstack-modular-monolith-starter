namespace Common.Abstractions.IntegrationEvents;

public interface IIntegrationEvent
{
    Guid Id { get; }
    DateTime OccurredAt { get; }
    Guid CorrelationId { get; }
}
