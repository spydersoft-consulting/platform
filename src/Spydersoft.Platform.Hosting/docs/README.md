# Spydersoft.Platform.Hosting

Provide extension methods for configuring ASP.NET Core projects with standardized telemetry and authentication.

## Added Services

This library provides functionality for adding the following to your ASP.NET Core project:

- [Serilog Console Hosting](./Serilog.md)
- [OpenTelemetry Logging, Metrics, and Tracing](./OpenTelemetry.md)
- [ASP.NET Health Checks](./HealthChecks.md)
- [JWT Bearer Authentication & Authorization](./Identity.md)

## Getting Started

[Spydersoft.Platform.Hosting.ApiTests](../Spydersoft.Platform.Hosting.ApiTests/) is a small application setup to test these extensions but can also be used as an example.

## NSwag with Telemetry and Health Checks

If your project is using NSwag to generate API specifications, it will attempt to execute your API project in order to scrape the endpoints. There is [an issue](https://github.com/RicoSuter/NSwag/issues/4676) where reflection during startup can cause NSwag to fail.

To work around this in your project, do the following:

1. In your NSwag configuration, set `aspNetCoreEnvironment` to something specific for NSwag generation, like `NSwag`.
2. Create an `appsettings.NSwag.json` file, where `NSwag` is the name you gave to aspNetCoreEnvironment.
3. In `appsettings.NSwag.json`, disable health checks and telemetry.

```json
{
  "HealthCheck": {
    "Enabled": false
  },
  "Telemetry": {
    "Enabled": false
  }
}
```

This will disable health checks and telemetry when NSwag executes and prevent errors.
