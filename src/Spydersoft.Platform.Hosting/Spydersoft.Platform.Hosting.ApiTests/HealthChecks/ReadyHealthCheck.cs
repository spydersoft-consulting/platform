﻿using Microsoft.Extensions.Diagnostics.HealthChecks;
using Spydersoft.Platform.Attributes;
using Spydersoft.Platform.Hosting.ApiTests.Services;

namespace Spydersoft.Platform.Hosting.ApiTests.HealthChecks
{
    [SpydersoftHealthCheck(nameof(ReadyHealthCheck), failureStatus: HealthStatus.Degraded, "ready")]
    public class ReadyHealthCheck(ITestService myService) : IHealthCheck
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
