using System;
using System.Threading;
using System.Threading.Tasks;
using KC.Domain.Common.Repositories;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace KC.Application.Common.Repositories
{
    public interface IDbContext : IAsyncDisposable, IUnitOfWork
    {
        DatabaseFacade Database { get; }

        bool HasActiveTransaction { get; }

        bool SupportsTransactions { get; }

        bool IsSql { get; }

        bool IsDocument { get; }

        bool IsDisposed { get; }

        Task<Guid?> BeginTransactionAsync(CancellationToken cancellationToken = default);

        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        void RollbackTransaction();

        IDbContextTransaction? GetCurrentTransaction();

        void Migrate();

        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    }
}
