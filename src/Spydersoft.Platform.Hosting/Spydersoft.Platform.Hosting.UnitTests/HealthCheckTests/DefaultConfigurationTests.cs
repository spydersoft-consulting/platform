using Spydersoft.Platform.Hosting.HealthChecks;
using Spydersoft.Platform.Hosting.UnitTests.Helpers;
using System.Net;
using System.Text.Json;

namespace Spydersoft.Platform.Hosting.UnitTests.HealthCheckTests;
public class DefaultConfigurationTests : IDisposable
{
    private UnitTestWebApplicationFactory _factory;
    private HttpClient _client;
    private bool _disposed = false;
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _factory = new UnitTestWebApplicationFactory("Production");
    }

    [SetUp]
    public void Setup()
    {
        _client = _factory.CreateClient();
    }

    [Test]
    public async Task Livez_ShouldReturn_Http200()
    {
        var result = await _client.GetAsync($"livez");

        using var jsonResult = JsonDocument.Parse(await result.Content.ReadAsStringAsync());

        var telemetryNode = jsonResult.RootElement;

        var details = JsonSerializer.Deserialize<HealthCheckResponseResult>(

                telemetryNode,
                _options
        );
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }


    [Test]
    public async Task Startup_ShouldReturn_Http200()
    {
        var result = await _client.GetAsync($"startup");

        using var jsonResult = JsonDocument.Parse(await result.Content.ReadAsStringAsync());

        var telemetryNode = jsonResult.RootElement;

        var details = JsonSerializer.Deserialize<HealthCheckResponseResult>(

                telemetryNode,
                _options
        );
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _client?.Dispose();
                _factory?.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}