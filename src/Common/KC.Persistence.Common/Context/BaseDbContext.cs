using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using MediatR;
using KC.Application.Common.Repositories;
using KC.Domain.Common;
using KC.Domain.Common.Identity;
using KC.Persistence.Common.Utils;
using KC.Utils.Common.Crypto;

namespace KC.Persistence.Common
{
    public abstract class BaseDbContext<TContext> : DbContext, IDbContext
        where TContext : DbContext
    {
        public virtual string DefaultSchema => "base";

        protected ICurrentUser CurrentUser { get; init; }
        protected IDateTime DateTime { get; init; }

        protected ILogger<BaseDbContext<TContext>> Logger { get; init; }

        protected ICryptoProvider CryptoProvider { get; init; }
        protected IMediator Mediator { get; init; }
        private IDbContextTransaction? _currentTransaction;

        protected DbContextOptions<TContext> Options { get; init; }

        public IDbContextTransaction? GetCurrentTransaction() => _currentTransaction;

        public bool HasActiveTransaction => _currentTransaction != null;

        protected BaseDbContext(DbContextOptions<TContext> options, ILogger<BaseDbContext<TContext>> logger, ICryptoProvider cryptoProvider, ICurrentUser currentUser, IDateTime dateTime, IMediator mediator)
            : base(options)
        {
            Logger = logger;
            CryptoProvider = cryptoProvider;
            CurrentUser = currentUser;
            DateTime = dateTime;
            Mediator = mediator;
            Options = options;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                Logger.LogInformation("BaseDbContext OnConfiguring: {Type}. Guid: {Guid}.", typeof(TContext), Guid.NewGuid());
                optionsBuilder.EnableDetailedErrors();
                optionsBuilder.EnableSensitiveDataLogging();
            }
        }

        public bool IsDisposed { get; private set; }

        public bool IsSql
        {
            get
            {
                return Database.IsSqlServer();
            }
        }

        public bool IsDocument
        {
            get
            {
                return Database.IsCosmos();
            }
        }

        public bool SupportsTransactions
        {
            get
            {
                return Database.IsRelational();
            }
        }

        #region IDisposable Methods

        public sealed override void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        // https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync
        public override async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);
            Dispose(disposing: false);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                base.Dispose();
            }
            IsDisposed = true;
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            // dispose resources
            await base.DisposeAsync();
        }

        #endregion

        protected void InitEncryptionValueConverter(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    var annotation = property.FindAnnotation(CustomAnnotations.Encryption);
                    if (annotation is not null)
                    {
                        var converter = new EncryptedStringValueConverter(CryptoProvider);
                        property.SetValueConverter(converter);

                        // adjust max length to accomodate for length of encrypted strings
                        var maxLength = property.GetMaxLength();
                        if (maxLength.HasValue)
                        {
                            var testValue = new string('A', maxLength.Value);
                            var convertedValue = converter.ConvertToProvider.Invoke(testValue) as string;
                            if (convertedValue is not null)
                            {
                                property.SetMaxLength(convertedValue.Length);
                            }
                        }
                    }
                }
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ChangeTracker.DetectChanges();
            this.TrackEntityChanges(CurrentUser, DateTime);
            return await base.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            // Dispatch Domain Events collection.
            // Choices:
            // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including
            // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
            // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions.
            // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers.

            // After executing this line all the changes (from the Command Handler and Domain Event Handlers)
            // performed through the DbContext will be committed
            await SaveChangesAsync(cancellationToken);

            // PR 5/25/22: Decided to do this after Save() so that Views will get proper IDs on EntityCreated events.
            await Mediator.DispatchDomainEventsAsync(this, CurrentUser, cancellationToken);

            return true;
        }

        public async Task<Guid?> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (!SupportsTransactions) return null;
            if (_currentTransaction != null) return _currentTransaction.TransactionId;
            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted,
                cancellationToken: cancellationToken);
            return _currentTransaction.TransactionId;
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            // if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            // if (!HasActiveTransaction) throw new InvalidOperationException("No active transaction is present.");
            if (!HasActiveTransaction) return;

            try
            {
                await SaveChangesAsync(cancellationToken);
                if (_currentTransaction is not null)
                    await _currentTransaction.CommitAsync(cancellationToken);
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void Migrate()
        {
            var applied = this.GetService<IHistoryRepository>()
                .GetAppliedMigrations()
                .Select(m => m.MigrationId);

            var total = this.GetService<IMigrationsAssembly>()
                .Migrations
                .Select(m => m.Key);

            if (total.Except(applied).Any())
            {
                Database.Migrate();
            }
        }
    }
}
