namespace KC.Identity.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddIdentityApi(this IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
        {
            // services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            // var appSection = configuration.GetSection(Global.ApplicationName);
            // // services.Configure<PersistenceSettings>(Global.ApplicationName, appSection.GetSection("PersistenceSettings"));
            // services.Configure<ApiSettings>(Global.ApplicationName, appSection.GetSection("ApiSettings"));
            // services.Configure<BackgroundServiceSettings>(appSection.GetSection("BackgroundServiceSettings"));

            // var sqlSettings = configuration.GetSqlServerSettings(Global.ApplicationName);
            // services.AddPersistenceSql<IdentityDbContext>(sqlSettings, configuration.GetConnectionString("IdentityDbConnection")
            //     , typeof(Startup).Assembly.GetName().Name!);

            // services.AddScoped<IOrgRepository, OrgRepository>();
            // // services.AddSingleton<IEventLogSink<IdentityDbContext>, EventLogSink<IdentityDbContext>>();

            // add AutoMapper
            // services.AddAutoMapper(config => config.AddProfile<MappingProfile>());

            // ICachePolicy discovery and registration
            // https://andrewlock.net/using-scrutor-to-automatically-register-your-services-with-the-asp-net-core-di-container/
            // services.Scan(scan => scan
            //     .FromAssemblies(Assembly.GetExecutingAssembly())
            //     .AddClasses(classes => classes.AssignableTo(typeof(ICachePolicy<,>)))
            //     .AsImplementedInterfaces()
            //     .WithTransientLifetime());

            // // add FluentValidation for calling Assembly
            // services.AddValidatorsFromAssemblyContaining<Startup>();

            // // add jwt options to intercept token validation event
            // services.AddSingleton<IConfigureOptions<JwtBearerOptions>, JwtBearerOptionsSetup>();

            // // add service bus health checks
            // services.AddServiceBusHealthChecks(env, Assembly.GetExecutingAssembly());

            // // add integration event handlers
            // services.AddIntegrationEventHandlers(Assembly.GetExecutingAssembly());

            // // add MFA settings
            // services.Configure<MfaSettings>(appSection.GetSection("MFASettings"));

            // // add FD Settings
            // services.Configure<FreshDeskSettings>(appSection.GetSection("FreshDeskSettings"));

            // // add clients
            // services.AddSingleton<IConfigApiClient, ConfigApiClient>();

            return services;
        }

        public static IApplicationBuilder UseIdentityApi(this IApplicationBuilder app)
        {
            // app.RegisterServiceBusIntegrationEvents(Assembly.GetExecutingAssembly());
            // app.RegisterIntegrationEventHandlers(Assembly.GetExecutingAssembly());
            return app;
        }
    }
}