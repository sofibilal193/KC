using KC.Domain.Common.Events;

namespace KC.Identity.API.IntegrationEvents
{
    public record OrgDeletedIntegrationEvent : IntegrationEvent
    {
        public int OrgId { get; init; }
        public OrgDeletedIntegrationEvent() : base() { }

        public OrgDeletedIntegrationEvent(DomainEvent domainEvent, int orgId)
                : base(domainEvent)
        {
            OrgId = orgId;
        }
    }
}
