using Common.Abstractions.IntegrationEvents;

namespace Modules.Identity.IntegrationEvents;

public sealed record BackofficeUserCreatedIntegrationEvent(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string EmailVerificationToken,
    string PhonePrefix,
    string PhoneNumber) : IntegrationEvent;