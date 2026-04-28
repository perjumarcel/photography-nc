using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Photography.Application.Common.Behaviors;

/// <summary>
/// Logs request execution time and warns when it exceeds 500ms.
/// </summary>
public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private const int SlowRequestThresholdMs = 500;
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var name = typeof(TRequest).Name;
        var sw = Stopwatch.StartNew();
        try
        {
            return await next();
        }
        finally
        {
            sw.Stop();
            if (sw.ElapsedMilliseconds > SlowRequestThresholdMs)
                _logger.LogWarning("Slow handler {Request} took {ElapsedMs}ms", name, sw.ElapsedMilliseconds);
            else
                _logger.LogDebug("Handler {Request} took {ElapsedMs}ms", name, sw.ElapsedMilliseconds);
        }
    }
}
