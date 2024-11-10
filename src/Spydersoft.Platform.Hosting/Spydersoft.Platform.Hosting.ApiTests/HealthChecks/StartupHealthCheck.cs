using Microsoft.Extensions.Diagnostics.HealthChecks;
using Spydersoft.Platform.Hosting.ApiTests.Services;
using Spydersoft.Platform.Hosting.Attributes;

namespace Spydersoft.Platform.Hosting.ApiTests.HealthChecks
{
    [SpydersoftHealthCheck(nameof(StartupHealthCheck), failureStatus: HealthStatus.Degraded, "ready")]
    public class StartupHealthCheck(ITestService myService) : IHealthCheck
    {
        private readonly ITestService _myService = myService;

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            if (_myService.IsRunning())
            {
                var versionDictionary = new Dictionary<string, string>()
            {
                { "ApiVersionNumber", "1.2.3.4" },
                { "WebServerName", Environment.MachineName },
            };

                return Task.FromResult(new HealthCheckResult(HealthStatus.Healthy,
                    "Works ok",
                    null,
                    new Dictionary<string, object> { { "details", versionDictionary }, { "errors", new { } } }));
            }
            else
            {
                return Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus));
            }
        }
    }
}
