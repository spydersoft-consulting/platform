using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spydersoft.Platform.Exceptions;
using Spydersoft.Platform.Hosting.Options;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Backplane.StackExchangeRedis;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

namespace Spydersoft.Platform.Hosting.StartupExtensions;

public static class FusionCacheExtensions
{
    /// <summary>
    /// Adds the aspire fusion cache.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>WebApplicationBuilder.</returns>
    public static WebApplicationBuilder AddSpydersoftFusionCache(this WebApplicationBuilder builder, Action<FusionCacheConfigOptions>? additionalConfiguration = null, Action<IFusionCacheBuilder>? additionalBuilder = null)
    {
        var cacheOptions = new FusionCacheConfigOptions();
        builder.Configuration.GetSection(FusionCacheConfigOptions.SectionName).Bind(cacheOptions);

        additionalConfiguration?.Invoke(cacheOptions);

        builder.Services.ConfigureFusionCache(cacheOptions, additionalBuilder);

        return builder;
    }

    /// <summary>
    /// Configures the fusion cache.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="options">The options.</param>
    /// <returns>IServiceCollection.</returns>
    public static IServiceCollection ConfigureFusionCache(this IServiceCollection services, FusionCacheConfigOptions options, Action<IFusionCacheBuilder>? additionalBuilder = null)
    {
        if (!options.Enabled)
        {
            return services;
        }

        services.AddMemoryCache(setupAction =>
        {
            setupAction.SizeLimit = 1024 * 1024 * options.MemoryCacheLimitMB;
        });

        // Could provide options.CacheName for keyed services, but this only 
        //  supports a single FusionCache instance per application now, not 
        //  necessary to make it keyed.
        var fusionCache = services.AddFusionCache()
            .WithOptions(options.CacheOptions)
            .WithDefaultEntryOptions(options.DefaultEntryOptions);

        switch (options.DistributedCacheType)
        {
            case CacheType.Memory:
                services.AddDistributedMemoryCache();
                break;
            case CacheType.Redis:
                if (string.IsNullOrWhiteSpace(options.Redis.ConnectionString))
                {
                    throw new ConfigurationException("Redis connection string must be provided when using Redis cache.");
                }
                fusionCache.ConfigureRedisWithBackplane(options);

                break;
            case CacheType.None:
                // No distributed cache configured
                break;
            default:
                throw new NotSupportedException($"Distributed cache type '{options.DistributedCacheType}' is not supported.");
        }

        if (additionalBuilder != null)
        {
            additionalBuilder(fusionCache);
        }

        // add configuration options
        services.AddOptions<FusionCacheConfigOptions>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection(FusionCacheConfigOptions.SectionName).Bind(settings);
            });

        return services;
    }

    [ExcludeFromCodeCoverage(Justification = "Requires Integtation Tests to verify Redis configuration")]
    private static void ConfigureRedisWithBackplane(this IFusionCacheBuilder fusionCache, FusionCacheConfigOptions options)
    {
        // ADD JSON.NET BASED SERIALIZATION FOR FUSION CACHE
        fusionCache
        .WithSerializer(
            new FusionCacheSystemTextJsonSerializer(new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            }))
        .WithDistributedCache(new RedisCache(
                        new RedisCacheOptions()
                        {
                            Configuration = options.Redis.ConnectionString,
                            InstanceName = options.Redis.InstanceName
                        }
        ))
        // ADD THE FUSION CACHE BACKPLANE FOR REDIS
        .WithBackplane(
            new RedisBackplane(new RedisBackplaneOptions() { Configuration = options.Redis.ConnectionString })
        );
    }
}
