using System.Text.Json;
using Common.Abstractions.IntegrationEvents;
using Common.Infrastructure.Inbox;
using Common.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Common.Infrastructure.Database;

public static class ApplicationDbContextExtensions
{
    extension(ApplicationDbContext dbContext)
    {
        public async Task PublishIntegrationEvent<TEvent>(TEvent integrationEvent)
            where TEvent : IIntegrationEvent
        {
            if (!dbContext.EnableOutbox)
                throw new InvalidOperationException("Cannot publish integration event in this module. Outbox is disabled.");

            var eventType = integrationEvent.GetType();
            var eventTypeName = eventType.Name;
            var eventTypeAssemblyQualifiedName = eventType.AssemblyQualifiedName!;
            var eventPayload = JsonSerializer.Serialize(integrationEvent, eventType);

            var outboxMessage = OutboxMessage.Create(
                integrationEvent.Id,
                eventTypeName,
                eventTypeAssemblyQualifiedName,
                eventPayload,
                integrationEvent.OccurredAt);

            await dbContext.OutboxMessages.AddAsync(outboxMessage);
        }

        public async Task<bool> IsIntegrationEventProcessed(
            Guid eventId,
            string eventHandlerTypeName,
            CancellationToken cancellationToken = default)
        {
            if (!dbContext.EnableInbox)
                return false;

            var isProcessed = await dbContext.InboxMessages.AnyAsync(x =>
                    x.EventId == eventId &&
                    x.EventHandlerTypeName == eventHandlerTypeName,
                cancellationToken);

            return isProcessed;
        }

        public async Task MarkIntegrationEventAsProcessed<TEvent>(
            TEvent @event,
            string eventHandlerTypeName,
            Guid? correlationId = null,
            CancellationToken cancellationToken = default) where TEvent : IIntegrationEvent
        {
            if (!dbContext.EnableInbox)
                return;

            var inboxMessage = InboxMessage.Create(
                @event.Id,
                @event.GetType().Name,
                eventHandlerTypeName,
                correlationId);

            await dbContext.InboxMessages.AddAsync(inboxMessage, cancellationToken);
        }
    }
}
