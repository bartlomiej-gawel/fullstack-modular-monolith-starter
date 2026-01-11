using Common.Abstractions.Endpoints;
using Common.Infrastructure.Endpoints;
using ErrorOr;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Modules.Identity.Domain.Users;
using Modules.Identity.Infrastructure.Database;

namespace Modules.Identity.Endpoints.BackofficeUsers.Get;

internal abstract class GetBackofficeUserEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("{userId:guid}", async (
                Guid userId,
                IdentityModuleDbContext dbContext,
                CancellationToken cancellationToken) =>
            {
                var user = await dbContext.Users
                    .AsNoTracking()
                    .Where(x =>
                        x.Id == UserId.CreateFrom(userId) &&
                        x.Role == UserRole.Backoffice)
                    .Select(x => new GetBackofficeUserResponse(
                        x.Id.Value,
                        x.FirstName,
                        x.LastName,
                        x.Email.Value,
                        x.Phone.Prefix,
                        x.Phone.Number,
                        x.Role,
                        x.Status))
                    .FirstOrDefaultAsync(cancellationToken);

                return user is null
                    ? Errors.NotFound.ToProblemResult()
                    : TypedResults.Ok(user);
            })
            .WithName("get-backoffice-user")
            .WithSummary("Get backoffice user by id")
            .Produces<GetBackofficeUserResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static class Errors
    {
        public static readonly Error NotFound = Error.NotFound(
            "Identity.GetBackofficeUser.NotFound",
            "Backoffice user with provided id was not found.");
    }
}
