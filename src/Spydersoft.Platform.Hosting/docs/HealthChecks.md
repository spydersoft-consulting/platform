# Health Checks

This library provides extensions to configure (`AddSpydersoftHealthChecks`) and use (`UseSpydersoftHealthChecks`) health checks to provide standard endpoints (`/livez`, `/readyz`, and `/startup`).

## Configuring New Health Checks

By default, a simple check will return healthy for all endpoints. To add your own health check:

- Create a new class that implements `IHealthCheck`
- Decorate that class with the `HealthCheckAttribute`, adding tags to control which endpoint(s) will execute the check.
- Call `AddSpydersoftHealthChecks` and `UseSpydersoftHealthChecks` in your `Program.cs`

The [Spydersoft.Platform.Hosting.ApiTests](../Spydersoft.Platform.Hosting.ApiTests/) project has some examples, including some that utilize services in the dependency injection system.

## Health Check Configuration

Configuration is controlled by configuration entries in `appsettings.json` or environment variables. Below are the possible settings with their default values.

```json
"HealthChecks": {
  "Enabled": true,
  "LiveTags": "live",
  "ReadyTags": "ready",
  "StartupTags": "startup"

}
```

| Setting     | Description                                                       |
| ----------- | ----------------------------------------------------------------- |
| Enabled     | Whether to enable health checks or not                            |
| LiveTags    | comma separated list of tags to execute for the /livez endpoint   |
| ReadyTags   | comma separated list of tags to execute for the /readyz endpoint  |
| StartupTags | comma separated list of tags to execute for the /startup endpoint |
