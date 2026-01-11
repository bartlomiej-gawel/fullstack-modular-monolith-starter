using Common.Abstractions.Domain;
using Common.Infrastructure.Database;
using Modules.Identity.Domain.Users;
using Modules.Identity.Domain.Users.Events;
using Modules.Identity.Infrastructure.Database;
using Modules.Identity.IntegrationEvents;

namespace Modules.Identity.DomainEventHandlers;

public sealed class UserCreatedDomainEventHandler : IDomainEventHandler<UserCreatedDomainEvent>
{
    private readonly IdentityModuleDbContext _dbContext;

    public UserCreatedDomainEventHandler(IdentityModuleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task HandleAsync(UserCreatedDomainEvent @event, CancellationToken cancellationToken = default)
    {
        if (@event is { Role: UserRole.Backoffice, Status: UserStatus.Active })
        {
            await _dbContext.PublishIntegrationEvent(new BackofficeUserInitializedIntegrationEvent(
                @event.UserId.Value,
                @event.FirstName,
                @event.LastName,
                @event.Email.Value,
                @event.Phone.Prefix,
                @event.Phone.Number));
        }

        if (@event is { Role: UserRole.Backoffice, Status: UserStatus.Inactive })
        {
            await _dbContext.PublishIntegrationEvent(new BackofficeUserCreatedIntegrationEvent(
                @event.UserId.Value,
                @event.FirstName,
                @event.LastName,
                @event.Email.Value,
                @event.EmailVerificationToken!.Value,
                @event.Phone.Prefix,
                @event.Phone.Number));
        }
    }
}
