using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using KC.Application.Common.BackgroundServices;
using KC.Application.Common.Settings;

namespace KC.Application.Common.Events
{
    [ExcludeFromCodeCoverage]
    public class EventLogSinkBackgroundService : SingleBackgroundService
    {
        public EventLogSinkBackgroundService(IServiceProvider serviceProvider, IHostApplicationLifetime lifetime,
            ILogger<EventLogSinkBackgroundService> logger, IOptionsMonitor<BackgroundServiceSettings> settings)
            : base(serviceProvider, lifetime, logger, settings.CurrentValue.Services["EventLogSinkBackgroundService"])
        {
        }

        public override async Task DoWorkAsync()
        {
            if (ServiceProvider != null)
            {
                using var scope = ServiceProvider.CreateScope();
                var sink = scope.ServiceProvider.GetService<IEventLogSink>();
                if (sink is not null)
                {
                    await sink.FlushQueueAsync();
                }
            }
        }
    }
}
