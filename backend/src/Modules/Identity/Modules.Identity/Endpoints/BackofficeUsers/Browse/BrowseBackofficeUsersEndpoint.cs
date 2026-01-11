using System.Linq.Expressions;
using Common.Abstractions.Endpoints;
using Common.Infrastructure.Database;
using Common.Infrastructure.Endpoints.Browse;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Modules.Identity.Domain.Users;
using Modules.Identity.Infrastructure.Database;

namespace Modules.Identity.Endpoints.BackofficeUsers.Browse;

internal abstract class BrowseBackofficeUsersEndpoint : IEndpoint
{
    private static readonly Expression<Func<User, string?>>[] SearchProperties =
    [
        x => x.FirstName,
        x => x.LastName,
        x => x.Email.Value
    ];

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("browse", async (
                BrowseBackofficeUsersRequest request,
                IdentityModuleDbContext dbContext,
                CancellationToken cancellationToken) =>
            {
                var response = await dbContext.Users
                    .AsNoTracking()
                    .Where(x => x.Role == UserRole.Backoffice)
                    .WhereIf(request.Status.HasValue, x => x.Status == request.Status!.Value)
                    .BrowseAsync(
                        request,
                        defaultSortBy: "Id",
                        SearchProperties,
                        cancellationToken)
                    .MapBrowseResult<User, BrowseBackofficeUsersResponse>();

                return TypedResults.Ok(response);
            })
            .WithName("browse-backoffice-users")
            .WithSummary("Browse backoffice users")
            .Produces<BrowseResult<BrowseBackofficeUsersResponse>>();
    }
}
