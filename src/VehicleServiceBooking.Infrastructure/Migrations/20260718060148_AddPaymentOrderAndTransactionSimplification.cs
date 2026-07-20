using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VehicleServiceBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentOrderAndTransactionSimplification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CurrencyLookups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Symbol = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    DecimalPlaces = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyLookups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentProviderLookups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Provider = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentProviderLookups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransactionStatusLookups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactionStatusLookups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceTypePrices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTypePrices", x => x.Id);
                    table.CheckConstraint("CK_ServiceTypePrice_Price_NonNegative", "\"Price\" >= 0");
                    table.ForeignKey(
                        name: "FK_ServiceTypePrice_Currency",
                        column: x => x.CurrencyId,
                        principalTable: "CurrencyLookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceTypePrice_ServiceType",
                        column: x => x.ServiceTypeId,
                        principalTable: "ServiceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderAppointments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    AppointmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderAppointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderAppointment_Appointment",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderAppointment_Order",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceTypeOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTypeOrders", x => x.Id);
                    table.CheckConstraint("CK_ServiceTypeOrder_Quantity_Positive", "\"Quantity\" > 0");
                    table.ForeignKey(
                        name: "FK_ServiceTypeOrder_Order",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceTypeOrder_ServiceType",
                        column: x => x.ServiceTypeId,
                        principalTable: "ServiceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FromAccount = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    ToAccount = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Direction = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CurrencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    StatusId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.Id);
                    table.CheckConstraint("CK_PaymentTransaction_Amount_NonNegative", "\"Amount\" >= 0");
                    table.ForeignKey(
                        name: "FK_PaymentTransaction_Currency",
                        column: x => x.CurrencyId,
                        principalTable: "CurrencyLookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentTransaction_StatusLookup",
                        column: x => x.StatusId,
                        principalTable: "PaymentTransactionStatusLookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentOrder_Order",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentOrder_PaymentTransaction",
                        column: x => x.TransactionId,
                        principalTable: "PaymentTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.InsertData(
                table: "CurrencyLookups",
                columns: new[] { "Id", "Code", "CreatedAt", "DecimalPlaces", "IsActive", "Name", "Symbol", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0004-000000000001"), "VND", new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(5010), 0, true, "Vietnamese Dong", "VND", new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(5010) },
                    { new Guid("00000000-0000-0000-0004-000000000002"), "USD", new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(5010), 2, true, "US Dollar", "USD", new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(5020) }
                });

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

            migrationBuilder.InsertData(
                table: "PaymentProviderLookups",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "Provider", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0003-000000000001"), new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9030), "Zalo Pay e-wallet and gateway", true, "Zalo Pay", 1, new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9030) },
                    { new Guid("00000000-0000-0000-0003-000000000002"), new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9040), "Momo wallet payments", true, "Momo", 2, new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9040) },
                    { new Guid("00000000-0000-0000-0003-000000000003"), new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9040), "Apple Pay card tokenization gateway", true, "Apple Pay", 3, new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9040) },
                    { new Guid("00000000-0000-0000-0003-000000000004"), new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9060), "Visa card network", true, "Visa", 4, new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9060) },
                    { new Guid("00000000-0000-0000-0003-000000000005"), new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9060), "MasterCard card network", true, "Master Card", 5, new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9060) },
                    { new Guid("00000000-0000-0000-0003-000000000006"), new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9070), "VNPay online payment gateway", true, "VNPay", 6, new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9070) },
                    { new Guid("00000000-0000-0000-0003-000000000007"), new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9070), "Domestic ATM transfer", true, "ATM Transfer", 7, new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9070) },
                    { new Guid("00000000-0000-0000-0003-000000000008"), new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9080), "Internal system wallet balance", true, "Internal Wallet", 8, new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9080) }
                });

            migrationBuilder.InsertData(
                table: "PaymentTransactionStatusLookups",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0005-000000000001"), new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9430), "Transaction is created and waiting for processing", true, "Pending", 1, new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9430) },
                    { new Guid("00000000-0000-0000-0005-000000000002"), new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9430), "Transaction is being processed", true, "In Progress", 2, new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9440) },
                    { new Guid("00000000-0000-0000-0005-000000000003"), new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9440), "Transaction has been completed successfully", true, "Completed", 3, new DateTime(2026, 7, 18, 6, 1, 48, 327, DateTimeKind.Utc).AddTicks(9440) }
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyLookup_Code_Unique",
                table: "CurrencyLookups",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderAppointment_Appointment_Unique",
                table: "OrderAppointments",
                column: "AppointmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderAppointment_Unique_Order_Appointment",
                table: "OrderAppointments",
                columns: new[] { "OrderId", "AppointmentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_OrderCode_Unique",
                table: "Orders",
                column: "OrderCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOrder_OrderId",
                table: "PaymentOrders",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOrder_TransactionId_Unique",
                table: "PaymentOrders",
                column: "TransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentProviderLookup_Provider_Unique",
                table: "PaymentProviderLookups",
                column: "Provider",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransaction_TransactionCode_Unique",
                table: "PaymentTransactions",
                column: "TransactionCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_CurrencyId",
                table: "PaymentTransactions",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_StatusId",
                table: "PaymentTransactions",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactionStatusLookup_Status_Unique",
                table: "PaymentTransactionStatusLookups",
                column: "Status",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTypeOrder_Unique_Order_ServiceType",
                table: "ServiceTypeOrders",
                columns: new[] { "OrderId", "ServiceTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTypeOrders_ServiceTypeId",
                table: "ServiceTypeOrders",
                column: "ServiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTypePrice_Unique_ServiceType_Currency",
                table: "ServiceTypePrices",
                columns: new[] { "ServiceTypeId", "CurrencyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTypePrices_CurrencyId",
                table: "ServiceTypePrices",
                column: "CurrencyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderAppointments");

            migrationBuilder.DropTable(
                name: "PaymentOrders");

            migrationBuilder.DropTable(
                name: "PaymentProviderLookups");

            migrationBuilder.DropTable(
                name: "ServiceTypeOrders");

            migrationBuilder.DropTable(
                name: "ServiceTypePrices");

            migrationBuilder.DropTable(
                name: "PaymentTransactions");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "CurrencyLookups");

            migrationBuilder.DropTable(
                name: "PaymentTransactionStatusLookups");

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
        }
    }
}
