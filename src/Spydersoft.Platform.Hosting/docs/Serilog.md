# Serilog

The `AddSpydersoftSerilog` extension on `WebApplicationBuilder` adds a default Serilog console logger plus any Serilog configuration provided in your app settings. See [Serilog.Settings.Configuration][1] for more details.

> [!IMPORTANT]
> If you add Telemetry above, `AddSpydersoftSerilog` MUST be called **after** `AddSpydersoftTelemetry`, and with `writeToProviders=true`. Additionally, Serilog's section of the
> appsettings will override the log levels, so log levels must be set in the `"Serilog"` section.

```csharp
builder.AddSpydersoftTelemetry(typeof(Program).Assembly)
       .AddSpydersoftSerilog(true);
```

`AddSpydersoftTelemetry` clears the default logging providers that `WebApplicationBuilder.CreateBuilder()` registers automatically, so that Serilog's console sink (forwarded to other providers via `writeToProviders: true`) isn't duplicated by the framework's own default console logger. Calling these two methods in the reverse order will produce duplicate console output.

## Console format

The console sink always uses a human-readable, colorized, multi-line template, in every environment — it's for local/interactive tailing (`kubectl logs`, `dotnet run`) only, not the collector ingestion path. Log delivery to your backend (e.g. Grafana Loki) happens through the OpenTelemetry logging pipeline (`Telemetry:Log:Type=otlp`, see [OpenTelemetry](./OpenTelemetry.md)), which Serilog feeds via `writeToProviders: true` — the console sink is a separate, parallel output.

Do not reformat the console sink as structured JSON to try to make a log backend's output more readable. That only matters if stdout is actually being scraped by a log collector, which most services on this stack aren't set up for (check for a `logs.spydersoft.io/collect`-style pod annotation, or equivalent, before assuming it is). If your service's logs aren't showing up in your backend, verify the OTLP export path first (`Telemetry:Log:Type`/`Telemetry:Log:Otlp:Endpoint`, and that `writeToProviders: true` is set) rather than adjusting console formatting.

## Request logging

Add `UseSpydersoftRequestLogging()` (a thin wrapper over Serilog's `UseSerilogRequestLogging()`) immediately after `UseRouting()` in your middleware pipeline:

```csharp
app.UseRouting();
app.UseSpydersoftRequestLogging();
app.UseCors();
```

This produces a single structured log line per request (method, path, status code, elapsed time) and removes the need for hand-written "processing request" log calls scattered across controller actions. Pair it with a `Serilog:MinimumLevel:Override` entry for `Microsoft.AspNetCore` set to `Warning` in your appsettings so ASP.NET Core's own built-in `Request starting`/`Request finished` diagnostic logs don't also fire at `Information` alongside it:

```json
"Serilog": {
  "MinimumLevel": {
    "Default": "Warning",
    "Override": {
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

[1]: https://github.com/serilog/serilog-settings-configuration "Serilog.Settings.Configuration"
