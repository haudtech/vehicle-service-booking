using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VehicleServiceBooking.Auth.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a006"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "View orders", true, "order:view", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a007"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "Edit orders", true, "order:edit", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a008"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "Cancel orders", true, "order:cancel", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a009"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "Complete orders", true, "order:complete", new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a006"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a007"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a008"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a009"));
        }
    }
}
