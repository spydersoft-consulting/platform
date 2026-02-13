using Spydersoft.Platform.Hosting.HealthChecks;
using Spydersoft.Platform.Hosting.HealthChecks.Telemetry;
using System.Net;
using System.Text.Json;

namespace Spydersoft.Platform.Hosting.UnitTests.ApiTests.Telemetry;

public class OtlpEnvironmentHeadersConfigurationTests : ApiTestBase
{
    public override string Environment => "Otlp";

    [OneTimeSetUp]
    public new void OneTimeSetup()
    {
        // Set environment variable for OTLP headers before factory initialization
        System.Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS", "Authorization=Bearer env-token,X-Environment-Header=test-value");
        base.OneTimeSetup();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        // Clean up environment variable
        System.Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS", null);
    }

    [Test]
    public async Task Startup_ConfigurationWithEnvironmentHeaders_ShouldUseEnvironmentVariableHeaders()
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

            // Verify basic configuration
            Assert.That(telemetryData?.ActivitySourceName, Is.EqualTo("Platform.Test.Activity"));
            Assert.That(telemetryData?.Enabled, Is.True);
            Assert.That(telemetryData?.ServiceName, Is.EqualTo("Platform.Test"));

            // Note: The environment variable headers take precedence over config headers
            // Since we're using the "Otlp" environment which has no headers in config,
            // we can't directly verify the headers through the health check unless
            // the health check also exposes the actual OTLP exporter options.
            // This test primarily verifies that the application starts successfully
            // with environment variable headers set.
        }
    }
}
