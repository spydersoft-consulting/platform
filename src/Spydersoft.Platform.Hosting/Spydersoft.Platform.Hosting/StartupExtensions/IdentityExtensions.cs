using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spydersoft.Platform.Hosting.Options;
using Spydersoft.Platform.Hosting.StartupExtensions;

namespace Spydersoft.Platform.Hosting.StartupExtensions;
public static class IdentityExtensions
{
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

    public static IApplicationBuilder UseAuthentication(this IApplicationBuilder app, bool authInstalled)
    {
        if (authInstalled)
        {
            app.UseAuthentication();
        }
        return app;
    }

    public static IApplicationBuilder UseAuthorization(this IApplicationBuilder app, bool authInstalled)
    {
        if (authInstalled)
        {
            app.UseAuthorization();
        }
        return app;
    }

}