using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleServiceBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceBookingConflictInvariant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Services_DealershipId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_ServiceBayId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_TechnicianId",
                table: "Services");

            migrationBuilder.AddColumn<DateOnly>(
                name: "BookingDate",
                table: "Services",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EstimatedEndSlotSequenceExclusive",
                table: "Services",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EstimatedStartSlotSequence",
                table: "Services",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS btree_gist;");

            migrationBuilder.Sql(@"
                UPDATE ""Services"" AS s
                SET
                    ""BookingDate"" = (
                        SELECT a.""AppointmentDate""
                        FROM ""Appointments"" AS a
                        WHERE a.""Id"" = s.""AppointmentId""
                    ),
                    ""EstimatedStartSlotSequence"" = (
                        SELECT ts.""SequenceOrder""
                        FROM ""TimeSlots"" AS ts
                        WHERE ts.""Id"" = s.""EstimatedStartTimeSlotId""
                    ),
                    ""EstimatedEndSlotSequenceExclusive"" = (
                        SELECT ts.""SequenceOrder"" + 1
                        FROM ""TimeSlots"" AS ts
                        WHERE ts.""Id"" = s.""EstimatedEndTimeSlotId""
                    );
            ");

            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM ""Services""
                        WHERE ""BookingDate"" IS NULL
                           OR ""EstimatedStartSlotSequence"" IS NULL
                           OR ""EstimatedEndSlotSequenceExclusive"" IS NULL
                           OR ""EstimatedEndSlotSequenceExclusive"" <= ""EstimatedStartSlotSequence""
                    ) THEN
                        RAISE EXCEPTION 'Cannot enforce booking invariant columns because backfill produced invalid values.';
                    END IF;
                END
                $$;
            ");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "BookingDate",
                table: "Services",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EstimatedStartSlotSequence",
                table: "Services",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EstimatedEndSlotSequenceExclusive",
                table: "Services",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Service_Dealership_BookingDate",
                table: "Services",
                columns: new[] { "DealershipId", "BookingDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Service_ServiceBay_BookingDate",
                table: "Services",
                columns: new[] { "ServiceBayId", "BookingDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Service_Technician_BookingDate",
                table: "Services",
                columns: new[] { "TechnicianId", "BookingDate" });

            migrationBuilder.Sql(@"
                ALTER TABLE ""Services""
                ADD CONSTRAINT ""EXC_Service_Tech_Date_SeqRange_NoOverlap""
                EXCLUDE USING gist (
                    ""BookingDate"" WITH =,
                    ""TechnicianId"" WITH =,
                    int4range(""EstimatedStartSlotSequence"", ""EstimatedEndSlotSequenceExclusive"", '[)') WITH &&
                )
                WHERE (""TechnicianId"" IS NOT NULL AND ""IsActive"");
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE ""Services""
                ADD CONSTRAINT ""EXC_Service_Bay_Date_SeqRange_NoOverlap""
                EXCLUDE USING gist (
                    ""BookingDate"" WITH =,
                    ""ServiceBayId"" WITH =,
                    int4range(""EstimatedStartSlotSequence"", ""EstimatedEndSlotSequenceExclusive"", '[)') WITH &&
                )
                WHERE (""ServiceBayId"" IS NOT NULL AND ""IsActive"");
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE \"Services\" DROP CONSTRAINT IF EXISTS \"EXC_Service_Tech_Date_SeqRange_NoOverlap\";");
            migrationBuilder.Sql("ALTER TABLE \"Services\" DROP CONSTRAINT IF EXISTS \"EXC_Service_Bay_Date_SeqRange_NoOverlap\";");

            migrationBuilder.DropIndex(
                name: "IX_Service_Dealership_BookingDate",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Service_ServiceBay_BookingDate",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Service_Technician_BookingDate",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "BookingDate",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "EstimatedEndSlotSequenceExclusive",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "EstimatedStartSlotSequence",
                table: "Services");

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 51, 2, 861, DateTimeKind.Utc).AddTicks(6800), new DateTime(2026, 6, 28, 7, 51, 2, 861, DateTimeKind.Utc).AddTicks(6800) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 51, 2, 861, DateTimeKind.Utc).AddTicks(6810), new DateTime(2026, 6, 28, 7, 51, 2, 861, DateTimeKind.Utc).AddTicks(6810) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 51, 2, 861, DateTimeKind.Utc).AddTicks(6820), new DateTime(2026, 6, 28, 7, 51, 2, 861, DateTimeKind.Utc).AddTicks(6820) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 51, 2, 861, DateTimeKind.Utc).AddTicks(6820), new DateTime(2026, 6, 28, 7, 51, 2, 861, DateTimeKind.Utc).AddTicks(6820) });

            migrationBuilder.UpdateData(
                table: "AppointmentStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 51, 2, 861, DateTimeKind.Utc).AddTicks(6820), new DateTime(2026, 6, 28, 7, 51, 2, 861, DateTimeKind.Utc).AddTicks(6820) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 51, 2, 862, DateTimeKind.Utc).AddTicks(1140), new DateTime(2026, 6, 28, 7, 51, 2, 862, DateTimeKind.Utc).AddTicks(1140) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 51, 2, 862, DateTimeKind.Utc).AddTicks(1140), new DateTime(2026, 6, 28, 7, 51, 2, 862, DateTimeKind.Utc).AddTicks(1140) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 51, 2, 862, DateTimeKind.Utc).AddTicks(1150), new DateTime(2026, 6, 28, 7, 51, 2, 862, DateTimeKind.Utc).AddTicks(1150) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 51, 2, 862, DateTimeKind.Utc).AddTicks(1150), new DateTime(2026, 6, 28, 7, 51, 2, 862, DateTimeKind.Utc).AddTicks(1150) });

            migrationBuilder.UpdateData(
                table: "ServiceStatusLookups",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 51, 2, 862, DateTimeKind.Utc).AddTicks(1150), new DateTime(2026, 6, 28, 7, 51, 2, 862, DateTimeKind.Utc).AddTicks(1150) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 51, 2, 865, DateTimeKind.Utc).AddTicks(5850), new DateTime(2026, 6, 28, 7, 51, 2, 865, DateTimeKind.Utc).AddTicks(5850) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 51, 2, 865, DateTimeKind.Utc).AddTicks(5860), new DateTime(2026, 6, 28, 7, 51, 2, 865, DateTimeKind.Utc).AddTicks(5860) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 51, 2, 865, DateTimeKind.Utc).AddTicks(5870), new DateTime(2026, 6, 28, 7, 51, 2, 865, DateTimeKind.Utc).AddTicks(5870) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 51, 2, 865, DateTimeKind.Utc).AddTicks(5880), new DateTime(2026, 6, 28, 7, 51, 2, 865, DateTimeKind.Utc).AddTicks(5880) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 51, 2, 865, DateTimeKind.Utc).AddTicks(5880), new DateTime(2026, 6, 28, 7, 51, 2, 865, DateTimeKind.Utc).AddTicks(5880) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 51, 2, 865, DateTimeKind.Utc).AddTicks(5890), new DateTime(2026, 6, 28, 7, 51, 2, 865, DateTimeKind.Utc).AddTicks(5890) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 51, 2, 865, DateTimeKind.Utc).AddTicks(5900), new DateTime(2026, 6, 28, 7, 51, 2, 865, DateTimeKind.Utc).AddTicks(5900) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 51, 2, 865, DateTimeKind.Utc).AddTicks(5900), new DateTime(2026, 6, 28, 7, 51, 2, 865, DateTimeKind.Utc).AddTicks(5900) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000009"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 51, 2, 865, DateTimeKind.Utc).AddTicks(5910), new DateTime(2026, 6, 28, 7, 51, 2, 865, DateTimeKind.Utc).AddTicks(5910) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000010"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 51, 2, 865, DateTimeKind.Utc).AddTicks(5920), new DateTime(2026, 6, 28, 7, 51, 2, 865, DateTimeKind.Utc).AddTicks(5920) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000011"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 51, 2, 865, DateTimeKind.Utc).AddTicks(5920), new DateTime(2026, 6, 28, 7, 51, 2, 865, DateTimeKind.Utc).AddTicks(5920) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000012"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 51, 2, 865, DateTimeKind.Utc).AddTicks(5930), new DateTime(2026, 6, 28, 7, 51, 2, 865, DateTimeKind.Utc).AddTicks(5930) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000013"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 51, 2, 865, DateTimeKind.Utc).AddTicks(5990), new DateTime(2026, 6, 28, 7, 51, 2, 865, DateTimeKind.Utc).AddTicks(5990) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000014"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 51, 2, 865, DateTimeKind.Utc).AddTicks(6000), new DateTime(2026, 6, 28, 7, 51, 2, 865, DateTimeKind.Utc).AddTicks(6000) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000015"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 51, 2, 865, DateTimeKind.Utc).AddTicks(6000), new DateTime(2026, 6, 28, 7, 51, 2, 865, DateTimeKind.Utc).AddTicks(6000) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000016"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 51, 2, 865, DateTimeKind.Utc).AddTicks(6010), new DateTime(2026, 6, 28, 7, 51, 2, 865, DateTimeKind.Utc).AddTicks(6010) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000017"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 51, 2, 865, DateTimeKind.Utc).AddTicks(6010), new DateTime(2026, 6, 28, 7, 51, 2, 865, DateTimeKind.Utc).AddTicks(6010) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000018"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 28, 7, 51, 2, 865, DateTimeKind.Utc).AddTicks(6020), new DateTime(2026, 6, 28, 7, 51, 2, 865, DateTimeKind.Utc).AddTicks(6020) });

            migrationBuilder.CreateIndex(
                name: "IX_Services_DealershipId",
                table: "Services",
                column: "DealershipId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_ServiceBayId",
                table: "Services",
                column: "ServiceBayId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_TechnicianId",
                table: "Services",
                column: "TechnicianId");
        }
    }
}
