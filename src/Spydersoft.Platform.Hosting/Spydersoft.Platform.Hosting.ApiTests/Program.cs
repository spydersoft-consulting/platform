using Spydersoft.Platform.Hosting.ApiTests.Services;
using Spydersoft.Platform.Hosting.Options;
using Spydersoft.Platform.Hosting.StartupExtensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<ITestService, TestService>();

builder.AddSpydersoftSerilog();
builder.AddSpydersoftTelemetry(typeof(Program).Assembly);
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

public partial class Program { }
