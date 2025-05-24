using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KC.Identity.API.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "id");

            migrationBuilder.CreateTable(
                name: "Orgs",
                schema: "id",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LegalName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Public\",\"InformationType\":\"Contact Info\",\"Rank\":\"NONE\"}"),
                    Fax = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TaxId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                        .Annotation("DynamicDataMask", "default()")
                        .Annotation("SensitivityClassification", "{\"Label\":\"Confidential\",\"InformationType\":\"National ID\",\"Rank\":\"CRITICAL\"}"),
                    LicenseNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                        .Annotation("DynamicDataMask", "default()")
                        .Annotation("SensitivityClassification", "{\"Label\":\"Public\",\"InformationType\":\"Other\",\"Rank\":\"NONE\"}"),
                    LicenseState = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsTest = table.Column<bool>(type: "bit", nullable: false),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: false),
                    CreateDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    CreateUserId = table.Column<int>(type: "int", nullable: true),
                    CreateUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreateSource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifyDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    ModifyUserId = table.Column<int>(type: "int", nullable: true),
                    ModifyUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifySource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart"),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orgs", x => x.Id);
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.CreateTable(
                name: "Permissions",
                schema: "id",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsTest = table.Column<bool>(type: "bit", nullable: false),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: false),
                    CreateDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    CreateUserId = table.Column<int>(type: "int", nullable: true),
                    CreateUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreateSource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifyDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    ModifyUserId = table.Column<int>(type: "int", nullable: true),
                    ModifyUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifySource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart"),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Permissions")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "id",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuthProvider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AuthProviderId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Confidential\",\"InformationType\":\"Name\",\"Rank\":\"MEDIUM\"}"),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Confidential\",\"InformationType\":\"Name\",\"Rank\":\"MEDIUM\"}"),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Confidential\",\"InformationType\":\"Contact Info\",\"Rank\":\"MEDIUM\"}"),
                    MobilePhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                        .Annotation("DynamicDataMask", "default()")
                        .Annotation("SensitivityClassification", "{\"Label\":\"Confidential\",\"InformationType\":\"Contact Info\",\"Rank\":\"MEDIUM\"}"),
                    LastLoginDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastLogoutDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsTest = table.Column<bool>(type: "bit", nullable: false),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: false),
                    CreateDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    CreateUserId = table.Column<int>(type: "int", nullable: true),
                    CreateUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreateSource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifyDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    ModifyUserId = table.Column<int>(type: "int", nullable: true),
                    ModifyUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifySource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart"),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Users")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.CreateTable(
                name: "OrgAddresses",
                schema: "id",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrgId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Address1 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Public\",\"InformationType\":\"Contact Info\",\"Rank\":\"NONE\"}"),
                    Address2 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Public\",\"InformationType\":\"Contact Info\",\"Rank\":\"NONE\"}"),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Public\",\"InformationType\":\"Contact Info\",\"Rank\":\"NONE\"}"),
                    County = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Public\",\"InformationType\":\"Contact Info\",\"Rank\":\"NONE\"}"),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Public\",\"InformationType\":\"Contact Info\",\"Rank\":\"NONE\"}"),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                        .Annotation("SensitivityClassification", "{\"Label\":\"Public\",\"InformationType\":\"Contact Info\",\"Rank\":\"NONE\"}"),
                    TimeZone = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    GooglePlaceId = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart"),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrgAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrgAddresses_Orgs_OrgId",
                        column: x => x.OrgId,
                        principalSchema: "id",
                        principalTable: "Orgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_OrgAddresses")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "id",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    OrgType = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    OrgId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsTest = table.Column<bool>(type: "bit", nullable: false),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: false),
                    CreateDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    CreateUserId = table.Column<int>(type: "int", nullable: true),
                    CreateUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreateSource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifyDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    ModifyUserId = table.Column<int>(type: "int", nullable: true),
                    ModifyUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifySource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart"),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Roles_Orgs_OrgId",
                        column: x => x.OrgId,
                        principalSchema: "id",
                        principalTable: "Orgs",
                        principalColumn: "Id");
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Roles")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.CreateTable(
                name: "OrgUsers",
                schema: "id",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrgId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsInvited = table.Column<bool>(type: "bit", nullable: false),
                    IsInviteProcessed = table.Column<bool>(type: "bit", nullable: false),
                    IsTest = table.Column<bool>(type: "bit", nullable: false),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: false),
                    CreateDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    CreateUserId = table.Column<int>(type: "int", nullable: true),
                    CreateUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreateSource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifyDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    ModifyUserId = table.Column<int>(type: "int", nullable: true),
                    ModifyUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifySource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart"),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrgUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrgUsers_Orgs_OrgId",
                        column: x => x.OrgId,
                        principalSchema: "id",
                        principalTable: "Orgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrgUsers_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "id",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrgUsers_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "id",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_OrgUsers")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                schema: "id",
                columns: table => new
                {
                    PermissionsId = table.Column<int>(type: "int", nullable: false),
                    RolesId = table.Column<int>(type: "int", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart"),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.PermissionsId, x.RolesId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionsId",
                        column: x => x.PermissionsId,
                        principalSchema: "id",
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RolesId",
                        column: x => x.RolesId,
                        principalSchema: "id",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_RolePermissions")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.CreateIndex(
                name: "IX_OrgAddresses_OrgId",
                schema: "id",
                table: "OrgAddresses",
                column: "OrgId");

            migrationBuilder.CreateIndex(
                name: "IX_Orgs_Code",
                schema: "id",
                table: "Orgs",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OrgUsers_OrgId_UserId",
                schema: "id",
                table: "OrgUsers",
                columns: new[] { "OrgId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrgUsers_RoleId",
                schema: "id",
                table: "OrgUsers",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgUsers_UserId",
                schema: "id",
                table: "OrgUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Name",
                schema: "id",
                table: "Permissions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RolesId",
                schema: "id",
                table: "RolePermissions",
                column: "RolesId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_OrgId",
                schema: "id",
                table: "Roles",
                column: "OrgId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AuthProvider_AuthProviderId",
                schema: "id",
                table: "Users",
                columns: new[] { "AuthProvider", "AuthProviderId" },
                unique: true,
                filter: "[AuthProvider] IS NOT NULL AND [AuthProviderId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "id",
                table: "Users",
                column: "Email",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrgAddresses",
                schema: "id")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_OrgAddresses")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.DropTable(
                name: "OrgUsers",
                schema: "id")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_OrgUsers")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.DropTable(
                name: "RolePermissions",
                schema: "id")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_RolePermissions")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "id")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "id")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Users")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.DropTable(
                name: "Permissions",
                schema: "id")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Permissions")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "id")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Roles")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.DropTable(
                name: "Orgs",
                schema: "id")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "History_Orgs")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");
        }
    }
}
