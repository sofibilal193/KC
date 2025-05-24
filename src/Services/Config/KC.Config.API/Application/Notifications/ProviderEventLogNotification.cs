using KC.Domain.Common.Notifications;

namespace KC.Config.API.Application
{
    public record ProviderEventLogNotification : EventLogNotification
    {
        public new int? UserId { get; init; }

        public new int? OrgId { get; init; }

        /// <summary>
        /// The ProviderId
        /// </summary>
        public new int RecordId { get; init; }

        public ProviderEventLogNotification(string @event, DateTime dateTimeUtc,
            string? source, string? description, int? userId, int? orgId, int providerId)
                : base(@event, dateTimeUtc, source, description, userId, orgId, providerId)
        {
            UserId = userId;
            OrgId = orgId;
            RecordId = providerId;
        }
    }
}
