# ITelemetryClient - Telemetry Abstraction

## Overview

`ITelemetryClient` is a vendor-neutral interface for tracking telemetry data in .NET applications. It abstracts the underlying telemetry implementation, allowing you to switch between different telemetry providers without changing your application code.

## Supported Implementations

### 1. Application Insights (Legacy SDK)
- **Class**: `ApplicationInsightsTelemetryClient`
- **Backend**: Microsoft Application Insights SDK
- **Use Case**: Applications using the traditional Application Insights SDK

### 2. OpenTelemetry Metrics
- **Class**: `MeterTelemetryClient`
- **Backend**: System.Diagnostics.Metrics and System.Diagnostics.ActivitySource
- **Use Case**: Modern applications using OpenTelemetry standards

## Features

The interface provides comprehensive telemetry tracking capabilities:

### Metrics
- **TrackMetric**: Generic metric tracking
- **RecordCounter**: Monotonically increasing counters
- **RecordHistogram**: Value distribution measurements
- **RecordGauge**: Point-in-time measurements

### Events
- **TrackEvent**: Custom application events with properties and metrics

### Dependencies
- **TrackDependency**: External dependency calls (HTTP, database, etc.)

### Exceptions
- **TrackException**: Exception tracking with context

### Traces/Logs
- **TrackTrace**: Log messages with severity levels

### Requests
- **TrackRequest**: Incoming request tracking

### Availability
- **TrackAvailability**: Availability test results

## Usage Examples

### Using Application Insights Implementation

```csharp
using Microsoft.ApplicationInsights;
using Spydersoft.Platform.Telemetry;

// In Startup.cs or Program.cs
services.AddApplicationInsightsTelemetry();
services.AddSingleton<ITelemetryClient>(sp =>
{
    var aiClient = sp.GetRequiredService<TelemetryClient>();
    return new ApplicationInsightsTelemetryClient(aiClient);
});
```

### Using Meter Implementation

```csharp
using System.Diagnostics.Metrics;
using Spydersoft.Platform.Telemetry;

// In Startup.cs or Program.cs
services.AddSingleton<ITelemetryClient>(sp =>
{
    var meter = new Meter("MyApplication", "1.0.0");
    return new MeterTelemetryClient(meter);
});

// Configure OpenTelemetry to export the metrics
services.AddOpenTelemetry()
    .WithMetrics(builder => builder
        .AddMeter("MyApplication")
        .AddPrometheusExporter());
```

### Tracking Metrics

```csharp
public class MyService
{
    private readonly ITelemetryClient _telemetry;

    public MyService(ITelemetryClient telemetry)
    {
        _telemetry = telemetry;
    }

    public void ProcessOrder(Order order)
    {
        // Track a counter
        _telemetry.RecordCounter("orders.processed", 1, new Dictionary<string, object?>
        {
            ["order.type"] = order.Type,
            ["customer.region"] = order.CustomerRegion
        });

        // Track a histogram (e.g., order value)
        _telemetry.RecordHistogram("order.value", order.TotalAmount, new Dictionary<string, object?>
        {
            ["currency"] = order.Currency
        });

        // Track a metric with properties
        _telemetry.TrackMetric("order.items.count", order.Items.Count, new Dictionary<string, string>
        {
            ["order.id"] = order.Id.ToString()
        });
    }
}
```

### Tracking Events

```csharp
public void OnUserLogin(string userId, string loginMethod)
{
    _telemetry.TrackEvent("UserLogin", 
        properties: new Dictionary<string, string>
        {
            ["user.id"] = userId,
            ["login.method"] = loginMethod
        },
        metrics: new Dictionary<string, double>
        {
            ["login.attempt.count"] = 1
        });
}
```

### Tracking Dependencies

```csharp
public async Task<Customer> GetCustomerAsync(string customerId)
{
    var startTime = DateTimeOffset.UtcNow;
    var stopwatch = Stopwatch.StartNew();
    bool success = false;

    try
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);
        success = true;
        return customer;
    }
    finally
    {
        stopwatch.Stop();
        _telemetry.TrackDependency(
            dependencyTypeName: "SQL",
            target: "CustomerDatabase",
            dependencyName: "GetCustomer",
            data: $"CustomerId={customerId}",
            startTime: startTime,
            duration: stopwatch.Elapsed,
            success: success,
            properties: new Dictionary<string, string>
            {
                ["customer.id"] = customerId
            });
    }
}
```

### Tracking Exceptions

```csharp
public void ProcessPayment(Payment payment)
{
    try
    {
        // Process payment logic
    }
    catch (PaymentException ex)
    {
        _telemetry.TrackException(ex, 
            properties: new Dictionary<string, string>
            {
                ["payment.id"] = payment.Id.ToString(),
                ["payment.amount"] = payment.Amount.ToString(),
                ["payment.method"] = payment.Method
            },
            metrics: new Dictionary<string, double>
            {
                ["payment.failed.count"] = 1
            });
        throw;
    }
}
```

### Tracking Traces

```csharp
public void ImportData(string filePath)
{
    _telemetry.TrackTrace(
        $"Starting data import from {filePath}",
        TelemetrySeverityLevel.Information,
        new Dictionary<string, string>
        {
            ["file.path"] = filePath,
            ["import.type"] = "batch"
        });

    // Import logic...

    _telemetry.TrackTrace(
        "Data import completed successfully",
        TelemetrySeverityLevel.Information);
}
```

### Tracking Requests (typically in middleware)

```csharp
public async Task InvokeAsync(HttpContext context)
{
    var startTime = DateTimeOffset.UtcNow;
    var stopwatch = Stopwatch.StartNew();
    
    await _next(context);
    
    stopwatch.Stop();
    
    _telemetry.TrackRequest(
        name: $"{context.Request.Method} {context.Request.Path}",
        startTime: startTime,
        duration: stopwatch.Elapsed,
        responseCode: context.Response.StatusCode.ToString(),
        success: context.Response.StatusCode < 400,
        properties: new Dictionary<string, string>
        {
            ["http.method"] = context.Request.Method,
            ["http.path"] = context.Request.Path
        });
}
```

### Tracking Availability

```csharp
public async Task RunHealthCheckAsync()
{
    var startTime = DateTimeOffset.UtcNow;
    var stopwatch = Stopwatch.StartNew();
    bool success = false;
    string? message = null;

    try
    {
        await _healthCheckService.CheckAsync();
        success = true;
        message = "Health check passed";
    }
    catch (Exception ex)
    {
        message = $"Health check failed: {ex.Message}";
    }
    finally
    {
        stopwatch.Stop();
        _telemetry.TrackAvailability(
            name: "ServiceHealthCheck",
            startTime: startTime,
            duration: stopwatch.Elapsed,
            runLocation: Environment.MachineName,
            success: success,
            message: message);
    }
}
```

## Design Principles

1. **Vendor Neutrality**: The interface doesn't expose any vendor-specific types
2. **Flexibility**: Supports multiple backend implementations
3. **Comprehensive**: Covers all common telemetry scenarios
4. **Type Safety**: Uses strongly-typed parameters instead of magic strings where appropriate
5. **Extensibility**: Easy to add new implementations or extend existing ones

## Migration Guide

### From Direct Application Insights Usage

**Before:**
```csharp
private readonly TelemetryClient _telemetryClient;

public void DoWork()
{
    _telemetryClient.TrackMetric("work.done", 1);
}
```

**After:**
```csharp
private readonly ITelemetryClient _telemetryClient;

public void DoWork()
{
    _telemetryClient.TrackMetric("work.done", 1);
}
```

### From Direct Meter Usage

**Before:**
```csharp
private readonly Meter _meter;
private readonly Counter<long> _counter;

public MyService()
{
    _meter = new Meter("MyApp");
    _counter = _meter.CreateCounter<long>("work.done");
}

public void DoWork()
{
    _counter.Add(1);
}
```

**After:**
```csharp
private readonly ITelemetryClient _telemetryClient;

public MyService(ITelemetryClient telemetryClient)
{
    _telemetryClient = telemetryClient;
}

public void DoWork()
{
    _telemetryClient.RecordCounter("work.done", 1);
}
```

## Best Practices

1. **Use Dependency Injection**: Register `ITelemetryClient` in your DI container
2. **Consistent Naming**: Use consistent naming conventions for metrics and events
3. **Add Context**: Include relevant properties/tags with your telemetry
4. **Handle Exceptions**: Always track exceptions for better diagnostics
5. **Flush on Shutdown**: Call `FlushAsync()` before application shutdown to ensure all telemetry is sent

```csharp
public class ApplicationLifetime : IHostedService
{
    private readonly ITelemetryClient _telemetry;

    public ApplicationLifetime(ITelemetryClient telemetry)
    {
        _telemetry = telemetry;
    }

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _telemetry.FlushAsync(cancellationToken);
    }
}
```

## Advanced Scenarios

### Custom Implementation

You can create your own implementation for other telemetry backends:

```csharp
public class CustomTelemetryClient : ITelemetryClient
{
    private readonly ICustomBackend _backend;

    public CustomTelemetryClient(ICustomBackend backend)
    {
        _backend = backend;
    }

    public void TrackMetric(string name, double value, IDictionary<string, string>? properties = null)
    {
        // Implement using your custom backend
        _backend.SendMetric(name, value, properties);
    }

    // Implement other methods...
}
```

### Composite Implementation

Track to multiple backends simultaneously:

```csharp
public class CompositeTelemetryClient : ITelemetryClient
{
    private readonly IEnumerable<ITelemetryClient> _clients;

    public CompositeTelemetryClient(IEnumerable<ITelemetryClient> clients)
    {
        _clients = clients;
    }

    public void TrackMetric(string name, double value, IDictionary<string, string>? properties = null)
    {
        foreach (var client in _clients)
        {
            client.TrackMetric(name, value, properties);
        }
    }

    // Implement other methods similarly...
}
```

## Testing

For unit testing, you can create a mock implementation:

```csharp
public class MockTelemetryClient : ITelemetryClient
{
    public List<string> TrackedMetrics { get; } = new();
    public List<string> TrackedEvents { get; } = new();

    public void TrackMetric(string name, double value, IDictionary<string, string>? properties = null)
    {
        TrackedMetrics.Add(name);
    }

    public void TrackEvent(string eventName, IDictionary<string, string>? properties = null, IDictionary<string, double>? metrics = null)
    {
        TrackedEvents.Add(eventName);
    }

    // Implement other methods as no-ops or recording methods...
}
```

## Performance Considerations

- **Meter Implementation**: Uses lazy initialization for metrics instruments
- **Application Insights**: Buffers telemetry before sending
- **Thread Safety**: Both implementations are thread-safe
- **Minimal Overhead**: Interface calls have minimal performance impact
