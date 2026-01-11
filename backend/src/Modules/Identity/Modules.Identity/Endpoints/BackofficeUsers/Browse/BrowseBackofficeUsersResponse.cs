using System.Linq.Expressions;
using Common.Abstractions.Mapping;
using Modules.Identity.Domain.Users;

namespace Modules.Identity.Endpoints.BackofficeUsers.Browse;

internal sealed record BrowseBackofficeUsersResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    UserStatus Status) : IWithExpressionMapFrom<User, BrowseBackofficeUsersResponse>
{
    public static Expression<Func<User, BrowseBackofficeUsersResponse>> MapExpression =>
        user => new BrowseBackofficeUsersResponse(
            user.Id.Value,
            user.FirstName,
            user.LastName,
            user.Email.Value,
            user.Status);
}
