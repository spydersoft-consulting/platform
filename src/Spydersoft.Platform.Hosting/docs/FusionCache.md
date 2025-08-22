# FusionCache

This library provides extensions to configure (`AddSpydersoftFusionCache`) and use FusionCache, a high-performance, multi-layer caching library that provides fail-safe mechanisms and distributed caching capabilities with configurable timeouts and behaviors.

## Configuration

FusionCache can be configured in your `Program.cs` using the `AddSpydersoftFusionCache` extension method. You can provide delegates for both options and builder customization:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add FusionCache with default configuration
builder.AddSpydersoftFusionCache();

// Or add FusionCache with additional configuration for options and builder
builder.AddSpydersoftFusionCache(
    options =>
    {
        options.MemoryCacheLimitMB = 512;
        options.CacheName = "MyAppCache";
        // You can also set DefaultEntryOptions and CacheOptions here
    },
    fusionCacheBuilder =>
    {
        // Additional FusionCache builder configuration
    }
);

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
    "MemoryCacheLimitMB": 200,
    "DefaultEntryOptions": {
      // See FusionCacheEntryOptions documentation for all available options
    },
    "CacheOptions": {
      // See FusionCacheOptions documentation for all available options
    },
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
| `MemoryCacheLimitMB` | The memory cache size limit in megabytes. | `200` |
| `DefaultEntryOptions` | Default entry options for all cache entries (see FusionCacheEntryOptions). | `{}` |
| `CacheOptions` | Global FusionCache options (see FusionCacheOptions). | `{}` |

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

## Advanced Usage Example

You can configure advanced options and entry defaults either in code or via configuration:

```csharp
builder.AddSpydersoftFusionCache(
    options =>
    {
        options.DefaultEntryOptions.Duration = TimeSpan.FromMinutes(30);
        options.CacheOptions.AllowBackgroundDistributedCacheOperations = true;
    }
);
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
- **Configurable memory cache size**: Set via `MemoryCacheLimitMB`
- **Advanced entry and cache options**: Use `DefaultEntryOptions` and `CacheOptions` for fine-tuning
- **Fail-safe mechanisms**: Returns stale data when data sources are unavailable
- **Configurable timeouts**: Separate soft and hard timeouts for different operations
- **Redis backplane**: Automatic cache invalidation across multiple instances when using Redis
- **JSON serialization**: Built-in support for System.Text.Json serialization
- **Background operations**: Allows cache operations to continue in the background for better performance