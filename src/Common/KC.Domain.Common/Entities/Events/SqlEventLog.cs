using System;
using KC.Domain.Common.Notifications;

namespace KC.Domain.Common.Entities.Events
{
    public class SqlEventLog : EventLog
    {
        public int Id { get; }

        /// <summary>
        /// Id of User who triggered the event
        /// </summary>
        public new int? UserId { get; }

        /// <summary>
        /// Id of Org who event is related to
        /// </summary>
        public new int? OrgId { get; }

        /// <summary>
        /// Id of record who event is related to e.g. unit, deal etc.
        /// </summary>
        public new int? RecordId { get; }

        public virtual byte[]? Timestamp { get; }

        public SqlEventLog() : base() { }

        public SqlEventLog(string @event, DateTime dateTimeUtc, string? description, string source,
            int? userId, int? orgId, int? recordId)
            : base(@event, dateTimeUtc, description, source)
        {
            UserId = userId;
            OrgId = orgId;
            RecordId = recordId;
        }

        public SqlEventLog(EventLogNotification @event)
            : base(@event.Event, @event.DateTimeUtc, @event.Description, @event.Source)
        {
            UserId = @event.UserId;
            OrgId = @event.OrgId;
            RecordId = @event.RecordId;
        }
    }
}
