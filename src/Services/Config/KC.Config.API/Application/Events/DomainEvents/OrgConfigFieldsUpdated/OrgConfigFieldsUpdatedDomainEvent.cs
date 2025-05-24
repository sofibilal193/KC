using KC.Domain.Common.Events;

namespace KC.Config.API.Application.Events
{
    public record OrgConfigFieldsUpdatedDomainEvent : DomainEvent
    {
        public List<string> DefaultSettingNames { get; init; }

        public OrgConfigFieldsUpdatedDomainEvent(List<string> defaultSettingNames)
        {
            DefaultSettingNames = defaultSettingNames;
        }
    }
}
