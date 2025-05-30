using KC.Application.Common;
using KC.Infrastructure.Common;
using KC.Infrastructure.Common.AppConfig;
using Microsoft.ApplicationInsights.Extensibility;

namespace KC.Mono.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostEnvironment Environment { get; }
        private IServiceCollection _services = new ServiceCollection();

        private readonly ILoggerFactory _loggerFactory;

        public Startup(IConfiguration configuration,
             IHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
            _loggerFactory = LoggerFactory.Create(builder => Environment.AddLogging(builder));
        }

        // Register services
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication(Configuration, Environment, _loggerFactory.CreateLogger<Startup>());
            services.AddInfrastructure(Configuration, Environment);
            services.AddMonoApi(Configuration, Environment);
            services.AddEndpointsApiExplorer();
            services.AddMonoApi(Configuration, Environment);

            _services = services;

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env, TelemetryConfiguration configuration, IConfiguration config)
        {
            app.UseApplication(env, _services, true);
            app.UseInfrastructure(env, configuration);
            app.UseMonoApi(configuration);
        }
    }

}