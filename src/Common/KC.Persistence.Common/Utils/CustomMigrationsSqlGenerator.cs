using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using KC.Persistence.Common.DataSecurity;

namespace KC.Persistence.Common.Utils
{
    [ExcludeFromCodeCoverage]
    public class CustomMigrationsSqlGenerator : SqlServerMigrationsSqlGenerator
    {
        private readonly ISqlGenerationHelper _sqlHelper;

        public CustomMigrationsSqlGenerator(MigrationsSqlGeneratorDependencies dependencies, ICommandBatchPreparer commandBatchPreparer)
            : base(dependencies, commandBatchPreparer)
        {
            _sqlHelper = dependencies.SqlGenerationHelper;
        }

        protected override void Generate(CreateTableOperation operation, IModel? model, MigrationCommandListBuilder builder, bool terminate = true)
        {
            base.Generate(operation, model, builder, terminate);
            foreach (var columnOperation in operation.Columns)
            {
                GenerateSensitivityClassification(builder, columnOperation);
                GenerateDynamicDataMask(builder, columnOperation);
            }
        }

        protected override void Generate(AddColumnOperation operation, IModel? model, MigrationCommandListBuilder builder, bool terminate)
        {
            base.Generate(operation, model, builder, terminate);
            GenerateSensitivityClassification(builder, operation);
            GenerateDynamicDataMask(builder, operation);
        }

        protected override void Generate(AlterColumnOperation operation, IModel? model, MigrationCommandListBuilder builder)
        {
            base.Generate(operation, model, builder);
            GenerateSensitivityClassification(builder, operation);
            GenerateDynamicDataMask(builder, operation);
        }

        private void GenerateSensitivityClassification(MigrationCommandListBuilder builder, ColumnOperation operation)
        {
            var identifier = $"{_sqlHelper.DelimitIdentifier(operation.Table, operation.Schema)}"
                + $".{_sqlHelper.DelimitIdentifier(operation.Name)}";
            var annotation = operation.FindAnnotation(CustomAnnotations.SensitivityClassification);
            if (annotation?.Value is null)
            {
                if (operation is AlterColumnOperation alterOperation
                    && alterOperation.OldColumn.FindAnnotation(CustomAnnotations.SensitivityClassification) is not null)
                {
                    builder.Append($"DROP SENSITIVITY CLASSIFICATION FROM {identifier}")
                        .AppendLine(_sqlHelper.StatementTerminator)
                        .EndCommand();
                }
                return;
            }
            var options = new JsonSerializerOptions();
            options.Converters.Add(new JsonStringEnumConverter());
            var classification = JsonSerializer.Deserialize<SensitivityClassification>((string)annotation.Value, options)!;

            builder.Append($"ADD SENSITIVITY CLASSIFICATION TO {identifier} ")
                .Append($"WITH (LABEL = '{classification.Label}', ")
                .Append($"INFORMATION_TYPE = '{classification.InformationType}', ")
                .Append($"RANK = {classification.Rank})")
                .AppendLine(_sqlHelper.StatementTerminator)
                .EndCommand();
        }

        private void GenerateDynamicDataMask(MigrationCommandListBuilder builder, ColumnOperation operation)
        {
            var annotation = operation.FindAnnotation(CustomAnnotations.DynamicDataMask);
            var oldAnnotation = operation is AlterColumnOperation alterOperation ?
                alterOperation.OldColumn.FindAnnotation(CustomAnnotations.DynamicDataMask) : null;
            var hasAnnotation = annotation?.Value is not null;
            var hadAnnotation = oldAnnotation?.Value is not null;
            if (!hasAnnotation && !hadAnnotation)
            {
                return;
            }
            if (hasAnnotation && hadAnnotation && annotation?.Value == oldAnnotation?.Value)
            {
                return;
            }

            builder.Append($"ALTER TABLE {_sqlHelper.DelimitIdentifier(operation.Table, operation.Schema)} ")
                .Append($"ALTER COLUMN {_sqlHelper.DelimitIdentifier(operation.Name)} ");
            if (hadAnnotation && !hasAnnotation)
            {
                builder.Append("DROP MASKED");
            }
            else
            {
                if (!hadAnnotation && hasAnnotation)
                {
                    builder.Append("ADD ");
                }
                builder.Append($"MASKED WITH (FUNCTION = '{annotation?.Value}')");
            }
            builder.AppendLine(_sqlHelper.StatementTerminator).EndCommand();
        }
    }
}
