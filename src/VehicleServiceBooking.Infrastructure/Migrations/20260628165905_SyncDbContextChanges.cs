using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleServiceBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncDbContextChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 16, 59, 5, 31, DateTimeKind.Utc).AddTicks(8640), new DateTime(2026, 6, 28, 16, 59, 5, 31, DateTimeKind.Utc).AddTicks(8640) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 16, 59, 5, 31, DateTimeKind.Utc).AddTicks(8650), new DateTime(2026, 6, 28, 16, 59, 5, 31, DateTimeKind.Utc).AddTicks(8650) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 16, 59, 5, 31, DateTimeKind.Utc).AddTicks(8650), new DateTime(2026, 6, 28, 16, 59, 5, 31, DateTimeKind.Utc).AddTicks(8650) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 16, 59, 5, 31, DateTimeKind.Utc).AddTicks(8650), new DateTime(2026, 6, 28, 16, 59, 5, 31, DateTimeKind.Utc).AddTicks(8650) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 16, 59, 5, 31, DateTimeKind.Utc).AddTicks(8660), new DateTime(2026, 6, 28, 16, 59, 5, 31, DateTimeKind.Utc).AddTicks(8660) });

            migrationBuilder.UpdateData(
                table: "IdempotencyRequestStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 16, 59, 5, 32, DateTimeKind.Utc).AddTicks(3180), new DateTime(2026, 6, 28, 16, 59, 5, 32, DateTimeKind.Utc).AddTicks(3180) });

            migrationBuilder.UpdateData(
                table: "IdempotencyRequestStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 16, 59, 5, 32, DateTimeKind.Utc).AddTicks(3190), new DateTime(2026, 6, 28, 16, 59, 5, 32, DateTimeKind.Utc).AddTicks(3190) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 16, 59, 5, 32, DateTimeKind.Utc).AddTicks(4940), new DateTime(2026, 6, 28, 16, 59, 5, 32, DateTimeKind.Utc).AddTicks(4940) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 16, 59, 5, 32, DateTimeKind.Utc).AddTicks(4950), new DateTime(2026, 6, 28, 16, 59, 5, 32, DateTimeKind.Utc).AddTicks(4950) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 16, 59, 5, 32, DateTimeKind.Utc).AddTicks(4960), new DateTime(2026, 6, 28, 16, 59, 5, 32, DateTimeKind.Utc).AddTicks(4960) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 16, 59, 5, 32, DateTimeKind.Utc).AddTicks(4960), new DateTime(2026, 6, 28, 16, 59, 5, 32, DateTimeKind.Utc).AddTicks(4960) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 16, 59, 5, 32, DateTimeKind.Utc).AddTicks(4970), new DateTime(2026, 6, 28, 16, 59, 5, 32, DateTimeKind.Utc).AddTicks(4970) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 16, 59, 5, 36, DateTimeKind.Utc).AddTicks(2310), new DateTime(2026, 6, 28, 16, 59, 5, 36, DateTimeKind.Utc).AddTicks(2310) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 16, 59, 5, 36, DateTimeKind.Utc).AddTicks(2320), new DateTime(2026, 6, 28, 16, 59, 5, 36, DateTimeKind.Utc).AddTicks(2320) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 16, 59, 5, 36, DateTimeKind.Utc).AddTicks(2320), new DateTime(2026, 6, 28, 16, 59, 5, 36, DateTimeKind.Utc).AddTicks(2320) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 16, 59, 5, 36, DateTimeKind.Utc).AddTicks(2330), new DateTime(2026, 6, 28, 16, 59, 5, 36, DateTimeKind.Utc).AddTicks(2330) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 16, 59, 5, 36, DateTimeKind.Utc).AddTicks(2340), new DateTime(2026, 6, 28, 16, 59, 5, 36, DateTimeKind.Utc).AddTicks(2340) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 16, 59, 5, 36, DateTimeKind.Utc).AddTicks(2340), new DateTime(2026, 6, 28, 16, 59, 5, 36, DateTimeKind.Utc).AddTicks(2340) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 16, 59, 5, 36, DateTimeKind.Utc).AddTicks(2350), new DateTime(2026, 6, 28, 16, 59, 5, 36, DateTimeKind.Utc).AddTicks(2350) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 16, 59, 5, 36, DateTimeKind.Utc).AddTicks(2360), new DateTime(2026, 6, 28, 16, 59, 5, 36, DateTimeKind.Utc).AddTicks(2360) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000009"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 16, 59, 5, 36, DateTimeKind.Utc).AddTicks(2360), new DateTime(2026, 6, 28, 16, 59, 5, 36, DateTimeKind.Utc).AddTicks(2360) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000010"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 16, 59, 5, 36, DateTimeKind.Utc).AddTicks(2370), new DateTime(2026, 6, 28, 16, 59, 5, 36, DateTimeKind.Utc).AddTicks(2370) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000011"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 16, 59, 5, 36, DateTimeKind.Utc).AddTicks(2380), new DateTime(2026, 6, 28, 16, 59, 5, 36, DateTimeKind.Utc).AddTicks(2380) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000012"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 16, 59, 5, 36, DateTimeKind.Utc).AddTicks(2380), new DateTime(2026, 6, 28, 16, 59, 5, 36, DateTimeKind.Utc).AddTicks(2380) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000013"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 16, 59, 5, 36, DateTimeKind.Utc).AddTicks(2390), new DateTime(2026, 6, 28, 16, 59, 5, 36, DateTimeKind.Utc).AddTicks(2390) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000014"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 16, 59, 5, 36, DateTimeKind.Utc).AddTicks(2390), new DateTime(2026, 6, 28, 16, 59, 5, 36, DateTimeKind.Utc).AddTicks(2390) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000015"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 16, 59, 5, 36, DateTimeKind.Utc).AddTicks(2420), new DateTime(2026, 6, 28, 16, 59, 5, 36, DateTimeKind.Utc).AddTicks(2420) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000016"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 16, 59, 5, 36, DateTimeKind.Utc).AddTicks(2430), new DateTime(2026, 6, 28, 16, 59, 5, 36, DateTimeKind.Utc).AddTicks(2430) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000017"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 16, 59, 5, 36, DateTimeKind.Utc).AddTicks(2430), new DateTime(2026, 6, 28, 16, 59, 5, 36, DateTimeKind.Utc).AddTicks(2430) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000018"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 16, 59, 5, 36, DateTimeKind.Utc).AddTicks(2440), new DateTime(2026, 6, 28, 16, 59, 5, 36, DateTimeKind.Utc).AddTicks(2440) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 15, 45, 5, 309, DateTimeKind.Utc).AddTicks(730), new DateTime(2026, 6, 28, 15, 45, 5, 309, DateTimeKind.Utc).AddTicks(730) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 15, 45, 5, 309, DateTimeKind.Utc).AddTicks(730), new DateTime(2026, 6, 28, 15, 45, 5, 309, DateTimeKind.Utc).AddTicks(730) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 15, 45, 5, 309, DateTimeKind.Utc).AddTicks(740), new DateTime(2026, 6, 28, 15, 45, 5, 309, DateTimeKind.Utc).AddTicks(740) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 15, 45, 5, 309, DateTimeKind.Utc).AddTicks(740), new DateTime(2026, 6, 28, 15, 45, 5, 309, DateTimeKind.Utc).AddTicks(740) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 15, 45, 5, 309, DateTimeKind.Utc).AddTicks(750), new DateTime(2026, 6, 28, 15, 45, 5, 309, DateTimeKind.Utc).AddTicks(750) });

            migrationBuilder.UpdateData(
                table: "IdempotencyRequestStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 15, 45, 5, 309, DateTimeKind.Utc).AddTicks(4910), new DateTime(2026, 6, 28, 15, 45, 5, 309, DateTimeKind.Utc).AddTicks(4910) });

            migrationBuilder.UpdateData(
                table: "IdempotencyRequestStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 15, 45, 5, 309, DateTimeKind.Utc).AddTicks(4920), new DateTime(2026, 6, 28, 15, 45, 5, 309, DateTimeKind.Utc).AddTicks(4920) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 15, 45, 5, 309, DateTimeKind.Utc).AddTicks(6610), new DateTime(2026, 6, 28, 15, 45, 5, 309, DateTimeKind.Utc).AddTicks(6610) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 15, 45, 5, 309, DateTimeKind.Utc).AddTicks(6630), new DateTime(2026, 6, 28, 15, 45, 5, 309, DateTimeKind.Utc).AddTicks(6630) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 15, 45, 5, 309, DateTimeKind.Utc).AddTicks(6630), new DateTime(2026, 6, 28, 15, 45, 5, 309, DateTimeKind.Utc).AddTicks(6630) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 15, 45, 5, 309, DateTimeKind.Utc).AddTicks(6630), new DateTime(2026, 6, 28, 15, 45, 5, 309, DateTimeKind.Utc).AddTicks(6630) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 15, 45, 5, 309, DateTimeKind.Utc).AddTicks(6640), new DateTime(2026, 6, 28, 15, 45, 5, 309, DateTimeKind.Utc).AddTicks(6640) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 15, 45, 5, 314, DateTimeKind.Utc).AddTicks(3140), new DateTime(2026, 6, 28, 15, 45, 5, 314, DateTimeKind.Utc).AddTicks(3150) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 15, 45, 5, 314, DateTimeKind.Utc).AddTicks(3160), new DateTime(2026, 6, 28, 15, 45, 5, 314, DateTimeKind.Utc).AddTicks(3160) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 15, 45, 5, 314, DateTimeKind.Utc).AddTicks(3160), new DateTime(2026, 6, 28, 15, 45, 5, 314, DateTimeKind.Utc).AddTicks(3160) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 15, 45, 5, 314, DateTimeKind.Utc).AddTicks(3170), new DateTime(2026, 6, 28, 15, 45, 5, 314, DateTimeKind.Utc).AddTicks(3170) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 15, 45, 5, 314, DateTimeKind.Utc).AddTicks(3180), new DateTime(2026, 6, 28, 15, 45, 5, 314, DateTimeKind.Utc).AddTicks(3180) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 15, 45, 5, 314, DateTimeKind.Utc).AddTicks(3180), new DateTime(2026, 6, 28, 15, 45, 5, 314, DateTimeKind.Utc).AddTicks(3180) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 15, 45, 5, 314, DateTimeKind.Utc).AddTicks(3190), new DateTime(2026, 6, 28, 15, 45, 5, 314, DateTimeKind.Utc).AddTicks(3190) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 15, 45, 5, 314, DateTimeKind.Utc).AddTicks(3190), new DateTime(2026, 6, 28, 15, 45, 5, 314, DateTimeKind.Utc).AddTicks(3190) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000009"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 15, 45, 5, 314, DateTimeKind.Utc).AddTicks(3200), new DateTime(2026, 6, 28, 15, 45, 5, 314, DateTimeKind.Utc).AddTicks(3200) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000010"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 15, 45, 5, 314, DateTimeKind.Utc).AddTicks(3210), new DateTime(2026, 6, 28, 15, 45, 5, 314, DateTimeKind.Utc).AddTicks(3210) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000011"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 15, 45, 5, 314, DateTimeKind.Utc).AddTicks(3210), new DateTime(2026, 6, 28, 15, 45, 5, 314, DateTimeKind.Utc).AddTicks(3220) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000012"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 15, 45, 5, 314, DateTimeKind.Utc).AddTicks(3220), new DateTime(2026, 6, 28, 15, 45, 5, 314, DateTimeKind.Utc).AddTicks(3220) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000013"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 15, 45, 5, 314, DateTimeKind.Utc).AddTicks(3230), new DateTime(2026, 6, 28, 15, 45, 5, 314, DateTimeKind.Utc).AddTicks(3230) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000014"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 15, 45, 5, 314, DateTimeKind.Utc).AddTicks(3230), new DateTime(2026, 6, 28, 15, 45, 5, 314, DateTimeKind.Utc).AddTicks(3230) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000015"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 15, 45, 5, 314, DateTimeKind.Utc).AddTicks(3240), new DateTime(2026, 6, 28, 15, 45, 5, 314, DateTimeKind.Utc).AddTicks(3240) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000016"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 15, 45, 5, 314, DateTimeKind.Utc).AddTicks(3250), new DateTime(2026, 6, 28, 15, 45, 5, 314, DateTimeKind.Utc).AddTicks(3250) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000017"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 15, 45, 5, 314, DateTimeKind.Utc).AddTicks(3250), new DateTime(2026, 6, 28, 15, 45, 5, 314, DateTimeKind.Utc).AddTicks(3250) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000018"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 15, 45, 5, 314, DateTimeKind.Utc).AddTicks(3260), new DateTime(2026, 6, 28, 15, 45, 5, 314, DateTimeKind.Utc).AddTicks(3260) });
        }
    }
}
