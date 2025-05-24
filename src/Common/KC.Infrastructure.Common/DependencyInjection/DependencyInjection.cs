using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.SignalR;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.FeatureFilters;
using Microsoft.Graph;
using Azure.Core;
using Azure.Identity;
using KC.Application.Common.AzureAD;
using KC.Application.Common.Cacheing;
using KC.Application.Common.Extensions;
using KC.Application.Common.Files;
using KC.Application.Common.IntegrationEvents;
using KC.Application.Common.Messaging;
using KC.Application.Common.Storage;
using KC.Domain.Common;
using KC.Domain.Common.ApiServices;
using KC.Infrastructure.Common.AppConfig;
using KC.Infrastructure.Common.AppInsights;
using KC.Infrastructure.Common.AzureAD;
using KC.Infrastructure.Common.Cacheing;
using KC.Infrastructure.Common.Crypto;
using KC.Infrastructure.Common.Files;
using KC.Infrastructure.Common.Logging;
using KC.Infrastructure.Common.Messaging;
using KC.Infrastructure.Common.RabbitMQ;
using KC.Infrastructure.Common.ServiceBus;
using KC.Infrastructure.Common.SignalR;
using KC.Infrastructure.Common.Storage;
using KC.Utils.Common;
using KC.Utils.Common.Crypto;
using Polly;
using RabbitMQ.Client;
using SendGrid;
using Twilio.Clients;

namespace KC.Infrastructure.Common
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {
        private static readonly InfraSettings _settings = new();
        private static readonly CryptoSettings _cryptoSettings = new();

        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration, IHostEnvironment env)
        {
            services.AddSingleton(configuration);
            services.Configure<InfraSettings>(configuration.GetSection("InfraSettings"));
            services.Configure<CryptoSettings>(configuration.GetSection("CryptoSettings"));
            configuration.GetSection("InfraSettings").Bind(_settings);
            configuration.GetSection("CryptoSettings").Bind(_cryptoSettings);

            services.AddTransient<IDateTime, UtcDateTime>();

            // add Azure clients for logging https://docs.microsoft.com/en-us/dotnet/azure/sdk/logging
            services.AddAzureClientsCore();

            // add secret repository
            if (env.IsDevelopment() || env.IsTest())
            {
                services.AddSingleton<ISecretRepository, LocalSecretRepository>();
                services.AddRabbmitMQ(_settings.RabbitMQSettings);
            }
            else
            {
                services.AddServiceBus();
            }

            // add redis cache
            services.AddSingleton<RedisCacheOptions, GetRedisCacheOptions>();
            services.AddSingleton<ICache, RedisCache>();
            services.AddHealthChecks().AddRedis(_settings.RedisCacheSettings.ConnectionString, "redis-check", HealthStatus.Unhealthy);

            services.AddAzureClients(env);

            // add Azure SignalR service
            services.AddSingleton<IUserIdProvider, HubUserIdProvider>();
            if (_settings.SignalRSettings.AzureServiceEnabled)
            {
                services.AddSignalR().AddAzureSignalR(option =>
                {
                    option.Endpoints = new ServiceEndpoint[]
                    {
                        new (new Uri(_settings.SignalRSettings.AzureServiceUri), new DefaultAzureCredential())
                    };
                });
            }
            else
            {
                services.AddSignalR();
            }

            // add Azure file storage
            services.AddSingleton<AzureStorageFileOptions, GetAzureStorageFileOptions>();
            services.AddSingleton<IFileProvider, AzureStorageFileProvider>();

            // add crypto provider
            services.AddSingleton<CryptoOptions, GetCryptoOptions>();
            services.AddSingleton<ICryptoProvider, CryptoProvider>();

            // add application insights telemetry - https://docs.microsoft.com/en-us/azure/azure-monitor/app/asp-net-core
            services.AddApplicationInsightsTelemetry(configuration);
            services.AddApplicationInsightsKubernetesEnricher();
            services.AddApplicationInsightsTelemetryProcessor<AppInsightsProcessor>();
            services.AddSingleton<Microsoft.ApplicationInsights.Extensibility.ITelemetryInitializer, AppInsightsInitializer>();

            // add Feature Management
            services.AddFeatureManagement()
                .AddFeatureFilter<TargetingFilter>();
            services.AddSingleton<ITargetingContextAccessor, OrgTargetingContextAccessor>();

            // add HttpClient logging
            services.AddTransient<HttpClientLoggingHandler>();

            if (!string.IsNullOrEmpty(_settings.SendGridApiKey))
            {
                services.AddSingleton<ISendGridClient>(new SendGridClient(_settings.SendGridApiKey));
                services.AddSingleton<IEmailProvider, EmailProvider>();
            }

            services.AddHttpClient<ITwilioRestClient, TwilioClient>().AddHttpMessageHandler<HttpClientLoggingHandler>();
            services.AddSingleton<TwilioClientOptions, GetTwilioClientOptions>();
            services.AddSingleton<ISmsProvider, SmsProvider>();

            services.AddHttpClient<IBitlyUrlHttpClient, BitlyUrlHttpClient>().AddHttpMessageHandler<HttpClientLoggingHandler>();
            services.AddSingleton<BitlyOptions, GetBitlyOptions>();
            services.AddSingleton<IUrlProvider, BitlyUrlProvider>();

            services.AddSingleton<IPdfProvider, PdfProvider>();
            services.AddSingleton<IZipProvider, ZipProvider>();

            services.AddApiService(_settings.ApiServiceSettings);

            services.AddAzureAD(_settings.AzureAD);
            return services;
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app,
            IHostEnvironment env, Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration configuration)
        {
            if (env.IsDevelopment() || env.IsEnvironment("Test"))
            {
                // disable application insights telemetry in dev/test environments
                configuration.DisableTelemetry = true;
            }

            return app;
        }

        private static IServiceCollection AddRabbmitMQ(this IServiceCollection services, RabbitMQOptions options)
        {
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            if (!string.IsNullOrEmpty(options.HostName) && !string.IsNullOrEmpty(options.UserName) && !string.IsNullOrEmpty(options.Password))
            {
                services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();
                    var factory = new ConnectionFactory()
                    {
                        HostName = options.HostName,
                        UserName = options.UserName,
                        Password = options.Password,
                        DispatchConsumersAsync = options.DispatchConsumersAsync
                    };

                    return new DefaultRabbitMQPersistentConnection(factory, logger, options);
                });

                services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
                {
                    var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                    var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                    var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
                    return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, sp,
                        eventBusSubcriptionsManager, options);
                });

                var tags = new string[] { "rabbitmqbus" };
                services.AddHealthChecks()
                    .AddRabbitMQ(
                        $"amqp://{options.HostName}",
                        name: "rabbitmqbus-check",
                        tags: tags);
            }

            return services;
        }

        private static IServiceCollection AddServiceBus(this IServiceCollection services)
        {
            services.AddSingleton(_settings.ServiceBusSettings);

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            services.AddSingleton<IServiceBusPersisterConnection, DefaultServiceBusPersisterConnection>();
            services.AddSingleton<IEventBus, EventBusServiceBus>();

            return services;
        }

        public static IApplicationBuilder RegisterServiceBusIntegrationEvents(this IApplicationBuilder app, params Assembly[] assemblies)
        {
            var sb = app.ApplicationServices.GetService<IServiceBusPersisterConnection>();

            if (assemblies?.Length > 0 && sb is not null)
            {
                foreach (var assembly in assemblies)
                {
                    var types = assembly.GetTypes().Where(t => t.Name.EndsWith("IntegrationEvent"));
                    if (types?.Count() > 0)
                    {
                        foreach (var type in types)
                        {
                            var topicName = sb.GetTopicName(type.Name);
                            sb.CreateTopicAsync(topicName).GetAwaiter().GetResult();
                        }
                    }
                }
            }

            return app;
        }

        public static IServiceCollection AddServiceBusHealthChecks(this IServiceCollection services, IHostEnvironment env, params Assembly[] assemblies)
        {
            if (assemblies?.Length > 0 && !env.IsDevelopment() && !env.IsTest())
            {
                var tags = new string[] { "servicebus" };
                foreach (var assembly in assemblies)
                {
                    var types = assembly.GetTypes().Where(t => t.Name.EndsWith("IntegrationEvent"));
                    if (types.Any())
                    {
                        foreach (var type in types)
                        {
                            var topicName = type.Name.Replace("IntegrationEvent", "").ToLowerInvariant();
                            services.AddHealthChecks()
                                .AddAzureServiceBusTopic(
                                    fullyQualifiedNamespace: _settings.ServiceBusSettings.Namespace,
                                    topicName: topicName,
                                    tokenCredential: new DefaultAzureCredential(),
                                    name: string.Concat(_settings.ServiceBusSettings.SubscriptionClientName, "-", topicName),
                                    tags: tags
                                );
                        }
                    }
                }
            }
            return services;
        }

        // https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/extensions/Microsoft.Extensions.Azure/README.md
        private static IServiceCollection AddAzureClients(this IServiceCollection services,
            IHostEnvironment env)
        {
            services.AddAzureClients(builder =>
            {
                if (!env.IsDevelopment() && !env.IsTest())
                {
                    // add secret repo
                    builder.AddSecretClient(new Uri(_cryptoSettings.AzureKeyVaultUri));
                    builder.AddKeyClient(new Uri(_cryptoSettings.AzureKeyVaultUri));
                    services.AddSingleton<SecretRepoOptions, GetSecretRepoOptions>();
                    services.AddSingleton<ISecretRepository, SecretRepository>();
                }

                // Configures environment credential to be used by default for all clients that require TokenCredential and doesn't override it on per registration level
                builder.UseCredential(new DefaultAzureCredential());

                // Configure global retry mode
                builder.ConfigureDefaults(opts =>
                {
                    opts.Retry.MaxRetries = 5;
                    opts.Retry.Mode = RetryMode.Exponential;
                });
            });

            return services;
        }
        private static IServiceCollection AddApiService(this IServiceCollection services, ApiServiceSettings settings)
        {
            foreach (var key in settings.Apis.Keys)
            {
                services.AddHttpClient(key, client => client.BaseAddress = settings.GetBaseUri(key))
                    .AddHttpMessageHandler<HttpClientLoggingHandler>()
                    .AddTransientHttpErrorPolicy(policyBuilder =>
                        policyBuilder.WaitAndRetryAsync(settings.GetRetryCount(key), _ => settings.GetRetryDelay(key)));
            }
            return services.AddTransient<IApiService, ApiService>();
        }

        private static IServiceCollection AddAzureAD(this IServiceCollection services, AzureADSettings settings)
        {
            if (!string.IsNullOrEmpty(settings.TenantId) && !string.IsNullOrEmpty(settings.ClientId) && !string.IsNullOrEmpty(settings.ClientSecret))
            {
                var scopes = new[] { "https://graph.microsoft.com/.default" };
                services.AddSingleton(settings);
                services.AddSingleton(new GraphServiceClient(
                    new ClientSecretCredential(settings.TenantId,
                    settings.ClientId, settings.ClientSecret, new TokenCredentialOptions
                    {
                        AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
                    }), scopes));

                services.AddSingleton<IAzureADProvider, AzureADProvider>();
            }
            return services;
        }
    }
}