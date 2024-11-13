using Spydersoft.Platform.Hosting.ApiTests.Services;
using Spydersoft.Platform.Hosting.Options;
using Spydersoft.Platform.Hosting.StartupExtensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<ITestService, TestService>();

builder.AddSpydersoftTelemetry(typeof(Program).Assembly);
builder.AddSpydersoftSerilog(true);
AppHealthCheckOptions healthCheckOptions = builder.AddSpydersoftHealthChecks();

bool authInstalled = builder.AddSpydersoftIdentity();

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
