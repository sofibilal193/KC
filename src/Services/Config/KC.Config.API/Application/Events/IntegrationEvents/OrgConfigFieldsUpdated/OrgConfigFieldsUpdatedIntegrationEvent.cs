using KC.Domain.Common.Events;

namespace KC.Config.API.Application.Events
{
    public record OrgConfigFieldsUpdatedIntegrationEvent : IntegrationEvent
    {
        public List<string> DefaultSettingNames { get; init; } = new();

        public OrgConfigFieldsUpdatedIntegrationEvent() { }

        public OrgConfigFieldsUpdatedIntegrationEvent(OrgConfigFieldsUpdatedDomainEvent domainEvent)
            : base(domainEvent)
        {
            DefaultSettingNames = domainEvent.DefaultSettingNames;
        }
    }
}
