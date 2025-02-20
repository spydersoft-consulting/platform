# Serilog

The `AddSpydersoftSerilog` extension on `WebApplicationBuilder` adds a default Serilog console logger plus any Serilog configuration provided in your app settings. See [Serilog.Settings.Configuration][1] for more details.

> [!IMPORTANT]
> If you add Telemetry above, `AddSpydersoftSerilog` MUST be called with `writeToProviders=true`. Additionally, Serilog's section of the
> appsettings will override the log levels, so log levels must be set in the `"Serilog`" section.

[1]: https://github.com/serilog/serilog-settings-configuration "Serilog.Settings.Configuration"
