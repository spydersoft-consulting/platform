# Configuring ITelemetryClient with Dependency Injection

## Overview

The `ITelemetryClient` interface is automatically configured when you use `AddSpydersoftTelemetry()`. This guide shows how the `MeterTelemetryClient` implementation is integrated into your ASP.NET Core application.

## Quick Start

### Automatic Configuration (Built-in)

When you call `AddSpydersoftTelemetry()`, it automatically registers:
- `Meter` singleton
- `ActivitySource` singleton  
- `ITelemetryClient` singleton (using `MeterTelemetryClient`)

```csharp
using Spydersoft.Platform.Hosting.StartupExtensions;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// This automatically registers Meter, ActivitySource, and ITelemetryClient
builder.AddSpydersoftTelemetry(Assembly.GetExecutingAssembly());

var app = builder.Build();
```

The registration uses your existing `TelemetryOptions` configuration from `appsettings.json`:
- Reads `MeterName` and `ActivitySourceName` from configuration
- Uses the assembly version automatically
- When telemetry is disabled, registers `NullTelemetryClient` instead

### Custom Configuration (Advanced)

If you need to bypass the automatic registration, you can manually configure:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Register Meter
builder.Services.AddSingleton(sp => 
    new Meter("MyApplication", "1.0.0"));

// Register ActivitySource
builder.Services.AddSingleton(sp => 
    new ActivitySource("MyApplication", "1.0.0"));

// Register ITelemetryClient
builder.Services.AddSingleton<ITelemetryClient>(sp =>
{
    var meter = sp.GetRequiredService<Meter>();
    var activitySource = sp.GetRequiredService<ActivitySource>();
    return new MeterTelemetryClient(meter, activitySource);
});

var app = builder.Build();
```

## Configuration

When using Option 1, ensure your `appsettings.json` has the telemetry configuration:

```json
{
  "Telemetry": {
    "Enabled": true,
    "ServiceName": "MyApplication",
    "MeterName": "MyApplication",
    "ActivitySourceName": "MyApplication",
    "Metrics": {
      "Type": "prometheus"
    }
  }
}
```

## Usage in Your Services

Once configured, inject `ITelemetryClient` into your services:

```csharp
public class OrderService
{
    private readonly ITelemetryClient _telemetry;
    private readonly ILogger<OrderService> _logger;

    public OrderService(ITelemetryClient telemetry, ILogger<OrderService> logger)
    {
        _telemetry = telemetry;
        _logger = logger;
    }

    public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
    {
        // Track order creation counter
        _telemetry.RecordCounter("orders.created", 1, new Dictionary<string, object?>
        {
            ["order.type"] = request.OrderType,
            ["customer.region"] = request.CustomerRegion
        });

        // Track order value histogram
        _telemetry.RecordHistogram("order.value", request.TotalAmount, new Dictionary<string, object?>
        {
            ["currency"] = request.Currency
        });

        var order = await ProcessOrderAsync(request);

        // Track event
        _telemetry.TrackEvent("OrderCreated", new Dictionary<string, string>
        {
            ["order.id"] = order.Id.ToString(),
            ["customer.id"] = request.CustomerId.ToString()
        });

        return order;
    }
}
```

## How It Works

The `AddSpydersoftTelemetry()` method automatically:

1. **Registers the telemetry components:**
   - Creates a `Meter` singleton with the configured `MeterName` and version
   - Creates an `ActivitySource` singleton with the configured `ActivitySourceName` and version
   - Registers `ITelemetryClient` as a `MeterTelemetryClient` using the above components

2. **Configures OpenTelemetry:**
   - Adds the Meter to OpenTelemetry via `builder.AddMeter(options.MeterName)`
   - Adds the ActivitySource to OpenTelemetry via `builder.AddSource(options.ActivitySourceName)`
   - Configures exporters (Prometheus, OTLP, Console, etc.)

3. **Handles disabled telemetry:**
   - When `Telemetry:Enabled` is `false`, registers `NullTelemetryClient.Instance` instead
   - Your code continues to work, but no telemetry is tracked

This means all metrics tracked via `ITelemetryClient` are automatically exported through your configured OpenTelemetry exporters.

## Complete Example

Here's a complete `Program.cs` example:

```csharp
using Microsoft.AspNetCore.Builder;
using Spydersoft.Platform.Hosting.StartupExtensions;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Serilog for logging
builder.AddSpydersoftSerilog(writeToProviders: true);

// Add OpenTelemetry - this automatically registers ITelemetryClient!
builder.AddSpydersoftTelemetry(Assembly.GetExecutingAssembly());

// Register your services that depend on ITelemetryClient
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Add Prometheus metrics endpoint if using Prometheus exporter
app.MapPrometheusScrapingEndpoint();

app.Run();
```

## Testing

For unit tests, use the `NullTelemetryClient`:

```csharp
using Spydersoft.Platform.Telemetry;
using Xunit;

public class OrderServiceTests
{
    [Fact]
    public async Task CreateOrder_TracksMetrics()
    {
        // Arrange
        var telemetry = NullTelemetryClient.Instance;
        var service = new OrderService(telemetry, NullLogger<OrderService>.Instance);

        // Act
        var order = await service.CreateOrderAsync(new CreateOrderRequest
        {
            OrderType = "Standard",
            TotalAmount = 100.00m
        });

        // Assert
        Assert.NotNull(order);
        // Telemetry calls won't throw, but won't track anything either
    }
}
```

Or use a mock:

```csharp
using Moq;

public class OrderServiceTests
{
    [Fact]
    public async Task CreateOrder_TracksCorrectMetrics()
    {
        // Arrange
        var mockTelemetry = new Mock<ITelemetryClient>();
        var service = new OrderService(mockTelemetry.Object, NullLogger<OrderService>.Instance);

        // Act
        await service.CreateOrderAsync(new CreateOrderRequest
        {
            OrderType = "Standard",
            TotalAmount = 100.00m
        });

        // Assert
        mockTelemetry.Verify(x => x.RecordCounter(
            "orders.created", 
            1, 
            It.IsAny<IDictionary<string, object?>>()), Times.Once);
        
        mockTelemetry.Verify(x => x.RecordHistogram(
            "order.value", 
            100.00, 
            It.IsAny<IDictionary<string, object?>>()), Times.Once);
    }
}
```

## Benefits

1. **Clean Separation**: Business logic separated from telemetry implementation
2. **Testable**: Easy to mock or use null implementation in tests
3. **Flexible**: Can switch between implementations via configuration
4. **Type-Safe**: Compile-time checking of telemetry calls
5. **Standards-Based**: Works seamlessly with OpenTelemetry infrastructure

## Troubleshooting

### Metrics Not Appearing

If your metrics aren't appearing in your monitoring system:

1. **Check OpenTelemetry Configuration**: Ensure `AddSpydersoftTelemetry()` is called
2. **Verify Meter Name**: The meter name in `AddSpydersoftTelemetryClient()` should match the one in OpenTelemetry's `AddMeter()`
3. **Check Exporter**: Ensure you have an exporter configured (Prometheus, OTLP, etc.)
4. **Verify Endpoint**: Check that your scraping endpoint or OTLP endpoint is correct

### Null Reference Exceptions

Ensure `AddSpydersoftTelemetryClient()` is called before services that depend on `ITelemetryClient`.

### Multiple Meter Instances

If you're getting multiple Meter instances, ensure you're using the singleton registrations and not creating new Meter instances elsewhere.

## See Also

- [ITelemetryClient Documentation](../../Spydersoft.Platform/docs/ITelemetryClient.md)
- [Implementation Comparison](../../Spydersoft.Platform/docs/ITelemetryClient-Comparison.md)
- [OpenTelemetry Documentation](./OpenTelemetry.md)
