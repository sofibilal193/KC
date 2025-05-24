using KC.Domain.Common.Events;
using KC.Identity.API.DomainEvents;

namespace KC.Identity.API.IntegrationEvents
{
    /// <summary>
    /// Integration Event that gets fired when a org is added, updated, enabled or disabled
    /// </summary>
    public record OrgUpsertedIntegrationEvent : IntegrationEvent
    {
        public int OrgId { get; init; }

        public string OrgName { get; init; } = string.Empty;

        public string ActionType { get; init; } = string.Empty;

        public OrgUpsertedIntegrationEvent() { }

        public OrgUpsertedIntegrationEvent(OrgUpsertedDomainEvent domainEvent)
            : base(domainEvent)
        {
            OrgId = domainEvent.OrgId;
            OrgName = domainEvent.OrgName;
            ActionType = domainEvent.ActionType;
        }
    }
}
