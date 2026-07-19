using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VehicleServiceBooking.Auth.Migrations
{
    /// <inheritdoc />
    public partial class AddUserOrderRolePermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "UpdatedAt" },
                values: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "Role for order resource operations", true, "user-order", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a005"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010") },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a006"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010") },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a007"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010") },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a008"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010") },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a009"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a005"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a006"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a007"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a008"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a009"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010") });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010"));
        }
    }
}
