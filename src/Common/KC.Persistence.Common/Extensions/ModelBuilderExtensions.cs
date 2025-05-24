using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KC.Persistence.Common.Extensions
{
    public static class ModelBuilderExtensions
    {
        private const string _historyTableFormat = "History_{0}";

        public static EntityTypeBuilder<TEntity> ToTableWithHistory<TEntity>(
            this EntityTypeBuilder<TEntity> entityTypeBuilder) where TEntity : class
        {
            var historyTable = string.Format(_historyTableFormat, entityTypeBuilder.Metadata.GetTableName());
            return entityTypeBuilder.ToTable(b => b.IsTemporal(t => t.UseHistoryTable(historyTable)));
        }

        public static EntityTypeBuilder ToTableWithHistory(
            this EntityTypeBuilder entityTypeBuilder, string name, string? schema)
        {
            var historyTable = string.Format(_historyTableFormat, name);
            return entityTypeBuilder.ToTable(name, schema, b => b.IsTemporal(t => t.UseHistoryTable(historyTable, schema)));
        }

        public static OwnedNavigationBuilder<TOwnerEntity, TRelatedEntity> ToTableWithHistory<TOwnerEntity, TRelatedEntity>(
            this OwnedNavigationBuilder<TOwnerEntity, TRelatedEntity> referenceOwnershipBuilder,
            string name, string? schema)
                where TOwnerEntity : class
                where TRelatedEntity : class
        {
            var historyTable = string.Format(_historyTableFormat, name);
            return referenceOwnershipBuilder.ToTable(name, schema,
                b => b.IsTemporal(t => t.UseHistoryTable(historyTable)));
        }
    }
}
