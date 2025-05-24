using KC.Persistence.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using KC.Identity.API.Entities;
using KC.Persistence.Common.Extensions;

namespace KC.Identity.API.Persistence
{
    public class OrgUserConfiguration : SqlEntityConfiguration<OrgUser>
    {
        public override void Configure(EntityTypeBuilder<OrgUser> builder)
        {
            base.Configure(builder);

            builder.ToTableWithHistory();
            builder.Property(e => e.OrgId)
                .HasColumnOrder(1);
            builder.Property(e => e.UserId)
                .HasColumnOrder(2);
            builder.Property(e => e.RoleId)
                .HasColumnOrder(3);
            builder.Property(e => e.Title)
                .HasColumnOrder(4)
                .HasMaxLength(50);
            builder.Property(e => e.IsDefault)
                .HasColumnOrder(5);
            builder.Property(e => e.IsInvited)
                .HasColumnOrder(6);
            builder.Property(e => e.IsInviteProcessed)
                .HasColumnOrder(7);

            builder.HasIndex(e => new { e.OrgId, e.UserId }).IsUnique();
        }
    }
}
