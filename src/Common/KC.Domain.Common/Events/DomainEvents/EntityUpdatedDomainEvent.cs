using KC.Domain.Common.Entities;

namespace KC.Domain.Common.Events
{
    public record EntityUpdatedDomainEvent<TEntity> : DomainEvent where TEntity : BaseEntity
    {
        public TEntity? Entity { get; private set; }

        public EntityUpdatedDomainEvent(TEntity? entity)
        {
            Entity = entity;
            SetUserSource(entity?.ModifyUserId, entity?.ModifyUserName, entity?.ModifySource);
        }

        public EntityUpdatedDomainEvent(TEntity? entity, dynamic? eventOrgId)
        {
            Entity = entity;
            SetOrg(eventOrgId);
            SetUserSource(entity?.ModifyUserId, entity?.ModifyUserName, entity?.ModifySource);
        }
    }
}
