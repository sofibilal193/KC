using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KC.Application.Common
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services,
            IConfiguration configuration, IHostEnvironment env, ILogger logger, bool useIntegrationEvents = true)
        {
            return services;
        }

        public static IApplicationBuilder UseApplication(this IApplicationBuilder app, IHostEnvironment env, IServiceCollection services, bool enableHttpLogging)
        {
            // use NSwag OpenApi
            app.UseSwaggerWithReverseProxySupport();
            return app;
        }

        private static IApplicationBuilder UseSwaggerWithReverseProxySupport(this IApplicationBuilder app)
        {
            var logger = app.ApplicationServices.GetService<ILogger>();

            app.UseOpenApi(config =>
            {
                // Without this the document will be cached with wrong URLs
                // and not work if later accessed from another host/path/scheme
                config.CreateDocumentCacheKey = request =>
                    string.Concat(
                    !string.IsNullOrEmpty(request.Headers["X-Azure-FDID"].FirstOrDefault())
                        ? request.Headers["X-Azure-FDID"].FirstOrDefault()
                        : request.Headers["X-Forwarded-Host"].FirstOrDefault() ?? request.Headers["X-ORIGINAL-HOST"].FirstOrDefault()
                    , request.Headers["X-Forwarded-PathBase"].FirstOrDefault()
                    , request.IsHttps);

                // Change document host and base path from headers (if set)
                config.PostProcess = (document, request) =>
                {
                    bool isAzureFD = !string.IsNullOrEmpty(request.Headers["X-Azure-FDID"].FirstOrDefault());
                    var originalHost = isAzureFD ? string.Empty : request.Headers["X-Forwarded-Host"].FirstOrDefault() ?? request.Headers["X-ORIGINAL-HOST"].FirstOrDefault();
                    var pathBase = request.Headers["X-Forwarded-PathBase"].FirstOrDefault();

                    logger?.LogInformation("OriginalHost: {host}. PathBase: {path}", originalHost, pathBase);

                    if (!string.IsNullOrEmpty(pathBase))
                        document.BasePath = pathBase;

                    if (!string.IsNullOrEmpty(originalHost))
                        document.Host = originalHost;
                };
            });

            app.UseSwaggerUi(settings =>
            {
                settings.TransformToExternalPath = (route, request) =>
                {
                    var pathBase = request.Headers["X-Forwarded-PathBase"].FirstOrDefault();
                    return !string.IsNullOrEmpty(pathBase)
                        ? $"{pathBase}{route}"
                        : route;
                };
            });

            return app;
        }
    }
}