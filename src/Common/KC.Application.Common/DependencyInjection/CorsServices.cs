using KC.Utils.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KC.Application.Common
{
    public static class CorsServices
    {
        public static IServiceCollection AddCorsPolicies(this IServiceCollection services,
            AppSettings settings, ILogger logger)
        {
            if (!string.IsNullOrEmpty(settings.AllowedCorsOrigins))
            {
                logger.LogInformation("Found CORS settings: {val}", settings.AllowedCorsOrigins);
                services.AddCors(options =>
                {
                    options.AddDefaultPolicy(builder =>
                    {
                        builder.WithOrigins(settings.AllowedCorsOrigins.SplitToList(';')!.ToArray())
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .WithExposedHeaders("request-context");
                    });
                });
            }

            return services;
        }
    }
}