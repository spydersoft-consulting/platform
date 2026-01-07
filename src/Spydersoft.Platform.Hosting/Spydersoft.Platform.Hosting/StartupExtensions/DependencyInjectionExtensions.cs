using Microsoft.Extensions.DependencyInjection;
using Spydersoft.Platform.Attributes;
using System.Reflection;

namespace Spydersoft.Platform.Hosting.StartupExtensions;

/// <summary>
/// Class DependencyInjectionExtensions.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds services decorated with <see cref="DependencyInjectionAttribute"/> to the service collection.
    /// Automatically discovers and registers services across all loaded assemblies.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSpydersoftDecoratedServices(this IServiceCollection services)
    {
        var rankedList = new List<Tuple<DependencyInjectionAttribute, Type>>();

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var diTypes = assembly.GetTypes()
                .Where(t => t.GetCustomAttributes(typeof(DependencyInjectionAttribute), false).Length != 0)
                .ToArray();

            foreach (var diType in diTypes)
            {
                var attribute = diType.GetCustomAttribute<DependencyInjectionAttribute>();
                if (attribute != null)
                {
                    rankedList.Add(new Tuple<DependencyInjectionAttribute, Type>(attribute, diType));
                }
            }
        }

        foreach (var attrType in rankedList.OrderBy(x => x.Item1.Rank))
        {
            var serviceInterface = attrType.Item1.ServiceInterface;
            var lifetime = attrType.Item1.Lifetime;

            if (serviceInterface.IsAssignableFrom(attrType.Item2))
            {
                switch (lifetime)
                {
                    case LifetimeOfService.Scoped:
                        services.AddScoped(serviceInterface, attrType.Item2);
                        break;

                    case LifetimeOfService.Transient:
                        services.AddTransient(serviceInterface, attrType.Item2);
                        break;

                    case LifetimeOfService.Singleton:
                        services.AddSingleton(serviceInterface, attrType.Item2);
                        break;
                }
            }
        }

        return services;
    }
}