using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KC.Config.API.Entities;
using KC.Persistence.Common;
using KC.Persistence.Common.Extensions;

namespace KC.Config.API.Persistence
{
    public class UserConfigFieldValueConfiguration : SqlEntityConfiguration<UserConfigFieldValue>
    {
        public override void Configure(EntityTypeBuilder<UserConfigFieldValue> builder)
        {
            base.Configure(builder);

            builder.ToTableWithHistory();
            builder.Property(e => e.UserConfigFieldId)
                .IsRequired()
                .HasColumnOrder(1);
            builder.Property(e => e.OrgId)
                .IsRequired()
                .HasColumnOrder(2);
            builder.Property(e => e.UserId)
                .IsRequired()
                .HasColumnOrder(3);
            builder.Property(e => e.Value)
                .HasColumnOrder(4);

            builder.HasIndex(e => new { e.UserConfigFieldId, e.OrgId, e.UserId }).IsUnique();
        }
    }
}
