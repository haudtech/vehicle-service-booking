using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleServiceBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceTypePrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "ServiceTypes",
                type: "numeric(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 47, 52, 163, DateTimeKind.Utc).AddTicks(2700), new DateTime(2026, 6, 28, 7, 47, 52, 163, DateTimeKind.Utc).AddTicks(2700) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 47, 52, 163, DateTimeKind.Utc).AddTicks(2700), new DateTime(2026, 6, 28, 7, 47, 52, 163, DateTimeKind.Utc).AddTicks(2700) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 47, 52, 163, DateTimeKind.Utc).AddTicks(2710), new DateTime(2026, 6, 28, 7, 47, 52, 163, DateTimeKind.Utc).AddTicks(2710) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 47, 52, 163, DateTimeKind.Utc).AddTicks(2710), new DateTime(2026, 6, 28, 7, 47, 52, 163, DateTimeKind.Utc).AddTicks(2710) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 47, 52, 163, DateTimeKind.Utc).AddTicks(2720), new DateTime(2026, 6, 28, 7, 47, 52, 163, DateTimeKind.Utc).AddTicks(2720) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 47, 52, 163, DateTimeKind.Utc).AddTicks(6830), new DateTime(2026, 6, 28, 7, 47, 52, 163, DateTimeKind.Utc).AddTicks(6830) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 47, 52, 163, DateTimeKind.Utc).AddTicks(6830), new DateTime(2026, 6, 28, 7, 47, 52, 163, DateTimeKind.Utc).AddTicks(6830) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 47, 52, 163, DateTimeKind.Utc).AddTicks(6840), new DateTime(2026, 6, 28, 7, 47, 52, 163, DateTimeKind.Utc).AddTicks(6840) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 47, 52, 163, DateTimeKind.Utc).AddTicks(6840), new DateTime(2026, 6, 28, 7, 47, 52, 163, DateTimeKind.Utc).AddTicks(6840) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 47, 52, 163, DateTimeKind.Utc).AddTicks(6840), new DateTime(2026, 6, 28, 7, 47, 52, 163, DateTimeKind.Utc).AddTicks(6840) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 47, 52, 167, DateTimeKind.Utc).AddTicks(2670), new DateTime(2026, 6, 28, 7, 47, 52, 167, DateTimeKind.Utc).AddTicks(2670) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 47, 52, 167, DateTimeKind.Utc).AddTicks(2700), new DateTime(2026, 6, 28, 7, 47, 52, 167, DateTimeKind.Utc).AddTicks(2700) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 47, 52, 167, DateTimeKind.Utc).AddTicks(2710), new DateTime(2026, 6, 28, 7, 47, 52, 167, DateTimeKind.Utc).AddTicks(2710) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 47, 52, 167, DateTimeKind.Utc).AddTicks(2720), new DateTime(2026, 6, 28, 7, 47, 52, 167, DateTimeKind.Utc).AddTicks(2720) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 47, 52, 167, DateTimeKind.Utc).AddTicks(2720), new DateTime(2026, 6, 28, 7, 47, 52, 167, DateTimeKind.Utc).AddTicks(2720) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 47, 52, 167, DateTimeKind.Utc).AddTicks(2730), new DateTime(2026, 6, 28, 7, 47, 52, 167, DateTimeKind.Utc).AddTicks(2730) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 47, 52, 167, DateTimeKind.Utc).AddTicks(2740), new DateTime(2026, 6, 28, 7, 47, 52, 167, DateTimeKind.Utc).AddTicks(2740) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 47, 52, 167, DateTimeKind.Utc).AddTicks(2740), new DateTime(2026, 6, 28, 7, 47, 52, 167, DateTimeKind.Utc).AddTicks(2740) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000009"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 47, 52, 167, DateTimeKind.Utc).AddTicks(2750), new DateTime(2026, 6, 28, 7, 47, 52, 167, DateTimeKind.Utc).AddTicks(2750) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000010"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 47, 52, 167, DateTimeKind.Utc).AddTicks(2760), new DateTime(2026, 6, 28, 7, 47, 52, 167, DateTimeKind.Utc).AddTicks(2760) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000011"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 47, 52, 167, DateTimeKind.Utc).AddTicks(2760), new DateTime(2026, 6, 28, 7, 47, 52, 167, DateTimeKind.Utc).AddTicks(2760) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000012"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 47, 52, 167, DateTimeKind.Utc).AddTicks(2770), new DateTime(2026, 6, 28, 7, 47, 52, 167, DateTimeKind.Utc).AddTicks(2770) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000013"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 47, 52, 167, DateTimeKind.Utc).AddTicks(2770), new DateTime(2026, 6, 28, 7, 47, 52, 167, DateTimeKind.Utc).AddTicks(2780) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000014"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 47, 52, 167, DateTimeKind.Utc).AddTicks(2780), new DateTime(2026, 6, 28, 7, 47, 52, 167, DateTimeKind.Utc).AddTicks(2780) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000015"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 47, 52, 167, DateTimeKind.Utc).AddTicks(2790), new DateTime(2026, 6, 28, 7, 47, 52, 167, DateTimeKind.Utc).AddTicks(2790) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000016"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 47, 52, 167, DateTimeKind.Utc).AddTicks(2790), new DateTime(2026, 6, 28, 7, 47, 52, 167, DateTimeKind.Utc).AddTicks(2790) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000017"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 47, 52, 167, DateTimeKind.Utc).AddTicks(2800), new DateTime(2026, 6, 28, 7, 47, 52, 167, DateTimeKind.Utc).AddTicks(2800) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000018"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 47, 52, 167, DateTimeKind.Utc).AddTicks(2810), new DateTime(2026, 6, 28, 7, 47, 52, 167, DateTimeKind.Utc).AddTicks(2810) });

            migrationBuilder.AddCheckConstraint(
                name: "CK_ServiceType_Price_NonNegative",
                table: "ServiceTypes",
                sql: "\"Price\" >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_ServiceType_Price_NonNegative",
                table: "ServiceTypes");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "ServiceTypes");

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 130, DateTimeKind.Utc).AddTicks(6330), new DateTime(2026, 6, 28, 7, 41, 59, 130, DateTimeKind.Utc).AddTicks(6330) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 130, DateTimeKind.Utc).AddTicks(6340), new DateTime(2026, 6, 28, 7, 41, 59, 130, DateTimeKind.Utc).AddTicks(6340) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 130, DateTimeKind.Utc).AddTicks(6350), new DateTime(2026, 6, 28, 7, 41, 59, 130, DateTimeKind.Utc).AddTicks(6350) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 130, DateTimeKind.Utc).AddTicks(6350), new DateTime(2026, 6, 28, 7, 41, 59, 130, DateTimeKind.Utc).AddTicks(6350) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 130, DateTimeKind.Utc).AddTicks(6350), new DateTime(2026, 6, 28, 7, 41, 59, 130, DateTimeKind.Utc).AddTicks(6350) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 131, DateTimeKind.Utc).AddTicks(460), new DateTime(2026, 6, 28, 7, 41, 59, 131, DateTimeKind.Utc).AddTicks(460) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 131, DateTimeKind.Utc).AddTicks(460), new DateTime(2026, 6, 28, 7, 41, 59, 131, DateTimeKind.Utc).AddTicks(460) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 131, DateTimeKind.Utc).AddTicks(460), new DateTime(2026, 6, 28, 7, 41, 59, 131, DateTimeKind.Utc).AddTicks(460) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 131, DateTimeKind.Utc).AddTicks(470), new DateTime(2026, 6, 28, 7, 41, 59, 131, DateTimeKind.Utc).AddTicks(470) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 131, DateTimeKind.Utc).AddTicks(470), new DateTime(2026, 6, 28, 7, 41, 59, 131, DateTimeKind.Utc).AddTicks(470) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 134, DateTimeKind.Utc).AddTicks(3480), new DateTime(2026, 6, 28, 7, 41, 59, 134, DateTimeKind.Utc).AddTicks(3480) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 134, DateTimeKind.Utc).AddTicks(3500), new DateTime(2026, 6, 28, 7, 41, 59, 134, DateTimeKind.Utc).AddTicks(3500) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 134, DateTimeKind.Utc).AddTicks(3500), new DateTime(2026, 6, 28, 7, 41, 59, 134, DateTimeKind.Utc).AddTicks(3500) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 134, DateTimeKind.Utc).AddTicks(3510), new DateTime(2026, 6, 28, 7, 41, 59, 134, DateTimeKind.Utc).AddTicks(3510) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 134, DateTimeKind.Utc).AddTicks(3520), new DateTime(2026, 6, 28, 7, 41, 59, 134, DateTimeKind.Utc).AddTicks(3520) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 134, DateTimeKind.Utc).AddTicks(3530), new DateTime(2026, 6, 28, 7, 41, 59, 134, DateTimeKind.Utc).AddTicks(3530) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 134, DateTimeKind.Utc).AddTicks(3530), new DateTime(2026, 6, 28, 7, 41, 59, 134, DateTimeKind.Utc).AddTicks(3530) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 134, DateTimeKind.Utc).AddTicks(3540), new DateTime(2026, 6, 28, 7, 41, 59, 134, DateTimeKind.Utc).AddTicks(3540) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000009"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 134, DateTimeKind.Utc).AddTicks(3550), new DateTime(2026, 6, 28, 7, 41, 59, 134, DateTimeKind.Utc).AddTicks(3550) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000010"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 134, DateTimeKind.Utc).AddTicks(3560), new DateTime(2026, 6, 28, 7, 41, 59, 134, DateTimeKind.Utc).AddTicks(3560) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000011"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 134, DateTimeKind.Utc).AddTicks(3560), new DateTime(2026, 6, 28, 7, 41, 59, 134, DateTimeKind.Utc).AddTicks(3560) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000012"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 134, DateTimeKind.Utc).AddTicks(3570), new DateTime(2026, 6, 28, 7, 41, 59, 134, DateTimeKind.Utc).AddTicks(3570) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000013"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 134, DateTimeKind.Utc).AddTicks(3580), new DateTime(2026, 6, 28, 7, 41, 59, 134, DateTimeKind.Utc).AddTicks(3580) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000014"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 134, DateTimeKind.Utc).AddTicks(3580), new DateTime(2026, 6, 28, 7, 41, 59, 134, DateTimeKind.Utc).AddTicks(3580) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000015"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 134, DateTimeKind.Utc).AddTicks(3590), new DateTime(2026, 6, 28, 7, 41, 59, 134, DateTimeKind.Utc).AddTicks(3590) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000016"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 134, DateTimeKind.Utc).AddTicks(3590), new DateTime(2026, 6, 28, 7, 41, 59, 134, DateTimeKind.Utc).AddTicks(3590) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000017"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 134, DateTimeKind.Utc).AddTicks(3600), new DateTime(2026, 6, 28, 7, 41, 59, 134, DateTimeKind.Utc).AddTicks(3600) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000018"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 134, DateTimeKind.Utc).AddTicks(3610), new DateTime(2026, 6, 28, 7, 41, 59, 134, DateTimeKind.Utc).AddTicks(3610) });
        }
    }
}
