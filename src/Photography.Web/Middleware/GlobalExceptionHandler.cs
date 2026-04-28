using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Photography.Web.Middleware;

/// <summary>
/// .NET 8+ <see cref="IExceptionHandler"/> producing RFC 7807 ProblemDetails for any
/// unhandled exception. Replaces hand-rolled try/catch in controllers and ensures we
/// never leak stack traces to API clients.
/// </summary>
public sealed class GlobalExceptionHandler(
    IHostEnvironment env,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception while processing {Method} {Path}",
            httpContext.Request.Method, httpContext.Request.Path);

        var (status, title) = exception switch
        {
            BadHttpRequestException => (StatusCodes.Status400BadRequest, "Bad request"),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Not found"),
            OperationCanceledException => (StatusCodes.Status499ClientClosedRequest, "Client closed request"),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred"),
        };

        var problem = new ProblemDetails
        {
            Type = $"https://httpstatuses.io/{status}",
            Status = status,
            Title = title,
            Detail = env.IsDevelopment() ? exception.Message : null,
            Instance = httpContext.Request.Path,
        };

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}

/// <summary>Status code reserved by Nginx but absent from <see cref="StatusCodes"/> in older runtimes.</summary>
internal static class StatusCodesExt
{
    public const int Status499ClientClosedRequest = 499;
}
