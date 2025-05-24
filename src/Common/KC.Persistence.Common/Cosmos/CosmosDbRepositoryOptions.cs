using System.Collections.Generic;
using Microsoft.Azure.Cosmos;

namespace KC.Persistence.Common.Cosmos
{
    public record CosmosDbRepositoryOptions()
    {
        public string ApplicationName { get; init; } = "";

        public string? SubscriptionId { get; init; }

        public string? ResourceGroupName { get; init; }

        public string? AccountName { get; init; }

        public string EndPointUri { get; init; } = "";

        public string? EmulatorKey { get; init; }

        public string DbId { get; init; } = "";

        public string PreferredRegion { get; init; } = "";

        public int DbMaxThroughput { get; init; }

        public int ConnectionTimeoutSeconds { get; init; } = 30;

        public int SlidingCacheExpirationSeconds { get; init; } = 120;

        public IList<CosmosDbContainer>? Containers { get; init; }
    }

    public record CosmosDbContainer()
    {
        public string Id { get; init; } = "";

        public string Type { get; init; } = "";

        public string PartitionKeyPath { get; init; } = "";

        public int MaxThroughput { get; init; }

        public UniqueKeyPolicy? UniqueKeyPolicy { get; init; }

        public IndexingPolicy? IndexingPolicy { get; init; }

        public string SelfLink { get; private set; } = "";

        public void SetLink(string link)
        {
            SelfLink = link;
        }
    }
}
