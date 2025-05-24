using KC.Domain.Common.Entities.IntegrationEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KC.Persistence.Common
{
    public class DocIntegrationEventLogConfiguration : IEntityTypeConfiguration<DocIntegrationEventLog>
    {
        private readonly string _eventLogContainer = "integrationevents";

        public void Configure(EntityTypeBuilder<DocIntegrationEventLog> builder)
        {
            builder.ToContainer(_eventLogContainer);
            builder.HasPartitionKey(e => e.PartitionKey);
            builder.HasNoDiscriminator();
            builder.UseETagConcurrency();
            builder.HasKey(e => e.EventId);
            builder.Property(e => e.EventId)
                .IsRequired().HasColumnOrder(1);
            builder.Property(e => e.HostName)
                .IsRequired().HasMaxLength(100)
                .HasColumnOrder(2);
            builder.Property(e => e.EventTypeName)
                .IsRequired().HasMaxLength(100)
                .HasColumnOrder(3);
            builder.Property(e => e.State)
                .IsRequired().HasConversion<string>()
                .HasMaxLength(25).HasColumnOrder(4);
            builder.Property(e => e.RetryCount).HasColumnOrder(5);
            builder.Property(e => e.TransactionId).HasColumnOrder(6);
            builder.Property(e => e.UserId).HasColumnOrder(7);
            builder.Property(e => e.OrgId).HasColumnOrder(8);
            builder.Property(e => e.Source)
                .HasMaxLength(25).HasColumnOrder(9);
            builder.Property(e => e.EventData)
                .IsRequired().HasColumnOrder(10);
            builder.Property(e => e.CreateDateTimeUtc)
                .IsRequired().HasColumnOrder(21);
            builder.Property(e => e.ModifyDateTimeUtc)
                .IsRequired().HasColumnOrder(22);

            builder.HasIndex(e => new { e.State, e.EventTypeName });
        }
    }
}
