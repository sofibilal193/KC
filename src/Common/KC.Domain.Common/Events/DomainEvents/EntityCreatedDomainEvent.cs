using KC.Domain.Common.Entities;

namespace KC.Domain.Common.Events
{
    public record EntityCreatedDomainEvent<TEntity> : DomainEvent where TEntity : BaseEntity
    {
        public TEntity? Entity { get; private set; }

        public EntityCreatedDomainEvent(TEntity? entity)
        {
            Entity = entity;
            SetUserSource(entity?.CreateUserId, entity?.CreateUserName, entity?.CreateSource);
        }

        public EntityCreatedDomainEvent(TEntity? entity, dynamic? eventOrgId)
        {
            Entity = entity;
            SetOrg(eventOrgId);
            SetUserSource(entity?.CreateUserId, entity?.CreateUserName, entity?.CreateSource);
        }
    }
}
