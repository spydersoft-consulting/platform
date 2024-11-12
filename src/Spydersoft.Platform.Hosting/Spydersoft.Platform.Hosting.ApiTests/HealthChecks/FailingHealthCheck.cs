using Microsoft.Extensions.Diagnostics.HealthChecks;
using Spydersoft.Platform.Attributes;

namespace Spydersoft.Platform.Hosting.ApiTests.HealthChecks
{
    [SpydersoftHealthCheck(nameof(FailingHealthCheck), failureStatus: HealthStatus.Unhealthy, "fails")]
    public class FailingHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus));
        }
    }
}
