using KC.Domain.Common.Entities;

namespace KC.Persistence.Common.Cosmos
{
    public interface IContainerContext<in T> where T : DocEntity
    {
        string GenerateId(T entity);

        string ResolvePartitionKey(string entityId);
    }
}
