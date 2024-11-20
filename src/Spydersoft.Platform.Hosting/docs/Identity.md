# Identity

This library contains `WebApplicationBuilder` and `IApplicationBuilder` extensions for configuring and using simple JwtBearer authentication and authorization.

A sample implementation in `Program.cs`:

```csharp
var builder = WebApplication.CreateBuilder(args);

var authInstalled = builder.AddSpydersoftIdentity();

// Other service additions

var app = builder.Build();

app.UseAuthentication(authInstalled)
    .UseRouting()
    .UseAuthorization(authInstalled)
    .UseEndpoints(endpoints => endpoints.MapControllers())
    .UseDefaultFiles()
    .UseStaticFiles();

// Other app Use

app.UseHsts();
await app.RunAsync();

```

Notice that the provided `UseAuthentication` and `UseAuthorization` methods accept a boolean parameter. If that parameter is true, Authentication/Authorization will be configured using the standard extensions. These methods must be used to ensure that `UseAuthentication` and `UseAuthorization` are not called if no authentication is configured.

## Identity Configuration

```json
"Identity": {
  "ApplicationName": "spydersoft-application",
  "Authority": "",
  "Enabled": false
}
```

| Setting         | Description                                                                       |
| --------------- | --------------------------------------------------------------------------------- |
| ApplicationName | The name of the application in the oAuth providers, used to set `ValidAudiences`  |
| Authority       | The URL of the token authority, used to set `ValidIssuers` and bearer `Authority` |
| Enabled         | Whether to enable identity protection or not                                      |
