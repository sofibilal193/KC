using KC.Domain.Common.Entities.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KC.Persistence.Common
{
    public class SqlEventLogConfiguration : IEntityTypeConfiguration<SqlEventLog>
    {
        public void Configure(EntityTypeBuilder<SqlEventLog> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnOrder(1);
            builder.Property(e => e.Event).HasMaxLength(100).IsRequired().HasColumnOrder(2);
            builder.Property(e => e.DateTimeUtc).IsRequired().HasColumnOrder(3);
            builder.Property(e => e.Source).HasMaxLength(50).HasColumnOrder(4);
            builder.Property(e => e.Description).HasColumnOrder(5);
            builder.Property(e => e.UserId).HasColumnOrder(6);
            builder.Property(e => e.OrgId).HasColumnOrder(7);
            builder.Property(e => e.RecordId).HasColumnOrder(8);
            builder.Property(e => e.Timestamp).IsRowVersion().HasColumnOrder(99);
        }
    }
}
