using Microsoft.Extensions.Diagnostics.HealthChecks;
using Photography.Application.Storage;

namespace Photography.Web.Middleware;

/// <summary>
/// Health check that verifies the configured storage backend is reachable by issuing
/// a cheap HEAD-style call against a sentinel key. For the local filesystem provider
/// this confirms the root directory exists and is writeable.
/// </summary>
public sealed class StorageHealthCheck(IStorageService storage) : IHealthCheck
{
    private const string Probe = "_health/ping.txt";

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var stream = new MemoryStream("ok"u8.ToArray());
            await storage.UploadAsync(Probe, stream, "text/plain", cancellationToken);
            await storage.DeleteAsync(Probe, cancellationToken);
            return HealthCheckResult.Healthy("Storage write/delete probe succeeded");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Storage probe failed", ex);
        }
    }
}
