using Common.Abstractions.Domain;

namespace Common.Infrastructure.Domain;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken = default);
}