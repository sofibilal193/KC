using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KC.Identity.API.Entities;
using KC.Persistence.Common;
using KC.Persistence.Common.DataSecurity;
using KC.Persistence.Common.Extensions;

namespace KC.Identity.API.Persistence
{
    public class OrgConfiguration : SqlEntityConfiguration<Org>
    {
        public override void Configure(EntityTypeBuilder<Org> builder)
        {
            base.Configure(builder);

            builder.ToTableWithHistory();
            builder.Property(e => e.Type)
                .HasConversion<string>()
                .HasMaxLength(25)
                .HasColumnOrder(1);
            builder.Property(e => e.Code)
                .IsRequired(false)
                .HasMaxLength(25)
                .HasColumnOrder(2);
            builder.HasIndex(e => e.Code)
                .IsUnique();
            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnOrder(4);
            builder.Property(e => e.LegalName)
                .HasMaxLength(100)
                .HasColumnOrder(5);
            builder.Property(e => e.Phone)
                .HasMaxLength(50)
                .HasColumnOrder(6)
                .HasSensitivityClassification(SensitivityLabels.Public,
                    SensitivityInformationTypes.ContactInfo, SensitivityRank.NONE);
            builder.Property(e => e.Fax)
                .HasMaxLength(50)
                .HasColumnOrder(7);
            builder.Property(e => e.Website)
                .HasMaxLength(255)
                .HasColumnOrder(8);
            builder.Property(e => e.TaxId)
                .HasDataMask()
                .HasMaxLength(50)
                .HasColumnOrder(9)
                .HasSensitivityClassification(SensitivityLabels.Confidential,
                    SensitivityInformationTypes.NationalId, SensitivityRank.CRITICAL);
            builder.Property(e => e.LicenseNo)
                .HasMaxLength(50)
                .HasDataMask()
                .HasColumnOrder(10)
                .HasSensitivityClassification(SensitivityLabels.Public,
                    SensitivityInformationTypes.Other, SensitivityRank.NONE);
            builder.Property(e => e.LicenseState)
                .HasMaxLength(50)
                .HasColumnOrder(11);
            builder.Property(e => e.Tags)
                .HasJsonConversion()
                .HasColumnOrder(12);

            builder.Ignore(e => e.PrimaryAddress);

            //Address value object persisted as owned entity type supported since EF Core 2.0
            builder
                .OwnsMany(e => e.Addresses, a =>
                {
                    a.ToTableWithHistory("OrgAddresses", "id");
                    a.WithOwner().HasForeignKey("OrgId");
                    a.Property<int>("Id").HasColumnOrder(1);
                    a.HasKey("Id");
                    a.Property("OrgId").HasColumnOrder(2);
                    a.Property(e => e.Type)
                        .HasColumnName("Type")
                        .IsRequired()
                        .HasMaxLength(25)
                        .HasColumnOrder(20);
                    a.Property(e => e.Address1)
                        .HasColumnName("Address1")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnOrder(21)
                        .HasSensitivityClassification(SensitivityLabels.Public,
                            SensitivityInformationTypes.ContactInfo, SensitivityRank.NONE);
                    a.Property(e => e.Address2)
                        .HasColumnName("Address2")
                        .HasMaxLength(100)
                        .HasColumnOrder(22)
                        .HasSensitivityClassification(SensitivityLabels.Public,
                            SensitivityInformationTypes.ContactInfo, SensitivityRank.NONE);
                    a.Property(e => e.City)
                        .HasColumnName("City")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnOrder(23)
                        .HasSensitivityClassification(SensitivityLabels.Public,
                            SensitivityInformationTypes.ContactInfo, SensitivityRank.NONE);
                    a.Property(e => e.County)
                        .HasColumnName("County")
                        .HasMaxLength(50)
                        .HasColumnOrder(24)
                        .HasSensitivityClassification(SensitivityLabels.Public,
                            SensitivityInformationTypes.ContactInfo, SensitivityRank.NONE);
                    a.Property(e => e.State)
                        .HasColumnName("State")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnOrder(25)
                        .HasSensitivityClassification(SensitivityLabels.Public,
                            SensitivityInformationTypes.ContactInfo, SensitivityRank.NONE);
                    a.Property(e => e.Country)
                        .HasColumnName("Country")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnOrder(26);
                    a.Property(e => e.ZipCode)
                        .HasColumnName("ZipCode")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnOrder(27)
                        .HasSensitivityClassification(SensitivityLabels.Public,
                            SensitivityInformationTypes.ContactInfo, SensitivityRank.NONE);
                    a.Property(e => e.TimeZone)
                        .HasColumnName("TimeZone")
                        .HasMaxLength(5)
                        .HasColumnOrder(28);
                    a.Property(e => e.GooglePlaceId)
                        .HasColumnName("GooglePlaceId")
                        .HasMaxLength(500)
                        .HasColumnOrder(29);
                });

            builder.OwnsMany(e => e.Groups, g =>
            {
                g.ToTable("OrgGroups", "id");
                g.WithOwner().HasForeignKey("OrgId");
                g.Property<int>("Id").HasColumnOrder(1);
                g.HasKey("Id");
                g.Property("OrgId").HasColumnOrder(2);
                g.Property(e => e.ParentOrgId)
                    .IsRequired()
                    .HasColumnOrder(3);
            });
            builder.Navigation(e => e.Groups).AutoInclude(false);
        }
    }
}
