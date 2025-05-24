using System;
using KC.Domain.Common.Notifications;
using MediatR;

namespace KC.Identity.API.Notifications
{
    public record IdentityEventLogNotification
        : EventLogNotification
    {
        public new int? UserId { get; init; }

        public new int? OrgId { get; init; }

        public new int? RecordId { get; init; }

        public IdentityEventLogNotification(string @event, DateTime dateTimeUtc,
            string? source, string description, int? userId, int? orgId, int? recordId)
                : base(@event, dateTimeUtc, source, description, userId, orgId, recordId)
        {
        }
    }
}
