using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleServiceBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIdempotencyRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IdempotencyRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdempotencyKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RequestPath = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RequestHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ResponseStatusCode = table.Column<int>(type: "integer", nullable: true),
                    ResponseBody = table.Column<string>(type: "text", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdempotencyRequests", x => x.Id);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_IdempotencyRequest_ExpiresAt",
                table: "IdempotencyRequests",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_IdempotencyRequest_Key_Path_Unique",
                table: "IdempotencyRequests",
                columns: new[] { "IdempotencyKey", "RequestPath" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdempotencyRequests");

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 11, 40, 15, 756, DateTimeKind.Utc).AddTicks(9050), new DateTime(2026, 6, 28, 11, 40, 15, 756, DateTimeKind.Utc).AddTicks(9050) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 11, 40, 15, 756, DateTimeKind.Utc).AddTicks(9060), new DateTime(2026, 6, 28, 11, 40, 15, 756, DateTimeKind.Utc).AddTicks(9060) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 11, 40, 15, 756, DateTimeKind.Utc).AddTicks(9060), new DateTime(2026, 6, 28, 11, 40, 15, 756, DateTimeKind.Utc).AddTicks(9060) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 11, 40, 15, 756, DateTimeKind.Utc).AddTicks(9070), new DateTime(2026, 6, 28, 11, 40, 15, 756, DateTimeKind.Utc).AddTicks(9070) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 11, 40, 15, 756, DateTimeKind.Utc).AddTicks(9070), new DateTime(2026, 6, 28, 11, 40, 15, 756, DateTimeKind.Utc).AddTicks(9070) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 11, 40, 15, 757, DateTimeKind.Utc).AddTicks(4590), new DateTime(2026, 6, 28, 11, 40, 15, 757, DateTimeKind.Utc).AddTicks(4590) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 11, 40, 15, 757, DateTimeKind.Utc).AddTicks(4600), new DateTime(2026, 6, 28, 11, 40, 15, 757, DateTimeKind.Utc).AddTicks(4600) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 11, 40, 15, 757, DateTimeKind.Utc).AddTicks(4610), new DateTime(2026, 6, 28, 11, 40, 15, 757, DateTimeKind.Utc).AddTicks(4610) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 11, 40, 15, 757, DateTimeKind.Utc).AddTicks(4610), new DateTime(2026, 6, 28, 11, 40, 15, 757, DateTimeKind.Utc).AddTicks(4610) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 11, 40, 15, 757, DateTimeKind.Utc).AddTicks(4620), new DateTime(2026, 6, 28, 11, 40, 15, 757, DateTimeKind.Utc).AddTicks(4620) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 11, 40, 15, 763, DateTimeKind.Utc).AddTicks(2220), new DateTime(2026, 6, 28, 11, 40, 15, 763, DateTimeKind.Utc).AddTicks(2220) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 11, 40, 15, 763, DateTimeKind.Utc).AddTicks(2230), new DateTime(2026, 6, 28, 11, 40, 15, 763, DateTimeKind.Utc).AddTicks(2230) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 11, 40, 15, 763, DateTimeKind.Utc).AddTicks(2240), new DateTime(2026, 6, 28, 11, 40, 15, 763, DateTimeKind.Utc).AddTicks(2240) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 11, 40, 15, 763, DateTimeKind.Utc).AddTicks(2250), new DateTime(2026, 6, 28, 11, 40, 15, 763, DateTimeKind.Utc).AddTicks(2250) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 11, 40, 15, 763, DateTimeKind.Utc).AddTicks(2260), new DateTime(2026, 6, 28, 11, 40, 15, 763, DateTimeKind.Utc).AddTicks(2260) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 11, 40, 15, 763, DateTimeKind.Utc).AddTicks(2270), new DateTime(2026, 6, 28, 11, 40, 15, 763, DateTimeKind.Utc).AddTicks(2270) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 11, 40, 15, 763, DateTimeKind.Utc).AddTicks(2280), new DateTime(2026, 6, 28, 11, 40, 15, 763, DateTimeKind.Utc).AddTicks(2280) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 11, 40, 15, 763, DateTimeKind.Utc).AddTicks(2310), new DateTime(2026, 6, 28, 11, 40, 15, 763, DateTimeKind.Utc).AddTicks(2310) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000009"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 11, 40, 15, 763, DateTimeKind.Utc).AddTicks(2320), new DateTime(2026, 6, 28, 11, 40, 15, 763, DateTimeKind.Utc).AddTicks(2320) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000010"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 11, 40, 15, 763, DateTimeKind.Utc).AddTicks(2330), new DateTime(2026, 6, 28, 11, 40, 15, 763, DateTimeKind.Utc).AddTicks(2330) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000011"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 11, 40, 15, 763, DateTimeKind.Utc).AddTicks(2340), new DateTime(2026, 6, 28, 11, 40, 15, 763, DateTimeKind.Utc).AddTicks(2340) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000012"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 11, 40, 15, 763, DateTimeKind.Utc).AddTicks(2350), new DateTime(2026, 6, 28, 11, 40, 15, 763, DateTimeKind.Utc).AddTicks(2350) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000013"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 11, 40, 15, 763, DateTimeKind.Utc).AddTicks(2360), new DateTime(2026, 6, 28, 11, 40, 15, 763, DateTimeKind.Utc).AddTicks(2360) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000014"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 11, 40, 15, 763, DateTimeKind.Utc).AddTicks(2370), new DateTime(2026, 6, 28, 11, 40, 15, 763, DateTimeKind.Utc).AddTicks(2370) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000015"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 11, 40, 15, 763, DateTimeKind.Utc).AddTicks(2380), new DateTime(2026, 6, 28, 11, 40, 15, 763, DateTimeKind.Utc).AddTicks(2380) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000016"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 11, 40, 15, 763, DateTimeKind.Utc).AddTicks(2390), new DateTime(2026, 6, 28, 11, 40, 15, 763, DateTimeKind.Utc).AddTicks(2390) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000017"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 11, 40, 15, 763, DateTimeKind.Utc).AddTicks(2400), new DateTime(2026, 6, 28, 11, 40, 15, 763, DateTimeKind.Utc).AddTicks(2400) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000018"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 11, 40, 15, 763, DateTimeKind.Utc).AddTicks(2400), new DateTime(2026, 6, 28, 11, 40, 15, 763, DateTimeKind.Utc).AddTicks(2410) });
        }
    }
}
