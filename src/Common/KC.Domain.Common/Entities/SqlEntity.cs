namespace KC.Domain.Common.Entities
{
    public abstract class SqlEntity : BaseEntity, IAggregateRoot
    {
        public virtual int Id { get; }

        public virtual byte[]? Timestamp { get; }

        protected SqlEntity()
        {
        }

        public override dynamic GetId()
        {
            return Id;
        }

        public override bool IsTransient()
        {
            return Id == default;
        }
    }
}
