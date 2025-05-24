using Microsoft.Extensions.Options;

namespace KC.Persistence.Common.Cosmos
{
    public record GetCosmosDbRepositoryOptions : CosmosDbRepositoryOptions
    {
        public GetCosmosDbRepositoryOptions(IOptionsMonitor<PersistenceSettings> settingsAccessor)
            : base(settingsAccessor.CurrentValue.CosmosDb)
        {
        }
    }
}
