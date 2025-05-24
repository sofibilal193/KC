using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KC.Identity.API.Migrations
{
    /// <inheritdoc />
    public partial class OrgParentOrgId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Website",
                schema: "id",
                table: "Orgs",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 8)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart")
                .OldAnnotation("Relational:ColumnOrder", 7)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", "id")
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.AlterColumn<string>(
                name: "TaxId",
                schema: "id",
                table: "Orgs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true)
                .Annotation("DynamicDataMask", "default()")
                .Annotation("Relational:ColumnOrder", 9)
                .Annotation("SensitivityClassification", "{\"Label\":\"Confidential\",\"InformationType\":\"National ID\",\"Rank\":\"CRITICAL\"}")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart")
                .OldAnnotation("DynamicDataMask", "default()")
                .OldAnnotation("Relational:ColumnOrder", 8)
                .OldAnnotation("SensitivityClassification", "{\"Label\":\"Confidential\",\"InformationType\":\"National ID\",\"Rank\":\"CRITICAL\"}")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", "id")
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.AlterColumn<string>(
                name: "Tags",
                schema: "id",
                table: "Orgs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)")
                .Annotation("Relational:ColumnOrder", 12)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart")
                .OldAnnotation("Relational:ColumnOrder", 11)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", "id")
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                schema: "id",
                table: "Orgs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 6)
                .Annotation("SensitivityClassification", "{\"Label\":\"Public\",\"InformationType\":\"Contact Info\",\"Rank\":\"NONE\"}")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart")
                .OldAnnotation("Relational:ColumnOrder", 5)
                .OldAnnotation("SensitivityClassification", "{\"Label\":\"Public\",\"InformationType\":\"Contact Info\",\"Rank\":\"NONE\"}")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", "id")
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "id",
                table: "Orgs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100)
                .Annotation("Relational:ColumnOrder", 4)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart")
                .OldAnnotation("Relational:ColumnOrder", 3)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", "id")
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.AlterColumn<string>(
                name: "LicenseState",
                schema: "id",
                table: "Orgs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 11)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart")
                .OldAnnotation("Relational:ColumnOrder", 10)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", "id")
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.AlterColumn<string>(
                name: "LicenseNo",
                schema: "id",
                table: "Orgs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true)
                .Annotation("DynamicDataMask", "default()")
                .Annotation("Relational:ColumnOrder", 10)
                .Annotation("SensitivityClassification", "{\"Label\":\"Public\",\"InformationType\":\"Other\",\"Rank\":\"NONE\"}")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart")
                .OldAnnotation("DynamicDataMask", "default()")
                .OldAnnotation("Relational:ColumnOrder", 9)
                .OldAnnotation("SensitivityClassification", "{\"Label\":\"Public\",\"InformationType\":\"Other\",\"Rank\":\"NONE\"}")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", "id")
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.AlterColumn<string>(
                name: "LegalName",
                schema: "id",
                table: "Orgs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 5)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart")
                .OldAnnotation("Relational:ColumnOrder", 4)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", "id")
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.AlterColumn<string>(
                name: "Fax",
                schema: "id",
                table: "Orgs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 7)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart")
                .OldAnnotation("Relational:ColumnOrder", 6)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", "id")
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.AddColumn<int>(
                name: "ParentOrgId",
                schema: "id",
                table: "Orgs",
                type: "int",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 3)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentOrgId",
                schema: "id",
                table: "Orgs")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.AlterColumn<string>(
                name: "Website",
                schema: "id",
                table: "Orgs",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 7)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart")
                .OldAnnotation("Relational:ColumnOrder", 8)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", "id")
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.AlterColumn<string>(
                name: "TaxId",
                schema: "id",
                table: "Orgs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true)
                .Annotation("DynamicDataMask", "default()")
                .Annotation("Relational:ColumnOrder", 8)
                .Annotation("SensitivityClassification", "{\"Label\":\"Confidential\",\"InformationType\":\"National ID\",\"Rank\":\"CRITICAL\"}")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart")
                .OldAnnotation("DynamicDataMask", "default()")
                .OldAnnotation("Relational:ColumnOrder", 9)
                .OldAnnotation("SensitivityClassification", "{\"Label\":\"Confidential\",\"InformationType\":\"National ID\",\"Rank\":\"CRITICAL\"}")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", "id")
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.AlterColumn<string>(
                name: "Tags",
                schema: "id",
                table: "Orgs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)")
                .Annotation("Relational:ColumnOrder", 11)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart")
                .OldAnnotation("Relational:ColumnOrder", 12)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", "id")
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                schema: "id",
                table: "Orgs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 5)
                .Annotation("SensitivityClassification", "{\"Label\":\"Public\",\"InformationType\":\"Contact Info\",\"Rank\":\"NONE\"}")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart")
                .OldAnnotation("Relational:ColumnOrder", 6)
                .OldAnnotation("SensitivityClassification", "{\"Label\":\"Public\",\"InformationType\":\"Contact Info\",\"Rank\":\"NONE\"}")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", "id")
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "id",
                table: "Orgs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100)
                .Annotation("Relational:ColumnOrder", 3)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart")
                .OldAnnotation("Relational:ColumnOrder", 4)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", "id")
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.AlterColumn<string>(
                name: "LicenseState",
                schema: "id",
                table: "Orgs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 10)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart")
                .OldAnnotation("Relational:ColumnOrder", 11)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", "id")
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.AlterColumn<string>(
                name: "LicenseNo",
                schema: "id",
                table: "Orgs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true)
                .Annotation("DynamicDataMask", "default()")
                .Annotation("Relational:ColumnOrder", 9)
                .Annotation("SensitivityClassification", "{\"Label\":\"Public\",\"InformationType\":\"Other\",\"Rank\":\"NONE\"}")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart")
                .OldAnnotation("DynamicDataMask", "default()")
                .OldAnnotation("Relational:ColumnOrder", 10)
                .OldAnnotation("SensitivityClassification", "{\"Label\":\"Public\",\"InformationType\":\"Other\",\"Rank\":\"NONE\"}")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", "id")
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.AlterColumn<string>(
                name: "LegalName",
                schema: "id",
                table: "Orgs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 4)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart")
                .OldAnnotation("Relational:ColumnOrder", 5)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", "id")
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.AlterColumn<string>(
                name: "Fax",
                schema: "id",
                table: "Orgs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 6)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart")
                .OldAnnotation("Relational:ColumnOrder", 7)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", "id")
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");
        }
    }
}
