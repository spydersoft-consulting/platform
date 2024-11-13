using Spydersoft.Platform.Hosting.HealthChecks;
using Spydersoft.Platform.Hosting.HealthChecks.Telemetry;
using System.Net;
using System.Text.Json;

namespace Spydersoft.Platform.Hosting.UnitTests.ApiTests.Telemetry;
public class ZipPromConfigurationTests : ApiTestBase
{
    public override string Environment => "ZipProm";

    [Test]
    public async Task Startup_ConfigurationCheck()
    {
        var result = await Client.GetAsync($"startup");

        using var jsonResult = JsonDocument.Parse(await result.Content.ReadAsStringAsync());

        var telemetryNode = jsonResult.RootElement;

        var details = telemetryNode.Deserialize<HealthCheckResponseResult>(
                JsonOptions
        );

        Assert.Multiple(() =>
        {
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(details, Is.Not.Null);
            Assert.That(details?.Status, Is.EqualTo("Healthy"));
            Assert.That(details?.Results, Contains.Key(nameof(TelemetryHealthCheck)));

            var telemeteryHealthCheckResults = details?.Results?[nameof(TelemetryHealthCheck)];
            Assert.That(telemeteryHealthCheckResults, Is.Not.Null);
            Assert.That(telemeteryHealthCheckResults?.Status, Is.EqualTo("Healthy"));
            Assert.That(telemeteryHealthCheckResults?.ResultData, Contains.Key("details"));

            Assert.That(telemeteryHealthCheckResults?.ResultData?["details"], Is.TypeOf<TelemetryHealthCheckDetails>());
            var telemetryData = telemeteryHealthCheckResults?.ResultData?["details"] as TelemetryHealthCheckDetails;

            Assert.That(telemetryData?.ActivitySourceName, Is.EqualTo("Platform.Test.Activity"));
            Assert.That(telemetryData?.Enabled, Is.True);
            Assert.That(telemetryData?.Metrics.HistogramAggregation, Is.EqualTo("exponential"));
            Assert.That(telemetryData?.Log.Type, Is.EqualTo("console"));
            Assert.That(telemetryData?.LogPresent, Is.True);
            Assert.That(telemetryData?.MeterName, Is.EqualTo("Platform.Test.Meter"));
            Assert.That(telemetryData?.Metrics.Type, Is.EqualTo("prometheus"));
            Assert.That(telemetryData?.MetricsPresent, Is.True);
            Assert.That(telemetryData?.ServiceName, Is.EqualTo("Platform.Test"));
            Assert.That(telemetryData?.Trace.Type, Is.EqualTo("zipkin"));
            Assert.That(telemetryData?.TracePresent, Is.True);
        });
    }
}