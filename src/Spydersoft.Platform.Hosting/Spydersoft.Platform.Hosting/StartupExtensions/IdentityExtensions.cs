using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spydersoft.Platform.Hosting.Options;
using Spydersoft.Platform.Hosting.StartupExtensions;

namespace Spydersoft.Platform.Hosting.StartupExtensions;

/// <summary>
/// Extension methods for configuring JWT Bearer authentication and authorization.
/// </summary>
public static class IdentityExtensions
{
    /// <summary>
    /// Adds JWT Bearer authentication to the application based on configuration.
    /// Configures authentication and authorization services when identity is enabled.
    /// </summary>
    /// <param name="appBuilder">The web application builder.</param>
    /// <returns><c>true</c> if authentication was configured; otherwise, <c>false</c>.</returns>
    public static bool AddSpydersoftIdentity(this WebApplicationBuilder appBuilder)
    {
        var authInstalled = false;
        var identityOption = new IdentityOptions();
        appBuilder.Configuration.GetSection(IdentityOptions.SectionName).Bind(identityOption);

        if (identityOption.Enabled && identityOption.Authority != null)
        {
            appBuilder.Services
                .AddAuthentication(o =>
                {
                    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(o =>
                {
                    o.IncludeErrorDetails = true;

                    o.Authority = identityOption.Authority;
                    o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidAudiences =
                                        [
                                            identityOption.ApplicationName
                                        ],
                        ValidIssuers =
                                        [
                                            identityOption.Authority
                                        ]
                    };
                });

            appBuilder.Services.AddAuthorization();
            authInstalled = true;
        }
        return authInstalled;
    }

    /// <summary>
    /// Adds authentication middleware to the application pipeline if authentication was configured.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="authInstalled">Value indicating whether authentication is configured.</param>
    /// <returns>The application builder for chaining.</returns>
    public static IApplicationBuilder UseAuthentication(this IApplicationBuilder app, bool authInstalled)
    {
        if (authInstalled)
        {
            app.UseAuthentication();
        }
        return app;
    }

    /// <summary>
    /// Adds authorization middleware to the application pipeline if authentication was configured.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="authInstalled">Value indicating whether authentication is configured.</param>
    /// <returns>The application builder for chaining.</returns>
    public static IApplicationBuilder UseAuthorization(this IApplicationBuilder app, bool authInstalled)
    {
        if (authInstalled)
        {
            app.UseAuthorization();
        }
        return app;
    }

}