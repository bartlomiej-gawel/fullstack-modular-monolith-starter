using System.Text.Json;
using Common.Abstractions.IntegrationEvents;
using Common.Infrastructure.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Infrastructure.Outbox;

public static class OutboxExtensions
{
    extension(IServiceCollection services)
    {
        internal void AddOutbox(IConfiguration configuration)
        {
            services.AddOptions<OutboxOptions>()
                .Bind(configuration.GetSection(OutboxOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }
    }

    public static async Task PublishIntegrationEvent<TEvent>(this ApplicationDbContext dbContext, TEvent integrationEvent)
        where TEvent : IIntegrationEvent
    {
        if (!dbContext.EnableOutbox)
            throw new InvalidOperationException("Cannot publish integration event. Outbox is disabled");

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

        await dbContext.Set<OutboxMessage>().AddAsync(outboxMessage);
    }
}
