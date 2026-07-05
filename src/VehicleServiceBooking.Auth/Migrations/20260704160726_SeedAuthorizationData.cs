using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VehicleServiceBooking.Auth.Migrations
{
    /// <inheritdoc />
    public partial class SeedAuthorizationData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a002"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "Create appointments", true, "appointment:create", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a003"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "View appointment availability and details", true, "appointment:view", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a004"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "Complete appointments", true, "appointment:complete", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "UpdatedAt" },
                values: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a001"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "Default role for booking service API access", true, "booking-user", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a002"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a001") },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a003"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a001") },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a004"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a001") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a002"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a003"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a004"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a001") });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a002"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a003"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a004"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a001"));
        }
    }
}
