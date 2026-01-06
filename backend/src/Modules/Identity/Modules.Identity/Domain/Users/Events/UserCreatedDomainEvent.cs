using Common.Abstractions.Domain;
using Common.Abstractions.Domain.ValueObjects;

namespace Modules.Identity.Domain.Users.Events;

public sealed record UserCreatedDomainEvent(
    UserId UserId,
    string FirstName,
    string LastName,
    Email Email,
    Phone Phone,
    UserStatus Status,
    UserRole Role,
    EmailVerificationToken? EmailVerificationToken = null) : DomainEvent;