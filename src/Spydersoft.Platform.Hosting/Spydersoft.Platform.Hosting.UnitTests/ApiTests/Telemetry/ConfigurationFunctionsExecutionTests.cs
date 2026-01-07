using Spydersoft.Platform.Hosting.ApiTests.Models;
using Spydersoft.Platform.Hosting.HealthChecks;
using Spydersoft.Platform.Hosting.HealthChecks.Telemetry;
using System.Net;
using System.Text.Json;

namespace Spydersoft.Platform.Hosting.UnitTests.ApiTests.Telemetry;

public class ConfigurationFunctionsExecutionTests : ApiTestBase
{
    public override string Environment => "AdvancedTelemetryConfig";

    [Test]
    public async Task Startup_ConfigurationFunctionsExecution_ShouldBeHealthy()
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
        }
    }

    [Test]
    public async Task ConfigurationFunctions_AreExecuted()
    {
        // Just make a request to trigger the telemetry functions
		_ = await Client.GetAsync($"/service");

        var result = await Client.GetAsync($"/TelemetryFunctionsAccess");

        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var content = await result.Content.ReadAsStringAsync();

        var functionTrackerData = JsonSerializer.Deserialize<TestConfigurationFunctionTrackerData>(content, JsonOptions);
        Assert.That(functionTrackerData, Is.Not.Null);
        Assert.That(functionTrackerData!.AspNetFilterFunctionCalled, Is.GreaterThan(0), "AspNetFilterFunctionCalled should have been called.");
        Assert.That(functionTrackerData!.AspNetRequestEnrichActionCalled, Is.GreaterThan(0), "AspNetRequestEnrichAction should have been called.");
        Assert.That(functionTrackerData!.AspNetResponseEnrichActionCalled, Is.GreaterThan(0), "AspNetResponseEnrichAction should have been called.");
        Assert.That(functionTrackerData!.AspNetExceptionEnrichActionCalled, Is.EqualTo(0), "AspNetExceptionEnrichAction should not have been called.");
    }
}
