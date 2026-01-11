using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Common.Infrastructure.Endpoints;

public static class EndpointExtensions
{
    public static RouteHandlerBuilder WithRequestValidation<TRequest>(this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter<EndpointRequestValidationFilter<TRequest>>();
    }
}
