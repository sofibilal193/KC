using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MediatR;
using KC.Domain.Common;
using KC.Domain.Common.Entities.Events;
using KC.Domain.Common.Entities.IntegrationEvents;
using KC.Domain.Common.Identity;
using KC.Utils.Common.Crypto;

namespace KC.Persistence.Common.Events
{
    public class EventSqlDbContext : BaseSqlDbContext<EventSqlDbContext>
    {
        public override string DefaultSchema => "event";

        public DbSet<SqlEventLog> EventLogs => Set<SqlEventLog>();

        public DbSet<SqlIntegrationEventLog> IntegrationEventLogs => Set<SqlIntegrationEventLog>();

        public EventSqlDbContext(DbContextOptions<EventSqlDbContext> options, ILogger<EventSqlDbContext> logger, ICryptoProvider cryptoProvider, ICurrentUser currentUser, IDateTime dateTime, IMediator mediator)
            : base(options, logger, cryptoProvider, currentUser, dateTime, mediator)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration<SqlEventLog>(new SqlEventLogConfiguration());
            modelBuilder.ApplyConfiguration<SqlIntegrationEventLog>(new SqlIntegrationEventLogConfiguration());
        }
    }
}
