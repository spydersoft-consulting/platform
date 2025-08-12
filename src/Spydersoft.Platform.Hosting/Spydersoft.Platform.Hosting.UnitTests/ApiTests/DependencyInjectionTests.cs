
using Spydersoft.Platform.Hosting.ApiTests.Services;
using System.Net;
using System.Text.Json;

namespace Spydersoft.Platform.Hosting.UnitTests.ApiTests;

public class DependencyInjectionTests : ApiTestBase
{
    public override string Environment => "Production";

    [Test]
    public async Task VerifyScope()
    {
        var firstCall = await Client.GetAsync("service");
        var secondCall = await Client.GetAsync("service");

        var firstString = await firstCall.Content.ReadAsStringAsync();
        var secondString = await secondCall.Content.ReadAsStringAsync();

        JsonSerializerOptions options = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var firstResult = JsonSerializer.Deserialize<List<ServiceInfo>>(firstString, options);
        var secondResult = JsonSerializer.Deserialize<List<ServiceInfo>>(secondString, options);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(firstCall.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(secondCall.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Assert.That(firstResult, Is.Not.Null);
            Assert.That(secondResult, Is.Not.Null);

            ServiceInfo? call1Info1 = firstResult?[0];
            ServiceInfo? call1Info2 = firstResult?[1];

            ServiceInfo? call2Info1 = secondResult?[0];
            ServiceInfo? call2Info2 = secondResult?[1];

            // Verify transient IDs are all different
            Assert.That(call1Info1?.TransientId, Is.Not.EqualTo(call1Info2?.TransientId));
            Assert.That(call1Info1?.TransientId, Is.Not.EqualTo(call2Info1?.TransientId));
            Assert.That(call2Info1?.TransientId, Is.Not.EqualTo(call2Info2?.TransientId));

            // Verify scoped IDs are the same for every call
            Assert.That(call1Info1?.ScopedId, Is.EqualTo(call1Info2?.ScopedId));
            Assert.That(call2Info1?.ScopedId, Is.EqualTo(call2Info2?.ScopedId));

            // Verify Scope IDs are different between calls
            Assert.That(call1Info1?.ScopedId, Is.Not.EqualTo(call2Info1?.ScopedId));

            // Verify Singletons are all the same
            Assert.That(call1Info1?.SingletonId, Is.EqualTo(call1Info2?.SingletonId));
            Assert.That(call1Info2?.SingletonId, Is.EqualTo(call2Info1?.SingletonId));
            Assert.That(call2Info1?.SingletonId, Is.EqualTo(call2Info2?.SingletonId));
        }
    }
}
