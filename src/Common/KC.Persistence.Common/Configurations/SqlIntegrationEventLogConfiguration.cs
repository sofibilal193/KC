using KC.Domain.Common.Entities.IntegrationEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KC.Persistence.Common
{
    public class SqlIntegrationEventLogConfiguration : IEntityTypeConfiguration<SqlIntegrationEventLog>
    {
        public void Configure(EntityTypeBuilder<SqlIntegrationEventLog> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnOrder(1);
            builder.Property(e => e.EventId)
                .IsRequired().HasColumnOrder(2);
            builder.Property(e => e.HostName)
                .IsRequired().HasMaxLength(100)
                .HasColumnOrder(3);
            builder.Property(e => e.EventTypeName)
                .IsRequired().HasMaxLength(100)
                .HasColumnOrder(4);
            builder.Property(e => e.State)
                .IsRequired().HasConversion<string>()
                .HasMaxLength(25).HasColumnOrder(5);
            builder.Property(e => e.RetryCount).HasColumnOrder(6);
            builder.Property(e => e.TransactionId).HasColumnOrder(7);
            builder.Property(e => e.UserId).HasColumnOrder(8);
            builder.Property(e => e.OrgId).HasColumnOrder(9);
            builder.Property(e => e.Source)
                .HasMaxLength(50).HasColumnOrder(10);
            builder.Property(e => e.EventData)
                .IsRequired().HasColumnOrder(11);
            builder.Property(e => e.CreateDateTimeUtc)
                .IsRequired().HasColumnOrder(21);
            builder.Property(e => e.ModifyDateTimeUtc)
                .IsRequired().HasColumnOrder(22);
            builder.Property(e => e.Timestamp)
                .IsRowVersion().HasColumnOrder(99);

            builder.HasIndex(e => e.EventId).IsUnique();
            builder.HasIndex(e => new { e.State, e.EventTypeName });
        }
    }
}
