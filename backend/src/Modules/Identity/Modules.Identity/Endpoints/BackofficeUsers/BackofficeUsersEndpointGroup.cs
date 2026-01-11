using Common.Abstractions.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Identity.Endpoints.BackofficeUsers.Browse;
using Modules.Identity.Endpoints.BackofficeUsers.Create;
using Modules.Identity.Endpoints.BackofficeUsers.Get;

namespace Modules.Identity.Endpoints.BackofficeUsers;

internal abstract class BackofficeUsersEndpointGroup : IEndpointGroup
{
    public static RouteGroupBuilder MapGroup(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/backoffice/identity-module/users")
            .WithTags("Backoffice users");

        BrowseBackofficeUsersEndpoint.Map(group);
        CreateBackofficeUserEndpoint.Map(group);
        GetBackofficeUserEndpoint.Map(group);

        return group;
    }
}
