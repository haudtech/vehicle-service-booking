-- Debug dataset for Vehicle Service Booking
-- Purpose:
--   1) Happy-path data for booking and availability checks
--   2) Edge-case data (restricted schedules, inactive bay, long duration service)
--   3) Failure-path setup data (slot conflicts, missing skill, cancelled overlap)
--
-- Safe to run multiple times: deterministic IDs + cleanup of only this script's records.
--
-- Usage example:
--   conn=$(grep '^CONNECTIONSTRINGS__DEFAULTCONNECTION=' .env | head -1 | cut -d= -f2-)
--   host=$(echo "$conn" | sed -n 's/.*Host=\([^;]*\).*/\1/p')
--   port=$(echo "$conn" | sed -n 's/.*Port=\([^;]*\).*/\1/p')
--   db=$(echo "$conn" | sed -n 's/.*Database=\([^;]*\).*/\1/p')
--   user=$(echo "$conn" | sed -n 's/.*Username=\([^;]*\).*/\1/p')
--   pass=$(echo "$conn" | sed -n 's/.*Password=\([^;]*\).*/\1/p')
--   PGPASSWORD="$pass" psql -h "$host" -p "$port" -U "$user" -d "$db" -f docs/sql/populate_debug_test_data.sql

BEGIN;

-- ============================================================
-- SECTION 0: Deterministic IDs
-- ============================================================
-- Dealerships
--   MAIN: used for most scenarios
--   OTHER: used to verify dealership isolation

-- ============================================================
-- SECTION 1: Cleanup previously seeded rows from this script
-- ============================================================
DELETE FROM "Services"
WHERE "Id" IN (
    '11111111-1111-1111-1111-100000000001',
    '11111111-1111-1111-1111-100000000002',
    '11111111-1111-1111-1111-100000000003',
    '11111111-1111-1111-1111-100000000004',
    '11111111-1111-1111-1111-100000000005'
);

DELETE FROM "Appointments"
WHERE "Id" IN (
    '11111111-1111-1111-1111-090000000001',
    '11111111-1111-1111-1111-090000000002',
    '11111111-1111-1111-1111-090000000003',
    '11111111-1111-1111-1111-090000000004',
    '11111111-1111-1111-1111-090000000005'
);

DELETE FROM "TechnicianSkills"
WHERE "Id" IN (
    '11111111-1111-1111-1111-080000000001',
    '11111111-1111-1111-1111-080000000002',
    '11111111-1111-1111-1111-080000000003'
);

DELETE FROM "TechnicianSchedules"
WHERE "Id" IN (
    '11111111-1111-1111-1111-070000000001',
    '11111111-1111-1111-1111-070000000002',
    '11111111-1111-1111-1111-070000000003'
);

DELETE FROM "BusinessHours"
WHERE "Id" IN (
    '11111111-1111-1111-1111-060000000001',
    '11111111-1111-1111-1111-060000000002'
);

DELETE FROM "ServiceBays"
WHERE "Id" IN (
    '11111111-1111-1111-1111-050000000001',
    '11111111-1111-1111-1111-050000000002',
    '11111111-1111-1111-1111-050000000003'
);

DELETE FROM "Technicians"
WHERE "Id" IN (
    '11111111-1111-1111-1111-040000000001',
    '11111111-1111-1111-1111-040000000002',
    '11111111-1111-1111-1111-040000000003',
    '11111111-1111-1111-1111-040000000004'
);

DELETE FROM "ServiceTypes"
WHERE "Id" IN (
    '11111111-1111-1111-1111-030000000001',
    '11111111-1111-1111-1111-030000000002',
    '11111111-1111-1111-1111-030000000003'
);

DELETE FROM "Vehicles"
WHERE "Id" IN (
    '11111111-1111-1111-1111-020000000001',
    '11111111-1111-1111-1111-020000000002',
    '11111111-1111-1111-1111-020000000003'
);

DELETE FROM "Customers"
WHERE "Id" IN (
    '11111111-1111-1111-1111-010000000001',
    '11111111-1111-1111-1111-010000000002',
    '11111111-1111-1111-1111-010000000003'
);

DELETE FROM "Dealerships"
WHERE "Id" IN (
    '11111111-1111-1111-1111-000000000001',
    '11111111-1111-1111-1111-000000000002'
);

-- ============================================================
-- SECTION 2: Core master data
-- ============================================================
INSERT INTO "Dealerships" ("Id", "Name", "Address") VALUES
    ('11111111-1111-1111-1111-000000000001', 'Debug Main Dealership', '101 Main St, Debug City'),
    ('11111111-1111-1111-1111-000000000002', 'Debug Other Dealership', '202 Other Ave, Debug City');

INSERT INTO "Customers" ("Id", "FirstName", "LastName", "Email", "PhoneNumber") VALUES
    ('11111111-1111-1111-1111-010000000001', 'Alice', 'Happy', 'alice.happy@example.com', '+15551000001'),
    ('11111111-1111-1111-1111-010000000002', 'Bob', 'Edge', 'bob.edge@example.com', '+15551000002'),
    ('11111111-1111-1111-1111-010000000003', 'Cara', 'Fail', 'cara.fail@example.com', '+15551000003');

INSERT INTO "Vehicles" ("Id", "CustomerId", "Vin", "Make", "Model", "Year") VALUES
    ('11111111-1111-1111-1111-020000000001', '11111111-1111-1111-1111-010000000001', '1HGCM82633A000001', 'Honda', 'Accord', 2021),
    ('11111111-1111-1111-1111-020000000002', '11111111-1111-1111-1111-010000000002', '1HGCM82633A000002', 'Toyota', 'Camry', 2019),
    ('11111111-1111-1111-1111-020000000003', '11111111-1111-1111-1111-010000000003', '1HGCM82633A000003', 'Ford', 'Escape', 2018);

INSERT INTO "ServiceTypes" ("Id", "Name", "DurationMinutes") VALUES
    ('11111111-1111-1111-1111-030000000001', 'Oil Change', 60),
    ('11111111-1111-1111-1111-030000000002', 'Brake Inspection', 90),
    ('11111111-1111-1111-1111-030000000003', 'Major Service', 240);

INSERT INTO "Technicians" ("Id", "DealershipId", "FirstName", "LastName") VALUES
    ('11111111-1111-1111-1111-040000000001', '11111111-1111-1111-1111-000000000001', 'Tom', 'SkilledAllDay'),
    ('11111111-1111-1111-1111-040000000002', '11111111-1111-1111-1111-000000000001', 'Rita', 'RestrictedHours'),
    ('11111111-1111-1111-1111-040000000003', '11111111-1111-1111-1111-000000000001', 'Ned', 'NoSkillForOil'),
    ('11111111-1111-1111-1111-040000000004', '11111111-1111-1111-1111-000000000002', 'Omar', 'OtherDealerTech');

INSERT INTO "ServiceBays" ("Id", "DealershipId", "Name", "IsActive") VALUES
    ('11111111-1111-1111-1111-050000000001', '11111111-1111-1111-1111-000000000001', 'Bay-A1', true),
    ('11111111-1111-1111-1111-050000000002', '11111111-1111-1111-1111-000000000001', 'Bay-A2', true),
    ('11111111-1111-1111-1111-050000000003', '11111111-1111-1111-1111-000000000001', 'Bay-A3-Inactive', false);

-- ============================================================
-- SECTION 3: Schedule data (dynamic for tomorrow)
-- ============================================================
WITH seed_day AS (
    SELECT
        (CURRENT_DATE + INTERVAL '1 day')::date AS d,
        EXTRACT(DOW FROM CURRENT_DATE + INTERVAL '1 day')::integer AS dow
)
INSERT INTO "BusinessHours" ("Id", "DealershipId", "DayOfWeek", "OpenTime", "CloseTime")
SELECT '11111111-1111-1111-1111-060000000001', '11111111-1111-1111-1111-000000000001', dow, '08:00', '17:00'
FROM seed_day;

WITH seed_day AS (
    SELECT
        (CURRENT_DATE + INTERVAL '1 day')::date AS d,
        EXTRACT(DOW FROM CURRENT_DATE + INTERVAL '1 day')::integer AS dow
)
INSERT INTO "BusinessHours" ("Id", "DealershipId", "DayOfWeek", "OpenTime", "CloseTime")
SELECT '11111111-1111-1111-1111-060000000002', '11111111-1111-1111-1111-000000000002', dow, '08:00', '17:00'
FROM seed_day;

WITH seed_day AS (
    SELECT EXTRACT(DOW FROM CURRENT_DATE + INTERVAL '1 day')::integer AS dow
)
INSERT INTO "TechnicianSchedules" ("Id", "TechnicianId", "DayOfWeek", "StartTime", "EndTime")
SELECT '11111111-1111-1111-1111-070000000001', '11111111-1111-1111-1111-040000000001', dow, '08:00', '17:00'
FROM seed_day;

WITH seed_day AS (
    SELECT EXTRACT(DOW FROM CURRENT_DATE + INTERVAL '1 day')::integer AS dow
)
INSERT INTO "TechnicianSchedules" ("Id", "TechnicianId", "DayOfWeek", "StartTime", "EndTime")
SELECT '11111111-1111-1111-1111-070000000002', '11111111-1111-1111-1111-040000000002', dow, '10:00', '14:00'
FROM seed_day;

WITH seed_day AS (
    SELECT EXTRACT(DOW FROM CURRENT_DATE + INTERVAL '1 day')::integer AS dow
)
INSERT INTO "TechnicianSchedules" ("Id", "TechnicianId", "DayOfWeek", "StartTime", "EndTime")
SELECT '11111111-1111-1111-1111-070000000003', '11111111-1111-1111-1111-040000000004', dow, '08:00', '17:00'
FROM seed_day;

-- Skills:
--   Tom: Oil + Brake
--   Rita: Brake only
--   Ned: Major only (no Oil skill: failure-path for oil booking)
INSERT INTO "TechnicianSkills" ("Id", "TechnicianId", "ServiceTypeId") VALUES
    ('11111111-1111-1111-1111-080000000001', '11111111-1111-1111-1111-040000000001', '11111111-1111-1111-1111-030000000001'),
    ('11111111-1111-1111-1111-080000000002', '11111111-1111-1111-1111-040000000001', '11111111-1111-1111-1111-030000000002'),
    ('11111111-1111-1111-1111-080000000003', '11111111-1111-1111-1111-040000000003', '11111111-1111-1111-1111-030000000003');

-- ============================================================
-- SECTION 4: Scenario appointments and services
-- ============================================================
-- Status lookup IDs (seeded by migration):
--   Appointment Booked    = 00000000-0000-0000-0000-000000000001
--   Appointment Cancelled = 00000000-0000-0000-0000-000000000004
--   Service Pending       = 00000000-0000-0000-0001-000000000001

-- Reference slot IDs (seeded by migration):
--   4: 09:30-10:00
--   5: 10:00-10:30
--   6: 10:30-11:00
--   7: 11:00-11:30
--   8: 11:30-12:00
--   9: 12:00-12:30

-- A1: BOOKED conflicting appointment (same vehicle and overlapping slots)
WITH seed_day AS (
    SELECT (CURRENT_DATE + INTERVAL '1 day')::date AS d
)
INSERT INTO "Appointments" ("Id", "DealershipId", "CustomerId", "VehicleId", "AppointmentDate", "StatusId", "Notes")
SELECT
    '11111111-1111-1111-1111-090000000001',
    '11111111-1111-1111-1111-000000000001',
    '11111111-1111-1111-1111-010000000001',
    '11111111-1111-1111-1111-020000000001',
    d,
    '00000000-0000-0000-0000-000000000001',
    'FAILURE CASE: creates overlap conflict for vehicle 020...0001'
FROM seed_day;

INSERT INTO "Services" ("Id", "AppointmentId", "ServiceTypeId", "TechnicianId", "ServiceBayId", "DealershipId", "ServiceStatusId", "EstimatedStartTimeSlotId", "EstimatedEndTimeSlotId", "BookingDate", "EstimatedStartSlotSequence", "EstimatedEndSlotSequenceExclusive", "SequenceOrder", "Notes") VALUES
    (
        '11111111-1111-1111-1111-100000000001',
        '11111111-1111-1111-1111-090000000001',
        '11111111-1111-1111-1111-030000000001',
        '11111111-1111-1111-1111-040000000001',
        '11111111-1111-1111-1111-050000000001',
        '11111111-1111-1111-1111-000000000001',
        '00000000-0000-0000-0001-000000000001',
        '00000000-0000-0000-0000-000000000004',
        '00000000-0000-0000-0000-000000000006',
        (SELECT "AppointmentDate" FROM "Appointments" WHERE "Id" = '11111111-1111-1111-1111-090000000001'),
        4,
        7,
        1,
        'Occupies Bay-A1 and Tom across slots 4-6'
    );

-- A2: CANCELLED overlapping appointment (should not block conflict checks)
WITH seed_day AS (
    SELECT (CURRENT_DATE + INTERVAL '1 day')::date AS d
)
INSERT INTO "Appointments" ("Id", "DealershipId", "CustomerId", "VehicleId", "AppointmentDate", "StatusId", "Notes")
SELECT
    '11111111-1111-1111-1111-090000000002',
    '11111111-1111-1111-1111-000000000001',
    '11111111-1111-1111-1111-010000000002',
    '11111111-1111-1111-1111-020000000002',
    d,
    '00000000-0000-0000-0000-000000000004',
    'EDGE CASE: cancelled overlapping appointment should not block'
FROM seed_day;

INSERT INTO "Services" ("Id", "AppointmentId", "ServiceTypeId", "TechnicianId", "ServiceBayId", "DealershipId", "ServiceStatusId", "EstimatedStartTimeSlotId", "EstimatedEndTimeSlotId", "BookingDate", "EstimatedStartSlotSequence", "EstimatedEndSlotSequenceExclusive", "SequenceOrder", "Notes") VALUES
    (
        '11111111-1111-1111-1111-100000000002',
        '11111111-1111-1111-1111-090000000002',
        '11111111-1111-1111-1111-030000000002',
        '11111111-1111-1111-1111-040000000002',
        '11111111-1111-1111-1111-050000000002',
        '11111111-1111-1111-1111-000000000001',
        '00000000-0000-0000-0001-000000000001',
        '00000000-0000-0000-0000-000000000004',
        '00000000-0000-0000-0000-000000000006',
        (SELECT "AppointmentDate" FROM "Appointments" WHERE "Id" = '11111111-1111-1111-1111-090000000002'),
        4,
        7,
        1,
        'Cancelled overlap for validation checks'
    );

-- A3: HAPPY CASE baseline appointment for retrieval/query tests
WITH seed_day AS (
    SELECT (CURRENT_DATE + INTERVAL '2 day')::date AS d
)
INSERT INTO "Appointments" ("Id", "DealershipId", "CustomerId", "VehicleId", "AppointmentDate", "StatusId", "Notes")
SELECT
    '11111111-1111-1111-1111-090000000003',
    '11111111-1111-1111-1111-000000000001',
    '11111111-1111-1111-1111-010000000003',
    '11111111-1111-1111-1111-020000000003',
    d,
    '00000000-0000-0000-0000-000000000001',
    'HAPPY CASE: valid booking in future date'
FROM seed_day;

INSERT INTO "Services" ("Id", "AppointmentId", "ServiceTypeId", "TechnicianId", "ServiceBayId", "DealershipId", "ServiceStatusId", "EstimatedStartTimeSlotId", "EstimatedEndTimeSlotId", "BookingDate", "EstimatedStartSlotSequence", "EstimatedEndSlotSequenceExclusive", "SequenceOrder", "Notes") VALUES
    (
        '11111111-1111-1111-1111-100000000003',
        '11111111-1111-1111-1111-090000000003',
        '11111111-1111-1111-1111-030000000002',
        '11111111-1111-1111-1111-040000000001',
        '11111111-1111-1111-1111-050000000002',
        '11111111-1111-1111-1111-000000000001',
        '00000000-0000-0000-0001-000000000001',
        '00000000-0000-0000-0000-000000000007',
        '00000000-0000-0000-0000-000000000009',
        (SELECT "AppointmentDate" FROM "Appointments" WHERE "Id" = '11111111-1111-1111-1111-090000000003'),
        7,
        10,
        1,
        'Valid future booking for happy-path tests'
    );

-- A4: Dealership isolation case (same slot pattern in other dealership)
WITH seed_day AS (
    SELECT (CURRENT_DATE + INTERVAL '1 day')::date AS d
)
INSERT INTO "Appointments" ("Id", "DealershipId", "CustomerId", "VehicleId", "AppointmentDate", "StatusId", "Notes")
SELECT
    '11111111-1111-1111-1111-090000000004',
    '11111111-1111-1111-1111-000000000002',
    '11111111-1111-1111-1111-010000000001',
    '11111111-1111-1111-1111-020000000001',
    d,
    '00000000-0000-0000-0000-000000000001',
    'EDGE CASE: other dealership should not affect main dealership availability'
FROM seed_day;

INSERT INTO "Services" ("Id", "AppointmentId", "ServiceTypeId", "TechnicianId", "ServiceBayId", "DealershipId", "ServiceStatusId", "EstimatedStartTimeSlotId", "EstimatedEndTimeSlotId", "BookingDate", "EstimatedStartSlotSequence", "EstimatedEndSlotSequenceExclusive", "SequenceOrder", "Notes") VALUES
    (
        '11111111-1111-1111-1111-100000000004',
        '11111111-1111-1111-1111-090000000004',
        '11111111-1111-1111-1111-030000000001',
        '11111111-1111-1111-1111-040000000004',
        NULL,
        '11111111-1111-1111-1111-000000000002',
        '00000000-0000-0000-0001-000000000001',
        '00000000-0000-0000-0000-000000000004',
        '00000000-0000-0000-0000-000000000006',
        (SELECT "AppointmentDate" FROM "Appointments" WHERE "Id" = '11111111-1111-1111-1111-090000000004'),
        4,
        7,
        1,
        'Other dealership occupancy for isolation assertions'
    );

-- A5: Edge long-duration service block in main dealership
WITH seed_day AS (
    SELECT (CURRENT_DATE + INTERVAL '3 day')::date AS d
)
INSERT INTO "Appointments" ("Id", "DealershipId", "CustomerId", "VehicleId", "AppointmentDate", "StatusId", "Notes")
SELECT
    '11111111-1111-1111-1111-090000000005',
    '11111111-1111-1111-1111-000000000001',
    '11111111-1111-1111-1111-010000000002',
    '11111111-1111-1111-1111-020000000002',
    d,
    '00000000-0000-0000-0000-000000000001',
    'EDGE CASE: long duration service occupies noon window'
FROM seed_day;

INSERT INTO "Services" ("Id", "AppointmentId", "ServiceTypeId", "TechnicianId", "ServiceBayId", "DealershipId", "ServiceStatusId", "EstimatedStartTimeSlotId", "EstimatedEndTimeSlotId", "BookingDate", "EstimatedStartSlotSequence", "EstimatedEndSlotSequenceExclusive", "SequenceOrder", "Notes") VALUES
    (
        '11111111-1111-1111-1111-100000000005',
        '11111111-1111-1111-1111-090000000005',
        '11111111-1111-1111-1111-030000000003',
        '11111111-1111-1111-1111-040000000003',
        '11111111-1111-1111-1111-050000000002',
        '11111111-1111-1111-1111-000000000001',
        '00000000-0000-0000-0001-000000000001',
        '00000000-0000-0000-0000-000000000008',
        '00000000-0000-0000-0000-000000000016',
        (SELECT "AppointmentDate" FROM "Appointments" WHERE "Id" = '11111111-1111-1111-1111-090000000005'),
        8,
        17,
        1,
        'Long duration service to test slot span logic'
    );

-- ============================================================
-- SECTION 5: Optional expected-failure probes (DB constraints)
-- ============================================================
-- This block intentionally attempts invalid inserts and catches errors.
-- No invalid row remains in DB.
DO $$
BEGIN
    BEGIN
        INSERT INTO "ServiceTypes" ("Id", "Name", "DurationMinutes")
        VALUES ('11111111-1111-1111-1111-030000009999', 'Invalid Duration', 10);
    EXCEPTION WHEN check_violation THEN
        RAISE NOTICE 'Expected failure captured: ServiceTypes.DurationMinutes check constraint';
    END;

    BEGIN
        INSERT INTO "Vehicles" ("Id", "CustomerId", "Vin", "Make", "Model", "Year")
        VALUES ('11111111-1111-1111-1111-020000009999', '11111111-1111-1111-1111-010000000001', 'SHORTVIN123', 'Test', 'InvalidVin', 2020);
    EXCEPTION WHEN check_violation THEN
        RAISE NOTICE 'Expected failure captured: Vehicles.Vin length check constraint';
    END;
END $$;

-- ============================================================
-- SECTION 6: Best-effort refresh for materialized views
-- ============================================================
DO $$
BEGIN
    BEGIN
        REFRESH MATERIALIZED VIEW "TechnicianAvailableSlots";
    EXCEPTION WHEN undefined_table OR wrong_object_type THEN
        NULL;
    END;

    BEGIN
        REFRESH MATERIALIZED VIEW "ServiceBayAvailableSlots";
    EXCEPTION WHEN undefined_table OR wrong_object_type THEN
        NULL;
    END;

    BEGIN
        REFRESH MATERIALIZED VIEW "ServiceTypeAvailability";
    EXCEPTION WHEN undefined_table OR wrong_object_type THEN
        NULL;
    END;
END $$;

COMMIT;

-- ============================================================
-- SECTION 7: Quick verification queries
-- ============================================================
-- 1) Scenario overview
SELECT
    COUNT(*) FILTER (WHERE a."Id" = '11111111-1111-1111-1111-090000000001') AS conflict_case,
    COUNT(*) FILTER (WHERE a."Id" = '11111111-1111-1111-1111-090000000002') AS cancelled_case,
    COUNT(*) FILTER (WHERE a."Id" = '11111111-1111-1111-1111-090000000003') AS happy_case,
    COUNT(*) FILTER (WHERE a."Id" = '11111111-1111-1111-1111-090000000004') AS isolation_case,
    COUNT(*) FILTER (WHERE a."Id" = '11111111-1111-1111-1111-090000000005') AS long_service_case
FROM "Appointments" a;

-- 2) Skill gap check (expected: technician Ned has no Oil Change skill)
SELECT t."FirstName", t."LastName", st."Name" AS service_type
FROM "Technicians" t
LEFT JOIN "TechnicianSkills" ts ON ts."TechnicianId" = t."Id"
LEFT JOIN "ServiceTypes" st ON st."Id" = ts."ServiceTypeId"
WHERE t."Id" = '11111111-1111-1111-1111-040000000003';

-- 3) Conflict reference row
SELECT
    a."Id" AS appointment_id,
    a."AppointmentDate",
    s."EstimatedStartTimeSlotId",
    s."EstimatedEndTimeSlotId",
    a."StatusId"
FROM "Appointments" a
JOIN "Services" s ON s."AppointmentId" = a."Id"
WHERE a."Id" = '11111111-1111-1111-1111-090000000001';
