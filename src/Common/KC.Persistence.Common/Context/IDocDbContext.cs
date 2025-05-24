using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace KC.Persistence.Common.Context
{
    public interface IDocDbContext
    {
        Task SaveJsonAsync(string containerName, string partitionKey, string json, string? id = null,
            TimeSpan? timeToLive = null, CancellationToken cancellationToken = default);

        Task<JsonDocument?> GetJsonByIdAsync(string containerName, string partitionKey,
            string id, CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> ExecuteQuery<T>(string containerName, string partitionKey, string query,
            Dictionary<string, object?>? parameters = null, CancellationToken cancellationToken = default);

        Task<IEnumerable<JsonDocument>> ExecuteQuery(string containerName, string partitionKey, string query,
            Dictionary<string, object?>? parameters = null, CancellationToken cancellationToken = default);

        Task DeleteItemAsync(string containerName, string partitionKey, string id, CancellationToken cancellationToken = default);
    }
}
