using Microsoft.AspNetCore.Mvc.Filters;
using Sphera.API.Shared.DTOs;
using System.ComponentModel.DataAnnotations;

namespace Sphera.API.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var failure = context.Exception switch
        {
            ValidationException => new FailureDTO(400, context.Exception.Message),
            UnauthorizedAccessException => new FailureDTO(401, context.Exception.Message),
            _ => new FailureDTO(500, context.Exception.Message)
        };

        var result = ResultDTO<object>.AsFailure(failure);

        context.Result = new Microsoft.AspNetCore.Mvc.ObjectResult(new
        {
            error = result.Failure?.Message,
            code = result.Failure?.Code,
            stackTrace = context.Exception.StackTrace
        })
        {
            StatusCode = MapStatusCode(result.Failure!)
        };
    }

    private static int MapStatusCode(FailureDTO failure) => failure.Code switch
    {
        404 => StatusCodes.Status404NotFound,
        400 => StatusCodes.Status400BadRequest,
        401 => StatusCodes.Status401Unauthorized,
        _ => StatusCodes.Status500InternalServerError
    };
}
