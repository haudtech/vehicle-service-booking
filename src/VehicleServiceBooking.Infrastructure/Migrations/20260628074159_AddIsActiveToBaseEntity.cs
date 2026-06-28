using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleServiceBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveToBaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Vehicles",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TechnicianSkills",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TechnicianSchedules",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Technicians",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ServiceTypes",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ServiceStatusLookups",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Services",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Dealerships",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Customers",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "BusinessHours",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AppointmentStatusLookups",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Appointments",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "IsActive", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 130, DateTimeKind.Utc).AddTicks(6330), true, new DateTime(2026, 6, 28, 7, 41, 59, 130, DateTimeKind.Utc).AddTicks(6330) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "IsActive", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 130, DateTimeKind.Utc).AddTicks(6340), true, new DateTime(2026, 6, 28, 7, 41, 59, 130, DateTimeKind.Utc).AddTicks(6340) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "IsActive", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 130, DateTimeKind.Utc).AddTicks(6350), true, new DateTime(2026, 6, 28, 7, 41, 59, 130, DateTimeKind.Utc).AddTicks(6350) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "IsActive", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 130, DateTimeKind.Utc).AddTicks(6350), true, new DateTime(2026, 6, 28, 7, 41, 59, 130, DateTimeKind.Utc).AddTicks(6350) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "IsActive", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 130, DateTimeKind.Utc).AddTicks(6350), true, new DateTime(2026, 6, 28, 7, 41, 59, 130, DateTimeKind.Utc).AddTicks(6350) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000001"),
                columns: new[] { "CreatedAt", "IsActive", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 131, DateTimeKind.Utc).AddTicks(460), true, new DateTime(2026, 6, 28, 7, 41, 59, 131, DateTimeKind.Utc).AddTicks(460) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000002"),
                columns: new[] { "CreatedAt", "IsActive", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 131, DateTimeKind.Utc).AddTicks(460), true, new DateTime(2026, 6, 28, 7, 41, 59, 131, DateTimeKind.Utc).AddTicks(460) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000003"),
                columns: new[] { "CreatedAt", "IsActive", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 131, DateTimeKind.Utc).AddTicks(460), true, new DateTime(2026, 6, 28, 7, 41, 59, 131, DateTimeKind.Utc).AddTicks(460) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000004"),
                columns: new[] { "CreatedAt", "IsActive", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 131, DateTimeKind.Utc).AddTicks(470), true, new DateTime(2026, 6, 28, 7, 41, 59, 131, DateTimeKind.Utc).AddTicks(470) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000005"),
                columns: new[] { "CreatedAt", "IsActive", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 41, 59, 131, DateTimeKind.Utc).AddTicks(470), true, new DateTime(2026, 6, 28, 7, 41, 59, 131, DateTimeKind.Utc).AddTicks(470) });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TechnicianSkills");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TechnicianSchedules");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Technicians");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ServiceTypes");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ServiceStatusLookups");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Dealerships");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "BusinessHours");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AppointmentStatusLookups");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Appointments");

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 27, 19, 1, 36, 220, DateTimeKind.Utc).AddTicks(7220), new DateTime(2026, 6, 27, 19, 1, 36, 220, DateTimeKind.Utc).AddTicks(7220) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 27, 19, 1, 36, 220, DateTimeKind.Utc).AddTicks(7220), new DateTime(2026, 6, 27, 19, 1, 36, 220, DateTimeKind.Utc).AddTicks(7220) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 27, 19, 1, 36, 220, DateTimeKind.Utc).AddTicks(7230), new DateTime(2026, 6, 27, 19, 1, 36, 220, DateTimeKind.Utc).AddTicks(7230) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 27, 19, 1, 36, 220, DateTimeKind.Utc).AddTicks(7230), new DateTime(2026, 6, 27, 19, 1, 36, 220, DateTimeKind.Utc).AddTicks(7230) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 27, 19, 1, 36, 220, DateTimeKind.Utc).AddTicks(7240), new DateTime(2026, 6, 27, 19, 1, 36, 220, DateTimeKind.Utc).AddTicks(7240) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 27, 19, 1, 36, 221, DateTimeKind.Utc).AddTicks(870), new DateTime(2026, 6, 27, 19, 1, 36, 221, DateTimeKind.Utc).AddTicks(870) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 27, 19, 1, 36, 221, DateTimeKind.Utc).AddTicks(880), new DateTime(2026, 6, 27, 19, 1, 36, 221, DateTimeKind.Utc).AddTicks(880) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 27, 19, 1, 36, 221, DateTimeKind.Utc).AddTicks(880), new DateTime(2026, 6, 27, 19, 1, 36, 221, DateTimeKind.Utc).AddTicks(880) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 27, 19, 1, 36, 221, DateTimeKind.Utc).AddTicks(890), new DateTime(2026, 6, 27, 19, 1, 36, 221, DateTimeKind.Utc).AddTicks(890) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 27, 19, 1, 36, 221, DateTimeKind.Utc).AddTicks(890), new DateTime(2026, 6, 27, 19, 1, 36, 221, DateTimeKind.Utc).AddTicks(890) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 27, 19, 1, 36, 224, DateTimeKind.Utc).AddTicks(1060), new DateTime(2026, 6, 27, 19, 1, 36, 224, DateTimeKind.Utc).AddTicks(1060) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 27, 19, 1, 36, 224, DateTimeKind.Utc).AddTicks(1070), new DateTime(2026, 6, 27, 19, 1, 36, 224, DateTimeKind.Utc).AddTicks(1070) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 27, 19, 1, 36, 224, DateTimeKind.Utc).AddTicks(1080), new DateTime(2026, 6, 27, 19, 1, 36, 224, DateTimeKind.Utc).AddTicks(1080) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 27, 19, 1, 36, 224, DateTimeKind.Utc).AddTicks(1080), new DateTime(2026, 6, 27, 19, 1, 36, 224, DateTimeKind.Utc).AddTicks(1080) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 27, 19, 1, 36, 224, DateTimeKind.Utc).AddTicks(1090), new DateTime(2026, 6, 27, 19, 1, 36, 224, DateTimeKind.Utc).AddTicks(1090) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 27, 19, 1, 36, 224, DateTimeKind.Utc).AddTicks(1100), new DateTime(2026, 6, 27, 19, 1, 36, 224, DateTimeKind.Utc).AddTicks(1100) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 27, 19, 1, 36, 224, DateTimeKind.Utc).AddTicks(1100), new DateTime(2026, 6, 27, 19, 1, 36, 224, DateTimeKind.Utc).AddTicks(1100) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 27, 19, 1, 36, 224, DateTimeKind.Utc).AddTicks(1110), new DateTime(2026, 6, 27, 19, 1, 36, 224, DateTimeKind.Utc).AddTicks(1110) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000009"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 27, 19, 1, 36, 224, DateTimeKind.Utc).AddTicks(1110), new DateTime(2026, 6, 27, 19, 1, 36, 224, DateTimeKind.Utc).AddTicks(1110) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000010"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 27, 19, 1, 36, 224, DateTimeKind.Utc).AddTicks(1120), new DateTime(2026, 6, 27, 19, 1, 36, 224, DateTimeKind.Utc).AddTicks(1120) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000011"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 27, 19, 1, 36, 224, DateTimeKind.Utc).AddTicks(1130), new DateTime(2026, 6, 27, 19, 1, 36, 224, DateTimeKind.Utc).AddTicks(1130) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000012"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 27, 19, 1, 36, 224, DateTimeKind.Utc).AddTicks(1130), new DateTime(2026, 6, 27, 19, 1, 36, 224, DateTimeKind.Utc).AddTicks(1130) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000013"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 27, 19, 1, 36, 224, DateTimeKind.Utc).AddTicks(1140), new DateTime(2026, 6, 27, 19, 1, 36, 224, DateTimeKind.Utc).AddTicks(1140) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000014"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 27, 19, 1, 36, 224, DateTimeKind.Utc).AddTicks(1140), new DateTime(2026, 6, 27, 19, 1, 36, 224, DateTimeKind.Utc).AddTicks(1140) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000015"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 27, 19, 1, 36, 224, DateTimeKind.Utc).AddTicks(1150), new DateTime(2026, 6, 27, 19, 1, 36, 224, DateTimeKind.Utc).AddTicks(1150) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000016"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 27, 19, 1, 36, 224, DateTimeKind.Utc).AddTicks(1160), new DateTime(2026, 6, 27, 19, 1, 36, 224, DateTimeKind.Utc).AddTicks(1160) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000017"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 27, 19, 1, 36, 224, DateTimeKind.Utc).AddTicks(1160), new DateTime(2026, 6, 27, 19, 1, 36, 224, DateTimeKind.Utc).AddTicks(1160) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000018"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 27, 19, 1, 36, 224, DateTimeKind.Utc).AddTicks(1170), new DateTime(2026, 6, 27, 19, 1, 36, 224, DateTimeKind.Utc).AddTicks(1170) });
        }
    }
}
