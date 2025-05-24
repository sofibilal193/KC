using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using KC.Application.Common.BackgroundServices;
using KC.Application.Common.Settings;

namespace KC.Application.Common.IntegrationEvents
{
    [ExcludeFromCodeCoverage]
    public class IntegrationEventBackgroundService : SingleBackgroundService
    {
        public IntegrationEventBackgroundService(IServiceProvider serviceProvider, IHostApplicationLifetime lifetime,
            ILogger<IntegrationEventBackgroundService> logger, IOptionsMonitor<BackgroundServiceSettings> settings)
            : base(serviceProvider, lifetime, logger, settings.CurrentValue.Services["IntegrationEventBackgroundService"])
        {
        }

        public override async Task DoWorkAsync()
        {
            if (ServiceProvider != null)
            {
                using var scope = ServiceProvider.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IIntegrationEventService>();
                if (service is not null)
                {
                    await service.PublishEventsThroughEventBusAsync();
                }
            }
        }
    }
}
