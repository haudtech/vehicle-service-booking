using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleServiceBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StabilizeBookingLookupSeedAuditValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 894, DateTimeKind.Utc).AddTicks(7550), new DateTime(2026, 7, 18, 10, 59, 32, 894, DateTimeKind.Utc).AddTicks(7550) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 894, DateTimeKind.Utc).AddTicks(7550), new DateTime(2026, 7, 18, 10, 59, 32, 894, DateTimeKind.Utc).AddTicks(7550) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 894, DateTimeKind.Utc).AddTicks(7550), new DateTime(2026, 7, 18, 10, 59, 32, 894, DateTimeKind.Utc).AddTicks(7550) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 894, DateTimeKind.Utc).AddTicks(7550), new DateTime(2026, 7, 18, 10, 59, 32, 894, DateTimeKind.Utc).AddTicks(7550) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 894, DateTimeKind.Utc).AddTicks(7550), new DateTime(2026, 7, 18, 10, 59, 32, 894, DateTimeKind.Utc).AddTicks(7550) });

            migrationBuilder.UpdateData(
                table: "IdempotencyRequestStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 895, DateTimeKind.Utc).AddTicks(3260), new DateTime(2026, 7, 18, 10, 59, 32, 895, DateTimeKind.Utc).AddTicks(3260) });

            migrationBuilder.UpdateData(
                table: "IdempotencyRequestStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 895, DateTimeKind.Utc).AddTicks(3260), new DateTime(2026, 7, 18, 10, 59, 32, 895, DateTimeKind.Utc).AddTicks(3260) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 895, DateTimeKind.Utc).AddTicks(5190), new DateTime(2026, 7, 18, 10, 59, 32, 895, DateTimeKind.Utc).AddTicks(5190) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 895, DateTimeKind.Utc).AddTicks(5190), new DateTime(2026, 7, 18, 10, 59, 32, 895, DateTimeKind.Utc).AddTicks(5190) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 895, DateTimeKind.Utc).AddTicks(5190), new DateTime(2026, 7, 18, 10, 59, 32, 895, DateTimeKind.Utc).AddTicks(5190) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 895, DateTimeKind.Utc).AddTicks(5190), new DateTime(2026, 7, 18, 10, 59, 32, 895, DateTimeKind.Utc).AddTicks(5190) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 895, DateTimeKind.Utc).AddTicks(5190), new DateTime(2026, 7, 18, 10, 59, 32, 895, DateTimeKind.Utc).AddTicks(5190) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 16, 39, 2, 745, DateTimeKind.Utc).AddTicks(1460), new DateTime(2026, 7, 18, 16, 39, 2, 745, DateTimeKind.Utc).AddTicks(1460) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 16, 39, 2, 745, DateTimeKind.Utc).AddTicks(1470), new DateTime(2026, 7, 18, 16, 39, 2, 745, DateTimeKind.Utc).AddTicks(1470) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 16, 39, 2, 745, DateTimeKind.Utc).AddTicks(1480), new DateTime(2026, 7, 18, 16, 39, 2, 745, DateTimeKind.Utc).AddTicks(1480) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 16, 39, 2, 745, DateTimeKind.Utc).AddTicks(1480), new DateTime(2026, 7, 18, 16, 39, 2, 745, DateTimeKind.Utc).AddTicks(1480) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 16, 39, 2, 745, DateTimeKind.Utc).AddTicks(1510), new DateTime(2026, 7, 18, 16, 39, 2, 745, DateTimeKind.Utc).AddTicks(1510) });

            migrationBuilder.UpdateData(
                table: "IdempotencyRequestStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 16, 39, 2, 745, DateTimeKind.Utc).AddTicks(7510), new DateTime(2026, 7, 18, 16, 39, 2, 745, DateTimeKind.Utc).AddTicks(7510) });

            migrationBuilder.UpdateData(
                table: "IdempotencyRequestStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 16, 39, 2, 745, DateTimeKind.Utc).AddTicks(7520), new DateTime(2026, 7, 18, 16, 39, 2, 745, DateTimeKind.Utc).AddTicks(7520) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 16, 39, 2, 746, DateTimeKind.Utc).AddTicks(620), new DateTime(2026, 7, 18, 16, 39, 2, 746, DateTimeKind.Utc).AddTicks(620) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 16, 39, 2, 746, DateTimeKind.Utc).AddTicks(630), new DateTime(2026, 7, 18, 16, 39, 2, 746, DateTimeKind.Utc).AddTicks(630) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 16, 39, 2, 746, DateTimeKind.Utc).AddTicks(630), new DateTime(2026, 7, 18, 16, 39, 2, 746, DateTimeKind.Utc).AddTicks(630) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 16, 39, 2, 746, DateTimeKind.Utc).AddTicks(640), new DateTime(2026, 7, 18, 16, 39, 2, 746, DateTimeKind.Utc).AddTicks(640) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 16, 39, 2, 746, DateTimeKind.Utc).AddTicks(640), new DateTime(2026, 7, 18, 16, 39, 2, 746, DateTimeKind.Utc).AddTicks(640) });
        }
    }
}
