// Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
// See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace KC.Persistence.Common.Utils
{
    [ExcludeFromCodeCoverage]
    public sealed class ResilientTransaction
    {
        private readonly DbContext _context;

        private ResilientTransaction(DbContext context) =>
            _context = context ?? throw new ArgumentNullException(nameof(context));

        public static ResilientTransaction New(DbContext context) =>
            new(context);

        public async Task ExecuteAsync(Func<Task> action)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                await action();
                await transaction.CommitAsync();
            });
        }
    }
}
