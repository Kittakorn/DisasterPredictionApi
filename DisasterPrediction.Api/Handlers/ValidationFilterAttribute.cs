using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DisasterPrediction.Api.Handlers;

public class ValidationFilterAttribute : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid)
            return;

        var errorDetails = context
            .ModelState
            .Where(e => e.Value.Errors.Any())
            .Select(e => new
            {
                Property = e.Key,
                Errors = e.Value.Errors.Select(er => er.ErrorMessage)
            })
            .ToList();

        var errors = new ProblemDetails
        {
            Status = 400,
            Title = "Bad Request",
            Detail = "One or more validation errors occurred",
            Extensions = new Dictionary<string, object>
                {
                    { "errors", errorDetails }
                }
        };

        context.Result = new BadRequestObjectResult(errors);
    }
    public void OnActionExecuted(ActionExecutedContext context)
    {

    }
}
