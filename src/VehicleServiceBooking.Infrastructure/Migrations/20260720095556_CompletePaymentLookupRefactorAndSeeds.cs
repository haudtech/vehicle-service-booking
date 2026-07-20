using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VehicleServiceBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CompletePaymentLookupRefactorAndSeeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000006"));

            migrationBuilder.DeleteData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000007"));

            migrationBuilder.DeleteData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000008"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAtUtc",
                table: "PaymentTransactions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FailedAtUtc",
                table: "PaymentTransactions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FailureCode",
                table: "PaymentTransactions",
                type: "character varying(60)",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FailureMessage",
                table: "PaymentTransactions",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentMethodId",
                table: "PaymentTransactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentProviderId",
                table: "PaymentTransactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ProviderEventId",
                table: "PaymentTransactions",
                type: "character varying(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProviderTransactionId",
                table: "PaymentTransactions",
                type: "character varying(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RawProviderPayload",
                table: "PaymentTransactions",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PaymentTransactionId",
                table: "PaymentOrders",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "CheckoutUrl",
                table: "PaymentOrders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAtUtc",
                table: "PaymentOrders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IntentCode",
                table: "PaymentOrders",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastProviderEventAtUtc",
                table: "PaymentOrders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastProviderEventId",
                table: "PaymentOrders",
                type: "character varying(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentMethodId",
                table: "PaymentOrders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentProviderId",
                table: "PaymentOrders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "StatusId",
                table: "PaymentOrders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "AmountPaid",
                table: "Orders",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAtUtc",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentStatusId",
                table: "Orders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "OrderPaymentStatusLookups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderPaymentStatusLookups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentIntentStatusLookups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentIntentStatusLookups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethodLookups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Method = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethodLookups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentWebhookProcessStatusLookups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentWebhookProcessStatusLookups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentWebhookInboxes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentProviderId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    SignatureHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Payload = table.Column<string>(type: "text", nullable: false),
                    ProcessStatusId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceivedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentWebhookInboxes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentWebhookInbox_PaymentProvider",
                        column: x => x.PaymentProviderId,
                        principalTable: "PaymentProviderLookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentWebhookInbox_ProcessStatusLookup",
                        column: x => x.ProcessStatusId,
                        principalTable: "PaymentWebhookProcessStatusLookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "OrderPaymentStatusLookups",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0003-000000000101"), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), "Order payment has not been completed yet", true, "Pending", 1, new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) },
                    { new Guid("00000000-0000-0000-0003-000000000102"), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), "Order payment has been completed successfully", true, "Paid", 2, new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) },
                    { new Guid("00000000-0000-0000-0003-000000000103"), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), "Order payment attempt failed", true, "Failed", 3, new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) },
                    { new Guid("00000000-0000-0000-0003-000000000104"), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), "Order payment expired before completion", true, "Expired", 4, new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) },
                    { new Guid("00000000-0000-0000-0003-000000000105"), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), "Order payment was cancelled", true, "Cancelled", 5, new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) },
                    { new Guid("00000000-0000-0000-0003-000000000106"), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), "Order payment was refunded", true, "Refunded", 6, new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) }
                });

            migrationBuilder.InsertData(
                table: "PaymentIntentStatusLookups",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0006-000000000101"), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), "Payment intent has been created", true, "Initiated", 1, new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) },
                    { new Guid("00000000-0000-0000-0006-000000000102"), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), "User has been redirected to payment provider", true, "Redirected", 2, new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) },
                    { new Guid("00000000-0000-0000-0006-000000000103"), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), "Payment intent completed successfully", true, "Paid", 3, new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) },
                    { new Guid("00000000-0000-0000-0006-000000000104"), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), "Payment intent failed", true, "Failed", 4, new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) },
                    { new Guid("00000000-0000-0000-0006-000000000105"), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), "Payment intent expired", true, "Expired", 5, new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) },
                    { new Guid("00000000-0000-0000-0006-000000000106"), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), "Payment intent was cancelled", true, "Cancelled", 6, new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) }
                });

            migrationBuilder.InsertData(
                table: "PaymentMethodLookups",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Method", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0008-000000000001"), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), "Domestic ATM card payments", true, 1, "ATM Card (Domestic)", new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) },
                    { new Guid("00000000-0000-0000-0008-000000000002"), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), "Credit/debit card payments", true, 2, "Credit Card", new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) },
                    { new Guid("00000000-0000-0000-0008-000000000003"), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), "Wallet app payments", true, 3, "E-Wallet", new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) },
                    { new Guid("00000000-0000-0000-0008-000000000004"), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), "Apple Pay tokenized payments", true, 4, "Apple Pay", new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) },
                    { new Guid("00000000-0000-0000-0008-000000000005"), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), "Internal balance wallet", true, 5, "Internal Wallet", new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) }
                });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000001"),
                columns: new[] { "Description", "Name" },
                values: new object[] { "ZaloPay external payment gateway", "ZaloPay" });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000002"),
                columns: new[] { "Description", "Name" },
                values: new object[] { "MoMo external payment gateway", "MoMo" });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000003"),
                columns: new[] { "Description", "Name" },
                values: new object[] { "VNPay external payment gateway", "VNPay" });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000004"),
                columns: new[] { "Description", "Name" },
                values: new object[] { "ShopeePay external payment gateway", "ShopeePay" });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000005"),
                columns: new[] { "Description", "Name" },
                values: new object[] { "OnePay external payment gateway", "OnePay" });

            migrationBuilder.InsertData(
                table: "PaymentTransactionStatusLookups",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0005-000000000004"), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), "Transaction failed during processing", true, "Failed", 4, new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) },
                    { new Guid("00000000-0000-0000-0005-000000000005"), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), "Transaction expired before completion", true, "Expired", 5, new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) },
                    { new Guid("00000000-0000-0000-0005-000000000006"), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), "Transaction was cancelled", true, "Cancelled", 6, new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) },
                    { new Guid("00000000-0000-0000-0005-000000000007"), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), "Transaction amount has been refunded", true, "Refunded", 7, new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) }
                });

            migrationBuilder.InsertData(
                table: "PaymentWebhookProcessStatusLookups",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0007-000000000101"), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), "Webhook event has been received", true, "Received", 1, new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) },
                    { new Guid("00000000-0000-0000-0007-000000000102"), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), "Webhook event has been processed successfully", true, "Processed", 2, new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) },
                    { new Guid("00000000-0000-0000-0007-000000000103"), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), "Webhook event was ignored as duplicate or irrelevant", true, "Ignored", 3, new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) },
                    { new Guid("00000000-0000-0000-0007-000000000104"), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), "Webhook event processing failed", true, "Failed", 4, new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) }
                });

            // Backfill valid seeded lookup references for existing rows before FK constraints are added.
            migrationBuilder.Sql(
                "UPDATE \"Orders\" SET \"PaymentStatusId\" = '00000000-0000-0000-0003-000000000101'::uuid WHERE \"PaymentStatusId\" = '00000000-0000-0000-0000-000000000000'::uuid;");

            migrationBuilder.Sql(
                "UPDATE \"PaymentOrders\" SET \"PaymentProviderId\" = '00000000-0000-0000-0003-000000000001'::uuid WHERE \"PaymentProviderId\" = '00000000-0000-0000-0000-000000000000'::uuid;");

            migrationBuilder.Sql(
                "UPDATE \"PaymentOrders\" SET \"PaymentMethodId\" = '00000000-0000-0000-0008-000000000001'::uuid WHERE \"PaymentMethodId\" = '00000000-0000-0000-0000-000000000000'::uuid;");

            migrationBuilder.Sql(
                "UPDATE \"PaymentOrders\" SET \"StatusId\" = '00000000-0000-0000-0006-000000000101'::uuid WHERE \"StatusId\" = '00000000-0000-0000-0000-000000000000'::uuid;");

            migrationBuilder.Sql(
                "UPDATE \"PaymentTransactions\" SET \"PaymentProviderId\" = '00000000-0000-0000-0003-000000000001'::uuid WHERE \"PaymentProviderId\" = '00000000-0000-0000-0000-000000000000'::uuid;");

            migrationBuilder.Sql(
                "UPDATE \"PaymentTransactions\" SET \"PaymentMethodId\" = '00000000-0000-0000-0008-000000000001'::uuid WHERE \"PaymentMethodId\" = '00000000-0000-0000-0000-000000000000'::uuid;");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransaction_PaymentMethodId",
                table: "PaymentTransactions",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransaction_PaymentProviderId",
                table: "PaymentTransactions",
                column: "PaymentProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransaction_Provider_Event_Unique",
                table: "PaymentTransactions",
                columns: new[] { "PaymentProviderId", "ProviderEventId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOrder_IntentCode_Unique",
                table: "PaymentOrders",
                column: "IntentCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOrder_PaymentMethodId",
                table: "PaymentOrders",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOrder_PaymentProviderId",
                table: "PaymentOrders",
                column: "PaymentProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOrder_StatusId",
                table: "PaymentOrders",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_PaymentStatusId",
                table: "Orders",
                column: "PaymentStatusId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Order_AmountPaid_Lte_TotalAmount",
                table: "Orders",
                sql: "\"AmountPaid\" <= \"TotalAmount\"");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Order_AmountPaid_NonNegative",
                table: "Orders",
                sql: "\"AmountPaid\" >= 0");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPaymentStatusLookup_Status_Unique",
                table: "OrderPaymentStatusLookups",
                column: "Status",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentIntentStatusLookup_Status_Unique",
                table: "PaymentIntentStatusLookups",
                column: "Status",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethodLookup_Method_Unique",
                table: "PaymentMethodLookups",
                column: "Method",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentWebhookInbox_ProcessStatusId",
                table: "PaymentWebhookInboxes",
                column: "ProcessStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentWebhookInbox_Provider_Event_Unique",
                table: "PaymentWebhookInboxes",
                columns: new[] { "PaymentProviderId", "EventId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentWebhookProcessStatusLookup_Status_Unique",
                table: "PaymentWebhookProcessStatusLookups",
                column: "Status",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_PaymentStatusLookup",
                table: "Orders",
                column: "PaymentStatusId",
                principalTable: "OrderPaymentStatusLookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentOrder_PaymentMethod",
                table: "PaymentOrders",
                column: "PaymentMethodId",
                principalTable: "PaymentMethodLookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentOrder_PaymentProvider",
                table: "PaymentOrders",
                column: "PaymentProviderId",
                principalTable: "PaymentProviderLookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentOrder_StatusLookup",
                table: "PaymentOrders",
                column: "StatusId",
                principalTable: "PaymentIntentStatusLookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTransaction_PaymentMethod",
                table: "PaymentTransactions",
                column: "PaymentMethodId",
                principalTable: "PaymentMethodLookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTransaction_PaymentProvider",
                table: "PaymentTransactions",
                column: "PaymentProviderId",
                principalTable: "PaymentProviderLookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_PaymentStatusLookup",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentOrder_PaymentMethod",
                table: "PaymentOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentOrder_PaymentProvider",
                table: "PaymentOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentOrder_StatusLookup",
                table: "PaymentOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTransaction_PaymentMethod",
                table: "PaymentTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTransaction_PaymentProvider",
                table: "PaymentTransactions");

            migrationBuilder.DropTable(
                name: "OrderPaymentStatusLookups");

            migrationBuilder.DropTable(
                name: "PaymentIntentStatusLookups");

            migrationBuilder.DropTable(
                name: "PaymentMethodLookups");

            migrationBuilder.DropTable(
                name: "PaymentWebhookInboxes");

            migrationBuilder.DropTable(
                name: "PaymentWebhookProcessStatusLookups");

            migrationBuilder.DropIndex(
                name: "IX_PaymentTransaction_PaymentMethodId",
                table: "PaymentTransactions");

            migrationBuilder.DropIndex(
                name: "IX_PaymentTransaction_PaymentProviderId",
                table: "PaymentTransactions");

            migrationBuilder.DropIndex(
                name: "IX_PaymentTransaction_Provider_Event_Unique",
                table: "PaymentTransactions");

            migrationBuilder.DropIndex(
                name: "IX_PaymentOrder_IntentCode_Unique",
                table: "PaymentOrders");

            migrationBuilder.DropIndex(
                name: "IX_PaymentOrder_PaymentMethodId",
                table: "PaymentOrders");

            migrationBuilder.DropIndex(
                name: "IX_PaymentOrder_PaymentProviderId",
                table: "PaymentOrders");

            migrationBuilder.DropIndex(
                name: "IX_PaymentOrder_StatusId",
                table: "PaymentOrders");

            migrationBuilder.DropIndex(
                name: "IX_Order_PaymentStatusId",
                table: "Orders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Order_AmountPaid_Lte_TotalAmount",
                table: "Orders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Order_AmountPaid_NonNegative",
                table: "Orders");

            migrationBuilder.DeleteData(
                table: "PaymentTransactionStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0005-000000000004"));

            migrationBuilder.DeleteData(
                table: "PaymentTransactionStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0005-000000000005"));

            migrationBuilder.DeleteData(
                table: "PaymentTransactionStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0005-000000000006"));

            migrationBuilder.DeleteData(
                table: "PaymentTransactionStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0005-000000000007"));

            migrationBuilder.DropColumn(
                name: "CompletedAtUtc",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "FailedAtUtc",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "FailureCode",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "FailureMessage",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "PaymentMethodId",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "PaymentProviderId",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "ProviderEventId",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "ProviderTransactionId",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "RawProviderPayload",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "CheckoutUrl",
                table: "PaymentOrders");

            migrationBuilder.DropColumn(
                name: "ExpiresAtUtc",
                table: "PaymentOrders");

            migrationBuilder.DropColumn(
                name: "IntentCode",
                table: "PaymentOrders");

            migrationBuilder.DropColumn(
                name: "LastProviderEventAtUtc",
                table: "PaymentOrders");

            migrationBuilder.DropColumn(
                name: "LastProviderEventId",
                table: "PaymentOrders");

            migrationBuilder.DropColumn(
                name: "PaymentMethodId",
                table: "PaymentOrders");

            migrationBuilder.DropColumn(
                name: "PaymentProviderId",
                table: "PaymentOrders");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "PaymentOrders");

            migrationBuilder.DropColumn(
                name: "AmountPaid",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaidAtUtc",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentStatusId",
                table: "Orders");

            migrationBuilder.AlterColumn<Guid>(
                name: "PaymentTransactionId",
                table: "PaymentOrders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000001"),
                columns: new[] { "Description", "Name" },
                values: new object[] { "Zalo Pay e-wallet and gateway", "Zalo Pay" });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000002"),
                columns: new[] { "Description", "Name" },
                values: new object[] { "Momo wallet payments", "Momo" });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000003"),
                columns: new[] { "Description", "Name" },
                values: new object[] { "Apple Pay card tokenization gateway", "Apple Pay" });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000004"),
                columns: new[] { "Description", "Name" },
                values: new object[] { "Visa card network", "Visa" });

            migrationBuilder.UpdateData(
                table: "PaymentProviderLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0003-000000000005"),
                columns: new[] { "Description", "Name" },
                values: new object[] { "MasterCard card network", "Master Card" });

            migrationBuilder.InsertData(
                table: "PaymentProviderLookups",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "Provider", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0003-000000000006"), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), "VNPay online payment gateway", true, "VNPay", 6, new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) },
                    { new Guid("00000000-0000-0000-0003-000000000007"), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), "Domestic ATM transfer", true, "ATM Transfer", 7, new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) },
                    { new Guid("00000000-0000-0000-0003-000000000008"), new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000), "Internal system wallet balance", true, "Internal Wallet", 8, new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000) }
                });
        }
    }
}
