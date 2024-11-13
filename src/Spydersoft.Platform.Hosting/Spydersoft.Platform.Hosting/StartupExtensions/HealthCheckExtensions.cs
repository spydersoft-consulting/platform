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
public static class HealthCheckExtensions
{
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
                .Where(t => t.GetCustomAttributes(typeof(SpydersoftHealthCheckAttribute), false).Length != 0)
                .ToArray();

            foreach (var healthCheckType in healthCheckTypes)
            {
                if (healthCheckType.GetCustomAttributes(typeof(SpydersoftHealthCheckAttribute), false)[0] is SpydersoftHealthCheckAttribute healthCheckAttribute)
                {
                    var genericAddCheckMethod = addCheckMethod.MakeGenericMethod(healthCheckType);
                    genericAddCheckMethod.Invoke(healthCheckBuilder, [healthCheckBuilder, healthCheckAttribute.Name, healthCheckAttribute.FailureStatus ?? HealthStatus.Unhealthy, healthCheckAttribute.Tags]);
                }
            }
        }

        return healthCheckOptions;
    }

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