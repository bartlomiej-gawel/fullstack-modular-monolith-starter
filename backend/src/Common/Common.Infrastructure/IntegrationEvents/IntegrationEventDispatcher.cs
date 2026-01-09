using System.Collections.Concurrent;
using System.Text.Json;
using Common.Abstractions.IntegrationEvents;
using Common.Infrastructure.Outbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Common.Infrastructure.IntegrationEvents;

public sealed class IntegrationEventDispatcher : IIntegrationEventDispatcher
{
    private static readonly ConcurrentDictionary<Type, Type> HandlerTypeCache = new();
    private static readonly ConcurrentDictionary<Type, Type> WrapperTypeCache = new();
    private static readonly ConcurrentDictionary<string, Type?> EventTypeCache = new();

    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<IntegrationEventDispatcher> _logger;

    public IntegrationEventDispatcher(
        IServiceProvider serviceProvider,
        ILogger<IntegrationEventDispatcher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task DispatchAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        var eventType = EventTypeCache.GetOrAdd(message.EventTypeAssemblyQualifiedName, Type.GetType);
        if (eventType is null)
            throw new InvalidOperationException($"Could not resolve event type '{message.EventTypeName}'");

        var @event = JsonSerializer.Deserialize(message.EventPayload, eventType);
        if (@event is null)
            throw new InvalidOperationException($"Failed to deserialize '{message.EventTypeName}'");

        var handlerType = HandlerTypeCache.GetOrAdd(
            eventType,
            type => typeof(IIntegrationEventHandler<>).MakeGenericType(type));

        var handlers = _serviceProvider.GetServices(handlerType).ToList();
        if (handlers.Count == 0)
        {
            _logger.LogTrace("Not found any registered integration handlers for '{EventType}'", message.EventTypeName);
            return;
        }

        var handlerWrappers = handlers
            .Where(h => h is not null)
            .Select(h => IntegrationHandlerWrapper.Create(h!, eventType))
            .ToList();

        foreach (var handlerWrapper in handlerWrappers)
            await handlerWrapper.HandleAsync((IIntegrationEvent)@event, cancellationToken);
    }

    private abstract class IntegrationHandlerWrapper
    {
        public abstract Task HandleAsync(IIntegrationEvent @event, CancellationToken cancellationToken);

        public static IntegrationHandlerWrapper Create(object handler, Type eventType)
        {
            var wrapperType = WrapperTypeCache.GetOrAdd(
                eventType,
                type => typeof(IntegrationHandlerWrapper<>).MakeGenericType(type));

            return (IntegrationHandlerWrapper)Activator.CreateInstance(wrapperType, handler)!;
        }
    }

    private sealed class IntegrationHandlerWrapper<TEvent>(object handler) : IntegrationHandlerWrapper
        where TEvent : IIntegrationEvent
    {
        private readonly IIntegrationEventHandler<TEvent> _handler = (IIntegrationEventHandler<TEvent>)handler;

        public override async Task HandleAsync(IIntegrationEvent @event, CancellationToken cancellationToken)
        {
            await _handler.HandleAsync((TEvent)@event, cancellationToken);
        }
    }
}
