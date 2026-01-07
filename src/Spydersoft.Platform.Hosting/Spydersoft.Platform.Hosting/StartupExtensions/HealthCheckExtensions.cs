using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Spydersoft.Platform.Attributes;
using Spydersoft.Platform.Exceptions;
using Spydersoft.Platform.Hosting.HealthChecks;
using Spydersoft.Platform.Hosting.Options;
using System.Reflection;

namespace Spydersoft.Platform.Hosting.StartupExtensions;

/// <summary>
/// Extension methods for configuring health checks in ASP.NET Core applications.
/// </summary>
public static class HealthCheckExtensions
{
    /// <summary>
    /// Adds Spydersoft health checks to the application.
    /// Automatically discovers and registers all health checks decorated with <see cref="HealthCheckAttribute"/>.
    /// </summary>
    /// <param name="appBuilder">The web application builder.</param>
    /// <returns>The health check configuration options.</returns>
    /// <exception cref="ConfigurationException">Thrown when required reflection methods cannot be found.</exception>
    public static AppHealthCheckOptions AddSpydersoftHealthChecks(this WebApplicationBuilder appBuilder)
    {
        var healthCheckOptions = new AppHealthCheckOptions();
        appBuilder.Configuration.GetSection(AppHealthCheckOptions.SectionName).Bind(healthCheckOptions);

        if (!healthCheckOptions.Enabled)
        {
            return healthCheckOptions;
        }

        var healthCheckBuilder = appBuilder.Services.AddHealthChecks()
            .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Ping Success"), tags: ["live"]);

        MethodInfo addCheckMethod = Array.Find(typeof(HealthChecksBuilderAddCheckExtensions).GetMethods(),
            m => m.Name == "AddCheck" && m.GetParameters().Length == 4 && m.IsGenericMethod) ?? throw new ConfigurationException("Unable to find AddCheck method on HealthChecksBuilderAddCheckExtensions");

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var healthCheckTypes = assembly.GetTypes()
                .Where(t => t.GetCustomAttributes(typeof(HealthCheckAttribute), false).Length != 0)
                .ToArray();

            foreach (var healthCheckType in healthCheckTypes)
            {
                if (healthCheckType.GetCustomAttributes(typeof(HealthCheckAttribute), false)[0] is HealthCheckAttribute healthCheckAttribute)
                {
                    var genericAddCheckMethod = addCheckMethod.MakeGenericMethod(healthCheckType);
                    genericAddCheckMethod.Invoke(healthCheckBuilder, [healthCheckBuilder, healthCheckAttribute.Name, healthCheckAttribute.FailureStatus ?? HealthStatus.Unhealthy, healthCheckAttribute.Tags]);
                }
            }
        }

        return healthCheckOptions;
    }

    /// <summary>
    /// Configures health check endpoints for Kubernetes-style probes.
    /// Creates /readyz, /livez, /startup, and /configuration endpoints.
    /// </summary>
    /// <param name="appBuilder">The application builder.</param>
    /// <param name="options">The health check configuration options.</param>
    /// <returns>The application builder for chaining.</returns>
    public static IApplicationBuilder UseSpydersoftHealthChecks(this IApplicationBuilder appBuilder, AppHealthCheckOptions options)
    {
        if (!options.Enabled)
        {
            return appBuilder;
        }
        appBuilder.UseHealthChecks("/readyz", new HealthCheckOptions
        {
            Predicate = (check) => options.ReadyTagsList().Exists(tag => check.Tags.Contains(tag)),
            ResponseWriter = HealthCheckWriter.WriteResponse
        });

        appBuilder.UseHealthChecks("/livez", new HealthCheckOptions
        {
            Predicate = (check) => options.LiveTagsList().Exists(tag => check.Tags.Contains(tag)),
            ResponseWriter = HealthCheckWriter.WriteResponse
        });

        appBuilder.UseHealthChecks("/startup", new HealthCheckOptions
        {
            Predicate = (check) => options.StartupTagsList().Exists(tag => check.Tags.Contains(tag)),
            ResponseWriter = HealthCheckWriter.WriteResponse
        });

        appBuilder.UseHealthChecks("/configuration", new HealthCheckOptions
        {
            Predicate = (check) => check.Tags.Contains("configuration"),
            ResponseWriter = HealthCheckWriter.WriteResponse
        });

        return appBuilder;
    }
}