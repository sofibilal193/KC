using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using KC.Application.Common.Events;

namespace KC.Identity.API.Notifications
{
    public class IdentityEventLogNotificationHandler : INotificationHandler<IdentityEventLogNotification>
    {
        private readonly ILogger<IdentityEventLogNotificationHandler> _logger;
        private readonly IEventLogSink _eventLogSink;

        public IdentityEventLogNotificationHandler(ILogger<IdentityEventLogNotificationHandler> logger,
            IEventLogSink eventLogSink)
        {
            _logger = logger;
            _eventLogSink = eventLogSink;
        }

        public async Task Handle(IdentityEventLogNotification notification,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Event: '{event}' was raised by User #{userId} for Org #{orgId} via {clientIp} for Record #{recordId} with description: '{description}'.",
                notification.Event, notification.UserId, notification.OrgId, notification.Source, notification.RecordId, notification.Description);
            await _eventLogSink.QueueEventAsync(notification, cancellationToken);
        }
    }
}
