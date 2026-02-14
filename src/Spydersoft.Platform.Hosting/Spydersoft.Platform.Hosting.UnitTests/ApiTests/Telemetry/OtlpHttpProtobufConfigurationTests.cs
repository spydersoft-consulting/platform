using Spydersoft.Platform.Hosting.HealthChecks;
using Spydersoft.Platform.Hosting.HealthChecks.Telemetry;
using System.Net;
using System.Text.Json;

namespace Spydersoft.Platform.Hosting.UnitTests.ApiTests.Telemetry;

public class OtlpHttpProtobufConfigurationTests : ApiTestBase
{
    public override string Environment => "OtlpHttpProtobuf";

    [Test]
    public async Task Startup_ConfigurationWithHttpProtobufProtocol_ShouldBeHealthy()
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

            // Verify OTLP endpoints
            Assert.That(telemetryData?.Log.Type, Is.EqualTo("otlp"));
            Assert.That(telemetryData?.Log.Otlp.Endpoint, Is.EqualTo("http://log.localhost:12345"));
            Assert.That(telemetryData?.Log.Otlp.Protocol, Is.EqualTo("http/protobuf"));

            Assert.That(telemetryData?.Metrics.Type, Is.EqualTo("otlp"));
            Assert.That(telemetryData?.Metrics.Otlp.Endpoint, Is.EqualTo("http://metrics.localhost:12345"));
            Assert.That(telemetryData?.Metrics.Otlp.Protocol, Is.EqualTo("http/protobuf"));

            Assert.That(telemetryData?.Trace.Type, Is.EqualTo("otlp"));
            Assert.That(telemetryData?.Trace.Otlp.Endpoint, Is.EqualTo("http://trace.localhost:12345"));
            Assert.That(telemetryData?.Trace.Otlp.Protocol, Is.EqualTo("http/protobuf"));

            // Verify that no headers are configured in this test
            Assert.That(telemetryData?.Log.Otlp.Headers, Is.Null.Or.Empty);
            Assert.That(telemetryData?.Metrics.Otlp.Headers, Is.Null.Or.Empty);
            Assert.That(telemetryData?.Trace.Otlp.Headers, Is.Null.Or.Empty);
        }
    }
}
