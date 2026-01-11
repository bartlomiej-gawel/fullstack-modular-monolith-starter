using Microsoft.AspNetCore.Routing;

namespace Common.Abstractions.Endpoints;

public interface IEndpoint
{
    static abstract void Map(IEndpointRouteBuilder app);
}

public interface IEndpointGroup
{
    static abstract RouteGroupBuilder MapGroup(IEndpointRouteBuilder app);
}
