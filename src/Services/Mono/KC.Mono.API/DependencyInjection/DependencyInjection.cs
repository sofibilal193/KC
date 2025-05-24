using KC.Identity.API;

namespace KC.Mono.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMonoApi(this IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
        {
            services.AddIdentityApi(configuration, env);
            return services;
        }

        public static IApplicationBuilder UseMonoApi(this IApplicationBuilder app, Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration configuration)
        {
            app.UseIdentityApi();
            return app;
        }

    }
}