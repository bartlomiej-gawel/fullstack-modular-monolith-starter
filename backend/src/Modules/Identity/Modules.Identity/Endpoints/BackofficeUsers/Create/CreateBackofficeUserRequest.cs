namespace Modules.Identity.Endpoints.BackofficeUsers.Create;

internal sealed record CreateBackofficeUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string PhonePrefix,
    string PhoneNumber);