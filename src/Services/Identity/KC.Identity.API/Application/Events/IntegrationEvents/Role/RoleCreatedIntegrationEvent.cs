using KC.Domain.Common.Events;
using KC.Identity.API.Entities;

namespace KC.Identity.API.IntegrationEvents
{
    /// <summary>
    /// Integration event used when a role is created.
    /// </summary>
    public record RoleCreatedIntegrationEvent : IntegrationEvent
    {
        public int RoleId { get; init; }
        public string? RoleName { get; init; }

        public RoleCreatedIntegrationEvent() { }

        public RoleCreatedIntegrationEvent(EntityCreatedDomainEvent<Role> domainEvent)
            : base(domainEvent)
        {
            RoleId = domainEvent.Entity?.Id ?? 0;
            RoleName = domainEvent.Entity?.Name;
        }
    }
}
