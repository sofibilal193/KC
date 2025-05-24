using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KC.Config.API.Entities;
using KC.Persistence.Common;
using KC.Persistence.Common.Extensions;

namespace KC.Config.API.Persistence
{
    public class UserConfigFieldConfiguration : SqlEntityConfiguration<UserConfigField>
    {
        public override void Configure(EntityTypeBuilder<UserConfigField> builder)
        {
            base.Configure(builder);

            builder.ToTableWithHistory();
            builder.Property(e => e.Category)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnOrder(1);
            builder.Property(e => e.FieldType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasColumnOrder(2);
            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnOrder(3);
            builder.Property(e => e.Description)
                .HasColumnOrder(4);
            builder.Property(e => e.DisplayOrder)
                .IsRequired()
                .HasColumnOrder(5);
            builder.Property(e => e.Values)
                .HasJsonConversion()
                .HasColumnOrder(6);
            builder.Property(e => e.DefaultValue)
                .HasColumnOrder(7);
            builder.Property(e => e.RegexValidator)
                .HasMaxLength(255)
                .HasColumnOrder(8);
            builder.Property(e => e.MinValue)
                .HasPrecision(12, 4)
                .HasColumnOrder(9);
            builder.Property(e => e.MaxValue)
                .HasPrecision(12, 4)
                .HasColumnOrder(10);
            builder.Property(e => e.IsUserVisible)
                .IsRequired()
                .HasColumnOrder(11);
        }
    }
}
