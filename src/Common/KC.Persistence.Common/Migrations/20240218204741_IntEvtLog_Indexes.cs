using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KC.Persistence.Common.Migrations
{
    /// <inheritdoc />
    public partial class IntEvtLog_Indexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_IntegrationEventLogs_HostName_State_CreateDateTimeUtc",
                schema: "event",
                table: "IntegrationEventLogs");

            migrationBuilder.CreateIndex(
                name: "IX_IntegrationEventLogs_State_EventTypeName",
                schema: "event",
                table: "IntegrationEventLogs",
                columns: new[] { "State", "EventTypeName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_IntegrationEventLogs_State_EventTypeName",
                schema: "event",
                table: "IntegrationEventLogs");

            migrationBuilder.CreateIndex(
                name: "IX_IntegrationEventLogs_HostName_State_CreateDateTimeUtc",
                schema: "event",
                table: "IntegrationEventLogs",
                columns: new[] { "HostName", "State", "CreateDateTimeUtc" });
        }
    }
}
