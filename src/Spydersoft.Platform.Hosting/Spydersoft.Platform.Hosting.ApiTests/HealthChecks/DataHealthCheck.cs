using Microsoft.Extensions.Diagnostics.HealthChecks;
using Spydersoft.Platform.Hosting.Attributes;

namespace Spydersoft.Platform.Hosting.ApiTests.HealthChecks
{
    [SpydersoftHealthCheck("DataHealthCheck", failureStatus: HealthStatus.Unhealthy, "ready")]
    public class DataHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var versionDictionary = new Dictionary<string, string>()
            {
                { "ApiVersionNumber", "1.2.3.4" },
                { "WebServerName", Environment.MachineName },
            };

            return Task.FromResult(new HealthCheckResult(HealthStatus.Healthy,
                "Works Great",
                null,
                new Dictionary<string, object> { { "details", versionDictionary }, { "errors", new { } } }));
        }
    }
}
