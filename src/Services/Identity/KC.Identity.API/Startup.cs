using System.Reflection;
using KC.Application.Common;
using KC.Identity.API;
using KC.Infrastructure.Common;
using KC.Infrastructure.Common.AppConfig;
using KC.Persistence.Common;
using Microsoft.ApplicationInsights.Extensibility;

namespace ODL.Identity.API
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // var sqlSettings = Configuration.GetSqlServerSettings(Global.ApplicationName);
            services.AddApplication(Configuration, Environment, _loggerFactory.CreateLogger<Startup>());
            services.AddInfrastructure(Configuration, Environment);
            services.AddIdentityApi(Configuration, Environment);
            // services.AddSqlEventsPersistence(sqlSettings, Configuration.GetConnectionString("IdentityDbConnection"));
            // services.AddSqlIntegrationEventLogService(Assembly.GetExecutingAssembly());

            // add health checks
            // services.AddAppHealthChecks();

            _services = services;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IConfiguration config, IHostEnvironment env, TelemetryConfiguration configuration)
        {
            var enableHttpLogging = config.GetSection("KC.API").GetValue<bool>("EnableHttpLogging");
            app.UseApplication(env, _services, enableHttpLogging);
            app.UseInfrastructure(env, configuration);
            app.UsePersistence(env);
            app.UseIdentityApi();
        }
    }
}