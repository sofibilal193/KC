using KC.Persistence.Common.Cosmos;
using KC.Persistence.Common.Settings;

namespace KC.Persistence.Common
{
    public record PersistenceSettings
    {
        public CosmosDbRepositoryOptions CosmosDb { get; init; } = new();

        public SqlServerSettings SqlServer { get; init; } = new();
    }
}
