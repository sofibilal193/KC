using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FluentValidation;
using KC.Application.Common;
using KC.Application.Common.Cacheing;
using KC.Application.Common.Settings;
using KC.Config.API.Application;
using KC.Config.API.Persistence;
using KC.Config.API.Repositories;
using KC.Infrastructure.Common;
using KC.Persistence.Common;
using KC.Persistence.Common.Extensions;

namespace KC.Config.API
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {
        public static IServiceCollection AddConfigApi(this IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
        {
            services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            // add AutoMapper
            services.AddAutoMapper(config => config.AddProfile<MappingProfile>());

            var appSection = configuration.GetSection(Global.ApplicationName);
            services.Configure<PersistenceSettings>(appSection.GetSection("PersistenceSettings"));
            services.Configure<BackgroundServiceSettings>(appSection.GetSection("BackgroundServiceSettings"));

            // var sqlSettings = configuration.GetSqlServerSettings(Global.ApplicationName);
            // services.AddPersistenceSql<ConfigDbContext>(sqlSettings, configuration.GetConnectionString("ConfigDbConnection")
            //     , typeof(Startup).Assembly.GetName().Name!);

            services.AddScoped<IConfigRepository, ConfigRepository>();
            services.AddScoped<IOrgConfigRepository, OrgConfigRepository>();
            services.AddScoped<IOrgConfigValueRepository, OrgConfigValueRepository>();
            services.AddScoped<IConfigUnitOfWork, ConfigUnitOfWork>();
            services.AddScoped<IUserConfigRepository, UserConfigRepository>();
            services.AddScoped<IUserConfigValueRepository, UserConfigValueRepository>();


            // add data seeding service
            services.AddScoped<ISeeder, Seeder>();

            // ICachePolicy discovery and registration
            // https://andrewlock.net/using-scrutor-to-automatically-register-your-services-with-the-asp-net-core-di-container/
            services.Scan(scan => scan
                .FromAssemblies(Assembly.GetExecutingAssembly())
                .AddClasses(classes => classes.AssignableTo(typeof(ICachePolicy<,>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime());

            // add FluentValidation for calling Assembly
            services.AddValidatorsFromAssemblyContaining<Startup>();

            // add service bus health checks

            // add integration event handlers

            return services;
        }

        public static IApplicationBuilder UseConfigApi(this IApplicationBuilder app)
        {
            return app;
        }
    }
}