using Modules.Identity.Domain.Users;

namespace Modules.Identity.Endpoints.BackofficeUsers.Get;

internal sealed record GetBackofficeUserResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string PhonePrefix,
    string PhoneNumber,
    UserRole Role,
    UserStatus Status);