using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KC.Persistence.Common.Migrations
{
    /// <inheritdoc />
    public partial class Index_Updates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_IntegrationEventLogs_HostName_State",
                schema: "event",
                table: "IntegrationEventLogs");

            migrationBuilder.CreateIndex(
                name: "IX_IntegrationEventLogs_HostName_State_CreateDateTimeUtc",
                schema: "event",
                table: "IntegrationEventLogs",
                columns: new[] { "HostName", "State", "CreateDateTimeUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_IntegrationEventLogs_HostName_State_CreateDateTimeUtc",
                schema: "event",
                table: "IntegrationEventLogs");

            migrationBuilder.CreateIndex(
                name: "IX_IntegrationEventLogs_HostName_State",
                schema: "event",
                table: "IntegrationEventLogs",
                columns: new[] { "HostName", "State" });
        }
    }
}
