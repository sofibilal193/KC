using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KC.Identity.API.Entities;
using KC.Persistence.Common;
using KC.Persistence.Common.Extensions;

namespace KC.Identity.API.Persistence.Configurations
{
    public class PermissionConfiguration : SqlEntityConfiguration<Permission>
    {
        public override void Configure(EntityTypeBuilder<Permission> builder)
        {
            base.Configure(builder);

            builder.ToTableWithHistory();
            builder.Property(e => e.Category)
                .IsRequired()
                .HasColumnOrder(1);
            builder.Property(e => e.Name)
                .IsRequired()
                .HasColumnOrder(2);
            builder.HasIndex(e => e.Name).IsUnique();
            builder.Property(e => e.Description)
                .HasColumnOrder(3);
            builder.HasIndex(e => e.Name).IsUnique();
        }
    }
}
