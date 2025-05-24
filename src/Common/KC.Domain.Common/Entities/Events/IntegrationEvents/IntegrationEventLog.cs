using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using KC.Domain.Common.Events;

namespace KC.Domain.Common.Entities.IntegrationEvents
{
    public abstract class IntegrationEventLog
    {
        [JsonIgnore]
        [NotMapped]
        public IntegrationEvent? Event { get; protected set; }

        public string EventData { get; init; } = "";

        public virtual Guid EventId { get; }

        public string HostName { get; } = Dns.GetHostName().ToLowerInvariant();

        public string? EventTypeName { get; }

        public IntegrationEventState State { get; private set; }

        public int RetryCount { get; private set; }

        public DateTime CreateDateTimeUtc { get; }

        public DateTime? ModifyDateTimeUtc { get; private set; }

        public Guid? TransactionId { get; }

        /// <summary>
        /// Id of User who triggered the event
        /// </summary>
        public virtual dynamic? UserId { get; }

        /// <summary>
        /// Id of Org who event is related to
        /// </summary>
        public virtual dynamic? OrgId { get; }

        /// <summary>
        /// Client Ip address of user who triggered the event
        /// </summary>
        public string? Source { get; }

        [NotMapped]
        [JsonIgnore]
        public string? EventTypeShortName => EventTypeName?.Split('.')[^1];

        public IntegrationEventLog DeserializeJsonContent(Type type)
        {
            Event = JsonSerializer.Deserialize(EventData, type, new JsonSerializerOptions()
            { PropertyNameCaseInsensitive = true }) as IntegrationEvent;
            return this;
        }

        private readonly int _maxRetryCount = 5;

        public void MarkFailed()
        {
            if (State == IntegrationEventState.NotPublished)
            {
                RetryCount++;
                State = IntegrationEventState.InProgress;
            }
            else if (State == IntegrationEventState.InProgress
                && RetryCount < _maxRetryCount)
            {
                RetryCount++;
            }
            else
            {
                State = IntegrationEventState.PublishFailed;
            }
            ModifyDateTimeUtc = DateTime.UtcNow;
        }

        #region Constructors

        protected IntegrationEventLog() { }

        protected IntegrationEventLog(IntegrationEvent integrationEvent, Guid? transactionId)
        {
            Event = integrationEvent;
            EventId = integrationEvent.Id;
            CreateDateTimeUtc = integrationEvent.CreateDateTimeUtc;
            ModifyDateTimeUtc = integrationEvent.CreateDateTimeUtc;
            EventTypeName = integrationEvent.GetType().FullName;
            State = IntegrationEventState.NotPublished;
            RetryCount = 0;
            TransactionId = transactionId;
            Source = integrationEvent.EventSource;
            UserId = integrationEvent.EventUserId;
            OrgId = integrationEvent.EventOrgId;

            EventData = JsonSerializer.Serialize(integrationEvent, integrationEvent.GetType(), new JsonSerializerOptions
            {
                WriteIndented = false
            });
        }

        #endregion
    }
}
