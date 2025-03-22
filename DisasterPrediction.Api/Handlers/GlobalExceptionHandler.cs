using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DisasterPrediction.Api.Handlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails();

        switch (exception)
        {
            case BadRequestException:
                problemDetails.Status = 400;
                problemDetails.Title = "Bad Request";
                problemDetails.Detail = exception.Message;
                _logger.LogWarning(exception.Message);
                break;
            default:
                problemDetails.Status = 500;
                problemDetails.Title = "Internal Server Error";
                problemDetails.Detail = "An unexpected error occurred.";
                _logger.LogError(exception, exception.Message);
                break;
        }

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
