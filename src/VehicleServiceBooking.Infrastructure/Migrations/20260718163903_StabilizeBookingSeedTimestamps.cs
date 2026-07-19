using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleServiceBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StabilizeBookingSeedTimestamps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) });

            migrationBuilder.UpdateData(
                table: "PaymentTransactionStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0005-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) });

            migrationBuilder.UpdateData(
                table: "PaymentTransactionStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0005-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) });

            migrationBuilder.UpdateData(
                table: "PaymentTransactionStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0005-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) });

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

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000009"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000010"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000011"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000012"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000013"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000014"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000015"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000016"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000017"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 894, DateTimeKind.Utc).AddTicks(7560), new DateTime(2026, 7, 18, 10, 59, 32, 894, DateTimeKind.Utc).AddTicks(7560) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 894, DateTimeKind.Utc).AddTicks(7560), new DateTime(2026, 7, 18, 10, 59, 32, 894, DateTimeKind.Utc).AddTicks(7560) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 894, DateTimeKind.Utc).AddTicks(7570), new DateTime(2026, 7, 18, 10, 59, 32, 894, DateTimeKind.Utc).AddTicks(7570) });

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
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 895, DateTimeKind.Utc).AddTicks(3270), new DateTime(2026, 7, 18, 10, 59, 32, 895, DateTimeKind.Utc).AddTicks(3270) });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 897, DateTimeKind.Utc).AddTicks(740), new DateTime(2026, 7, 18, 10, 59, 32, 897, DateTimeKind.Utc).AddTicks(740) });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 897, DateTimeKind.Utc).AddTicks(750), new DateTime(2026, 7, 18, 10, 59, 32, 897, DateTimeKind.Utc).AddTicks(750) });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 897, DateTimeKind.Utc).AddTicks(750), new DateTime(2026, 7, 18, 10, 59, 32, 897, DateTimeKind.Utc).AddTicks(750) });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 897, DateTimeKind.Utc).AddTicks(750), new DateTime(2026, 7, 18, 10, 59, 32, 897, DateTimeKind.Utc).AddTicks(750) });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 897, DateTimeKind.Utc).AddTicks(760), new DateTime(2026, 7, 18, 10, 59, 32, 897, DateTimeKind.Utc).AddTicks(760) });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 897, DateTimeKind.Utc).AddTicks(760), new DateTime(2026, 7, 18, 10, 59, 32, 897, DateTimeKind.Utc).AddTicks(760) });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 897, DateTimeKind.Utc).AddTicks(770), new DateTime(2026, 7, 18, 10, 59, 32, 897, DateTimeKind.Utc).AddTicks(770) });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 897, DateTimeKind.Utc).AddTicks(770), new DateTime(2026, 7, 18, 10, 59, 32, 897, DateTimeKind.Utc).AddTicks(770) });

            migrationBuilder.UpdateData(
                table: "PaymentTransactionStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0005-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 897, DateTimeKind.Utc).AddTicks(1230), new DateTime(2026, 7, 18, 10, 59, 32, 897, DateTimeKind.Utc).AddTicks(1230) });

            migrationBuilder.UpdateData(
                table: "PaymentTransactionStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0005-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 897, DateTimeKind.Utc).AddTicks(1240), new DateTime(2026, 7, 18, 10, 59, 32, 897, DateTimeKind.Utc).AddTicks(1240) });

            migrationBuilder.UpdateData(
                table: "PaymentTransactionStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0005-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 897, DateTimeKind.Utc).AddTicks(1240), new DateTime(2026, 7, 18, 10, 59, 32, 897, DateTimeKind.Utc).AddTicks(1240) });

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
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 895, DateTimeKind.Utc).AddTicks(5200), new DateTime(2026, 7, 18, 10, 59, 32, 895, DateTimeKind.Utc).AddTicks(5200) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 895, DateTimeKind.Utc).AddTicks(5200), new DateTime(2026, 7, 18, 10, 59, 32, 895, DateTimeKind.Utc).AddTicks(5200) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 895, DateTimeKind.Utc).AddTicks(5200), new DateTime(2026, 7, 18, 10, 59, 32, 895, DateTimeKind.Utc).AddTicks(5200) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 895, DateTimeKind.Utc).AddTicks(5210), new DateTime(2026, 7, 18, 10, 59, 32, 895, DateTimeKind.Utc).AddTicks(5210) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(4870), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(4870) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(4880), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(4880) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(4880), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(4880) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(4890), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(4890) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(4900), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(4900) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(4900), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(4900) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(4910), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(4910) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(4920), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(4920) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000009"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(4920), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(4920) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000010"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(4930), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(4930) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000011"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(4940), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(4940) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000012"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(4940), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(4940) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000013"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(4950), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(4950) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000014"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(4960), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(4960) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000015"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(4980), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(4980) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000016"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(4990), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(4990) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000017"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(4990), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(4990) });
        }
    }
}
