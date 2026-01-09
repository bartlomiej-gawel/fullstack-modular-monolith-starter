using Common.Infrastructure.Outbox;

namespace Common.Infrastructure.IntegrationEvents;

public interface IIntegrationEventDispatcher
{
    Task DispatchAsync(OutboxMessage message, CancellationToken cancellationToken = default);
}
