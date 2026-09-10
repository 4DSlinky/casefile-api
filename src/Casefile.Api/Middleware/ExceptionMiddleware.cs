using Casefile.Domain;

namespace Casefile.Api.Middleware;

public sealed class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await WriteAsync(context, ex);
        }
    }

    private async Task WriteAsync(HttpContext context, Exception ex)
    {
        var (status, title) = ex switch
        {
            CaseValidationException => (StatusCodes.Status400BadRequest, "Validation error"),
            CaseNotFoundException => (StatusCodes.Status404NotFound, "Not found"),
            CaseConflictException => (StatusCodes.Status409Conflict, "Conflict"),
            _ => (StatusCodes.Status500InternalServerError, "Unexpected error")
        };

        if (status == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(ex, "Unhandled exception");
        }
        else
        {
            logger.LogInformation(ex, "Handled domain exception ({Title})", title);
        }

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(new
        {
            title,
            status,
            detail = status == StatusCodes.Status500InternalServerError
                ? "An unexpected error occurred."
                : ex.Message
        });
    }
}
