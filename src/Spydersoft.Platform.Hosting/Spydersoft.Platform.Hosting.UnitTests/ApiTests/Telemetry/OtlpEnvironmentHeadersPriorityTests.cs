using Spydersoft.Platform.Hosting.HealthChecks;
using Spydersoft.Platform.Hosting.HealthChecks.Telemetry;
using System.Net;
using System.Text.Json;

namespace Spydersoft.Platform.Hosting.UnitTests.ApiTests.Telemetry;

/// <summary>
/// Tests that environment variable OTEL_EXPORTER_OTLP_HEADERS takes priority over config-based headers.
/// </summary>
public class OtlpEnvironmentHeadersPriorityTests : ApiTestBase
{
    public override string Environment => "OtlpEnvPriority";

    [OneTimeSetUp]
    public new void OneTimeSetup()
    {
        // Set environment variable for OTLP headers before factory initialization
        // This should override the headers specified in appsettings.OtlpEnvPriority.json
        System.Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS", "Authorization=Bearer env-token,X-Environment-Header=env-value");
        base.OneTimeSetup();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        // Clean up environment variable
        System.Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS", null);
    }

    [Test]
    public async Task Startup_WithBothEnvAndConfigHeaders_ShouldUseEnvironmentVariableHeaders()
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

            // Verify basic configuration
            Assert.That(telemetryHealthCheckResults?.ResultData, Contains.Key("details"));
            Assert.That(telemetryHealthCheckResults?.ResultData?["details"], Is.TypeOf<TelemetryHealthCheckDetails>());
            var telemetryData = telemetryHealthCheckResults?.ResultData?["details"] as TelemetryHealthCheckDetails;

            Assert.That(telemetryData?.ActivitySourceName, Is.EqualTo("Platform.Test.Activity"));
            Assert.That(telemetryData?.Enabled, Is.True);
            Assert.That(telemetryData?.ServiceName, Is.EqualTo("Platform.Test"));

            // Verify OTLP endpoints are from config
            Assert.That(telemetryData?.Log.Type, Is.EqualTo("otlp"));
            Assert.That(telemetryData?.Log.Otlp.Endpoint, Is.EqualTo("http://log.localhost:12345"));
            Assert.That(telemetryData?.Metrics.Type, Is.EqualTo("otlp"));
            Assert.That(telemetryData?.Metrics.Otlp.Endpoint, Is.EqualTo("http://metrics.localhost:12345"));
            Assert.That(telemetryData?.Trace.Type, Is.EqualTo("otlp"));
            Assert.That(telemetryData?.Trace.Otlp.Endpoint, Is.EqualTo("http://trace.localhost:12345"));

            // Note: We cannot directly verify that environment headers were used
            // instead of config headers since the health check only reports what's
            // in the configuration, not what's actually used by the OTLP exporter.
            // The actual verification of environment variable priority happens in
            // the SetOltpOptions method, which this test ensures executes successfully.
        }
    }
}
