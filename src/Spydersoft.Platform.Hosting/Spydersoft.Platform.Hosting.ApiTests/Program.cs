using NeoSmart.Caching.Sqlite;
using Spydersoft.Platform.Hosting.ApiTests.Services;
using Spydersoft.Platform.Hosting.Options;
using Spydersoft.Platform.Hosting.StartupExtensions;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<ITestService, TestService>();

builder.AddSpydersoftTelemetry(typeof(Program).Assembly);
builder.AddSpydersoftSerilog(true);

var fusionCacheOptions = new FusionCacheConfigOptions();
builder.Configuration.GetSection(FusionCacheConfigOptions.SectionName).Bind(fusionCacheOptions);

if (fusionCacheOptions.Enabled && fusionCacheOptions.DistributedCacheType == CacheType.Memory)
{
    var cacheFileName = $".\\cache{DateTime.UtcNow:yyyyMMddHHmmss}.db";
    builder.Services.AddSqliteCache(options =>
    {
        options.CachePath = cacheFileName;
    });
    builder.AddSpydersoftFusionCache(builder =>
    {
        builder
        .WithSerializer(new FusionCacheSystemTextJsonSerializer())
        .WithDistributedCache(new NeoSmart.Caching.Sqlite.SqliteCache(new NeoSmart.Caching.Sqlite.SqliteCacheOptions()
        {
            CachePath = cacheFileName,

        }));
    });
}
else
{
    builder.AddSpydersoftFusionCache();
}
AppHealthCheckOptions healthCheckOptions = builder.AddSpydersoftHealthChecks();

builder.AddSpydersoftOptions(["root"]);
builder.AddSpydersoftOptions(["nested"], "MySection");

bool authInstalled = builder.AddSpydersoftIdentity();

builder.Services.AddSpydersoftDecoratedServices();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseAuthentication(authInstalled);
app.UseAuthorization(authInstalled);
app.UseSpydersoftHealthChecks(healthCheckOptions);

if (authInstalled)
{
    app.MapControllers().RequireAuthorization();
}
else
{
    app.MapControllers();
}

await app.RunAsync();

[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell",
    "S1118:Utility classes should not have public constructors",
    Justification = "Test Application, this is necessary to use WebApplicationFactory in tests.")]
public partial class Program { }
