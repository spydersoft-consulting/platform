using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Spydersoft.Platform.Attributes;
using Spydersoft.Platform.Exceptions;
using System.Reflection;

namespace Spydersoft.Platform.Hosting.StartupExtensions;

public static class OptionsExtensions
{
    public static void AddSpydersoftOptions(this WebApplicationBuilder appBuilder, IEnumerable<string> tagsToInclude, string? sectionPrefix = null)
    {
        MethodInfo addCheckMethod = Array.Find(typeof(OptionsConfigurationServiceCollectionExtensions).GetMethods(),
            m => m.Name == "Configure" && m.GetParameters().Length == 2 && m.IsGenericMethod) ?? throw new ConfigurationException("Unable to find Configure method on OptionsConfigurationServiceCollectionExtensions");

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var optionTypes = assembly.GetTypes()
                .Where(t => t.GetCustomAttributes(typeof(SpydersoftOptionsAttribute), false).Length != 0)
                .ToArray();

            foreach (var optionType in optionTypes)
            {
                if (optionType.GetCustomAttributes(typeof(SpydersoftOptionsAttribute), false)[0] is SpydersoftOptionsAttribute optionsAttribute)
                {
                    // Options with no tags are always added
                    // If the option has tags, only add options which match tags to include
                    if (optionsAttribute.Tags.Count() > 0 && !optionsAttribute.Tags.Any(t => tagsToInclude.Contains(t)))
                    {
                        continue;
                    }

                    var sectionName = !string.IsNullOrEmpty(sectionPrefix) ? $"{sectionPrefix}:{optionsAttribute.SectionName}" : optionsAttribute.SectionName;

                    var sectionConfig = appBuilder.Configuration.GetSection(sectionName);

                    var genericAddCheckMethod = addCheckMethod.MakeGenericMethod(optionType);

                    genericAddCheckMethod.Invoke(appBuilder.Services, [appBuilder.Services, sectionConfig]);
                }
            }
        }
    }
}