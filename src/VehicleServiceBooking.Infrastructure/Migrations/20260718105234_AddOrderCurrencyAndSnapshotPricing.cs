using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleServiceBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderCurrencyAndSnapshotPricing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TransactionId",
                table: "PaymentOrders",
                newName: "PaymentTransactionId");

            migrationBuilder.AddColumn<decimal>(
                name: "LineTotal",
                table: "ServiceTypeOrders",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "ServiceTypeOrders",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "CurrencyId",
                table: "Orders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "Orders",
                type: "numeric(18,2)",
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
                table: "CurrencyLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0004-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 110, DateTimeKind.Utc).AddTicks(7100), new DateTime(2026, 7, 18, 10, 52, 34, 110, DateTimeKind.Utc).AddTicks(7100) });

            migrationBuilder.UpdateData(
                table: "CurrencyLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0004-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 10, 52, 34, 110, DateTimeKind.Utc).AddTicks(7100), new DateTime(2026, 7, 18, 10, 52, 34, 110, DateTimeKind.Utc).AddTicks(7100) });

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
                name: "CK_ServiceTypeOrder_LineTotal_NonNegative",
                table: "ServiceTypeOrders",
                sql: "\"LineTotal\" >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ServiceTypeOrder_UnitPrice_NonNegative",
                table: "ServiceTypeOrders",
                sql: "\"UnitPrice\" >= 0");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CurrencyId",
                table: "Orders",
                column: "CurrencyId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Order_TotalAmount_NonNegative",
                table: "Orders",
                sql: "\"TotalAmount\" >= 0");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Currency",
                table: "Orders",
                column: "CurrencyId",
                principalTable: "CurrencyLookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Currency",
                table: "Orders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ServiceTypeOrder_LineTotal_NonNegative",
                table: "ServiceTypeOrders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ServiceTypeOrder_UnitPrice_NonNegative",
                table: "ServiceTypeOrders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CurrencyId",
                table: "Orders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Order_TotalAmount_NonNegative",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "LineTotal",
                table: "ServiceTypeOrders");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "ServiceTypeOrders");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "PaymentTransactionId",
                table: "PaymentOrders",
                newName: "TransactionId");

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 325, DateTimeKind.Utc).AddTicks(9780), new DateTime(2026, 7, 18, 6, 1, 48, 325, DateTimeKind.Utc).AddTicks(9780) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 325, DateTimeKind.Utc).AddTicks(9790), new DateTime(2026, 7, 18, 6, 1, 48, 325, DateTimeKind.Utc).AddTicks(9790) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 325, DateTimeKind.Utc).AddTicks(9790), new DateTime(2026, 7, 18, 6, 1, 48, 325, DateTimeKind.Utc).AddTicks(9790) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 325, DateTimeKind.Utc).AddTicks(9790), new DateTime(2026, 7, 18, 6, 1, 48, 325, DateTimeKind.Utc).AddTicks(9790) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 325, DateTimeKind.Utc).AddTicks(9800), new DateTime(2026, 7, 18, 6, 1, 48, 325, DateTimeKind.Utc).AddTicks(9800) });

            migrationBuilder.UpdateData(
                table: "CurrencyLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0004-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(5010), new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(5010) });

            migrationBuilder.UpdateData(
                table: "CurrencyLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0004-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(5010), new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(5020) });

            migrationBuilder.UpdateData(
                table: "IdempotencyRequestStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 326, DateTimeKind.Utc).AddTicks(3840), new DateTime(2026, 7, 18, 6, 1, 48, 326, DateTimeKind.Utc).AddTicks(3840) });

            migrationBuilder.UpdateData(
                table: "IdempotencyRequestStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 326, DateTimeKind.Utc).AddTicks(3840), new DateTime(2026, 7, 18, 6, 1, 48, 326, DateTimeKind.Utc).AddTicks(3840) });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9030), new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9030) });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9040), new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9040) });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9040), new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9040) });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9060), new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9060) });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9060), new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9060) });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9070), new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9070) });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9070), new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9070) });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9080), new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9080) });

            migrationBuilder.UpdateData(
                table: "PaymentTransactionStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0005-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9430), new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9430) });

            migrationBuilder.UpdateData(
                table: "PaymentTransactionStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0005-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9430), new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9440) });

            migrationBuilder.UpdateData(
                table: "PaymentTransactionStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0005-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9440), new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9440) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 326, DateTimeKind.Utc).AddTicks(5580), new DateTime(2026, 7, 18, 6, 1, 48, 326, DateTimeKind.Utc).AddTicks(5580) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 326, DateTimeKind.Utc).AddTicks(5590), new DateTime(2026, 7, 18, 6, 1, 48, 326, DateTimeKind.Utc).AddTicks(5590) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 326, DateTimeKind.Utc).AddTicks(5590), new DateTime(2026, 7, 18, 6, 1, 48, 326, DateTimeKind.Utc).AddTicks(5590) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 326, DateTimeKind.Utc).AddTicks(5590), new DateTime(2026, 7, 18, 6, 1, 48, 326, DateTimeKind.Utc).AddTicks(5590) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 326, DateTimeKind.Utc).AddTicks(5600), new DateTime(2026, 7, 18, 6, 1, 48, 326, DateTimeKind.Utc).AddTicks(5600) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 331, DateTimeKind.Utc).AddTicks(660), new DateTime(2026, 7, 18, 6, 1, 48, 331, DateTimeKind.Utc).AddTicks(660) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 331, DateTimeKind.Utc).AddTicks(670), new DateTime(2026, 7, 18, 6, 1, 48, 331, DateTimeKind.Utc).AddTicks(670) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 331, DateTimeKind.Utc).AddTicks(680), new DateTime(2026, 7, 18, 6, 1, 48, 331, DateTimeKind.Utc).AddTicks(680) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 331, DateTimeKind.Utc).AddTicks(690), new DateTime(2026, 7, 18, 6, 1, 48, 331, DateTimeKind.Utc).AddTicks(690) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 331, DateTimeKind.Utc).AddTicks(690), new DateTime(2026, 7, 18, 6, 1, 48, 331, DateTimeKind.Utc).AddTicks(690) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 331, DateTimeKind.Utc).AddTicks(700), new DateTime(2026, 7, 18, 6, 1, 48, 331, DateTimeKind.Utc).AddTicks(700) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 331, DateTimeKind.Utc).AddTicks(700), new DateTime(2026, 7, 18, 6, 1, 48, 331, DateTimeKind.Utc).AddTicks(700) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 331, DateTimeKind.Utc).AddTicks(710), new DateTime(2026, 7, 18, 6, 1, 48, 331, DateTimeKind.Utc).AddTicks(710) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000009"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 331, DateTimeKind.Utc).AddTicks(720), new DateTime(2026, 7, 18, 6, 1, 48, 331, DateTimeKind.Utc).AddTicks(720) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000010"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 331, DateTimeKind.Utc).AddTicks(720), new DateTime(2026, 7, 18, 6, 1, 48, 331, DateTimeKind.Utc).AddTicks(720) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000011"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 331, DateTimeKind.Utc).AddTicks(730), new DateTime(2026, 7, 18, 6, 1, 48, 331, DateTimeKind.Utc).AddTicks(730) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000012"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 331, DateTimeKind.Utc).AddTicks(740), new DateTime(2026, 7, 18, 6, 1, 48, 331, DateTimeKind.Utc).AddTicks(740) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000013"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 331, DateTimeKind.Utc).AddTicks(740), new DateTime(2026, 7, 18, 6, 1, 48, 331, DateTimeKind.Utc).AddTicks(740) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000014"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 331, DateTimeKind.Utc).AddTicks(750), new DateTime(2026, 7, 18, 6, 1, 48, 331, DateTimeKind.Utc).AddTicks(750) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000015"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 331, DateTimeKind.Utc).AddTicks(760), new DateTime(2026, 7, 18, 6, 1, 48, 331, DateTimeKind.Utc).AddTicks(760) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000016"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 331, DateTimeKind.Utc).AddTicks(760), new DateTime(2026, 7, 18, 6, 1, 48, 331, DateTimeKind.Utc).AddTicks(760) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000017"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 331, DateTimeKind.Utc).AddTicks(770), new DateTime(2026, 7, 18, 6, 1, 48, 331, DateTimeKind.Utc).AddTicks(770) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000018"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 7, 18, 6, 1, 48, 331, DateTimeKind.Utc).AddTicks(770), new DateTime(2026, 7, 18, 6, 1, 48, 331, DateTimeKind.Utc).AddTicks(770) });
        }
    }
}
