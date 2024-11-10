using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Spydersoft.Platform.Hosting.UnitTests.Helpers;
internal class UnitTestWebApplicationFactory(string environment) : WebApplicationFactory<Program>
{
    private readonly string _environment = environment;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(_environment);
        builder.ConfigureAppConfiguration((host, ConfigurationBuilder) =>
        {


        });
    }
}