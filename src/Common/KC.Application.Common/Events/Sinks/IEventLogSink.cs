using System.Threading;
using System.Threading.Tasks;
using KC.Domain.Common.Notifications;

namespace KC.Application.Common.Events
{
    public interface IEventLogSink
    {
        Task QueueEventAsync(EventLogNotification eventNotification,
            CancellationToken cancellationToken = default);

        Task FlushQueueAsync(CancellationToken cancellationToken = default);
    }
}
