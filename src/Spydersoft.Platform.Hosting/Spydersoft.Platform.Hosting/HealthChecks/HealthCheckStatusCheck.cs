using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Spydersoft.Platform.Attributes;
using Spydersoft.Platform.Hosting.Options;

namespace Spydersoft.Platform.Hosting.HealthChecks;

[HealthCheck(nameof(HealthCheckStatusCheck), HealthStatus.Unhealthy, "startup")]
internal class HealthCheckStatusCheck(IOptions<AppHealthCheckOptions> options) : IHealthCheck
{
    private readonly AppHealthCheckOptions _options = options.Value;

    public Task<Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult(HealthStatus.Healthy,
            "Health Check Options",
            null,
            new Dictionary<string, object> {
                    { "details", _options }
            }));
    }
}