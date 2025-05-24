using KC.Domain.Common.Events;

namespace KC.Domain.Common.Entities.IntegrationEvents
{
    public class SqlIntegrationEventLog : IntegrationEventLog
    {
        public int Id { get; }

        public virtual byte[]? Timestamp { get; }

        /// <summary>
        /// Id of User who triggered the event
        /// </summary>
        public new int? UserId { get; }

        /// <summary>
        /// Id of Org who event is related to
        /// </summary>
        public new int? OrgId { get; }

        #region Constructors

        public SqlIntegrationEventLog() : base() { }

        public SqlIntegrationEventLog(IntegrationEvent @event, Guid? transactionId)
            : base(@event, transactionId)
        {
        }

        #endregion

    }
}
