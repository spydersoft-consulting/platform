using Spydersoft.Platform.Hosting.HealthChecks;
using Spydersoft.Platform.Hosting.HealthChecks.Telemetry;
using System.Net;
using System.Text.Json;

namespace Spydersoft.Platform.Hosting.UnitTests.ApiTests.Telemetry;

public class ExponentialHistogramConfigurationTests : ApiTestBase
{
    public override string Environment => "ExponentialHistogram";

    [Test]
    public async Task Startup_ExponentialHistogramConfiguration_ShouldBeHealthy()
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
            Assert.That(details?.Status, Is.EqualTo("Healthy"));
            Assert.That(details?.Results, Contains.Key(nameof(TelemetryHealthCheck)));

            var telemetryHealthCheckResults = details?.Results?[nameof(TelemetryHealthCheck)];
            Assert.That(telemetryHealthCheckResults, Is.Not.Null);
            Assert.That(telemetryHealthCheckResults?.Status, Is.EqualTo("Healthy"));
            Assert.That(telemetryHealthCheckResults?.ResultData, Contains.Key("details"));

            Assert.That(telemetryHealthCheckResults?.ResultData?["details"], Is.TypeOf<TelemetryHealthCheckDetails>());
            var telemetryData = telemetryHealthCheckResults?.ResultData?["details"] as TelemetryHealthCheckDetails;

            // Verify histogram aggregation is set to exponential
            Assert.That(telemetryData?.Metrics.HistogramAggregation, Is.EqualTo("exponential"));
            Assert.That(telemetryData?.Metrics.Type, Is.EqualTo("console"));
            Assert.That(telemetryData?.Enabled, Is.True);
        }
    }
}
