using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KC.Domain.Common.Entities;
using KC.Persistence.Common.Cosmos;

namespace KC.Persistence.Common.Repositories
{
    /// <summary>
    /// Base repository for managing entity models configured with Cosmos Database.
    /// </summary>
    /// <typeparam name="TEntity">Entity model type</typeparam>
    public abstract class DocRepository<TEntity, TContext> : DbRepository<TEntity>, IDocRepository<TEntity>
        where TEntity : DocEntity where TContext : DbContext
    {
        private readonly BaseDocDbContext<TContext> _context;

        /// <summary>
        /// Creates a new database repository with the specified logger.
        /// </summary>
        /// <param name="context">Database context containing entity definitions.</param>
        /// <param name="containerName">Name of database container.</param>
        protected DocRepository(BaseDocDbContext<TContext> context) : base(context)
        {
            _context = context;
        }

        public string GenerateId(TEntity entity) => CosmosUtils.GenerateId(entity);

        public string ResolvePartitionKey(string entityId) => CosmosUtils.ResolvePartitionKey(entityId);

        /// <summary>
        /// The change tracker will not track any of the entities that are returned from
        /// a LINQ query. If the entity instances are modified, this will not be detected
        /// by the change tracker and Microsoft.EntityFrameworkCore.DbContext.SaveChanges
        /// will not persist those changes to the database.
        /// </summary>
        /// <returns>The same IDocRepository implementation so that multiple calls can be chained.</returns>
        public new IDocRepository<TEntity> AsNoTracking()
        {
            base.AsNoTracking();
            return this;
        }

        /// <summary>
        /// Specifies related entities to include in the query results. The navigation property
        /// to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <param name="navigationPropertyPath">A lambda expression representing the navigation
        /// property to be included (t => t.Property1).</param>
        /// <returns>The same IDocRepository implementation so that multiple calls can be chained.</returns>
        public new IDocRepository<TEntity> Include(Expression<Func<TEntity, object?>> navigationPropertyPath)
        {
            base.Include(navigationPropertyPath);
            return this;
        }

        /// <summary>
        /// Specifies related entities to include in the query results. The navigation property
        /// to be included is specified starting with the type of entity being queried (TEntity).
        /// Further navigation properties to be included can be appended, separated by the '.' character.
        /// </summary>
        /// <param name="navigationPropertyPath">A string of '.' separated navigation property names to be included.</param>
        /// <returns>The same IDocRepository implementation so that multiple calls can be chained.</returns>
        public new IDocRepository<TEntity> Include(string navigationPropertyPath)
        {
            base.Include(navigationPropertyPath);
            return this;
        }

        /// <summary>
        /// Gets an entity with the specified Id.
        /// </summary>
        /// <param name="id">Id of entity to get.</param>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>The entity found, or null.</returns>
        public async Task<TEntity?> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return await Entities.UsePartitionKey(_context.Database.IsCosmos(), ResolvePartitionKey(id))
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        /// <summary>
        /// Returns entities matching the given query
        /// </summary>
        /// <param name="partitionKey">The partition key.</param>
        /// <param name="predicate">The query predicate.</param>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A list of entities that matched the query predicate.</returns>
        public async Task<List<TEntity>> GetAsync(string partitionKey, Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await Entities.UsePartitionKey(_context.Database.IsCosmos(), partitionKey)
                .Where(predicate).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Returns all entities with the specified partition key.
        /// </summary>
        /// <param name="partitionKey">The partition key.</param>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A list of entities.</returns>
        public async Task<List<TEntity>> GetAllAsync(string partitionKey, CancellationToken cancellationToken = default)
        {
            return await Entities.UsePartitionKey(_context.Database.IsCosmos(), partitionKey)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Adds an entity to the repository.
        /// </summary>
        /// <param name="entity">Entity to add to repository.</param>
        public override void Add(TEntity entity)
        {
            entity.SetId(GenerateId(entity));
            _context.Add(entity);
        }

        /// <summary>
        /// Adds a collection of entities to the repository.
        /// </summary>
        /// <param name="entities">Collection of entities to add.</param>
        public override void AddRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                entity.SetId(GenerateId(entity));
            }
            _context.AddRange(entities);
        }
    }
}
