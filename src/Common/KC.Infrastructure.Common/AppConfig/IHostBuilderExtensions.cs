using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Azure.Identity;

namespace KC.Infrastructure.Common.AppConfig
{
    public static class IHostBuilderExtensions
    {
        public static IHostBuilder CreateHostBuilder<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors
            | DynamicallyAccessedMemberTypes.PublicMethods)] TStartup>(this string[] args)
                where TStartup : class
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging((ctx, logging) =>
                {
                    var env = ctx.HostingEnvironment;
                    AddLogging(env, logging);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(serverOptions =>
                    {
                        // Set properties and call methods on options
                        serverOptions.AddServerHeader = false;
                    })
                    .UseStartup<TStartup>();
                });
        }

        public static void AddLogging(this IHostEnvironment env, ILoggingBuilder loggingBuilder)
        {
            if (!env.IsDevelopment() && !env.IsEnvironment("Test"))
            {
                loggingBuilder.AddAzureWebAppDiagnostics();
                loggingBuilder.AddApplicationInsights();
            }
            else
            {
                loggingBuilder.AddDebug();
                loggingBuilder.AddConsole();
            }
        }

        public static IHostBuilder AddAppConfig(this IHostBuilder builder)
        {
            return builder.ConfigureAppConfiguration((builder) =>
            {
                var config = builder.Build();
                var uri = config?.GetSection("InfraSettings")?.GetValue<string>("AzureAppConfigUri");
                if (!string.IsNullOrEmpty(uri))
                {
                    var creds = new DefaultAzureCredential();
                    var sentinelKey = config?.GetSection("InfraSettings")?.GetValue<string>("AzureAppConfigSentinelKey");
                    var cacheDuration = config?.GetSection("InfraSettings")?.GetValue<TimeSpan?>("AzureAppConfigCachePeriod");

                    builder.AddAzureAppConfiguration(options =>
                    {
                        options.Connect(uri.GetUris(), creds)
                        .UseFeatureFlags()
                        .ConfigureKeyVault(kv => kv.SetCredential(creds));
                        if (!string.IsNullOrEmpty(sentinelKey) || cacheDuration.HasValue)
                        {
                            options.ConfigureRefresh(refresh =>
                            {
                                if (!string.IsNullOrEmpty(sentinelKey))
                                {
                                    refresh.Register(key: sentinelKey, refreshAll: true);
                                }
                                if (cacheDuration.HasValue)
                                {
                                    refresh.SetCacheExpiration(cacheDuration.Value);
                                }
                            });
                        }
                    });
                }
            });
        }

        private static Uri[] GetUris(this string value)
        {
            List<Uri> endpoints = new();
            if (value.Contains(';'))
            {
                var uris = value.Split(";", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                foreach (var uri in uris)
                {
                    endpoints.Add(new Uri(uri));
                }
            }
            else
            {
                endpoints.Add(new Uri(value));
            }
            return endpoints.ToArray();
        }
    }
}