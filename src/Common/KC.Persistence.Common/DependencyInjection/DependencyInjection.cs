using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.CosmosDB;
using Azure.ResourceManager.CosmosDB.Models;
using KC.Application.Common.Events;
using KC.Application.Common.IntegrationEvents;
using KC.Application.Common.Repositories;
using KC.Persistence.Common.Cosmos;
using KC.Persistence.Common.Events;
using KC.Persistence.Common.IntegrationEvents;
using KC.Persistence.Common.Settings;

namespace KC.Persistence.Common
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {
        private static List<string> _healthChecks = new();

        public static IServiceCollection AddSqlEventsPersistence(this IServiceCollection services,
            SqlServerSettings settings, string? connectionString)
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                services.AddDbContext<EventSqlDbContext>(options =>
                {
                    options.UseSqlServer(connectionString,
                    sqlServerOptionsAction: sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
                            sqlOptions.EnableRetryOnFailure(
                                maxRetryCount: settings.MaxRetryCount,
                                maxRetryDelay: TimeSpan.FromSeconds(settings.MaxRetryDelaySeconds),
                                errorNumbersToAdd: null);
                        });
                    options.AddInterceptors(new AzureAdAuthDbConnectionInterceptor());
                });

                services.AddHealthChecks().AddDbContextCheck<EventSqlDbContext>();
                services.AddSingleton<IEventLogSink, EventLogSink<EventSqlDbContext>>();
            }

            return services;
        }

        public static IServiceCollection AddPersistenceSql<TContext>(this IServiceCollection services,
            SqlServerSettings settings, string? connectionString, string migrationsAssemblyName)
            where TContext : BaseSqlDbContext<TContext>
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                services.AddDbContext<TContext>(options =>
                {
                    options.UseSqlServer(connectionString,
                    sqlServerOptionsAction: sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(migrationsAssemblyName);
                            sqlOptions.EnableRetryOnFailure(
                                maxRetryCount: settings.MaxRetryCount,
                                maxRetryDelay: TimeSpan.FromSeconds(settings.MaxRetryDelaySeconds),
                                errorNumbersToAdd: null);
                        });
                    options.AddInterceptors(new AzureAdAuthDbConnectionInterceptor());
                });
                services.AddScoped<IDbContext>(x => x.GetRequiredService<TContext>());
                services.AddHealthChecks().AddDbContextCheck<TContext>();
            }
            services.AddDatabaseDeveloperPageExceptionFilter();
            return services;
        }

        public static IServiceCollection AddSqlIntegrationEventLogService(this IServiceCollection services,
            params Assembly[] assemblies)
        {
            // integration event log service
            services.AddScoped<IIntegrationEventLogService,
                SqlIntegrationEventLogService>(sp =>
            {
                var context = sp.GetRequiredService<EventSqlDbContext>();
                return new SqlIntegrationEventLogService(context, assemblies);
            });
            return services;
        }

        public static IServiceCollection AddPersistenceCosmos<TContext>(this IServiceCollection services,
            CosmosDbRepositoryOptions cosmosOptions)
            where TContext : BaseDocDbContext<TContext>
        {
            if (!string.IsNullOrEmpty(cosmosOptions.EmulatorKey)
                && !string.IsNullOrEmpty(cosmosOptions.DbId))
            {
                services.AddDbContext<TContext>(options => options.UseCosmos(
                    cosmosOptions.EndPointUri, cosmosOptions.EmulatorKey, cosmosOptions.DbId));
            }
            else if (!string.IsNullOrEmpty(cosmosOptions.EndPointUri)
                && !string.IsNullOrEmpty(cosmosOptions.DbId))
            {
                services.AddDbContext<TContext>(options =>
                    options.UseCosmos(cosmosOptions.EndPointUri,
                        new DefaultAzureCredential(), cosmosOptions.DbId,
                        co =>
                        {
                            if (!string.IsNullOrEmpty(cosmosOptions.PreferredRegion))
                                co.Region(cosmosOptions.PreferredRegion);
                        }), optionsLifetime: ServiceLifetime.Singleton
                );
            }

            services.AddScoped<IDbContext>(
                x => x.GetRequiredService<TContext>()
            );

            // add health check if needed
            var hcName = $"cosmos-check-{cosmosOptions.DbId}";
            if (!_healthChecks.Contains(hcName))
            {
                services.AddHealthChecks().AddAzureCosmosDB(name: hcName, failureStatus: HealthStatus.Unhealthy,
                    clientFactory: s =>
                    {
                        var context = s.GetRequiredService<TContext>();
                        return context.Database.GetCosmosClient();
                    });
                _healthChecks.Add(hcName);
            }
            services.AddDatabaseDeveloperPageExceptionFilter();

            return services;
        }

        public static IServiceCollection AddCosmosIntegrationEventLogService<TContext>(this IServiceCollection services,
            params Assembly[] assemblies)
            where TContext : BaseDocDbContext<TContext>
        {
            // integration event log service
            services.AddScoped<IIntegrationEventLogService,
                DocIntegrationEventLogService<TContext>>(sp =>
            {
                var context = sp.GetRequiredService<TContext>();
                return new DocIntegrationEventLogService<TContext>(context, assemblies);
            });
            return services;
        }

        public static IApplicationBuilder UsePersistence(this IApplicationBuilder app, IHostEnvironment env)
        {
            if (!env.IsProduction())
            {
                app.UseMigrationsEndPoint();
            }
            return app;
        }

        public static async Task<IApplicationBuilder> UseCosmosDatabase<TContext>(
            this IApplicationBuilder app, CosmosDbRepositoryOptions options)
            where TContext : BaseDocDbContext<TContext>
        {
            // ensure database and containers are created
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TContext>();

            if (!string.IsNullOrEmpty(options.EmulatorKey))
            {
                // get client
                var client = context.Database.GetCosmosClient();

                // get/create database
                var response = await client.CreateDatabaseIfNotExistsAsync(options.DbId, options.DbMaxThroughput);

                // create containers specified in settings
                if (options.Containers?.Count > 0)
                {
                    foreach (var container in options.Containers)
                    {
                        await response.Database.CreateContainerIfNotExistsAsync(
                            container.Id, $"/{container.PartitionKeyPath}", container.MaxThroughput);
                    }
                }

                // create containers for doc entities
                foreach (var entity in context.Model.GetEntityTypes())
                {
                    var containerName = entity.GetContainer();
                    var partitionKeyPath = entity.GetPartitionKeyPropertyName();
                    if (containerName is not null && partitionKeyPath is not null)
                    {
                        await response.Database.CreateContainerIfNotExistsAsync(
                            containerName, $"/{partitionKeyPath}", options.DbMaxThroughput);
                    }
                }
            }
            else if (!string.IsNullOrEmpty(options.AccountName) && !string.IsNullOrEmpty(options.DbId)
                && !string.IsNullOrEmpty(options.SubscriptionId) && !string.IsNullOrEmpty(options.ResourceGroupName))
            {
                try
                {
                    // get DB account
                    var client = new ArmClient(new DefaultAzureCredential());
                    var account = client.GetCosmosDBAccountResource(CosmosDBAccountResource.CreateResourceIdentifier(
                        options.SubscriptionId, options.ResourceGroupName, options.AccountName));
                    var location = string.IsNullOrEmpty(options.PreferredRegion) ?
                        AzureLocation.WestUS : new AzureLocation(options.PreferredRegion);

                    // get/create database
                    var databases = account.GetCosmosDBSqlDatabases();
                    var database = await databases.ExistsAsync(options.DbId) ?
                        (await databases.GetAsync(options.DbId)).Value : null;
                    if (database is null)
                    {
                        var content = new CosmosDBSqlDatabaseCreateOrUpdateContent(location, new CosmosDBSqlDatabaseResourceInfo(options.DbId))
                        {
                            Options = new CosmosDBCreateUpdateConfig { Throughput = options.DbMaxThroughput }
                        };
                        var response = await databases.CreateOrUpdateAsync(Azure.WaitUntil.Completed, options.DbId, content);
                        database = response.Value;
                    }

                    // create containers specified in settings
                    var containers = database.GetCosmosDBSqlContainers();
                    if (options.Containers?.Count > 0)
                    {
                        foreach (var container in options.Containers)
                        {
                            if (!await containers.ExistsAsync(container.Id))
                            {
                                var content = ConfigureContainer(container.Id, container.PartitionKeyPath, location, container.MaxThroughput);
                                await containers.CreateOrUpdateAsync(Azure.WaitUntil.Completed, container.Id, content);
                            }
                        }
                    }

                    // create containers for doc entities
                    foreach (var entity in context.Model.GetEntityTypes())
                    {
                        var containerName = entity.GetContainer();
                        var partitionKeyPath = entity.GetPartitionKeyPropertyName();
                        if (containerName is not null && partitionKeyPath is not null && !await containers.ExistsAsync(containerName))
                        {
                            var content = ConfigureContainer(containerName, partitionKeyPath, location, options.DbMaxThroughput);
                            await containers.CreateOrUpdateAsync(Azure.WaitUntil.Completed, containerName, content);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                }
            }
            return app;
        }

        private static CosmosDBSqlContainerCreateOrUpdateContent ConfigureContainer(
            string containerName, string partitionKeyPath, string location, int maxThroughput)
        {
            var resource = new CosmosDBSqlContainerResourceInfo(containerName)
            {
                PartitionKey = new CosmosDBContainerPartitionKey()
            };
            resource.PartitionKey.Paths.Add($"/{partitionKeyPath}");
            return new CosmosDBSqlContainerCreateOrUpdateContent(location, resource)
            {
                Options = new CosmosDBCreateUpdateConfig { Throughput = maxThroughput }
            };
        }
    }
}