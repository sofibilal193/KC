using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Cosmos.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
using MediatR;
using Newtonsoft.Json.Linq;
using KC.Domain.Common;
using KC.Domain.Common.Entities;
using KC.Domain.Common.Entities.Events;
using KC.Domain.Common.Entities.IntegrationEvents;
using KC.Domain.Common.Identity;
using KC.Persistence.Common.Context;
using KC.Persistence.Common.Converters;
using KC.Utils.Common;
using KC.Utils.Common.Crypto;

namespace KC.Persistence.Common
{
    public abstract class BaseDocDbContext<TContext> : BaseDbContext<TContext>, IDocDbContext
        where TContext : DbContext
    {
        public DbSet<DocEventLog> EventLogs => Set<DocEventLog>();
        public DbSet<DocIntegrationEventLog> IntegrationEventLogs => Set<DocIntegrationEventLog>();

        public string? DbId { get; init; }

        protected BaseDocDbContext(DbContextOptions<TContext> options, ILogger<BaseDocDbContext<TContext>> logger,
            ICryptoProvider cryptoProvider, ICurrentUser currentUser, IDateTime dateTime, IMediator mediator)
            : base(options, logger, cryptoProvider, currentUser, dateTime, mediator)
        {
            var ext = Options.FindExtension<CosmosOptionsExtension>();
            DbId = ext?.DatabaseName;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            Logger.LogInformation("BaseDocDbContext:Assembly1: {Assembly}", GetType().Assembly);
            Logger.LogInformation("BaseDocDbContext:Assembly2: {Assembly}", Assembly.GetExecutingAssembly());

            // apply configurations for all types that inherit from DocEntity
            modelBuilder.ApplyConfigurationsFromAssembly(
                GetType().Assembly,
                t => t.GetInterfaces().ToList().Exists(i =>
                            i.IsGenericType &&
                            i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>) &&
                            typeof(DocEntity).IsAssignableFrom(i.GenericTypeArguments[0])));

            if (!Database.IsCosmos())
            {
                ConfigureInMemoryDatabase(modelBuilder);
            }
            InitEncryptionValueConverter(modelBuilder);
            modelBuilder.ApplyConfiguration(new DocEventLogConfiguration());
            modelBuilder.ApplyConfiguration(new DocIntegrationEventLogConfiguration());
        }

        private const string _nextIdProcedureName = "getNextId";
        private const string _autoNumberPartitionKey = "AutoNumber";

        public async Task<int> GetIdentityIdAsync(string container, string itemType,
            CancellationToken cancellationToken = default)
        {
            if (Database.IsCosmos())
            {
                var client = Database.GetCosmosClient();
                return await client.GetContainer(DbId, container)
                    .Scripts.ExecuteStoredProcedureAsync<int>(_nextIdProcedureName,
                        new PartitionKey(_autoNumberPartitionKey), new[] { itemType },
                        cancellationToken: cancellationToken);
            }
            else
            {
                return default;
            }
        }

        public async Task SaveJsonAsync(string containerName, string partitionKey, string json,
            string? id = null, TimeSpan? timeToLive = null, CancellationToken cancellationToken = default)
        {
            if (Database.IsCosmos() && DbId is not null)
            {
                var client = Database.GetCosmosClient();
                var container = client.GetContainer(DbId, containerName);
                var properties = await container.ReadContainerAsync(cancellationToken: cancellationToken);
                var partitionKeyPath = properties.Resource.PartitionKeyPath.TrimStart('/');
                try
                {
                    if (!string.IsNullOrEmpty(id))
                    {
                        // first attempt to get item
                        var itemResponse = await container.ReadItemAsync<JObject>(id: id,
                            partitionKey: new PartitionKey(partitionKey),
                            cancellationToken: cancellationToken);
                        if (itemResponse?.Resource != null)
                        {
                            var upsertResponse = await container.UpsertItemStreamAsync(json.GetJsonStream(partitionKeyPath,
                                partitionKey, id, itemResponse.ETag), new PartitionKey(partitionKey),
                                cancellationToken: cancellationToken);
                            upsertResponse.EnsureSuccessStatusCode();
                            return;
                        }
                    }
                }
                catch (CosmosException ce)
                {
                    if (ce.StatusCode != HttpStatusCode.NotFound)
                    {
                        Logger.LogError(ce, "An error has occurred while saving json. Container: {Container}. Item: {ItemId}. ParitionKey: {ParitionKey}",
                            containerName, id, partitionKey);
                        throw;
                    }
                }

                if (timeToLive.HasValue)
                {
                    json = json.Insert(json.Length - 1, $",\"ttl\": {(int)timeToLive.Value.TotalSeconds}");
                }
                var response = await container.CreateItemStreamAsync(json.GetJsonStream(partitionKeyPath,
                    partitionKey, id), new PartitionKey(partitionKey),
                    cancellationToken: cancellationToken);
                response.EnsureSuccessStatusCode();
                return;
            }
        }

        public async Task<JsonDocument?> GetJsonByIdAsync(string containerName,
            string partitionKey, string id, CancellationToken cancellationToken = default)
        {
            if (Database.IsCosmos() && DbId is not null)
            {
                var client = Database.GetCosmosClient();
                var container = client.GetContainer(DbId, containerName);
                try
                {
                    if (container != null)
                    {
                        var itemResponse = await container.ReadItemAsync<JObject>(id: id,
                            partitionKey: new PartitionKey(partitionKey),
                            cancellationToken: cancellationToken);
                        if (itemResponse?.Resource != null)
                            return JsonDocument.Parse(itemResponse.Resource.ToString());
                    }
                }
                catch (CosmosException ce)
                {
                    if (ce.StatusCode != HttpStatusCode.NotFound)
                    {
                        Logger.LogError(ce, "An error has occurred while retrieving the data. Container: {Container}. Item: {ItemId}. ParitionKey: {ParitionKey}",
                            containerName, id, partitionKey);
                        throw;
                    }
                }
            }
            return null;
        }

        public async Task<IEnumerable<T>> ExecuteQuery<T>(string containerName, string partitionKey,
            string query, Dictionary<string, object?>? parameters = null, CancellationToken cancellationToken = default)
        {
            var items = new List<T>();
            if (Database.IsCosmos() && DbId is not null)
            {
                var client = Database.GetCosmosClient();
                var container = client.GetContainer(DbId, containerName);
                if (container != null)
                {
                    var queryDefinition = new QueryDefinition(query);
                    if (parameters?.Count > 0)
                    {
                        foreach (var key in parameters.Keys)
                        {
                            queryDefinition = queryDefinition.WithParameter(key, parameters[key]);
                        }
                    }
                    var iterator = container.GetItemQueryIterator<T>(
                        queryDefinition, null, new QueryRequestOptions
                        {
                            PartitionKey = new PartitionKey(partitionKey)
                        });

                    while (iterator.HasMoreResults)
                    {
                        var response = await iterator.ReadNextAsync(cancellationToken);
                        items.AddRange(response.Resource);
                    }
                }
            }
            return items;
        }

        public async Task<IEnumerable<JsonDocument>> ExecuteQuery(string containerName,
            string partitionKey, string query, Dictionary<string, object?>? parameters = null, CancellationToken cancellationToken = default)
        {
            var items = new List<JsonDocument>();
            if (Database.IsCosmos() && DbId is not null)
            {
                var client = Database.GetCosmosClient();
                var container = client.GetContainer(DbId, containerName);
                if (container != null)
                {
                    var queryDefinition = new QueryDefinition(query);
                    if (parameters?.Count > 0)
                    {
                        foreach (var key in parameters.Keys)
                        {
                            queryDefinition = queryDefinition.WithParameter(key, parameters[key]);
                        }
                    }
                    var iterator = container.GetItemQueryIterator<JObject>(
                        queryDefinition, null, new QueryRequestOptions
                        {
                            PartitionKey = new PartitionKey(partitionKey)
                        });

                    while (iterator.HasMoreResults)
                    {
                        var response = await iterator.ReadNextAsync(cancellationToken);
                        items.AddRange(response.Resource.Select(r => JsonDocument.Parse(r.ToString())));
                        items.RemoveAll(i => i.RootElement.ToString() == "{}");
                    }
                }
            }
            return items;
        }

        public async Task DeleteItemAsync(string containerName, string partitionKey, string id, CancellationToken cancellationToken = default)
        {
            if (Database.IsCosmos() && DbId is not null)
            {
                var client = Database.GetCosmosClient();
                var container = client.GetContainer(DbId, containerName);
                if (container != null)
                {
                    await container.DeleteItemAsync<JsonDocument>(id, new PartitionKey(partitionKey), null, cancellationToken);
                }
            }
        }

        private static void ConfigureInMemoryDatabase(ModelBuilder modelBuilder)
        {
            // in-memory database doesn't support arrays, so set converter to convert IReadOnlyCollection to/from JSON
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    var propertyType = property.ClrType;
                    if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(IReadOnlyCollection<>))
                    {
                        var itemType = Nullable.GetUnderlyingType(propertyType.GenericTypeArguments[0]) ?? propertyType.GenericTypeArguments[0];
                        if (itemType.IsPrimitive || itemType == typeof(string) || itemType == typeof(decimal))
                        {
                            var converter = (ValueConverter?)Activator.CreateInstance(
                                typeof(JsonValueConverter<>).MakeGenericType(propertyType));
                            property.SetValueConverter(converter);
                        }
                    }
                }
            }
        }
    }
}
