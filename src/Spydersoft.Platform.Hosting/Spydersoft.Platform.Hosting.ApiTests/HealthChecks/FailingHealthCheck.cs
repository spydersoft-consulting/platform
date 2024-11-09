using Microsoft.Extensions.Diagnostics.HealthChecks;
using Spydersoft.Platform.Hosting.Attributes;

namespace Spydersoft.Platform.Hosting.ApiTests.HealthChecks
{
    [SpydersoftHealthCheck(nameof(FailingHealthCheck), failureStatus: HealthStatus.Unhealthy, "live")]
    public class FailingHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus));
        }
    }
}
