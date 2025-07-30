# FusionCache

This library provides extensions to configure (`AddSpydersoftFusionCache`) and use FusionCache, a high-performance, multi-layer caching library that provides fail-safe mechanisms and distributed caching capabilities with configurable timeouts and behaviors.

## Configuration

FusionCache can be easily configured in your `Program.cs` by calling the `AddSpydersoftFusionCache` extension method:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add FusionCache with default configuration
builder.AddSpydersoftFusionCache();

// Or add FusionCache with additional configuration
builder.AddSpydersoftFusionCache(fusionCache =>
{
    // Additional FusionCache configuration can be added here
});

var app = builder.Build();
```

## FusionCache Configuration Options

Configuration is controlled by configuration entries in `appsettings.json` or environment variables. Below are the possible settings with their default values:

```json
{
  "FusionCache": {
    "Enabled": true,
    "CacheName": "FusionCache",
    "DistributedCacheType": "None",
    "DefaultEntryDurationInMinutes": 5,
    "EnableFailSafe": true,
    "FailSafeMaxMinutes": 1440,
    "FailSafeThrottleMinutes": 0.5,
    "FactoryTimeoutSoftMs": 500,
    "FactoryTimeoutHardMs": 2000,
    "DistributedCacheSoftTimeoutMinutes": 60,
    "DistributedCacheHardTimeoutMinutes": 1440,
    "Redis": {
      "ConnectionString": "",
      "InstanceName": "FusionCache"
    }
  }
}
```

## Configuration Settings

| Setting | Description | Default Value |
| ------- | ----------- | ------------- |
| `Enabled` | Whether FusionCache is enabled. When disabled, caching functionality will not be available. | `true` |
| `CacheName` | The name of the cache instance. Used for identification in logging and diagnostics. | `"FusionCache"` |
| `DistributedCacheType` | The type of distributed cache to use. Options: `None`, `Memory`, `Redis` | `None` |
| `DefaultEntryDurationInMinutes` | Default duration in minutes for cache entries before expiring. | `5` |
| `EnableFailSafe` | Whether fail-safe mode is enabled. When enabled, stale cache entries can be returned if the underlying data source is unavailable. | `true` |
| `FailSafeMaxMinutes` | Maximum duration in minutes that a cache entry can be considered valid in fail-safe mode. | `1440` (24 hours) |
| `FailSafeThrottleMinutes` | Throttle duration in minutes for fail-safe mode. Controls how often the cache will attempt to refresh failed entries. | `0.5` (30 seconds) |
| `FactoryTimeoutSoftMs` | Soft timeout in milliseconds for factory operations. If exceeded, cached values may be used while the operation continues in the background. | `500` |
| `FactoryTimeoutHardMs` | Hard timeout in milliseconds for factory operations. If exceeded, the operation will be cancelled and fail-safe mechanisms may be triggered. | `2000` |
| `DistributedCacheSoftTimeoutMinutes` | Soft timeout in minutes for distributed cache operations. If exceeded, local cache values may be used while the operation continues in the background. | `60` (1 hour) |
| `DistributedCacheHardTimeoutMinutes` | Hard timeout in minutes for distributed cache operations. If exceeded, the operation will be cancelled and local cache mechanisms may be used. | `1440` (24 hours) |

## Redis Configuration

When using Redis as the distributed cache (`DistributedCacheType: "Redis"`), additional Redis-specific settings are available:

| Setting | Description | Default Value |
| ------- | ----------- | ------------- |
| `Redis.ConnectionString` | Redis connection string. Required when using Redis cache type. | `""` |
| `Redis.InstanceName` | Redis instance name for identifying the cache instance. | `"FusionCache"` |

## Distributed Cache Types

### None
No distributed cache is configured. Only local memory cache will be used.

```json
{
  "FusionCache": {
    "DistributedCacheType": "None"
  }
}
```

### Memory
Uses in-memory distributed cache. Suitable for single-instance applications or development scenarios.

```json
{
  "FusionCache": {
    "DistributedCacheType": "Memory"
  }
}
```

### Redis
Uses Redis as the distributed cache with backplane support for cache invalidation across multiple instances.

```json
{
  "FusionCache": {
    "DistributedCacheType": "Redis",
    "Redis": {
      "ConnectionString": "localhost:6379",
      "InstanceName": "MyApp"
    }
  }
}
```

## Usage Example

Once configured, FusionCache can be injected and used in your services:

```csharp
public class MyService
{
    private readonly IFusionCache _fusionCache;

    public MyService(IFusionCache fusionCache)
    {
        _fusionCache = fusionCache;
    }

    public async Task<string> GetDataAsync(string key)
    {
        return await _fusionCache.GetOrSetAsync(
            key,
            async _ => await FetchDataFromSourceAsync(key),
            TimeSpan.FromMinutes(10)
        );
    }

    private async Task<string> FetchDataFromSourceAsync(string key)
    {
        // Your data fetching logic here
        await Task.Delay(100);
        return $"Data for {key}";
    }
}
```

## Features

- **Multi-layer caching**: Combines memory cache with optional distributed cache
- **Fail-safe mechanisms**: Returns stale data when data sources are unavailable
- **Configurable timeouts**: Separate soft and hard timeouts for different operations
- **Redis backplane**: Automatic cache invalidation across multiple instances when using Redis
- **JSON serialization**: Built-in support for System.Text.Json serialization
- **Background operations**: Allows cache operations to continue in the background for better performance