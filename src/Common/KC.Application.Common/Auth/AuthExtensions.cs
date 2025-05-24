using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

namespace KC.Application.Common.Auth
{
    public static class AuthExtensions
    {
        public static AuthenticationBuilder AddJwtAuth(this AuthenticationBuilder builder, ILogger logger)
        {
            builder.Services.AddSingleton<IConfigureOptions<JwtBearerOptions>, JwtBearerOptionsSetup>();
            builder.AddJwtBearer(options =>
            {
                // add event to get access token from query string for websocket connections
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        logger.LogTrace("OnMessageReceived: Path: {Path}", context.Request.Path.Value);
                        if (context.Request.Path.Value?.StartsWith("/hubs/") == true)
                        {
                            var accessToken = context.Request.Query["access_token"];
                            if (!string.IsNullOrEmpty(accessToken))
                            {
                                logger.LogTrace("Setting access token from URL.");
                                context.Token = accessToken;
                            }
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            return builder;
        }

        // https://docs.microsoft.com/en-us/azure/active-directory-b2c/enable-authentication-web-api?tabs=csharpclient
        // https://learn.microsoft.com/en-us/azure/active-directory/develop/quickstart-web-api-aspnet-core-protect-api
        public static AuthenticationBuilder AddApiAuth(this AuthenticationBuilder builder, IConfiguration config)
        {
            builder.Services.AddSingleton<IConfigureOptions<MicrosoftIdentityOptions>, MicrosoftIdentityOptionsSetup>();
            builder.AddMicrosoftIdentityWebApi(config);
            return builder;
        }
    }
}
