using ErrorOr;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Common.Infrastructure.Endpoints;

public static class EndpointErrorExtensions
{
    public static IResult ToProblemResult(this List<Error> errors)
    {
        if (errors.Count is 0)
            return TypedResults.Problem();

        return errors.All(error => error.Type == ErrorType.Validation)
            ? errors.ToValidationProblemResult()
            : errors[0].ToProblemResult();
    }

    public static IResult ToProblemResult(this Error error)
    {
        if (error.Type == ErrorType.Validation)
            return error.ToErrorList().ToValidationProblemResult();

        var statusCode = error.Type switch
        {
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Failure or ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

        return TypedResults.Problem(
            statusCode: statusCode,
            title: error.Code,
            detail: error.Description);
    }

    private static ValidationProblem ToValidationProblemResult(this IEnumerable<Error> errors)
    {
        var validationErrors = errors
            .GroupBy(error => error.Code)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(error => error.Description)
                    .ToArray());

        return TypedResults.ValidationProblem(validationErrors);
    }

    private static List<Error> ToErrorList(this Error error)
    {
        return [error];
    }
}
