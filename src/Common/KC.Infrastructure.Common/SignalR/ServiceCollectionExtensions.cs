using System;
using System.Threading;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.SignalR;
using Microsoft.Azure.SignalR.Management;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Azure.Identity;
using System.Diagnostics.CodeAnalysis;

namespace KC.Infrastructure.Common.SignalR
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHubContext<THubClient, THub>(this IServiceCollection services, IConfiguration configuration)
            where THubClient : class
            where THub : Hub<THubClient>
        {
            var settings = configuration.GetSection("InfraSettings")
                .GetSection("SignalRSettings").Get<AzureSignalRSettings>();
            if (settings?.AzureServiceEnabled == true)
            {
                var manager = new ServiceManagerBuilder().WithOptions(options =>
                {
                    options.ServiceEndpoints = new ServiceEndpoint[]
                    {
                        new ServiceEndpoint(new Uri(settings.AzureServiceUri), new DefaultAzureCredential())
                    };
                }).BuildServiceManager();
                var hubContext = manager.CreateHubContextAsync<THubClient>(
                    typeof(THub).Name, CancellationToken.None).GetAwaiter().GetResult();
                services.AddSingleton(new ServiceDescriptor(typeof(IHubContext<THub>), hubContext));
            }

            return services;
        }
    }
}
