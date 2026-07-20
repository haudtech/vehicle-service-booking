using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleServiceBooking.Auth.Migrations
{
    /// <inheritdoc />
    public partial class RefactorJoinEntitiesToAuthBaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "UserRoles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "UserRoles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UserRoles",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "UserRoles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "UserGroups",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "UserGroups",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UserGroups",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "UserGroups",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "RolePermissions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "RolePermissions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "RolePermissions",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "RolePermissions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a002"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a001") },
                columns: new[] { "CreatedAt", "Id", "IsActive", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b001"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a003"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a001") },
                columns: new[] { "CreatedAt", "Id", "IsActive", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b002"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a004"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a001") },
                columns: new[] { "CreatedAt", "Id", "IsActive", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b003"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a005"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a001") },
                columns: new[] { "CreatedAt", "Id", "IsActive", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b004"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a005"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010") },
                columns: new[] { "CreatedAt", "Id", "IsActive", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b005"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a006"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010") },
                columns: new[] { "CreatedAt", "Id", "IsActive", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b006"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a007"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010") },
                columns: new[] { "CreatedAt", "Id", "IsActive", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b007"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a008"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010") },
                columns: new[] { "CreatedAt", "Id", "IsActive", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b008"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a009"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010") },
                columns: new[] { "CreatedAt", "Id", "IsActive", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b009"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "UserGroups");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserGroups");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UserGroups");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "UserGroups");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "RolePermissions");
        }
    }
}
