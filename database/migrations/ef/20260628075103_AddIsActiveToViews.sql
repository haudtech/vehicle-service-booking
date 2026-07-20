START TRANSACTION;


DROP VIEW IF EXISTS "ServiceTypeAvailability";
DROP VIEW IF EXISTS "ServiceBayAvailableSlots";
DROP VIEW IF EXISTS "TechnicianAvailableSlots";

CREATE VIEW "TechnicianAvailableSlots" AS
WITH query_dates AS (
    SELECT gs::date AS "QueryDate"
    FROM generate_series(CURRENT_DATE - INTERVAL '1 day', CURRENT_DATE + INTERVAL '30 day', INTERVAL '1 day') gs
),
occupied_slots AS (
    SELECT
        s."TechnicianId",
        s."DealershipId",
        a."AppointmentDate" AS "QueryDate",
        seq AS "SequenceOrder"
    FROM "Services" s
    INNER JOIN "Appointments" a ON a."Id" = s."AppointmentId"
    LEFT JOIN "AppointmentStatusLookups" asl ON asl."Id" = a."StatusId"
    INNER JOIN "TimeSlots" ts_start ON ts_start."Id" = s."EstimatedStartTimeSlotId"
    INNER JOIN "TimeSlots" ts_end ON ts_end."Id" = s."EstimatedEndTimeSlotId"
    CROSS JOIN LATERAL generate_series(ts_start."SequenceOrder", ts_end."SequenceOrder") seq
    WHERE s."TechnicianId" IS NOT NULL
      AND s."IsActive" = TRUE
      AND a."IsActive" = TRUE
      AND COALESCE(asl."Name", '') <> 'Cancelled'
)
SELECT
    ts."Id" AS "TimeSlotId",
    t."Id" AS "TechnicianId",
    ts."SequenceOrder",
    ts."SlotStartTime",
    ts."SlotEndTime",
    t."FirstName",
    t."LastName",
    t."DealershipId",
    qd."QueryDate",
    (occ."SequenceOrder" IS NULL) AS "IsAvailable",
    TRUE AS "IsActive"
FROM "Technicians" t
INNER JOIN query_dates qd ON TRUE
INNER JOIN "TechnicianSchedules" sch
    ON sch."TechnicianId" = t."Id"
   AND sch."DayOfWeek" = EXTRACT(DOW FROM qd."QueryDate")::integer
INNER JOIN "TimeSlots" ts
    ON ts."IsActive" = TRUE
   AND ts."SlotStartTime" >= sch."StartTime"
   AND ts."SlotEndTime" <= sch."EndTime"
LEFT JOIN occupied_slots occ
    ON occ."TechnicianId" = t."Id"
   AND occ."DealershipId" = t."DealershipId"
   AND occ."QueryDate" = qd."QueryDate"
   AND occ."SequenceOrder" = ts."SequenceOrder"
WHERE t."IsActive" = TRUE
  AND sch."IsActive" = TRUE;

CREATE VIEW "ServiceBayAvailableSlots" AS
WITH query_dates AS (
    SELECT gs::date AS "QueryDate"
    FROM generate_series(CURRENT_DATE - INTERVAL '1 day', CURRENT_DATE + INTERVAL '30 day', INTERVAL '1 day') gs
),
occupied_slots AS (
    SELECT
        s."ServiceBayId",
        s."DealershipId",
        a."AppointmentDate" AS "QueryDate",
        seq AS "SequenceOrder"
    FROM "Services" s
    INNER JOIN "Appointments" a ON a."Id" = s."AppointmentId"
    LEFT JOIN "AppointmentStatusLookups" asl ON asl."Id" = a."StatusId"
    INNER JOIN "TimeSlots" ts_start ON ts_start."Id" = s."EstimatedStartTimeSlotId"
    INNER JOIN "TimeSlots" ts_end ON ts_end."Id" = s."EstimatedEndTimeSlotId"
    CROSS JOIN LATERAL generate_series(ts_start."SequenceOrder", ts_end."SequenceOrder") seq
    WHERE s."ServiceBayId" IS NOT NULL
      AND s."IsActive" = TRUE
      AND a."IsActive" = TRUE
      AND COALESCE(asl."Name", '') <> 'Cancelled'
)
SELECT
    ts."Id" AS "TimeSlotId",
    sb."Id" AS "ServiceBayId",
    ts."SequenceOrder",
    ts."SlotStartTime",
    ts."SlotEndTime",
    sb."Name" AS "ServiceBayName",
    sb."DealershipId",
    qd."QueryDate",
    (occ."SequenceOrder" IS NULL) AS "IsAvailable",
    TRUE AS "IsActive"
FROM "ServiceBays" sb
INNER JOIN query_dates qd ON TRUE
INNER JOIN "TimeSlots" ts ON ts."IsActive" = TRUE
LEFT JOIN occupied_slots occ
    ON occ."ServiceBayId" = sb."Id"
   AND occ."DealershipId" = sb."DealershipId"
   AND occ."QueryDate" = qd."QueryDate"
   AND occ."SequenceOrder" = ts."SequenceOrder"
WHERE sb."IsActive" = TRUE;

CREATE VIEW "ServiceTypeAvailability" AS
WITH required AS (
    SELECT
        st."Id" AS "ServiceTypeId",
        st."Name" AS "ServiceTypeName",
        st."DurationMinutes",
        CEIL(st."DurationMinutes" / 30.0)::integer AS "RequiredSlots"
    FROM "ServiceTypes" st
    WHERE st."IsActive" = TRUE
)
SELECT
    r."ServiceTypeId",
    tas."TimeSlotId",
    tas."TechnicianId",
    sb_start."ServiceBayId",
    r."ServiceTypeName",
    r."DurationMinutes",
    r."RequiredSlots",
    tas."SequenceOrder",
    tas."SlotStartTime",
    tas."SlotEndTime",
    tas."FirstName",
    tas."LastName",
    sb_start."ServiceBayName",
    tas."DealershipId",
    tas."QueryDate",
    TRUE AS "CanFitService",
    TRUE AS "IsActive"
FROM required r
INNER JOIN "TechnicianSkills" skill
    ON skill."ServiceTypeId" = r."ServiceTypeId"
   AND skill."IsActive" = TRUE
INNER JOIN "TechnicianAvailableSlots" tas
    ON tas."TechnicianId" = skill."TechnicianId"
   AND tas."IsAvailable" = TRUE
   AND tas."IsActive" = TRUE
INNER JOIN "ServiceBayAvailableSlots" sb_start
    ON sb_start."DealershipId" = tas."DealershipId"
   AND sb_start."QueryDate" = tas."QueryDate"
   AND sb_start."SequenceOrder" = tas."SequenceOrder"
   AND sb_start."IsAvailable" = TRUE
   AND sb_start."IsActive" = TRUE
INNER JOIN LATERAL generate_series(
    tas."SequenceOrder",
    tas."SequenceOrder" + r."RequiredSlots" - 1
) seq ON TRUE
LEFT JOIN "TechnicianAvailableSlots" tas_window
    ON tas_window."TechnicianId" = tas."TechnicianId"
   AND tas_window."DealershipId" = tas."DealershipId"
   AND tas_window."QueryDate" = tas."QueryDate"
   AND tas_window."SequenceOrder" = seq
LEFT JOIN "ServiceBayAvailableSlots" sb_window
    ON sb_window."ServiceBayId" = sb_start."ServiceBayId"
   AND sb_window."DealershipId" = tas."DealershipId"
   AND sb_window."QueryDate" = tas."QueryDate"
   AND sb_window."SequenceOrder" = seq
GROUP BY
    r."ServiceTypeId",
    tas."TimeSlotId",
    tas."TechnicianId",
    sb_start."ServiceBayId",
    r."ServiceTypeName",
    r."DurationMinutes",
    r."RequiredSlots",
    tas."SequenceOrder",
    tas."SlotStartTime",
    tas."SlotEndTime",
    tas."FirstName",
    tas."LastName",
    sb_start."ServiceBayName",
    tas."DealershipId",
    tas."QueryDate"
HAVING
    COUNT(*) = r."RequiredSlots"
    AND BOOL_AND(COALESCE(tas_window."IsAvailable", FALSE))
    AND BOOL_AND(COALESCE(sb_window."IsAvailable", FALSE));


UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.86168Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.86168Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000001';

UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.861681Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.861681Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000002';

UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.861682Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.861682Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000003';

UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.861682Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.861682Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000004';

UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.861682Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.861682Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000005';

UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.862114Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.862114Z'
WHERE "Id" = '00000000-0000-0000-0001-000000000001';

UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.862114Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.862114Z'
WHERE "Id" = '00000000-0000-0000-0001-000000000002';

UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.862115Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.862115Z'
WHERE "Id" = '00000000-0000-0000-0001-000000000003';

UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.862115Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.862115Z'
WHERE "Id" = '00000000-0000-0000-0001-000000000004';

UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.862115Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.862115Z'
WHERE "Id" = '00000000-0000-0000-0001-000000000005';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865585Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865585Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000001';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865586Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865586Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000002';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865587Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865587Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000003';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865588Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865588Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000004';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865588Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865588Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000005';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865589Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865589Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000006';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.86559Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.86559Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000007';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.86559Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.86559Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000008';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865591Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865591Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000009';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865592Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865592Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000010';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865592Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865592Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000011';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865593Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865593Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000012';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865599Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865599Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000013';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.8656Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.8656Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000014';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.8656Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.8656Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000015';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865601Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865601Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000016';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865601Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865601Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000017';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865602Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865602Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000018';

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260628075103_AddIsActiveToViews', '8.0.0');

COMMIT;

