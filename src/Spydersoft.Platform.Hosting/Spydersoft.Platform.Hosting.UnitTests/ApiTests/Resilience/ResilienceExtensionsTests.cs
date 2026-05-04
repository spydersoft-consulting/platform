using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Spydersoft.Platform.Hosting.Options;
using Spydersoft.Platform.Hosting.StartupExtensions;

namespace Spydersoft.Platform.Hosting.UnitTests.ApiTests.Resilience;

public class ResilienceExtensionsTests
{
    [Test]
    public async Task AddSpydersoftResilience_WithConfigureCallback_InvokesCallback()
    {
        var builder = WebApplication.CreateBuilder();
        var callbackInvoked = false;

        builder.AddSpydersoftResilience(options => { callbackInvoked = true; });

        Assert.That(callbackInvoked, Is.True);
        await builder.Build().DisposeAsync();
    }

    [Test]
    public async Task AddSpydersoftResilience_WithConfigureStandardCallback_InvokesCallbackOnClientCreation()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddHttpClient();
        var standardCallbackInvoked = false;

        builder.AddSpydersoftResilience(configureStandard: opt => { standardCallbackInvoked = true; });

        await using var app = builder.Build();
        var factory = app.Services.GetRequiredService<IHttpClientFactory>();
        using var client = factory.CreateClient();

        Assert.That(standardCallbackInvoked, Is.True);
    }

    [Test]
    public async Task AddSpydersoftResilience_WhenDisabledViaCallback_StandardCallbackNotInvoked()
    {
        var builder = WebApplication.CreateBuilder();
        var standardCallbackInvoked = false;

        builder.AddSpydersoftResilience(
            configure: options => { options.Enabled = false; },
            configureStandard: opt => { standardCallbackInvoked = true; });

        Assert.That(standardCallbackInvoked, Is.False);
        await builder.Build().DisposeAsync();
    }
}
