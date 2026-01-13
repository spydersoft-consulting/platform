# Spydersoft.Platform

This library provides foundational classes, attributes, and enumerations for use across the Spydersoft.Platform ecosystem. It serves as the core dependency for building ASP.NET Core applications with attribute-based configuration.

## Features

### Attributes

The library provides three primary attributes for declarative configuration:

#### DependencyInjectionAttribute

Enables automatic service registration in the dependency injection container. Decorate your classes to have them automatically discovered and registered at application startup.

```csharp
[DependencyInjection(typeof(IMyService), LifetimeOfService.Scoped)]
public class MyService : IMyService
{
    // Implementation
}
```

**Parameters:**
- `serviceInterface` - The interface type to register
- `serviceLifetime` - Service lifetime (Transient, Scoped, Singleton). Default: Transient
- `rank` - Registration order for multiple implementations. Default: 0

#### HealthCheckAttribute

Marks health check classes for automatic discovery and registration in the health check pipeline.

```csharp
[HealthCheck("database", HealthStatus.Unhealthy, tags: "ready,live")]
public class DatabaseHealthCheck : IHealthCheck
{
    // Implementation
}
```

**Parameters:**
- `name` - Health check name
- `failureStatus` - Status to report on failure (Unhealthy, Degraded)
- `tags` - Comma-separated tags for categorization (e.g., "ready", "live", "startup")

#### InjectOptionsAttribute

Automatically binds configuration sections to options classes without manual setup.

```csharp
[InjectOptions("MySettings")]
public class MySettings
{
    public string Option1 { get; set; }
    public int Option2 { get; set; }
}
```

**Parameters:**
- `sectionName` - Configuration section name to bind
- `tags` - Optional comma-separated tags for filtering

### Enumerations

#### LifetimeOfService

Defines service lifetime for dependency injection:
- `Transient` - Created every time they are requested
- `Scoped` - Created once per request, reused within that request context
- `Singleton` - Created once for the application lifetime

### Exceptions

#### ConfigurationException

Thrown when configuration errors are detected, such as missing required settings or invalid configuration values.

## Usage

This library is typically used in conjunction with [Spydersoft.Platform.Hosting](../Spydersoft.Platform.Hosting/docs/README.md), which provides the scanning and registration extensions that discover and process these attributes.

For complete examples and usage patterns, see the [Spydersoft.Platform.Hosting documentation](../Spydersoft.Platform.Hosting/docs/README.md).
