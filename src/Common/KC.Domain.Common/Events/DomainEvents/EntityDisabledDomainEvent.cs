using KC.Domain.Common.Entities;

namespace KC.Domain.Common.Events
{
    public record EntityDisabledDomainEvent<TEntity> : DomainEvent where TEntity : BaseEntity
    {
        public TEntity? Entity { get; private set; }

        public EntityDisabledDomainEvent(TEntity? entity)
        {
            Entity = entity;
            SetUserSource(entity?.ModifyUserId, entity?.ModifyUserName, entity?.ModifySource);
        }

        public EntityDisabledDomainEvent(TEntity? entity, dynamic? eventOrgId)
        {
            Entity = entity;
            SetOrg(eventOrgId);
            SetUserSource(entity?.ModifyUserId, entity?.ModifyUserName, entity?.ModifySource);
        }
    }
}
