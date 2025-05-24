using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KC.Config.API.Entities;
using KC.Persistence.Common;
using KC.Persistence.Common.Extensions;

namespace KC.Config.API.Persistence
{
    public class OrgConfigFieldValueConfiguration : SqlEntityConfiguration<OrgConfigFieldValue>
    {
        public override void Configure(EntityTypeBuilder<OrgConfigFieldValue> builder)
        {
            base.Configure(builder);

            builder.ToTableWithHistory();
            builder.Property(e => e.OrgConfigFieldId)
                .IsRequired()
                .HasColumnOrder(1);
            builder.Property(e => e.OrgId)
                .IsRequired()
                .HasColumnOrder(2);
            builder.Property(e => e.Value)
                .HasColumnOrder(3);

            builder.HasIndex(e => new { e.OrgConfigFieldId, e.OrgId }).IsUnique();
        }
    }
}
