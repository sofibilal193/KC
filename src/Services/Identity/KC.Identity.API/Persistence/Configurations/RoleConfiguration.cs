using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KC.Identity.API.Entities;
using KC.Persistence.Common;
using KC.Persistence.Common.Extensions;

namespace KC.Identity.API.Persistence.Configurations
{
    public class RoleConfiguration : SqlEntityConfiguration<Role>
    {
        public override void Configure(EntityTypeBuilder<Role> builder)
        {
            base.Configure(builder);

            builder.ToTableWithHistory();
            builder.Property(e => e.Type)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(25)
                .HasColumnOrder(1);
            builder.Property(e => e.OrgType)
                .HasConversion<string>()
                .HasMaxLength(25)
                .HasColumnOrder(2);
            builder.Property(e => e.OrgId)
                .HasColumnOrder(3);
            builder.Property(e => e.Name)
                .IsRequired()
                .HasColumnOrder(4);
            builder.Property(e => e.Description)
                .HasColumnOrder(5);

            builder.HasMany(e => e.Permissions)
                .WithMany(e => e.Roles)
                .UsingEntity(e => e.ToTableWithHistory("RolePermissions", "id"));
        }
    }
}
