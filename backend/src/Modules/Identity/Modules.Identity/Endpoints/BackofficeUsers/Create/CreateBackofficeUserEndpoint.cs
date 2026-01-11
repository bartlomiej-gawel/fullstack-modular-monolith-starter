using Common.Abstractions.Domain.ValueObjects;
using Common.Abstractions.Endpoints;
using Common.Infrastructure.Endpoints;
using ErrorOr;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Modules.Identity.Domain.Users;
using Modules.Identity.Infrastructure.Database;

namespace Modules.Identity.Endpoints.BackofficeUsers.Create;

internal abstract class CreateBackofficeUserEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("", async (
                CreateBackofficeUserRequest request,
                IdentityModuleDbContext dbContext,
                CancellationToken cancellationToken) =>
            {
                var emailResult = Email.Create(request.Email);
                if (emailResult.IsError)
                    return emailResult.Errors.ToProblemResult();

                var emailExists = await dbContext.Users.AnyAsync(x => x.Email == emailResult.Value, cancellationToken);
                if (emailExists)
                    return Errors.EmailAlreadyExists.ToProblemResult();

                var phoneResult = Phone.Create(request.PhonePrefix, request.PhoneNumber);
                if (phoneResult.IsError)
                    return phoneResult.Errors.ToProblemResult();

                var createResult = User.CreateBackofficeUser(
                    request.FirstName,
                    request.LastName,
                    emailResult.Value,
                    phoneResult.Value);

                if (createResult.IsError)
                    return createResult.Errors.ToProblemResult();

                await dbContext.Users.AddAsync(createResult.Value, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);

                return TypedResults.Created();
            })
            .WithName("create-backoffice-user")
            .WithSummary("Create backoffice user")
            .WithRequestValidation<CreateBackofficeUserRequest>()
            .Produces(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }

    private static class Errors
    {
        public static readonly Error EmailAlreadyExists = Error.Conflict(
            "Identity.CreateBackofficeUser.EmailAlreadyExists",
            "Backoffice user with this email already exists.");
    }
}
