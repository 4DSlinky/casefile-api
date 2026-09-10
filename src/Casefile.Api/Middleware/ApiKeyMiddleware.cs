namespace Casefile.Api.Middleware;

/// <summary>
/// Demo auth only. A real system would use SSO / JWT. Health and OpenAPI stay public.
/// </summary>
public sealed class ApiKeyMiddleware(RequestDelegate next, IConfiguration config, ILogger<ApiKeyMiddleware> logger)
{
    public const string HeaderName = "X-Api-Key";

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        if (IsAnonymous(path))
        {
            await next(context);
            return;
        }

        var expected = config["ApiKey"];
        if (string.IsNullOrWhiteSpace(expected))
        {
            logger.LogError("ApiKey is not configured");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { title = "Server misconfigured", detail = "ApiKey is missing." });
            return;
        }

        if (!context.Request.Headers.TryGetValue(HeaderName, out var provided) ||
            !string.Equals(provided.ToString(), expected, StringComparison.Ordinal))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { title = "Unauthorized", detail = $"Missing or invalid {HeaderName}." });
            return;
        }

        await next(context);
    }

    private static bool IsAnonymous(string path)
    {
        return path.StartsWith("/health", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase)
            || path.Equals("/favicon.ico", StringComparison.OrdinalIgnoreCase);
    }
}
