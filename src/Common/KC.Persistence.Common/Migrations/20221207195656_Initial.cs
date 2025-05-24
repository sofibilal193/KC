using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KC.Persistence.Common.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "event");

            migrationBuilder.CreateTable(
                name: "EventLogs",
                schema: "event",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Event = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    OrgId = table.Column<int>(type: "int", nullable: true),
                    RecordId = table.Column<int>(type: "int", nullable: true),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IntegrationEventLogs",
                schema: "event",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HostName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EventTypeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    OrgId = table.Column<int>(type: "int", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EventData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifyDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegrationEventLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IntegrationEventLogs_EventId",
                schema: "event",
                table: "IntegrationEventLogs",
                column: "EventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IntegrationEventLogs_HostName_State",
                schema: "event",
                table: "IntegrationEventLogs",
                columns: new[] { "HostName", "State" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventLogs",
                schema: "event");

            migrationBuilder.DropTable(
                name: "IntegrationEventLogs",
                schema: "event");
        }
    }
}
