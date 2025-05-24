using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KC.Domain.Common.Entities;

namespace KC.Persistence.Common.Repositories
{
    /// <summary>
    /// Base repository for managing entity models configured with SQL Server Database.
    /// </summary>
    /// <typeparam name="TEntity">Entity model type</typeparam>
    public abstract class SqlRepository<TEntity> : DbRepository<TEntity>, ISqlRepository<TEntity> where TEntity : SqlEntity
    {
        protected SqlRepository(DbContext context) : base(context)
        {
        }

        /// <summary>
        /// The change tracker will not track any of the entities that are returned from
        /// a LINQ query. If the entity instances are modified, this will not be detected
        /// by the change tracker and Microsoft.EntityFrameworkCore.DbContext.SaveChanges
        /// will not persist those changes to the database.
        /// </summary>
        /// <returns>The same ISqlRepository implementation so that multiple calls can be chained.</returns>
        public new ISqlRepository<TEntity> AsNoTracking()
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
        /// <returns>The same ISqlRepository implementation so that multiple calls can be chained.</returns>
        public new ISqlRepository<TEntity> Include(Expression<Func<TEntity, object?>> navigationPropertyPath)
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
        /// <returns>The same ISqlRepository implementation so that multiple calls can be chained.</returns>
        public new ISqlRepository<TEntity> Include(string navigationPropertyPath)
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
        public async Task<TEntity?> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            return await Entities.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }
    }
}
