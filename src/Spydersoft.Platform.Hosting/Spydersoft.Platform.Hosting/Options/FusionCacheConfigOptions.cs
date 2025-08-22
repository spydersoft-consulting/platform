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

    public CacheType DistributedCacheType { get; set; } = CacheType.None;

    public RedisConfig Redis { get; set; } = new RedisConfig();

    public long MemoryCacheLimitMB { get; set; } = 200;

    public FusionCacheEntryOptions DefaultEntryOptions { get; set; } = new FusionCacheEntryOptions();

    public FusionCacheOptions CacheOptions { get; set; } = new FusionCacheOptions();
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
