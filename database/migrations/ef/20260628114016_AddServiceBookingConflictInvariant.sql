START TRANSACTION;

DROP INDEX "IX_Services_DealershipId";

DROP INDEX "IX_Services_ServiceBayId";

DROP INDEX "IX_Services_TechnicianId";

ALTER TABLE "Services" ADD "BookingDate" date;

ALTER TABLE "Services" ADD "EstimatedEndSlotSequenceExclusive" integer;

ALTER TABLE "Services" ADD "EstimatedStartSlotSequence" integer;

CREATE EXTENSION IF NOT EXISTS btree_gist;


                UPDATE "Services" AS s
                SET
                    "BookingDate" = (
                        SELECT a."AppointmentDate"
                        FROM "Appointments" AS a
                        WHERE a."Id" = s."AppointmentId"
                    ),
                    "EstimatedStartSlotSequence" = (
                        SELECT ts."SequenceOrder"
                        FROM "TimeSlots" AS ts
                        WHERE ts."Id" = s."EstimatedStartTimeSlotId"
                    ),
                    "EstimatedEndSlotSequenceExclusive" = (
                        SELECT ts."SequenceOrder" + 1
                        FROM "TimeSlots" AS ts
                        WHERE ts."Id" = s."EstimatedEndTimeSlotId"
                    );
            


                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM "Services"
                        WHERE "BookingDate" IS NULL
                           OR "EstimatedStartSlotSequence" IS NULL
                           OR "EstimatedEndSlotSequenceExclusive" IS NULL
                           OR "EstimatedEndSlotSequenceExclusive" <= "EstimatedStartSlotSequence"
                    ) THEN
                        RAISE EXCEPTION 'Cannot enforce booking invariant columns because backfill produced invalid values.';
                    END IF;
                END
                $$;
            

ALTER TABLE "Services" ALTER COLUMN "BookingDate" SET NOT NULL;

ALTER TABLE "Services" ALTER COLUMN "EstimatedStartSlotSequence" SET NOT NULL;

ALTER TABLE "Services" ALTER COLUMN "EstimatedEndSlotSequenceExclusive" SET NOT NULL;

UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.756905Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.756905Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000001';

UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.756906Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.756906Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000002';

UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.756906Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.756906Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000003';

UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.756907Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.756907Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000004';

UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.756907Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.756907Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000005';

UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.757459Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.757459Z'
WHERE "Id" = '00000000-0000-0000-0001-000000000001';

UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.75746Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.75746Z'
WHERE "Id" = '00000000-0000-0000-0001-000000000002';

UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.757461Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.757461Z'
WHERE "Id" = '00000000-0000-0000-0001-000000000003';

UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.757461Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.757461Z'
WHERE "Id" = '00000000-0000-0000-0001-000000000004';

UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.757462Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.757462Z'
WHERE "Id" = '00000000-0000-0000-0001-000000000005';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763222Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763222Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000001';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763223Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763223Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000002';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763224Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763224Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000003';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763225Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763225Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000004';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763226Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763226Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000005';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763227Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763227Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000006';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763228Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763228Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000007';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763231Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763231Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000008';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763232Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763232Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000009';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763233Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763233Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000010';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763234Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763234Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000011';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763235Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763235Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000012';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763236Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763236Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000013';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763237Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763237Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000014';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763238Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763238Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000015';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763239Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763239Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000016';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.76324Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.76324Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000017';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.76324Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763241Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000018';

CREATE INDEX "IX_Service_Dealership_BookingDate" ON "Services" ("DealershipId", "BookingDate");

CREATE INDEX "IX_Service_ServiceBay_BookingDate" ON "Services" ("ServiceBayId", "BookingDate");

CREATE INDEX "IX_Service_Technician_BookingDate" ON "Services" ("TechnicianId", "BookingDate");


                ALTER TABLE "Services"
                ADD CONSTRAINT "EXC_Service_Tech_Date_SeqRange_NoOverlap"
                EXCLUDE USING gist (
                    "BookingDate" WITH =,
                    "TechnicianId" WITH =,
                    int4range("EstimatedStartSlotSequence", "EstimatedEndSlotSequenceExclusive", '[)') WITH &&
                )
                WHERE ("TechnicianId" IS NOT NULL AND "IsActive");
            


                ALTER TABLE "Services"
                ADD CONSTRAINT "EXC_Service_Bay_Date_SeqRange_NoOverlap"
                EXCLUDE USING gist (
                    "BookingDate" WITH =,
                    "ServiceBayId" WITH =,
                    int4range("EstimatedStartSlotSequence", "EstimatedEndSlotSequenceExclusive", '[)') WITH &&
                )
                WHERE ("ServiceBayId" IS NOT NULL AND "IsActive");
            

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260628114016_AddServiceBookingConflictInvariant', '8.0.0');

COMMIT;

