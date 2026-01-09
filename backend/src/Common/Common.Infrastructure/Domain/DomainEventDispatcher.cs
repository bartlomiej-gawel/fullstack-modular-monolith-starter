using System.Collections.Concurrent;
using Common.Abstractions.Correlation;
using Common.Abstractions.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Common.Infrastructure.Domain;

public sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    private static readonly ConcurrentDictionary<Type, Type> HandlerTypeCache = new();
    private static readonly ConcurrentDictionary<Type, Type> WrapperTypeCache = new();

    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DomainEventDispatcher> _logger;

    public DomainEventDispatcher(
        IServiceProvider serviceProvider,
        ILogger<DomainEventDispatcher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken = default)
    {
        foreach (var @event in events)
        {
            var eventType = @event.GetType();
            var eventTypeName = eventType.Name;

            try
            {
                EventCorrelationContext.CorrelationId = @event.Id;

                var handlerType = HandlerTypeCache.GetOrAdd(
                    eventType,
                    type => typeof(IDomainEventHandler<>).MakeGenericType(type));

                var handlers = _serviceProvider.GetServices(handlerType).ToList();
                if (handlers.Count == 0)
                {
                    _logger.LogTrace("Not found any registered domain handlers for '{EventType}'", eventTypeName);
                    continue;
                }

                var handlerWrappers = handlers
                    .Where(h => h is not null)
                    .Select(h => DomainHandlerWrapper.Create(h!, eventType))
                    .ToList();

                foreach (var handlerWrapper in handlerWrappers)
                    await handlerWrapper.HandleAsync(@event, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during dispatching domain event '{EventType}'", eventTypeName);
                throw;
            }
            finally
            {
                EventCorrelationContext.CorrelationId = null;
            }
        }
    }

    private abstract class DomainHandlerWrapper
    {
        public abstract Task HandleAsync(IDomainEvent @event, CancellationToken cancellationToken);

        public static DomainHandlerWrapper Create(object handler, Type eventType)
        {
            var wrapperType = WrapperTypeCache.GetOrAdd(
                eventType,
                type => typeof(DomainHandlerWrapper<>).MakeGenericType(type));

            return (DomainHandlerWrapper)Activator.CreateInstance(wrapperType, handler)!;
        }
    }

    private sealed class DomainHandlerWrapper<TEvent>(object handler) : DomainHandlerWrapper
        where TEvent : IDomainEvent
    {
        private readonly IDomainEventHandler<TEvent> _handler = (IDomainEventHandler<TEvent>)handler;

        public override async Task HandleAsync(IDomainEvent @event, CancellationToken cancellationToken)
        {
            await _handler.HandleAsync((TEvent)@event, cancellationToken);
        }
    }
}
