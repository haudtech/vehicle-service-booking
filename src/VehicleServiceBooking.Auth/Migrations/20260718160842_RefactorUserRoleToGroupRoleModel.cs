using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VehicleServiceBooking.Auth.Migrations
{
    /// <inheritdoc />
    public partial class RefactorUserRoleToGroupRoleModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a005"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a001") });

            migrationBuilder.CreateTable(
                name: "GroupRoles",
                columns: table => new
                {
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupRoles", x => new { x.GroupId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_GroupRoles_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c001"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "Default group for standard end users", true, "user", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c002"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "Administrative group", true, "admin", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c003"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "Super administrator group with full role coverage", true, "superadmin", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a001"),
                column: "Description",
                value: "Default role for booking resource operations");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010"),
                columns: new[] { "Description", "Name" },
                values: new object[] { "Default role for order resource operations", "order-user" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a011"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "Administrator role for booking resource operations", true, "booking-admin", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a012"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "Administrator role for order resource operations", true, "order-admin", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "GroupRoles",
                columns: new[] { "GroupId", "RoleId", "CreatedAt", "Id", "IsActive", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c001"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a001"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5d001"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c001"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5d002"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c002"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a011"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5d003"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c002"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a012"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5d004"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c003"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a001"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5d006"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c003"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5d008"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c003"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a011"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5d005"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c003"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a012"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5d007"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId", "CreatedAt", "Id", "IsActive", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a002"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a011"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b011"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a003"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a011"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b012"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a004"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a011"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b013"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a005"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a012"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b014"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a006"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a012"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b015"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a007"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a012"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b016"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a008"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a012"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b017"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a009"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a012"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b018"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupRoles_RoleId",
                table: "GroupRoles",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupRoles");

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c001"));

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c002"));

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c003"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a002"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a011") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a003"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a011") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a004"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a011") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a005"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a012") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a006"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a012") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a007"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a012") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a008"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a012") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a009"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a012") });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a011"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a012"));

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId", "CreatedAt", "Id", "IsActive", "UpdatedAt" },
                values: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a005"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a001"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b004"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a001"),
                column: "Description",
                value: "Default role for booking service API access");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010"),
                columns: new[] { "Description", "Name" },
                values: new object[] { "Role for order resource operations", "user-order" });

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");
        }
    }
}
