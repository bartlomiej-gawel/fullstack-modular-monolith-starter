using Common.Abstractions.IntegrationEvents;

namespace Modules.Identity.IntegrationEvents;

public sealed record BackofficeUserInitializedIntegrationEvent(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string PhonePrefix,
    string PhoneNumber) : IntegrationEvent;