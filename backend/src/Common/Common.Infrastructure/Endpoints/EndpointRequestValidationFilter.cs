using ErrorOr;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Common.Infrastructure.Endpoints;

public sealed class EndpointRequestValidationFilter<TRequest> : IEndpointFilter
{
    private readonly ILogger<EndpointRequestValidationFilter<TRequest>> _logger;
    private readonly IValidator<TRequest>? _validator;

    public EndpointRequestValidationFilter(
        ILogger<EndpointRequestValidationFilter<TRequest>> logger,
        IValidator<TRequest>? validator = null)
    {
        _logger = logger;
        _validator = validator;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (_validator is null)
            return await next(context);

        var requestName = typeof(TRequest).FullName;
        var request = context.Arguments.OfType<TRequest>().First();

        var validationResult = await _validator.ValidateAsync(request, context.HttpContext.RequestAborted);
        if (validationResult.IsValid)
            return await next(context);

        _logger.LogError("{Request}: Validation failed", requestName);

        var validationErrors = validationResult.Errors
            .Select(failure => Error.Validation(
                failure.PropertyName,
                failure.ErrorMessage))
            .ToList();

        return validationErrors.ToProblemResult();
    }
}
