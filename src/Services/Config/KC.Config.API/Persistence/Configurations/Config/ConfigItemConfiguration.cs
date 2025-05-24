using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KC.Config.API.Entities;
using KC.Persistence.Common;
using KC.Persistence.Common.Extensions;

namespace KC.Config.API.Persistence
{
    public class ConfigItemConfiguration : SqlEntityConfiguration<ConfigItem>
    {
        public override void Configure(EntityTypeBuilder<ConfigItem> builder)
        {
            base.Configure(builder);

            builder.ToTableWithHistory();
            builder.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnOrder(1);
            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnOrder(2);
            builder.Property(e => e.Value)
                .HasColumnOrder(3);
            builder.Property(e => e.Description)
                .HasColumnOrder(4);
            builder.Property(e => e.DisplayOrder)
                .HasColumnOrder(5);
            builder.Property(e => e.IsInternal)
                .HasColumnOrder(6);
            builder.Property(e => e.IsEncrypted)
                .HasColumnOrder(7);

            builder.HasIndex(e => new { e.Type, e.Name })
                .IsUnique();
        }
    }
}
