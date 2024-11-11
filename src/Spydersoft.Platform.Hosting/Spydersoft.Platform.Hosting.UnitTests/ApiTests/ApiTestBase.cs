using System.Text.Json;

namespace Spydersoft.Platform.Hosting.UnitTests.ApiTests;
public abstract class ApiTestBase : IDisposable
{
    private UnitTestWebApplicationFactory _factory;
    private HttpClient _client;
    private bool _disposed = false;
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public abstract string Environment { get; }

    internal UnitTestWebApplicationFactory Factory => _factory;

    protected JsonSerializerOptions JsonOptions => _options;

    protected HttpClient Client => _client;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _factory = new UnitTestWebApplicationFactory(Environment);
    }

    [SetUp]
    public void Setup()
    {
        _client = _factory.CreateClient();
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