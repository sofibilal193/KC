using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KC.Identity.API.Migrations
{
    /// <inheritdoc />
    public partial class OrgGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrgGroups",
                schema: "id",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrgId = table.Column<int>(type: "int", nullable: false),
                    ParentOrgId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrgGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrgGroups_Orgs_OrgId",
                        column: x => x.OrgId,
                        principalSchema: "id",
                        principalTable: "Orgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrgGroups_OrgId",
                schema: "id",
                table: "OrgGroups",
                column: "OrgId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrgGroups",
                schema: "id");
        }
    }
}
