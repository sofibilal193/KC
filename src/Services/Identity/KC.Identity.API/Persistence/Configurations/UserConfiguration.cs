using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KC.Identity.API.Entities;
using KC.Persistence.Common;
using KC.Persistence.Common.DataSecurity;
using KC.Persistence.Common.Extensions;

namespace KC.Identity.API.Persistence
{
    public class UserConfiguration : SqlEntityConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);

            builder.ToTableWithHistory();
            builder.Property(e => e.AuthProvider)
                .HasMaxLength(50)
                .HasColumnOrder(1);
            builder.Property(e => e.AuthProviderId)
                .HasMaxLength(50)
                .HasColumnOrder(2);
            builder.HasIndex(e => new { e.AuthProvider, e.AuthProviderId }).IsUnique();
            builder.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnOrder(3)
                .HasSensitivityClassification(SensitivityLabels.Confidential,
                    SensitivityInformationTypes.Name, SensitivityRank.MEDIUM);
            builder.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnOrder(4)
                .HasSensitivityClassification(SensitivityLabels.Confidential,
                    SensitivityInformationTypes.Name, SensitivityRank.MEDIUM);
            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnOrder(5)
                .HasSensitivityClassification(SensitivityLabels.Confidential,
                    SensitivityInformationTypes.ContactInfo, SensitivityRank.MEDIUM);
            builder.HasIndex(e => e.Email).IsUnique();
            builder.Property(e => e.MobilePhone)
                .HasMaxLength(50)
                .HasDataMask()
                .HasColumnOrder(6)
                .HasSensitivityClassification(SensitivityLabels.Confidential,
                    SensitivityInformationTypes.ContactInfo, SensitivityRank.MEDIUM);
            builder.Property(e => e.LastLoginDateTimeUtc)
                .HasColumnOrder(7);
            builder.Property(e => e.LastLogoutDateTimeUtc)
                .HasColumnOrder(8);
        }
    }
}
