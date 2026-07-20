using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleServiceBooking.Auth.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderCreatePermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "UpdatedAt" },
                values: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a005"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "Create orders", true, "order:create", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a005"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a001") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a005"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a001") });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a005"));
        }
    }
}
