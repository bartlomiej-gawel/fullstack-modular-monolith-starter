using Common.Infrastructure.Endpoints.Browse;
using Modules.Identity.Domain.Users;

namespace Modules.Identity.Endpoints.BackofficeUsers.Browse;

internal sealed record BrowseBackofficeUsersRequest(UserStatus? Status) : BrowseModel;