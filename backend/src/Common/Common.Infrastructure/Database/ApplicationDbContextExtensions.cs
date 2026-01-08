using System.Text.Json;
using Common.Abstractions.IntegrationEvents;
using Common.Infrastructure.Outbox;

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
            var eventTypeAssemblyName = eventType.AssemblyQualifiedName!;
            var eventPayload = JsonSerializer.Serialize(integrationEvent, eventType);

            var outboxMessage = OutboxMessage.Create(
                integrationEvent.Id,
                eventTypeName,
                eventTypeAssemblyName,
                eventPayload,
                integrationEvent.OccurredAt);

            await dbContext.OutboxMessages.AddAsync(outboxMessage);
        }
    }
}
