using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Spydersoft.Platform.Attributes;
using Spydersoft.Platform.Exceptions;
using Spydersoft.Platform.Hosting.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Spydersoft.Platform.Hosting.StartupExtensions
{
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
                        // If tagsToInclude is specified, only include options with the specified tags
                        if (!optionsAttribute.Tags.Any(t => tagsToInclude.Contains(t)))
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
}