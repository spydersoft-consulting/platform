using ZiggyCreatures.Caching.Fusion;

namespace Spydersoft.Platform.Hosting.Options;

/// <summary>
/// Configuration options for FusionCache integration.
/// FusionCache is a high-performance, multi-layer caching library that provides fail-safe mechanisms
/// and distributed caching capabilities with configurable timeouts and behaviors.
/// </summary>
public class FusionCacheConfigOptions
{
    /// <summary>
    /// The section name for configuration binding.
    /// </summary>
    public const string SectionName = "FusionCache";

    /// <summary>
    /// Gets or sets a value indicating whether FusionCache is enabled.
    /// When disabled, caching functionality will not be available.
    /// </summary>
    /// <value>
    /// <c>true</c> if FusionCache is enabled; otherwise, <c>false</c>. Default is <c>false</c>.
    /// </value>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the name of the cache instance.
    /// This name is used to identify the cache instance for logging and diagnostics.
    /// </summary>
    /// <value>
    /// The cache name. Default is "FusionCache".
    /// </value>
    public string CacheName { get; set; } = "FusionCache";

    /// <summary>
    /// Gets or sets the distributed cache type.
    /// Determines which distributed cache provider to use for second-level caching.
    /// </summary>
    /// <value>
    /// The distributed cache type. Default is <see cref="CacheType.None"/>.
    /// </value>
    public CacheType DistributedCacheType { get; set; } = CacheType.None;

    /// <summary>
    /// Gets or sets the Redis configuration.
    /// Only used when <see cref="DistributedCacheType"/> is set to <see cref="CacheType.Redis"/>.
    /// </summary>
    /// <value>
    /// The Redis configuration.
    /// </value>
    public RedisConfig Redis { get; set; } = new RedisConfig();

    /// <summary>
    /// Gets or sets the memory cache limit in megabytes.
    /// Controls the maximum size of the in-memory L1 cache.
    /// </summary>
    /// <value>
    /// The memory cache limit in MB. Default is 200.
    /// </value>
    public long MemoryCacheLimitMB { get; set; } = 200;

    /// <summary>
    /// Gets or sets the default entry options for cached items.
    /// These options apply to all cache entries unless overridden.
    /// </summary>
    /// <value>
    /// The default entry options.
    /// </value>
    public FusionCacheEntryOptions DefaultEntryOptions { get; set; } = new FusionCacheEntryOptions();

    /// <summary>
    /// Gets or sets the global FusionCache options.
    /// Configures behavior such as serialization, events, and plugins.
    /// </summary>
    /// <value>
    /// The cache options.
    /// </value>
    public FusionCacheOptions CacheOptions { get; set; } = new FusionCacheOptions();
}

/// <summary>
/// Specifies the type of distributed cache to use.
/// </summary>
public enum CacheType
{
    /// <summary>
    /// No distributed cache. Only in-memory L1 caching is used.
    /// </summary>
    None,
    
    /// <summary>
    /// In-memory cache only.
    /// </summary>
    Memory,
    
    /// <summary>
    /// Redis-based distributed cache for L2 caching.
    /// </summary>
    Redis
}

/// <summary>
/// Configuration for Redis distributed cache connection.
/// </summary>
public sealed class RedisConfig
{
    /// <summary>
    /// Gets or sets the Redis connection string.
    /// </summary>
    /// <value>
    /// The connection string for connecting to Redis. Default is empty.
    /// </value>
    public string ConnectionString { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the Redis instance name.
    /// Used as a prefix for all cache keys to avoid collisions.
    /// </summary>
    /// <value>
    /// The instance name. Default is "FusionCache".
    /// </value>
    public string InstanceName { get; set; } = "FusionCache";
}
