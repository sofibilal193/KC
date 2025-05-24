using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KC.Domain.Common.Entities;

namespace KC.Persistence.Common
{
    public abstract class SqlEntityConfiguration<TBase> : IEntityTypeConfiguration<TBase>
        where TBase : SqlEntity
    {
        public virtual void Configure(EntityTypeBuilder<TBase> builder)
        {
            builder.Property(e => e.Id).HasColumnOrder(-1);
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Timestamp).IsRowVersion().HasColumnOrder(100);
            builder.Ignore(e => e.DomainEvents);
            builder.Property(e => e.IsTest).HasColumnOrder(81);
            builder.Property(e => e.IsDisabled).HasColumnOrder(82);
            builder.Property(e => e.CreateDateTimeUtc).HasDefaultValueSql("GETUTCDATE()").HasColumnOrder(91);
            builder.Property(e => e.CreateUserId).HasColumnOrder(92);
            builder.Property(e => e.CreateUserName).HasMaxLength(100).HasColumnOrder(93);
            builder.Property(e => e.CreateSource).HasMaxLength(50).HasColumnOrder(94);
            builder.Property(e => e.ModifyDateTimeUtc).HasDefaultValueSql("GETUTCDATE()").HasColumnOrder(95);
            builder.Property(e => e.ModifyUserId).HasColumnOrder(96);
            builder.Property(e => e.ModifyUserName).HasMaxLength(100).HasColumnOrder(97);
            builder.Property(e => e.ModifySource).HasMaxLength(50).HasColumnOrder(98);
        }
    }
}
