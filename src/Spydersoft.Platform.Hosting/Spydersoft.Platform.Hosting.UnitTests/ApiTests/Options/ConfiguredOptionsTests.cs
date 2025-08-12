using Spydersoft.Platform.Hosting.ApiTests.OptionsTests;
using Spydersoft.Platform.Hosting.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Spydersoft.Platform.Hosting.UnitTests.ApiTests.Options;

public class ConfiguredOptionsTests : ApiTestBase
{
    public override string Environment => "Options";

    [Test]
    public async Task RootOptions()
    {
        var result = await Client.GetAsync($"options/root");

        using var jsonResult = JsonDocument.Parse(await result.Content.ReadAsStringAsync());

        var telemetryNode = jsonResult.RootElement;

        var details = telemetryNode.Deserialize<RootOptionSection>(
                JsonOptions
        );
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(details, Is.Not.Null);
            Assert.That(details?.Option1, Is.EqualTo("Value1FromOptions"));
            Assert.That(details?.Option2, Is.EqualTo("Value2FromOptions"));
        }
    }

    [Test]
    public async Task NestedOptions()
    {
        var result = await Client.GetAsync($"options/nested");

        using var jsonResult = JsonDocument.Parse(await result.Content.ReadAsStringAsync());

        var telemetryNode = jsonResult.RootElement;

        var details = telemetryNode.Deserialize<NestedOptionSection>(
                JsonOptions
        );
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(details, Is.Not.Null);
            Assert.That(details?.NestedOption1, Is.EqualTo("NestedValue1FromOptions"));
            Assert.That(details?.NestedOption2, Is.EqualTo("NestedValue2FromOptions"));
        }
    }

    [Test]
    public async Task NotLoadedOptions()
    {
        var result = await Client.GetAsync($"options/notloaded");

        using var jsonResult = JsonDocument.Parse(await result.Content.ReadAsStringAsync());

        var telemetryNode = jsonResult.RootElement;

        var details = telemetryNode.Deserialize<TaggedNotLoadedOptions>(
                JsonOptions
        );
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(details, Is.Not.Null);
            Assert.That(details?.NotLoadedOption1, Is.EqualTo("NotLoadedOption1"));
        }
    }
}