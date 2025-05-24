using System;
using System.Linq;
using KC.Domain.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace KC.Persistence.Common.Cosmos
{
    public static class CosmosUtils
    {
        public static string GenerateId(DocEntity entity) => $"{entity.PartitionKey}:{Guid.NewGuid()}";

        public static string GenerateId(string partitionKey) => $"{partitionKey}:{Guid.NewGuid()}";

        public static string ResolvePartitionKey(string entityId) =>
            entityId.Contains(':') ? entityId[..entityId.LastIndexOf(':')] : "";

        public static string ResolveIdFromPartitionKey(string partitionKey) => partitionKey.Split(':')[^1];

        public static IQueryable<TEntity> UsePartitionKey<TEntity>(this IQueryable<TEntity> source, bool isCosmos, [NotParameterized] string partitionKey)
            where TEntity : class
        {
            if (isCosmos)
            {
                return source.WithPartitionKey(partitionKey);
            }
            else
            {
                return source;
            }
        }
    }
}
