using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KC.Identity.API.Migrations
{
    /// <inheritdoc />
    public partial class OrgParentOrgIdRemove : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
