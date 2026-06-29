using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VehicleServiceBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConvertIdempotencyStatusToLookup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IdempotencyRequestStatusLookups",
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
                    table.PrimaryKey("PK_IdempotencyRequestStatusLookups", x => x.Id);
                });

            migrationBuilder.AddColumn<Guid>(
                name: "StatusId",
                table: "IdempotencyRequests",
                type: "uuid",
                nullable: true);

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

            migrationBuilder.InsertData(
                table: "IdempotencyRequestStatusLookups",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0002-000000000001"), new DateTime(2026, 6, 28, 15, 45, 5, 309, DateTimeKind.Utc).AddTicks(4910), "Request processing started and not yet completed", true, "In Progress", 1, new DateTime(2026, 6, 28, 15, 45, 5, 309, DateTimeKind.Utc).AddTicks(4910) },
                    { new Guid("00000000-0000-0000-0002-000000000002"), new DateTime(2026, 6, 28, 15, 45, 5, 309, DateTimeKind.Utc).AddTicks(4920), "Request completed and response persisted for replay", true, "Completed", 2, new DateTime(2026, 6, 28, 15, 45, 5, 309, DateTimeKind.Utc).AddTicks(4920) }
                });

            migrationBuilder.Sql(@"
                UPDATE ""IdempotencyRequests""
                SET ""StatusId"" = CASE ""Status""
                    WHEN 2 THEN '00000000-0000-0000-0002-000000000002'::uuid
                    ELSE '00000000-0000-0000-0002-000000000001'::uuid
                END;
            ");

            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS (SELECT 1 FROM ""IdempotencyRequests"" WHERE ""StatusId"" IS NULL) THEN
                        RAISE EXCEPTION 'IdempotencyRequests StatusId backfill failed';
                    END IF;
                END
                $$;
            ");

            migrationBuilder.AlterColumn<Guid>(
                name: "StatusId",
                table: "IdempotencyRequests",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_IdempotencyRequests_StatusId",
                table: "IdempotencyRequests",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_IdempotencyRequestStatusLookup_Status_Unique",
                table: "IdempotencyRequestStatusLookups",
                column: "Status",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_IdempotencyRequest_StatusLookup",
                table: "IdempotencyRequests",
                column: "StatusId",
                principalTable: "IdempotencyRequestStatusLookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.DropColumn(
                name: "Status",
                table: "IdempotencyRequests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "IdempotencyRequests",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE ""IdempotencyRequests""
                SET ""Status"" = CASE ""StatusId""
                    WHEN '00000000-0000-0000-0002-000000000002'::uuid THEN 2
                    ELSE 1
                END;
            ");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "IdempotencyRequests",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.DropForeignKey(
                name: "FK_IdempotencyRequest_StatusLookup",
                table: "IdempotencyRequests");

            migrationBuilder.DropTable(
                name: "IdempotencyRequestStatusLookups");

            migrationBuilder.DropIndex(
                name: "IX_IdempotencyRequests_StatusId",
                table: "IdempotencyRequests");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "IdempotencyRequests");

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 13, 11, 4, 465, DateTimeKind.Utc).AddTicks(1960), new DateTime(2026, 6, 28, 13, 11, 4, 465, DateTimeKind.Utc).AddTicks(1960) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 13, 11, 4, 465, DateTimeKind.Utc).AddTicks(1970), new DateTime(2026, 6, 28, 13, 11, 4, 465, DateTimeKind.Utc).AddTicks(1970) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 13, 11, 4, 465, DateTimeKind.Utc).AddTicks(1970), new DateTime(2026, 6, 28, 13, 11, 4, 465, DateTimeKind.Utc).AddTicks(1970) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 13, 11, 4, 465, DateTimeKind.Utc).AddTicks(1980), new DateTime(2026, 6, 28, 13, 11, 4, 465, DateTimeKind.Utc).AddTicks(1980) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 13, 11, 4, 465, DateTimeKind.Utc).AddTicks(1980), new DateTime(2026, 6, 28, 13, 11, 4, 465, DateTimeKind.Utc).AddTicks(1980) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 13, 11, 4, 465, DateTimeKind.Utc).AddTicks(6530), new DateTime(2026, 6, 28, 13, 11, 4, 465, DateTimeKind.Utc).AddTicks(6530) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 13, 11, 4, 465, DateTimeKind.Utc).AddTicks(6540), new DateTime(2026, 6, 28, 13, 11, 4, 465, DateTimeKind.Utc).AddTicks(6540) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 13, 11, 4, 465, DateTimeKind.Utc).AddTicks(6540), new DateTime(2026, 6, 28, 13, 11, 4, 465, DateTimeKind.Utc).AddTicks(6540) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 13, 11, 4, 465, DateTimeKind.Utc).AddTicks(6540), new DateTime(2026, 6, 28, 13, 11, 4, 465, DateTimeKind.Utc).AddTicks(6540) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 13, 11, 4, 465, DateTimeKind.Utc).AddTicks(6550), new DateTime(2026, 6, 28, 13, 11, 4, 465, DateTimeKind.Utc).AddTicks(6550) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 13, 11, 4, 469, DateTimeKind.Utc).AddTicks(1680), new DateTime(2026, 6, 28, 13, 11, 4, 469, DateTimeKind.Utc).AddTicks(1680) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 13, 11, 4, 469, DateTimeKind.Utc).AddTicks(1690), new DateTime(2026, 6, 28, 13, 11, 4, 469, DateTimeKind.Utc).AddTicks(1690) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 13, 11, 4, 469, DateTimeKind.Utc).AddTicks(1690), new DateTime(2026, 6, 28, 13, 11, 4, 469, DateTimeKind.Utc).AddTicks(1690) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 13, 11, 4, 469, DateTimeKind.Utc).AddTicks(1700), new DateTime(2026, 6, 28, 13, 11, 4, 469, DateTimeKind.Utc).AddTicks(1700) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 13, 11, 4, 469, DateTimeKind.Utc).AddTicks(1710), new DateTime(2026, 6, 28, 13, 11, 4, 469, DateTimeKind.Utc).AddTicks(1710) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 13, 11, 4, 469, DateTimeKind.Utc).AddTicks(1710), new DateTime(2026, 6, 28, 13, 11, 4, 469, DateTimeKind.Utc).AddTicks(1710) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 13, 11, 4, 469, DateTimeKind.Utc).AddTicks(1720), new DateTime(2026, 6, 28, 13, 11, 4, 469, DateTimeKind.Utc).AddTicks(1720) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 13, 11, 4, 469, DateTimeKind.Utc).AddTicks(1730), new DateTime(2026, 6, 28, 13, 11, 4, 469, DateTimeKind.Utc).AddTicks(1730) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000009"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 13, 11, 4, 469, DateTimeKind.Utc).AddTicks(1730), new DateTime(2026, 6, 28, 13, 11, 4, 469, DateTimeKind.Utc).AddTicks(1730) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000010"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 13, 11, 4, 469, DateTimeKind.Utc).AddTicks(1740), new DateTime(2026, 6, 28, 13, 11, 4, 469, DateTimeKind.Utc).AddTicks(1740) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000011"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 13, 11, 4, 469, DateTimeKind.Utc).AddTicks(1750), new DateTime(2026, 6, 28, 13, 11, 4, 469, DateTimeKind.Utc).AddTicks(1750) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000012"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 13, 11, 4, 469, DateTimeKind.Utc).AddTicks(1750), new DateTime(2026, 6, 28, 13, 11, 4, 469, DateTimeKind.Utc).AddTicks(1750) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000013"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 13, 11, 4, 469, DateTimeKind.Utc).AddTicks(1760), new DateTime(2026, 6, 28, 13, 11, 4, 469, DateTimeKind.Utc).AddTicks(1760) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000014"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 13, 11, 4, 469, DateTimeKind.Utc).AddTicks(1760), new DateTime(2026, 6, 28, 13, 11, 4, 469, DateTimeKind.Utc).AddTicks(1760) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000015"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 13, 11, 4, 469, DateTimeKind.Utc).AddTicks(1770), new DateTime(2026, 6, 28, 13, 11, 4, 469, DateTimeKind.Utc).AddTicks(1770) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000016"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 13, 11, 4, 469, DateTimeKind.Utc).AddTicks(1780), new DateTime(2026, 6, 28, 13, 11, 4, 469, DateTimeKind.Utc).AddTicks(1780) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000017"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 13, 11, 4, 469, DateTimeKind.Utc).AddTicks(1780), new DateTime(2026, 6, 28, 13, 11, 4, 469, DateTimeKind.Utc).AddTicks(1780) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000018"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 13, 11, 4, 469, DateTimeKind.Utc).AddTicks(1790), new DateTime(2026, 6, 28, 13, 11, 4, 469, DateTimeKind.Utc).AddTicks(1790) });
        }
    }
}
