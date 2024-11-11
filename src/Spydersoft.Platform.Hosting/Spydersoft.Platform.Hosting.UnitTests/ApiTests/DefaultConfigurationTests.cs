using Spydersoft.Platform.Hosting.ApiTests.HealthChecks;
using Spydersoft.Platform.Hosting.HealthChecks;
using Spydersoft.Platform.Hosting.HealthChecks.Telemetry;
using System.Net;
using System.Text.Json;

namespace Spydersoft.Platform.Hosting.UnitTests.ApiTests;
public class DefaultConfigurationTests : ApiTestBase
{
    public override string Environment => "Production";

    [Test]
    public async Task Livez_ShouldReturn_Http200()
    {
        var result = await Client.GetAsync($"livez");

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
            Assert.That(details?.Results, Has.One.With.Property("Key").EqualTo("self"));
        });
    }

    [Test]
    public async Task Readyz_ShouldReturn_Http200()
    {
        var result = await Client.GetAsync($"readyz");

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
            Assert.That(details?.Results, Contains.Key(nameof(ReadyHealthCheck)));

            var readyHealthCheckResults = details?.Results?[nameof(ReadyHealthCheck)];
            Assert.That(readyHealthCheckResults, Is.Not.Null);
            Assert.That(readyHealthCheckResults?.Status, Is.EqualTo("Healthy"));
            Assert.That(readyHealthCheckResults?.ResultData, Contains.Key("details"));

            var readyDetails = readyHealthCheckResults?.ResultData?["details"];
            Assert.That(readyDetails, Is.TypeOf<Dictionary<string, string>>());
            Assert.That(readyDetails, Contains.Key("ApiVersionNumber"));
        });
    }

    [Test]
    public async Task Startup_ShouldReturn_Http200()
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

            Assert.That(telemetryData?.ActivitySourceName, Is.EqualTo("Spydersoft.Otel.Activity"));
            Assert.That(telemetryData?.Enabled, Is.True);
            Assert.That(telemetryData?.HistogramAggregation, Is.Empty);
            Assert.That(telemetryData?.LogExporter, Is.EqualTo("console"));
            Assert.That(telemetryData?.LogPresent, Is.True);
            Assert.That(telemetryData?.MeterName, Is.EqualTo("Spydersoft.Otel.Meter"));
            Assert.That(telemetryData?.MetricsExporter, Is.EqualTo("console"));
            Assert.That(telemetryData?.MetricsPresent, Is.True);
            Assert.That(telemetryData?.Otlp.Endpoint, Is.Null);
            Assert.That(telemetryData?.ServiceName, Is.EqualTo("spydersoft-otel-service"));
            Assert.That(telemetryData?.TraceExporter, Is.EqualTo("console"));
            Assert.That(telemetryData?.TracePresent, Is.True);
        });
    }
}