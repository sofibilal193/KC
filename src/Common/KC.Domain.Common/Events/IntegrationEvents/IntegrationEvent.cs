using System.Text.Json.Serialization;
using KC.Domain.Common.Converters;

namespace KC.Domain.Common.Events
{
    /// <summary>
    /// Base event for integration events
    /// </summary>
    public abstract record IntegrationEvent
    {
        /// <summary>
        /// Unique event identifier
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Event creation date and time
        /// </summary>
        public DateTime CreateDateTimeUtc { get; init; }

        /// <summary>
        /// Id of User who triggered the event
        /// </summary>
        [JsonConverter(typeof(DynamicJsonConverter))]
        public dynamic? EventUserId { get; init; }

        /// <summary>
        /// Name of user who triggered the event
        /// </summary>
        public string? EventUserName { get; init; }

        /// <summary>
        /// Id of Org who event is related to
        /// </summary>
        [JsonConverter(typeof(DynamicJsonConverter))]
        public dynamic? EventOrgId { get; init; }

        /// <summary>
        /// Client Ip address of user who triggered the event
        /// </summary>
        public string? EventSource { get; init; }

        protected IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreateDateTimeUtc = DateTime.UtcNow;
        }

        protected IntegrationEvent(Guid id, DateTime createDateTimeUtc)
        {
            Id = id;
            CreateDateTimeUtc = createDateTimeUtc;
        }

        protected IntegrationEvent(Guid id, DateTime createDateTimeUtc,
            dynamic eventUserId, string eventUserName, dynamic eventOrgId, string eventSource)
        {
            Id = id;
            CreateDateTimeUtc = createDateTimeUtc;
            EventUserId = eventUserId;
            EventUserName = eventUserName;
            EventOrgId = eventOrgId;
            EventSource = eventSource;
        }

        protected IntegrationEvent(dynamic eventUserId, string eventUserName, dynamic eventOrgId, string eventSource)
            : this()
        {
            EventUserId = eventUserId;
            EventUserName = eventUserName;
            EventOrgId = eventOrgId;
            EventSource = eventSource;
        }

        protected IntegrationEvent(DomainEvent? domainEvent)
            : this()
        {
            EventUserId = domainEvent?.EventUserId;
            EventUserName = domainEvent?.EventUserName;
            EventOrgId = domainEvent?.EventOrgId;
            EventSource = domainEvent?.EventSource;
        }
    }
}
