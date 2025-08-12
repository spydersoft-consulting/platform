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

    public CacheType DistributedCacheType { get; set; } = CacheType.None;

    public RedisConfig Redis { get; set; } = new RedisConfig();

    /// <summary>
    /// Gets or sets the default duration in minutes for cache entries.
    /// This value determines how long cache entries remain valid before expiring.
    /// </summary>
    /// <value>
    /// The default entry duration in minutes. Default is 5 minutes.
    /// </value>
    public int DefaultEntryDurationInMinutes { get; set; } = 5;

    /// <summary>
    /// Gets or sets a value indicating whether fail-safe mode is enabled.
    /// When enabled, stale cache entries can be returned if the underlying data source is unavailable,
    /// providing better resilience and availability.
    /// </summary>
    /// <value>
    /// <c>true</c> if fail-safe mode is enabled; otherwise, <c>false</c>. Default is <c>true</c>.
    /// </value>
    public bool EnableFailSafe { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum duration in minutes that a cache entry can be considered valid in fail-safe mode.
    /// This prevents extremely stale data from being served indefinitely.
    /// </summary>
    /// <value>
    /// The maximum fail-safe duration in minutes. Default is 1440 minutes (24 hours).
    /// </value>
    public double FailSafeMaxMinutes { get; set; } = 24 * 60;

    /// <summary>
    /// Gets or sets the throttle duration in minutes for fail-safe mode.
    /// This controls how often the cache will attempt to refresh failed entries,
    /// preventing excessive load on failing data sources.
    /// </summary>
    /// <value>
    /// The fail-safe throttle duration in minutes. Default is 0.5 minutes (30 seconds).
    /// </value>
    public double FailSafeThrottleMinutes { get; set; } = 0.5;

    /// <summary>
    /// Gets or sets the soft timeout in milliseconds for factory operations.
    /// This is the preferred timeout for cache value creation operations.
    /// If exceeded, cached values may be used while the operation continues in the background.
    /// </summary>
    /// <value>
    /// The factory soft timeout in milliseconds. Default is 500 milliseconds.
    /// </value>
    public int FactoryTimeoutSoftMs { get; set; } = 500;

    /// <summary>
    /// Gets or sets the hard timeout in milliseconds for factory operations.
    /// This is the maximum time allowed for cache value creation operations.
    /// If exceeded, the operation will be cancelled and fail-safe mechanisms may be triggered.
    /// </summary>
    /// <value>
    /// The factory hard timeout in milliseconds. Default is 2000 milliseconds (2 seconds).
    /// </value>
    public int FactoryTimeoutHardMs { get; set; } = 2000;

    /// <summary>
    /// Gets or sets the soft timeout in minutes for distributed cache operations.
    /// This is the preferred timeout for distributed cache read/write operations.
    /// If exceeded, local cache values may be used while the operation continues in the background.
    /// </summary>
    /// <value>
    /// The distributed cache soft timeout in minutes. Default is 60 minutes (1 hour).
    /// </value>
    public double DistributedCacheSoftTimeoutMinutes { get; set; } = 1 * 60;

    /// <summary>
    /// Gets or sets the hard timeout in minutes for distributed cache operations.
    /// This is the maximum time allowed for distributed cache read/write operations.
    /// If exceeded, the operation will be cancelled and local cache mechanisms may be used.
    /// </summary>
    /// <value>
    /// The distributed cache hard timeout in minutes. Default is 1440 minutes (24 hours).
    /// </value>
    public double DistributedCacheHardTimeoutMinutes { get; set; } = 24 * 60;
}

public enum CacheType
{
    None,
    Memory,
    Redis
}

public sealed class RedisConfig
{
    public string ConnectionString { get; set; } = string.Empty;
    public string InstanceName { get; set; } = "FusionCache";
}
