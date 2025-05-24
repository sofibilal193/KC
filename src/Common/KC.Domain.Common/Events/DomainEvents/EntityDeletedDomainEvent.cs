using KC.Domain.Common.Entities;

namespace KC.Domain.Common.Events
{
    public record EntityDeletedDomainEvent<TEntity> : DomainEvent where TEntity : BaseEntity
    {
        public TEntity? Entity { get; private set; }

        public EntityDeletedDomainEvent(TEntity? entity)
        {
            Entity = entity;
            SetUserSource(entity?.ModifyUserId, entity?.CreateUserName, entity?.ModifySource);
        }

        public EntityDeletedDomainEvent(TEntity? entity, dynamic? eventOrgId)
        {
            Entity = entity;
            SetOrg(eventOrgId);
            SetUserSource(entity?.ModifyUserId, entity?.CreateUserName, entity?.ModifySource);
        }
    }
}
