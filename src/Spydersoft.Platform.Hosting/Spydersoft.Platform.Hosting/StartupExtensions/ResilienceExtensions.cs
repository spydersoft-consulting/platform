using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Spydersoft.Platform.Hosting.Options;

namespace Spydersoft.Platform.Hosting.StartupExtensions;

/// <summary>
/// Extension methods for configuring HTTP client resilience in ASP.NET Core applications.
/// </summary>
public static class ResilienceExtensions
{
    /// <summary>
    /// Adds a standard resilience pipeline to all HTTP clients registered in the application.
    /// Applies retry, circuit breaker, and timeout policies sourced from configuration.
    /// </summary>
    /// <remarks>
    /// Configuration is read from the <c>Resilience</c> section of appsettings.json.
    /// Use <paramref name="configure"/> to override options programmatically before the pipeline
    /// is built, and <paramref name="configureStandard"/> to further tune the pipeline itself.
    /// </remarks>
    /// <param name="builder">The web application builder.</param>
    /// <param name="configure">Optional action to modify the top-level resilience options.</param>
    /// <param name="configureStandard">Optional action to further customize the standard pipeline options.</param>
    /// <returns>The web application builder for chaining.</returns>
    public static WebApplicationBuilder AddSpydersoftResilience(
        this WebApplicationBuilder builder,
        Action<ResilienceOptions>? configure = null,
        Action<HttpStandardResilienceOptions>? configureStandard = null)
    {
        var resilienceOptions = new ResilienceOptions();
        builder.Configuration.GetSection(ResilienceOptions.SectionName).Bind(resilienceOptions);
        configure?.Invoke(resilienceOptions);

        if (!resilienceOptions.Enabled)
        {
            return builder;
        }

        var standardOptions = resilienceOptions.Standard;

        builder.Services.ConfigureHttpClientDefaults(defaults =>
        {
            defaults.AddStandardResilienceHandler()
                .Configure(opt =>
                {
                    opt.TotalRequestTimeout = standardOptions.TotalRequestTimeout;
                    opt.Retry = standardOptions.Retry;
                    opt.CircuitBreaker = standardOptions.CircuitBreaker;
                    opt.AttemptTimeout = standardOptions.AttemptTimeout;
                    configureStandard?.Invoke(opt);
                });
        });

        return builder;
    }
}
