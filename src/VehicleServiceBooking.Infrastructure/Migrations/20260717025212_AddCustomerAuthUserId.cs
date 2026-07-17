using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleServiceBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerAuthUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AuthUserId",
                table: "Customers",
                type: "uuid",
                nullable: true);

            // Backfill existing rows with a unique value before applying NOT NULL + unique index.
            migrationBuilder.Sql("UPDATE \"Customers\" SET \"AuthUserId\" = \"Id\" WHERE \"AuthUserId\" IS NULL;");

            migrationBuilder.AlterColumn<Guid>(
                name: "AuthUserId",
                table: "Customers",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 17, 2, 52, 11, 847, DateTimeKind.Utc).AddTicks(1170), new DateTime(2026, 7, 17, 2, 52, 11, 847, DateTimeKind.Utc).AddTicks(1170) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 17, 2, 52, 11, 847, DateTimeKind.Utc).AddTicks(1180), new DateTime(2026, 7, 17, 2, 52, 11, 847, DateTimeKind.Utc).AddTicks(1180) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 17, 2, 52, 11, 847, DateTimeKind.Utc).AddTicks(1180), new DateTime(2026, 7, 17, 2, 52, 11, 847, DateTimeKind.Utc).AddTicks(1180) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 17, 2, 52, 11, 847, DateTimeKind.Utc).AddTicks(1180), new DateTime(2026, 7, 17, 2, 52, 11, 847, DateTimeKind.Utc).AddTicks(1180) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 17, 2, 52, 11, 847, DateTimeKind.Utc).AddTicks(1190), new DateTime(2026, 7, 17, 2, 52, 11, 847, DateTimeKind.Utc).AddTicks(1190) });

            migrationBuilder.UpdateData(
                table: "IdempotencyRequestStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 17, 2, 52, 11, 847, DateTimeKind.Utc).AddTicks(5060), new DateTime(2026, 7, 17, 2, 52, 11, 847, DateTimeKind.Utc).AddTicks(5060) });

            migrationBuilder.UpdateData(
                table: "IdempotencyRequestStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 17, 2, 52, 11, 847, DateTimeKind.Utc).AddTicks(5070), new DateTime(2026, 7, 17, 2, 52, 11, 847, DateTimeKind.Utc).AddTicks(5070) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 17, 2, 52, 11, 847, DateTimeKind.Utc).AddTicks(6690), new DateTime(2026, 7, 17, 2, 52, 11, 847, DateTimeKind.Utc).AddTicks(6690) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 17, 2, 52, 11, 847, DateTimeKind.Utc).AddTicks(6690), new DateTime(2026, 7, 17, 2, 52, 11, 847, DateTimeKind.Utc).AddTicks(6690) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 17, 2, 52, 11, 847, DateTimeKind.Utc).AddTicks(6700), new DateTime(2026, 7, 17, 2, 52, 11, 847, DateTimeKind.Utc).AddTicks(6700) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 17, 2, 52, 11, 847, DateTimeKind.Utc).AddTicks(6700), new DateTime(2026, 7, 17, 2, 52, 11, 847, DateTimeKind.Utc).AddTicks(6700) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 17, 2, 52, 11, 847, DateTimeKind.Utc).AddTicks(6700), new DateTime(2026, 7, 17, 2, 52, 11, 847, DateTimeKind.Utc).AddTicks(6700) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 17, 2, 52, 11, 850, DateTimeKind.Utc).AddTicks(9620), new DateTime(2026, 7, 17, 2, 52, 11, 850, DateTimeKind.Utc).AddTicks(9620) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 17, 2, 52, 11, 850, DateTimeKind.Utc).AddTicks(9630), new DateTime(2026, 7, 17, 2, 52, 11, 850, DateTimeKind.Utc).AddTicks(9630) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 17, 2, 52, 11, 850, DateTimeKind.Utc).AddTicks(9640), new DateTime(2026, 7, 17, 2, 52, 11, 850, DateTimeKind.Utc).AddTicks(9640) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 17, 2, 52, 11, 850, DateTimeKind.Utc).AddTicks(9650), new DateTime(2026, 7, 17, 2, 52, 11, 850, DateTimeKind.Utc).AddTicks(9650) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 17, 2, 52, 11, 850, DateTimeKind.Utc).AddTicks(9650), new DateTime(2026, 7, 17, 2, 52, 11, 850, DateTimeKind.Utc).AddTicks(9650) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 17, 2, 52, 11, 850, DateTimeKind.Utc).AddTicks(9660), new DateTime(2026, 7, 17, 2, 52, 11, 850, DateTimeKind.Utc).AddTicks(9660) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 17, 2, 52, 11, 850, DateTimeKind.Utc).AddTicks(9670), new DateTime(2026, 7, 17, 2, 52, 11, 850, DateTimeKind.Utc).AddTicks(9670) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 17, 2, 52, 11, 850, DateTimeKind.Utc).AddTicks(9670), new DateTime(2026, 7, 17, 2, 52, 11, 850, DateTimeKind.Utc).AddTicks(9670) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000009"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 17, 2, 52, 11, 850, DateTimeKind.Utc).AddTicks(9680), new DateTime(2026, 7, 17, 2, 52, 11, 850, DateTimeKind.Utc).AddTicks(9680) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000010"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 17, 2, 52, 11, 850, DateTimeKind.Utc).AddTicks(9680), new DateTime(2026, 7, 17, 2, 52, 11, 850, DateTimeKind.Utc).AddTicks(9680) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000011"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 17, 2, 52, 11, 850, DateTimeKind.Utc).AddTicks(9690), new DateTime(2026, 7, 17, 2, 52, 11, 850, DateTimeKind.Utc).AddTicks(9690) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000012"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 17, 2, 52, 11, 850, DateTimeKind.Utc).AddTicks(9700), new DateTime(2026, 7, 17, 2, 52, 11, 850, DateTimeKind.Utc).AddTicks(9700) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000013"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 17, 2, 52, 11, 850, DateTimeKind.Utc).AddTicks(9700), new DateTime(2026, 7, 17, 2, 52, 11, 850, DateTimeKind.Utc).AddTicks(9700) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000014"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 17, 2, 52, 11, 850, DateTimeKind.Utc).AddTicks(9710), new DateTime(2026, 7, 17, 2, 52, 11, 850, DateTimeKind.Utc).AddTicks(9710) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000015"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 17, 2, 52, 11, 850, DateTimeKind.Utc).AddTicks(9710), new DateTime(2026, 7, 17, 2, 52, 11, 850, DateTimeKind.Utc).AddTicks(9710) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000016"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 17, 2, 52, 11, 850, DateTimeKind.Utc).AddTicks(9720), new DateTime(2026, 7, 17, 2, 52, 11, 850, DateTimeKind.Utc).AddTicks(9720) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000017"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 17, 2, 52, 11, 850, DateTimeKind.Utc).AddTicks(9720), new DateTime(2026, 7, 17, 2, 52, 11, 850, DateTimeKind.Utc).AddTicks(9720) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000018"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 17, 2, 52, 11, 850, DateTimeKind.Utc).AddTicks(9730), new DateTime(2026, 7, 17, 2, 52, 11, 850, DateTimeKind.Utc).AddTicks(9730) });

            migrationBuilder.CreateIndex(
                name: "UX_Customers_AuthUserId",
                table: "Customers",
                column: "AuthUserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Customers_AuthUserId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "AuthUserId",
                table: "Customers");

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
    }
}
