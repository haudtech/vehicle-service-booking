using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleServiceBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveToViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DROP VIEW IF EXISTS ""ServiceTypeAvailability"";
DROP VIEW IF EXISTS ""ServiceBayAvailableSlots"";
DROP VIEW IF EXISTS ""TechnicianAvailableSlots"";

CREATE VIEW ""TechnicianAvailableSlots"" AS
WITH query_dates AS (
    SELECT gs::date AS ""QueryDate""
    FROM generate_series(CURRENT_DATE - INTERVAL '1 day', CURRENT_DATE + INTERVAL '30 day', INTERVAL '1 day') gs
),
occupied_slots AS (
    SELECT
        s.""TechnicianId"",
        s.""DealershipId"",
        a.""AppointmentDate"" AS ""QueryDate"",
        seq AS ""SequenceOrder""
    FROM ""Services"" s
    INNER JOIN ""Appointments"" a ON a.""Id"" = s.""AppointmentId""
    LEFT JOIN ""AppointmentStatusLookups"" asl ON asl.""Id"" = a.""StatusId""
    INNER JOIN ""TimeSlots"" ts_start ON ts_start.""Id"" = s.""EstimatedStartTimeSlotId""
    INNER JOIN ""TimeSlots"" ts_end ON ts_end.""Id"" = s.""EstimatedEndTimeSlotId""
    CROSS JOIN LATERAL generate_series(ts_start.""SequenceOrder"", ts_end.""SequenceOrder"") seq
    WHERE s.""TechnicianId"" IS NOT NULL
      AND s.""IsActive"" = TRUE
      AND a.""IsActive"" = TRUE
      AND COALESCE(asl.""Name"", '') <> 'Cancelled'
)
SELECT
    ts.""Id"" AS ""TimeSlotId"",
    t.""Id"" AS ""TechnicianId"",
    ts.""SequenceOrder"",
    ts.""SlotStartTime"",
    ts.""SlotEndTime"",
    t.""FirstName"",
    t.""LastName"",
    t.""DealershipId"",
    qd.""QueryDate"",
    (occ.""SequenceOrder"" IS NULL) AS ""IsAvailable"",
    TRUE AS ""IsActive""
FROM ""Technicians"" t
INNER JOIN query_dates qd ON TRUE
INNER JOIN ""TechnicianSchedules"" sch
    ON sch.""TechnicianId"" = t.""Id""
   AND sch.""DayOfWeek"" = EXTRACT(DOW FROM qd.""QueryDate"")::integer
INNER JOIN ""TimeSlots"" ts
    ON ts.""IsActive"" = TRUE
   AND ts.""SlotStartTime"" >= sch.""StartTime""
   AND ts.""SlotEndTime"" <= sch.""EndTime""
LEFT JOIN occupied_slots occ
    ON occ.""TechnicianId"" = t.""Id""
   AND occ.""DealershipId"" = t.""DealershipId""
   AND occ.""QueryDate"" = qd.""QueryDate""
   AND occ.""SequenceOrder"" = ts.""SequenceOrder""
WHERE t.""IsActive"" = TRUE
  AND sch.""IsActive"" = TRUE;

CREATE VIEW ""ServiceBayAvailableSlots"" AS
WITH query_dates AS (
    SELECT gs::date AS ""QueryDate""
    FROM generate_series(CURRENT_DATE - INTERVAL '1 day', CURRENT_DATE + INTERVAL '30 day', INTERVAL '1 day') gs
),
occupied_slots AS (
    SELECT
        s.""ServiceBayId"",
        s.""DealershipId"",
        a.""AppointmentDate"" AS ""QueryDate"",
        seq AS ""SequenceOrder""
    FROM ""Services"" s
    INNER JOIN ""Appointments"" a ON a.""Id"" = s.""AppointmentId""
    LEFT JOIN ""AppointmentStatusLookups"" asl ON asl.""Id"" = a.""StatusId""
    INNER JOIN ""TimeSlots"" ts_start ON ts_start.""Id"" = s.""EstimatedStartTimeSlotId""
    INNER JOIN ""TimeSlots"" ts_end ON ts_end.""Id"" = s.""EstimatedEndTimeSlotId""
    CROSS JOIN LATERAL generate_series(ts_start.""SequenceOrder"", ts_end.""SequenceOrder"") seq
    WHERE s.""ServiceBayId"" IS NOT NULL
      AND s.""IsActive"" = TRUE
      AND a.""IsActive"" = TRUE
      AND COALESCE(asl.""Name"", '') <> 'Cancelled'
)
SELECT
    ts.""Id"" AS ""TimeSlotId"",
    sb.""Id"" AS ""ServiceBayId"",
    ts.""SequenceOrder"",
    ts.""SlotStartTime"",
    ts.""SlotEndTime"",
    sb.""Name"" AS ""ServiceBayName"",
    sb.""DealershipId"",
    qd.""QueryDate"",
    (occ.""SequenceOrder"" IS NULL) AS ""IsAvailable"",
    TRUE AS ""IsActive""
FROM ""ServiceBays"" sb
INNER JOIN query_dates qd ON TRUE
INNER JOIN ""TimeSlots"" ts ON ts.""IsActive"" = TRUE
LEFT JOIN occupied_slots occ
    ON occ.""ServiceBayId"" = sb.""Id""
   AND occ.""DealershipId"" = sb.""DealershipId""
   AND occ.""QueryDate"" = qd.""QueryDate""
   AND occ.""SequenceOrder"" = ts.""SequenceOrder""
WHERE sb.""IsActive"" = TRUE;

CREATE VIEW ""ServiceTypeAvailability"" AS
WITH required AS (
    SELECT
        st.""Id"" AS ""ServiceTypeId"",
        st.""Name"" AS ""ServiceTypeName"",
        st.""DurationMinutes"",
        CEIL(st.""DurationMinutes"" / 30.0)::integer AS ""RequiredSlots""
    FROM ""ServiceTypes"" st
    WHERE st.""IsActive"" = TRUE
)
SELECT
    r.""ServiceTypeId"",
    tas.""TimeSlotId"",
    tas.""TechnicianId"",
    sb_start.""ServiceBayId"",
    r.""ServiceTypeName"",
    r.""DurationMinutes"",
    r.""RequiredSlots"",
    tas.""SequenceOrder"",
    tas.""SlotStartTime"",
    tas.""SlotEndTime"",
    tas.""FirstName"",
    tas.""LastName"",
    sb_start.""ServiceBayName"",
    tas.""DealershipId"",
    tas.""QueryDate"",
    TRUE AS ""CanFitService"",
    TRUE AS ""IsActive""
FROM required r
INNER JOIN ""TechnicianSkills"" skill
    ON skill.""ServiceTypeId"" = r.""ServiceTypeId""
   AND skill.""IsActive"" = TRUE
INNER JOIN ""TechnicianAvailableSlots"" tas
    ON tas.""TechnicianId"" = skill.""TechnicianId""
   AND tas.""IsAvailable"" = TRUE
   AND tas.""IsActive"" = TRUE
INNER JOIN ""ServiceBayAvailableSlots"" sb_start
    ON sb_start.""DealershipId"" = tas.""DealershipId""
   AND sb_start.""QueryDate"" = tas.""QueryDate""
   AND sb_start.""SequenceOrder"" = tas.""SequenceOrder""
   AND sb_start.""IsAvailable"" = TRUE
   AND sb_start.""IsActive"" = TRUE
INNER JOIN LATERAL generate_series(
    tas.""SequenceOrder"",
    tas.""SequenceOrder"" + r.""RequiredSlots"" - 1
) seq ON TRUE
LEFT JOIN ""TechnicianAvailableSlots"" tas_window
    ON tas_window.""TechnicianId"" = tas.""TechnicianId""
   AND tas_window.""DealershipId"" = tas.""DealershipId""
   AND tas_window.""QueryDate"" = tas.""QueryDate""
   AND tas_window.""SequenceOrder"" = seq
LEFT JOIN ""ServiceBayAvailableSlots"" sb_window
    ON sb_window.""ServiceBayId"" = sb_start.""ServiceBayId""
   AND sb_window.""DealershipId"" = tas.""DealershipId""
   AND sb_window.""QueryDate"" = tas.""QueryDate""
   AND sb_window.""SequenceOrder"" = seq
GROUP BY
    r.""ServiceTypeId"",
    tas.""TimeSlotId"",
    tas.""TechnicianId"",
    sb_start.""ServiceBayId"",
    r.""ServiceTypeName"",
    r.""DurationMinutes"",
    r.""RequiredSlots"",
    tas.""SequenceOrder"",
    tas.""SlotStartTime"",
    tas.""SlotEndTime"",
    tas.""FirstName"",
    tas.""LastName"",
    sb_start.""ServiceBayName"",
    tas.""DealershipId"",
    tas.""QueryDate""
HAVING
    COUNT(*) = r.""RequiredSlots""
    AND BOOL_AND(COALESCE(tas_window.""IsAvailable"", FALSE))
    AND BOOL_AND(COALESCE(sb_window.""IsAvailable"", FALSE));
");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DROP VIEW IF EXISTS ""ServiceTypeAvailability"";
DROP VIEW IF EXISTS ""ServiceBayAvailableSlots"";
DROP VIEW IF EXISTS ""TechnicianAvailableSlots"";

CREATE VIEW ""TechnicianAvailableSlots"" AS
WITH query_dates AS (
    SELECT gs::date AS ""QueryDate""
    FROM generate_series(CURRENT_DATE - INTERVAL '1 day', CURRENT_DATE + INTERVAL '30 day', INTERVAL '1 day') gs
),
occupied_slots AS (
    SELECT
        s.""TechnicianId"",
        s.""DealershipId"",
        a.""AppointmentDate"" AS ""QueryDate"",
        seq AS ""SequenceOrder""
    FROM ""Services"" s
    INNER JOIN ""Appointments"" a ON a.""Id"" = s.""AppointmentId""
    LEFT JOIN ""AppointmentStatusLookups"" asl ON asl.""Id"" = a.""StatusId""
    INNER JOIN ""TimeSlots"" ts_start ON ts_start.""Id"" = s.""EstimatedStartTimeSlotId""
    INNER JOIN ""TimeSlots"" ts_end ON ts_end.""Id"" = s.""EstimatedEndTimeSlotId""
    CROSS JOIN LATERAL generate_series(ts_start.""SequenceOrder"", ts_end.""SequenceOrder"") seq
    WHERE s.""TechnicianId"" IS NOT NULL
      AND COALESCE(asl.""Name"", '') <> 'Cancelled'
)
SELECT
    ts.""Id"" AS ""TimeSlotId"",
    t.""Id"" AS ""TechnicianId"",
    ts.""SequenceOrder"",
    ts.""SlotStartTime"",
    ts.""SlotEndTime"",
    t.""FirstName"",
    t.""LastName"",
    t.""DealershipId"",
    qd.""QueryDate"",
    (occ.""SequenceOrder"" IS NULL) AS ""IsAvailable""
FROM ""Technicians"" t
INNER JOIN query_dates qd ON TRUE
INNER JOIN ""TechnicianSchedules"" sch
    ON sch.""TechnicianId"" = t.""Id""
   AND sch.""DayOfWeek"" = EXTRACT(DOW FROM qd.""QueryDate"")::integer
INNER JOIN ""TimeSlots"" ts
    ON ts.""IsActive"" = TRUE
   AND ts.""SlotStartTime"" >= sch.""StartTime""
   AND ts.""SlotEndTime"" <= sch.""EndTime""
LEFT JOIN occupied_slots occ
    ON occ.""TechnicianId"" = t.""Id""
   AND occ.""DealershipId"" = t.""DealershipId""
   AND occ.""QueryDate"" = qd.""QueryDate""
   AND occ.""SequenceOrder"" = ts.""SequenceOrder""
WHERE t.""IsActive"" = TRUE;

CREATE VIEW ""ServiceBayAvailableSlots"" AS
WITH query_dates AS (
    SELECT gs::date AS ""QueryDate""
    FROM generate_series(CURRENT_DATE - INTERVAL '1 day', CURRENT_DATE + INTERVAL '30 day', INTERVAL '1 day') gs
),
occupied_slots AS (
    SELECT
        s.""ServiceBayId"",
        s.""DealershipId"",
        a.""AppointmentDate"" AS ""QueryDate"",
        seq AS ""SequenceOrder""
    FROM ""Services"" s
    INNER JOIN ""Appointments"" a ON a.""Id"" = s.""AppointmentId""
    LEFT JOIN ""AppointmentStatusLookups"" asl ON asl.""Id"" = a.""StatusId""
    INNER JOIN ""TimeSlots"" ts_start ON ts_start.""Id"" = s.""EstimatedStartTimeSlotId""
    INNER JOIN ""TimeSlots"" ts_end ON ts_end.""Id"" = s.""EstimatedEndTimeSlotId""
    CROSS JOIN LATERAL generate_series(ts_start.""SequenceOrder"", ts_end.""SequenceOrder"") seq
    WHERE s.""ServiceBayId"" IS NOT NULL
      AND COALESCE(asl.""Name"", '') <> 'Cancelled'
)
SELECT
    ts.""Id"" AS ""TimeSlotId"",
    sb.""Id"" AS ""ServiceBayId"",
    ts.""SequenceOrder"",
    ts.""SlotStartTime"",
    ts.""SlotEndTime"",
    sb.""Name"" AS ""ServiceBayName"",
    sb.""DealershipId"",
    qd.""QueryDate"",
    (occ.""SequenceOrder"" IS NULL) AS ""IsAvailable""
FROM ""ServiceBays"" sb
INNER JOIN query_dates qd ON TRUE
INNER JOIN ""TimeSlots"" ts ON ts.""IsActive"" = TRUE
LEFT JOIN occupied_slots occ
    ON occ.""ServiceBayId"" = sb.""Id""
   AND occ.""DealershipId"" = sb.""DealershipId""
   AND occ.""QueryDate"" = qd.""QueryDate""
   AND occ.""SequenceOrder"" = ts.""SequenceOrder""
WHERE sb.""IsActive"" = TRUE;

CREATE VIEW ""ServiceTypeAvailability"" AS
WITH required AS (
    SELECT
        st.""Id"" AS ""ServiceTypeId"",
        st.""Name"" AS ""ServiceTypeName"",
        st.""DurationMinutes"",
        CEIL(st.""DurationMinutes"" / 30.0)::integer AS ""RequiredSlots""
    FROM ""ServiceTypes"" st
)
SELECT
    r.""ServiceTypeId"",
    tas.""TimeSlotId"",
    tas.""TechnicianId"",
    sb_start.""ServiceBayId"",
    r.""ServiceTypeName"",
    r.""DurationMinutes"",
    r.""RequiredSlots"",
    tas.""SequenceOrder"",
    tas.""SlotStartTime"",
    tas.""SlotEndTime"",
    tas.""FirstName"",
    tas.""LastName"",
    sb_start.""ServiceBayName"",
    tas.""DealershipId"",
    tas.""QueryDate"",
    TRUE AS ""CanFitService""
FROM required r
INNER JOIN ""TechnicianSkills"" skill
    ON skill.""ServiceTypeId"" = r.""ServiceTypeId""
INNER JOIN ""TechnicianAvailableSlots"" tas
    ON tas.""TechnicianId"" = skill.""TechnicianId""
   AND tas.""IsAvailable"" = TRUE
INNER JOIN ""ServiceBayAvailableSlots"" sb_start
    ON sb_start.""DealershipId"" = tas.""DealershipId""
   AND sb_start.""QueryDate"" = tas.""QueryDate""
   AND sb_start.""SequenceOrder"" = tas.""SequenceOrder""
   AND sb_start.""IsAvailable"" = TRUE
INNER JOIN LATERAL generate_series(
    tas.""SequenceOrder"",
    tas.""SequenceOrder"" + r.""RequiredSlots"" - 1
) seq ON TRUE
LEFT JOIN ""TechnicianAvailableSlots"" tas_window
    ON tas_window.""TechnicianId"" = tas.""TechnicianId""
   AND tas_window.""DealershipId"" = tas.""DealershipId""
   AND tas_window.""QueryDate"" = tas.""QueryDate""
   AND tas_window.""SequenceOrder"" = seq
LEFT JOIN ""ServiceBayAvailableSlots"" sb_window
    ON sb_window.""ServiceBayId"" = sb_start.""ServiceBayId""
   AND sb_window.""DealershipId"" = tas.""DealershipId""
   AND sb_window.""QueryDate"" = tas.""QueryDate""
   AND sb_window.""SequenceOrder"" = seq
GROUP BY
    r.""ServiceTypeId"",
    tas.""TimeSlotId"",
    tas.""TechnicianId"",
    sb_start.""ServiceBayId"",
    r.""ServiceTypeName"",
    r.""DurationMinutes"",
    r.""RequiredSlots"",
    tas.""SequenceOrder"",
    tas.""SlotStartTime"",
    tas.""SlotEndTime"",
    tas.""FirstName"",
    tas.""LastName"",
    sb_start.""ServiceBayName"",
    tas.""DealershipId"",
    tas.""QueryDate""
HAVING
    COUNT(*) = r.""RequiredSlots""
    AND BOOL_AND(COALESCE(tas_window.""IsAvailable"", FALSE))
    AND BOOL_AND(COALESCE(sb_window.""IsAvailable"", FALSE));
");

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
        }
    }
}
