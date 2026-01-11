using Common.Abstractions.Domain.ValueObjects;
using Common.Abstractions.IntegrationEvents;
using Common.Infrastructure.Database;
using Modules.Communications.Domain.Recipients;
using Modules.Communications.Infrastructure.Database;
using Modules.Identity.IntegrationEvents;

namespace Modules.Communications.IntegrationEventHandlers;

public sealed class BackofficeUserCreatedIntegrationEventHandler : IIntegrationEventHandler<BackofficeUserCreatedIntegrationEvent>
{
    private const string EventHandlerTypeName = nameof(BackofficeUserCreatedIntegrationEventHandler);

    private readonly CommunicationsModuleDbContext _dbContext;

    public BackofficeUserCreatedIntegrationEventHandler(CommunicationsModuleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task HandleAsync(BackofficeUserCreatedIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        if (await _dbContext.IsIntegrationEventProcessed(@event.Id, EventHandlerTypeName, cancellationToken))
            return;

        var email = Email.Create(@event.Email);
        if (email.IsError)
            throw new InvalidOperationException($"Invalid email: {email.FirstError.Description}");

        var phone = Phone.Create(@event.PhonePrefix, @event.PhoneNumber);
        if (phone.IsError)
            throw new InvalidOperationException($"Invalid phone: {phone.FirstError.Description}");

        var recipient = Recipient.CreateBackofficeRecipient(
            @event.UserId,
            @event.FirstName,
            @event.LastName,
            email.Value,
            phone.Value);

        if (recipient.IsError)
            throw new InvalidOperationException($"Failed to create recipient: {recipient.FirstError.Description}");

        await _dbContext.Recipients.AddAsync(recipient.Value, cancellationToken);
        await _dbContext.MarkIntegrationEventAsProcessed(@event, EventHandlerTypeName, @event.CorrelationId, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
