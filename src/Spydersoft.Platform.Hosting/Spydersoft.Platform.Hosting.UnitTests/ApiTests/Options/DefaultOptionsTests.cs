using Spydersoft.Platform.Hosting.ApiTests.OptionsTests;
using System.Net;
using System.Text.Json;

namespace Spydersoft.Platform.Hosting.UnitTests.ApiTests.Options
{
    public class DefaultOptionsTests : ApiTestBase
    {
        public override string Environment => "Options2";

        [Test]
        public async Task RootOptions()
        {
            var result = await Client.GetAsync($"options/root");

            using var jsonResult = JsonDocument.Parse(await result.Content.ReadAsStringAsync());

            var telemetryNode = jsonResult.RootElement;

            var details = telemetryNode.Deserialize<RootOptionSection>(
                    JsonOptions
            );
            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(details, Is.Not.Null);
                Assert.That(details?.Option1, Is.EqualTo("Option1"));
                Assert.That(details?.Option2, Is.EqualTo("Option2"));
            });
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
            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(details, Is.Not.Null);
                Assert.That(details?.NestedOption1, Is.EqualTo("NestedOption1"));
                Assert.That(details?.NestedOption2, Is.EqualTo("NestedOption2"));
            });
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
            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(details, Is.Not.Null);
                Assert.That(details?.NotLoadedOption1, Is.EqualTo("NotLoadedOption1"));
            });
        }
    }
}