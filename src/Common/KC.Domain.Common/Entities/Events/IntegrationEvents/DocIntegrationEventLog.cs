using System;
using System.Text.Json.Serialization;
using KC.Domain.Common.Events;

namespace KC.Domain.Common.Entities.IntegrationEvents
{
    public class DocIntegrationEventLog : IntegrationEventLog
    {
        public string PartitionKey { get; set; } = "integration-events";

        [JsonPropertyName("id")]
        public override Guid EventId { get; }

        /// <summary>
        /// Id of User who triggered the event
        /// </summary>
        public override string? UserId { get; }

        /// <summary>
        /// Id of Org who event is related to
        /// </summary>
        public override string? OrgId { get; }

        [JsonPropertyName("_self")]
        public string? SelfLink { get; set; }

        [JsonPropertyName("_etag")]
        public string? ETag { get; set; }

        [JsonPropertyName("ttl")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? TimeToLive { get; set; }

        #region Constructors

        public DocIntegrationEventLog() : base() { }

        public DocIntegrationEventLog(IntegrationEvent @event, Guid? transactionId)
            : base(@event, transactionId)
        {
        }

        #endregion

    }
}
