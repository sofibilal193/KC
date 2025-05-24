using System;
using System.Text.Json.Serialization;
using KC.Domain.Common.Notifications;

namespace KC.Domain.Common.Entities.Events
{
    public class DocEventLog : EventLog
    {
        public string PartitionKey { get; set; } = "events";

        [JsonPropertyName("id")]
        public virtual string Id { get; set; } = "";

        /// <summary>
        /// Id of User who triggered the event
        /// </summary>
        public new string? UserId { get; }

        /// <summary>
        /// Id of Org who event is related to
        /// </summary>
        public new string? OrgId { get; }

        /// <summary>
        /// Id of record who event is related to e.g. unit, deal etc.
        /// </summary>
        public new string? RecordId { get; }

        [JsonPropertyName("_self")]
        public string? SelfLink { get; set; }

        [JsonPropertyName("_etag")]
        public string? ETag { get; set; }

        [JsonPropertyName("ttl")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? TimeToLive { get; set; }

        public DocEventLog() : base() { }

        public DocEventLog(string @event, DateTime dateTimeUtc, string? description, string source,
            string userId, string orgId, string recordId)
            : base(@event, dateTimeUtc, description, source)
        {
            Id = Guid.NewGuid().ToString();
            UserId = userId;
            OrgId = orgId;
            RecordId = recordId;
        }

        public DocEventLog(EventLogNotification @event)
            : base(@event.Event, @event.DateTimeUtc, @event.Description, @event.Source)
        {
            Id = Guid.NewGuid().ToString();
            UserId = @event.UserId;
            OrgId = @event.OrgId;
            RecordId = @event.RecordId;
        }
    }
}
