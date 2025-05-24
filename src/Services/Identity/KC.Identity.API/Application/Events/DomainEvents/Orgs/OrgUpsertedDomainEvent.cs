using KC.Domain.Common.Events;

namespace KC.Identity.API.DomainEvents
{
    /// <summary>
    /// Integration Event that gets fired when a org is added, updated, enabled or disabled
    /// </summary>
    public record OrgUpsertedDomainEvent : DomainEvent
    {
        public int OrgId { get; init; }

        public string OrgName { get; init; } = string.Empty;

        public string ActionType { get; init; } = string.Empty;

        public OrgUpsertedDomainEvent() { }

        public OrgUpsertedDomainEvent(int orgId, string orgName, string actionType)
        {
            OrgId = orgId;
            OrgName = orgName;
            ActionType = actionType;
        }
    }
}
