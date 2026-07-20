using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VehicleServiceBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveServiceTypeBasePriceAndSeedServiceTypePrices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.Sql(
                """
                INSERT INTO "ServiceTypes" ("Id", "CreatedAt", "DurationMinutes", "IsActive", "Name", "UpdatedAt")
                VALUES
                  ('11111111-1111-1111-1111-030000000001', TIMESTAMPTZ '2026-07-18T10:52:34.1107100Z', 60, TRUE, 'Oil Change', TIMESTAMPTZ '2026-07-18T10:52:34.1107100Z'),
                  ('11111111-1111-1111-1111-030000000002', TIMESTAMPTZ '2026-07-18T10:52:34.1107100Z', 90, TRUE, 'Brake Inspection', TIMESTAMPTZ '2026-07-18T10:52:34.1107100Z'),
                  ('11111111-1111-1111-1111-030000000003', TIMESTAMPTZ '2026-07-18T10:52:34.1107100Z', 240, TRUE, 'Major Service', TIMESTAMPTZ '2026-07-18T10:52:34.1107100Z')
                ON CONFLICT ("Id") DO NOTHING;
                """);

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

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000018"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) });

            migrationBuilder.Sql(
                """
                INSERT INTO "ServiceTypePrices" ("Id", "CreatedAt", "CurrencyId", "IsActive", "Price", "ServiceTypeId", "UpdatedAt")
                VALUES
                  ('11111111-1111-1111-1111-031000000001', TIMESTAMPTZ '2026-07-18T10:52:34.1107100Z', '00000000-0000-0000-0004-000000000001', TRUE, 500000, '11111111-1111-1111-1111-030000000001', TIMESTAMPTZ '2026-07-18T10:52:34.1107100Z'),
                  ('11111111-1111-1111-1111-031000000002', TIMESTAMPTZ '2026-07-18T10:52:34.1107100Z', '00000000-0000-0000-0004-000000000001', TRUE, 750000, '11111111-1111-1111-1111-030000000002', TIMESTAMPTZ '2026-07-18T10:52:34.1107100Z'),
                  ('11111111-1111-1111-1111-031000000003', TIMESTAMPTZ '2026-07-18T10:52:34.1107100Z', '00000000-0000-0000-0004-000000000001', TRUE, 2200000, '11111111-1111-1111-1111-030000000003', TIMESTAMPTZ '2026-07-18T10:52:34.1107100Z'),
                  ('11111111-1111-1111-1111-031000000004', TIMESTAMPTZ '2026-07-18T10:52:34.1107100Z', '00000000-0000-0000-0004-000000000002', TRUE, 19.99, '11111111-1111-1111-1111-030000000001', TIMESTAMPTZ '2026-07-18T10:52:34.1107100Z'),
                  ('11111111-1111-1111-1111-031000000005', TIMESTAMPTZ '2026-07-18T10:52:34.1107100Z', '00000000-0000-0000-0004-000000000002', TRUE, 29.99, '11111111-1111-1111-1111-030000000002', TIMESTAMPTZ '2026-07-18T10:52:34.1107100Z'),
                  ('11111111-1111-1111-1111-031000000006', TIMESTAMPTZ '2026-07-18T10:52:34.1107100Z', '00000000-0000-0000-0004-000000000002', TRUE, 89.99, '11111111-1111-1111-1111-030000000003', TIMESTAMPTZ '2026-07-18T10:52:34.1107100Z')
                ON CONFLICT ("Id") DO NOTHING;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ServiceTypePrices",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-031000000001"));

            migrationBuilder.DeleteData(
                table: "ServiceTypePrices",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-031000000002"));

            migrationBuilder.DeleteData(
                table: "ServiceTypePrices",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-031000000003"));

            migrationBuilder.DeleteData(
                table: "ServiceTypePrices",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-031000000004"));

            migrationBuilder.DeleteData(
                table: "ServiceTypePrices",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-031000000005"));

            migrationBuilder.DeleteData(
                table: "ServiceTypePrices",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-031000000006"));

            migrationBuilder.DeleteData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-030000000001"));

            migrationBuilder.DeleteData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-030000000002"));

            migrationBuilder.DeleteData(
                table: "ServiceTypes",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-030000000003"));

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
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 107, DateTimeKind.Utc).AddTicks(5140), new DateTime(2026, 7, 18, 10, 52, 34, 107, DateTimeKind.Utc).AddTicks(5140) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 107, DateTimeKind.Utc).AddTicks(5140), new DateTime(2026, 7, 18, 10, 52, 34, 107, DateTimeKind.Utc).AddTicks(5140) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 107, DateTimeKind.Utc).AddTicks(5150), new DateTime(2026, 7, 18, 10, 52, 34, 107, DateTimeKind.Utc).AddTicks(5150) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 107, DateTimeKind.Utc).AddTicks(5150), new DateTime(2026, 7, 18, 10, 52, 34, 107, DateTimeKind.Utc).AddTicks(5150) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 107, DateTimeKind.Utc).AddTicks(5150), new DateTime(2026, 7, 18, 10, 52, 34, 107, DateTimeKind.Utc).AddTicks(5150) });

            migrationBuilder.UpdateData(
                table: "IdempotencyRequestStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 108, DateTimeKind.Utc).AddTicks(30), new DateTime(2026, 7, 18, 10, 52, 34, 108, DateTimeKind.Utc).AddTicks(30) });

            migrationBuilder.UpdateData(
                table: "IdempotencyRequestStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 108, DateTimeKind.Utc).AddTicks(30), new DateTime(2026, 7, 18, 10, 52, 34, 108, DateTimeKind.Utc).AddTicks(30) });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 111, DateTimeKind.Utc).AddTicks(2730), new DateTime(2026, 7, 18, 10, 52, 34, 111, DateTimeKind.Utc).AddTicks(2730) });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 111, DateTimeKind.Utc).AddTicks(2740), new DateTime(2026, 7, 18, 10, 52, 34, 111, DateTimeKind.Utc).AddTicks(2740) });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 111, DateTimeKind.Utc).AddTicks(2740), new DateTime(2026, 7, 18, 10, 52, 34, 111, DateTimeKind.Utc).AddTicks(2750) });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 111, DateTimeKind.Utc).AddTicks(2760), new DateTime(2026, 7, 18, 10, 52, 34, 111, DateTimeKind.Utc).AddTicks(2760) });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 111, DateTimeKind.Utc).AddTicks(2760), new DateTime(2026, 7, 18, 10, 52, 34, 111, DateTimeKind.Utc).AddTicks(2760) });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 111, DateTimeKind.Utc).AddTicks(2770), new DateTime(2026, 7, 18, 10, 52, 34, 111, DateTimeKind.Utc).AddTicks(2770) });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 111, DateTimeKind.Utc).AddTicks(2770), new DateTime(2026, 7, 18, 10, 52, 34, 111, DateTimeKind.Utc).AddTicks(2770) });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 111, DateTimeKind.Utc).AddTicks(2780), new DateTime(2026, 7, 18, 10, 52, 34, 111, DateTimeKind.Utc).AddTicks(2780) });

            migrationBuilder.UpdateData(
                table: "PaymentTransactionStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0005-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 111, DateTimeKind.Utc).AddTicks(3210), new DateTime(2026, 7, 18, 10, 52, 34, 111, DateTimeKind.Utc).AddTicks(3210) });

            migrationBuilder.UpdateData(
                table: "PaymentTransactionStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0005-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 111, DateTimeKind.Utc).AddTicks(3220), new DateTime(2026, 7, 18, 10, 52, 34, 111, DateTimeKind.Utc).AddTicks(3220) });

            migrationBuilder.UpdateData(
                table: "PaymentTransactionStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0005-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 111, DateTimeKind.Utc).AddTicks(3230), new DateTime(2026, 7, 18, 10, 52, 34, 111, DateTimeKind.Utc).AddTicks(3230) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 108, DateTimeKind.Utc).AddTicks(1930), new DateTime(2026, 7, 18, 10, 52, 34, 108, DateTimeKind.Utc).AddTicks(1930) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 108, DateTimeKind.Utc).AddTicks(1940), new DateTime(2026, 7, 18, 10, 52, 34, 108, DateTimeKind.Utc).AddTicks(1940) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 108, DateTimeKind.Utc).AddTicks(1940), new DateTime(2026, 7, 18, 10, 52, 34, 108, DateTimeKind.Utc).AddTicks(1940) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 108, DateTimeKind.Utc).AddTicks(1940), new DateTime(2026, 7, 18, 10, 52, 34, 108, DateTimeKind.Utc).AddTicks(1940) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 108, DateTimeKind.Utc).AddTicks(1950), new DateTime(2026, 7, 18, 10, 52, 34, 108, DateTimeKind.Utc).AddTicks(1950) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 114, DateTimeKind.Utc).AddTicks(7550), new DateTime(2026, 7, 18, 10, 52, 34, 114, DateTimeKind.Utc).AddTicks(7550) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 114, DateTimeKind.Utc).AddTicks(7560), new DateTime(2026, 7, 18, 10, 52, 34, 114, DateTimeKind.Utc).AddTicks(7560) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 114, DateTimeKind.Utc).AddTicks(7570), new DateTime(2026, 7, 18, 10, 52, 34, 114, DateTimeKind.Utc).AddTicks(7570) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 114, DateTimeKind.Utc).AddTicks(7580), new DateTime(2026, 7, 18, 10, 52, 34, 114, DateTimeKind.Utc).AddTicks(7580) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 114, DateTimeKind.Utc).AddTicks(7580), new DateTime(2026, 7, 18, 10, 52, 34, 114, DateTimeKind.Utc).AddTicks(7580) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 114, DateTimeKind.Utc).AddTicks(7590), new DateTime(2026, 7, 18, 10, 52, 34, 114, DateTimeKind.Utc).AddTicks(7590) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 114, DateTimeKind.Utc).AddTicks(7600), new DateTime(2026, 7, 18, 10, 52, 34, 114, DateTimeKind.Utc).AddTicks(7600) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 114, DateTimeKind.Utc).AddTicks(7600), new DateTime(2026, 7, 18, 10, 52, 34, 114, DateTimeKind.Utc).AddTicks(7600) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000009"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 114, DateTimeKind.Utc).AddTicks(7610), new DateTime(2026, 7, 18, 10, 52, 34, 114, DateTimeKind.Utc).AddTicks(7610) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000010"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 114, DateTimeKind.Utc).AddTicks(7620), new DateTime(2026, 7, 18, 10, 52, 34, 114, DateTimeKind.Utc).AddTicks(7620) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000011"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 114, DateTimeKind.Utc).AddTicks(7620), new DateTime(2026, 7, 18, 10, 52, 34, 114, DateTimeKind.Utc).AddTicks(7620) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000012"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 114, DateTimeKind.Utc).AddTicks(7630), new DateTime(2026, 7, 18, 10, 52, 34, 114, DateTimeKind.Utc).AddTicks(7630) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000013"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 114, DateTimeKind.Utc).AddTicks(7640), new DateTime(2026, 7, 18, 10, 52, 34, 114, DateTimeKind.Utc).AddTicks(7640) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000014"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 114, DateTimeKind.Utc).AddTicks(7640), new DateTime(2026, 7, 18, 10, 52, 34, 114, DateTimeKind.Utc).AddTicks(7640) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000015"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 114, DateTimeKind.Utc).AddTicks(7650), new DateTime(2026, 7, 18, 10, 52, 34, 114, DateTimeKind.Utc).AddTicks(7650) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000016"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 114, DateTimeKind.Utc).AddTicks(7650), new DateTime(2026, 7, 18, 10, 52, 34, 114, DateTimeKind.Utc).AddTicks(7650) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000017"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 114, DateTimeKind.Utc).AddTicks(7660), new DateTime(2026, 7, 18, 10, 52, 34, 114, DateTimeKind.Utc).AddTicks(7660) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000018"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 114, DateTimeKind.Utc).AddTicks(7670), new DateTime(2026, 7, 18, 10, 52, 34, 114, DateTimeKind.Utc).AddTicks(7670) });

            migrationBuilder.AddCheckConstraint(
                name: "CK_ServiceType_Price_NonNegative",
                table: "ServiceTypes",
                sql: "\"Price\" >= 0");
        }
    }
}
