using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KC.Infrastructure.Common
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
        {
            return services;
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app,
            IHostEnvironment env, Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration configuration)
        {
            {
                return app;
            }
        }
    }
}