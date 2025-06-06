using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Diagnostics.CodeAnalysis;

namespace KC.Persistence.Common
{
    public static class IHostExtensions
    {
        public static bool IsInKubernetes(this IHost host)
        {
            var cfg = host.Services.GetService<IConfiguration>();
            var orchestratorType = cfg?.GetValue<string>("OrchestratorType");
            return orchestratorType?.ToUpper() == "K8S";
        }

        public static IHost MigrateDbContext<TContext>(this IHost host, Action<TContext, IServiceProvider> seeder)
            where TContext : BaseDbContext<TContext>
        {
            var underK8s = host.IsInKubernetes();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetRequiredService<TContext>();

                logger.LogInformation("Running in Kubernetes: {k8s}", underK8s);

                try
                {
                    logger.LogInformation("Starting migration of database associated with context {DbContextName}", typeof(TContext).Name);

                    if (underK8s)
                    {
                        InvokeSeeder(seeder, context, services);
                    }
                    else
                    {
                        const int retries = 10;
                        var retry = Policy.Handle<SqlException>()
                            .WaitAndRetry(
                                retryCount: retries,
                                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                                onRetry: (exception, _, retry, _) => logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", nameof(TContext), exception.GetType().Name, exception.Message, retry, retries));

                        //if the sql server container is not created on run docker compose this
                        //migration can't fail for network related exception. The retry options for DbContext only
                        //apply to transient exceptions
                        // Note that this is NOT applied when running some orchestrators (let the orchestrator to recreate the failing service)
                        retry.Execute(() => InvokeSeeder(seeder, context, services));
                    }

                    logger.LogInformation("Completed migration of database associated with context {DbContextName}", typeof(TContext).Name);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}", typeof(TContext).Name);
                    if (underK8s)
                    {
                        throw;          // Rethrow under k8s because we rely on k8s to re-run the pod
                    }
                }
            }

            return host;
        }

        [ExcludeFromCodeCoverage]
        private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context, IServiceProvider services)
            where TContext : BaseDbContext<TContext>
        {
            var env = services.GetRequiredService<IHostEnvironment>();

            // create database for cosmos
            if (context.IsDocument)
            {
                context.Database.EnsureCreated();
                (context as BaseDocDbContext<TContext>)?.RegisterCommonProcedures();
            }

            if (env.IsDevelopment() && context.IsSql)
            {
                context.Migrate();
            }
            seeder(context, services);
        }
    }
}
