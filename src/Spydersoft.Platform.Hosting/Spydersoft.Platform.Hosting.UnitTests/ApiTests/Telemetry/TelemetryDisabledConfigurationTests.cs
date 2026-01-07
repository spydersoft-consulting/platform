using Spydersoft.Platform.Hosting.HealthChecks;
using Spydersoft.Platform.Hosting.HealthChecks.Telemetry;
using System.Net;
using System.Text.Json;

namespace Spydersoft.Platform.Hosting.UnitTests.ApiTests.Telemetry;

public class TelemetryDisabledConfigurationTests : ApiTestBase
{
    public override string Environment => "TelemetryDisabled";

    [Test]
    public async Task Startup_TelemetryDisabled_ShouldShowDisabledStatus()
    {
        var result = await Client.GetAsync($"startup");

        using var jsonResult = JsonDocument.Parse(await result.Content.ReadAsStringAsync());

        var telemetryNode = jsonResult.RootElement;

        var details = telemetryNode.Deserialize<HealthCheckResponseResult>(
                JsonOptions
        );

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(details, Is.Not.Null);
            // When telemetry is disabled, overall status should be Degraded
            Assert.That(details?.Status, Is.EqualTo("Degraded"));
            Assert.That(details?.Results, Contains.Key(nameof(TelemetryHealthCheck)));

            var telemetryHealthCheckResults = details?.Results?[nameof(TelemetryHealthCheck)];
            Assert.That(telemetryHealthCheckResults, Is.Not.Null);
            // Telemetry health check status should be Degraded when disabled
            Assert.That(telemetryHealthCheckResults?.Status, Is.EqualTo("Degraded"));
            Assert.That(telemetryHealthCheckResults?.ResultData, Contains.Key("details"));

            Assert.That(telemetryHealthCheckResults?.ResultData?["details"], Is.TypeOf<TelemetryHealthCheckDetails>());
            var telemetryData = telemetryHealthCheckResults?.ResultData?["details"] as TelemetryHealthCheckDetails;

            // Verify telemetry is disabled
            Assert.That(telemetryData?.Enabled, Is.False);
            
            // When disabled, the providers should not be present
            Assert.That(telemetryData?.TracePresent, Is.False);
            Assert.That(telemetryData?.MetricsPresent, Is.False);
            Assert.That(telemetryData?.LogPresent, Is.False);
        }
    }
}
