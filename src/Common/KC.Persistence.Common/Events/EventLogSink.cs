using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using KC.Application.Common.Events;
using KC.Application.Common.Repositories;
using KC.Domain.Common.Entities.Events;
using KC.Domain.Common.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace KC.Persistence.Common.Events
{
    public class EventLogSink<TContext> : IEventLogSink
        where TContext : IDbContext
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentQueue<EventLogNotification> _events = new();

        public EventLogSink(IServiceProvider serviceProvider)//, IOptionsMonitor<AppSettings> settingsAccessor)
        {
            _serviceProvider = serviceProvider;
        }

        public Task QueueEventAsync(EventLogNotification eventNotification, CancellationToken cancellationToken = default)
        {
            _events.Enqueue(eventNotification);
            return Task.CompletedTask;
        }

        public async Task FlushQueueAsync(CancellationToken cancellationToken = default)
        {
            if (_serviceProvider != null)
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<TContext>();
                while (context != null && !context.IsDisposed && !_events.IsEmpty)
                {
                    if (_events.TryDequeue(out EventLogNotification? item))
                    {
                        if (context.IsDocument)
                        {
                            context.Entry<DocEventLog>(new DocEventLog(item)).State = EntityState.Added;
                        }
                        else // assume sql or in memory
                        {
                            context.Entry<SqlEventLog>(new SqlEventLog(item)).State = EntityState.Added;
                        }
                    }
                }
                if (context is not null)
                {
                    await context.SaveChangesAsync(CancellationToken.None);
                }
            }
        }
    }
}
