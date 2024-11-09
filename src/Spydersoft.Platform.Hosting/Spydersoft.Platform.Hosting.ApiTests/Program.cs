using Spydersoft.Platform.Hosting.ApiTests.Services;
using Spydersoft.Platform.Hosting.StartupExtensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<ITestService, TestService>();

var healthCheckOptions = builder.AddSpydersoftHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseSpydersoftHealthChecks(healthCheckOptions);
app.MapControllers();

await app.RunAsync();
