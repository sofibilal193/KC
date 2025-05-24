using System;

namespace KC.Domain.Common.Entities.Events
{
    public abstract class EventLog
    {
        /// <summary>
        /// Id of User who triggered the event
        /// </summary>
        public virtual dynamic? UserId { get; }

        /// <summary>
        /// Id of Org who event is related to
        /// </summary>
        public virtual dynamic? OrgId { get; }

        /// <summary>
        /// Id of record who event is related to e.g. unit, deal etc.
        /// </summary>
        public virtual dynamic? RecordId { get; }

        public string Event { get; } = "";

        public string? Description { get; } = "";

        public DateTime DateTimeUtc { get; }

        public string? Source { get; }

        protected EventLog()
        {
        }

        protected EventLog(string @event, DateTime dateTimeUtc, string? description, string? source)
            : this()
        {
            DateTimeUtc = dateTimeUtc;
            Event = @event;
            Description = description;
            Source = source;
        }
    }
}
