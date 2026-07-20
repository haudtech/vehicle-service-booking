CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    CREATE TABLE "AppointmentStatusLookups" (
        "Id" uuid NOT NULL,
        "Status" integer NOT NULL,
        "Name" character varying(50) NOT NULL,
        "Description" character varying(200),
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        CONSTRAINT "PK_AppointmentStatusLookups" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    CREATE TABLE "Customers" (
        "Id" uuid NOT NULL,
        "FirstName" character varying(100) NOT NULL,
        "LastName" character varying(100) NOT NULL,
        "Email" character varying(254) NOT NULL,
        "PhoneNumber" character varying(20) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        CONSTRAINT "PK_Customers" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    CREATE TABLE "Dealerships" (
        "Id" uuid NOT NULL,
        "Name" character varying(150) NOT NULL,
        "Address" character varying(500) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        CONSTRAINT "PK_Dealerships" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    CREATE TABLE "ServiceStatusLookups" (
        "Id" uuid NOT NULL,
        "Status" integer NOT NULL,
        "Name" character varying(50) NOT NULL,
        "Description" character varying(200) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        CONSTRAINT "PK_ServiceStatusLookups" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    CREATE TABLE "ServiceTypes" (
        "Id" uuid NOT NULL,
        "Name" character varying(100) NOT NULL,
        "DurationMinutes" integer NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        CONSTRAINT "PK_ServiceTypes" PRIMARY KEY ("Id"),
        CONSTRAINT "CK_ServiceType_DurationMinutes_Range" CHECK ("DurationMinutes" >= 30 AND "DurationMinutes" <= 480)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    CREATE TABLE "TimeSlots" (
        "Id" uuid NOT NULL,
        "SequenceOrder" integer NOT NULL,
        "SlotStartTime" time NOT NULL,
        "SlotEndTime" time NOT NULL,
        "IsActive" boolean NOT NULL DEFAULT TRUE,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        CONSTRAINT "PK_TimeSlots" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    CREATE TABLE "Vehicles" (
        "Id" uuid NOT NULL,
        "CustomerId" uuid NOT NULL,
        "Vin" character varying(17) NOT NULL,
        "Make" character varying(20) NOT NULL,
        "Model" character varying(50) NOT NULL,
        "Year" integer,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        CONSTRAINT "PK_Vehicles" PRIMARY KEY ("Id"),
        CONSTRAINT "CK_Vehicle_VIN_Length" CHECK (LENGTH("Vin") = 17),
        CONSTRAINT "CK_Vehicle_Year_Range" CHECK ("Year" IS NULL OR ("Year" >= 1900 AND "Year" <= 2100)),
        CONSTRAINT "FK_Vehicle_Customer" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    CREATE TABLE "BusinessHours" (
        "Id" uuid NOT NULL,
        "DealershipId" uuid NOT NULL,
        "DayOfWeek" integer NOT NULL,
        "OpenTime" time without time zone NOT NULL,
        "CloseTime" time without time zone NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        CONSTRAINT "PK_BusinessHours" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_BusinessHours_Dealership" FOREIGN KEY ("DealershipId") REFERENCES "Dealerships" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    CREATE TABLE "ServiceBays" (
        "Id" uuid NOT NULL,
        "DealershipId" uuid NOT NULL,
        "Name" character varying(50) NOT NULL,
        "IsActive" boolean NOT NULL DEFAULT TRUE,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        CONSTRAINT "PK_ServiceBays" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_ServiceBay_Dealership" FOREIGN KEY ("DealershipId") REFERENCES "Dealerships" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    CREATE TABLE "Technicians" (
        "Id" uuid NOT NULL,
        "DealershipId" uuid NOT NULL,
        "FirstName" character varying(100) NOT NULL,
        "LastName" character varying(100) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        CONSTRAINT "PK_Technicians" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Technician_Dealership" FOREIGN KEY ("DealershipId") REFERENCES "Dealerships" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    CREATE TABLE "Appointments" (
        "Id" uuid NOT NULL,
        "DealershipId" uuid NOT NULL,
        "CustomerId" uuid NOT NULL,
        "VehicleId" uuid NOT NULL,
        "AppointmentDate" date NOT NULL,
        "StatusId" uuid NOT NULL,
        "Notes" text NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        CONSTRAINT "PK_Appointments" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Appointment_AppointmentStatus" FOREIGN KEY ("StatusId") REFERENCES "AppointmentStatusLookups" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_Appointment_Customer" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_Appointment_Dealership" FOREIGN KEY ("DealershipId") REFERENCES "Dealerships" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_Appointment_Vehicle" FOREIGN KEY ("VehicleId") REFERENCES "Vehicles" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    CREATE TABLE "TechnicianSchedules" (
        "Id" uuid NOT NULL,
        "TechnicianId" uuid NOT NULL,
        "DayOfWeek" integer NOT NULL,
        "StartTime" time without time zone NOT NULL,
        "EndTime" time without time zone NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        CONSTRAINT "PK_TechnicianSchedules" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_TechnicianSchedule_Technician" FOREIGN KEY ("TechnicianId") REFERENCES "Technicians" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    CREATE TABLE "TechnicianSkills" (
        "Id" uuid NOT NULL,
        "TechnicianId" uuid NOT NULL,
        "ServiceTypeId" uuid NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        CONSTRAINT "PK_TechnicianSkills" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_TechnicianSkill_ServiceType" FOREIGN KEY ("ServiceTypeId") REFERENCES "ServiceTypes" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_TechnicianSkill_Technician" FOREIGN KEY ("TechnicianId") REFERENCES "Technicians" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    CREATE TABLE "Services" (
        "Id" uuid NOT NULL,
        "AppointmentId" uuid NOT NULL,
        "ServiceTypeId" uuid NOT NULL,
        "TechnicianId" uuid,
        "ServiceBayId" uuid,
        "DealershipId" uuid NOT NULL,
        "ServiceStatusId" uuid NOT NULL,
        "EstimatedStartTimeSlotId" uuid,
        "EstimatedEndTimeSlotId" uuid,
        "SequenceOrder" integer NOT NULL,
        "ActualStartTime" timestamp with time zone,
        "ActualEndTime" timestamp with time zone,
        "Notes" character varying(500) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        CONSTRAINT "PK_Services" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Service_Appointment" FOREIGN KEY ("AppointmentId") REFERENCES "Appointments" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_Service_Dealership" FOREIGN KEY ("DealershipId") REFERENCES "Dealerships" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_Service_ServiceBay" FOREIGN KEY ("ServiceBayId") REFERENCES "ServiceBays" ("Id") ON DELETE SET NULL,
        CONSTRAINT "FK_Service_ServiceStatus" FOREIGN KEY ("ServiceStatusId") REFERENCES "ServiceStatusLookups" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_Service_ServiceType" FOREIGN KEY ("ServiceTypeId") REFERENCES "ServiceTypes" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_Service_Technician" FOREIGN KEY ("TechnicianId") REFERENCES "Technicians" ("Id") ON DELETE SET NULL,
        CONSTRAINT "FK_Service_TimeSlot_End" FOREIGN KEY ("EstimatedEndTimeSlotId") REFERENCES "TimeSlots" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_Service_TimeSlot_Start" FOREIGN KEY ("EstimatedStartTimeSlotId") REFERENCES "TimeSlots" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    INSERT INTO "AppointmentStatusLookups" ("Id", "CreatedAt", "Description", "Name", "Status", "UpdatedAt")
    VALUES ('00000000-0000-0000-0000-000000000001', TIMESTAMPTZ '2026-06-27T17:30:46.128485Z', 'Appointment is scheduled', 'Booked', 1, TIMESTAMPTZ '2026-06-27T17:30:46.128485Z');
    INSERT INTO "AppointmentStatusLookups" ("Id", "CreatedAt", "Description", "Name", "Status", "UpdatedAt")
    VALUES ('00000000-0000-0000-0000-000000000002', TIMESTAMPTZ '2026-06-27T17:30:46.128486Z', 'Service is currently being performed', 'In Progress', 2, TIMESTAMPTZ '2026-06-27T17:30:46.128486Z');
    INSERT INTO "AppointmentStatusLookups" ("Id", "CreatedAt", "Description", "Name", "Status", "UpdatedAt")
    VALUES ('00000000-0000-0000-0000-000000000003', TIMESTAMPTZ '2026-06-27T17:30:46.128486Z', 'Service has been completed', 'Completed', 3, TIMESTAMPTZ '2026-06-27T17:30:46.128486Z');
    INSERT INTO "AppointmentStatusLookups" ("Id", "CreatedAt", "Description", "Name", "Status", "UpdatedAt")
    VALUES ('00000000-0000-0000-0000-000000000004', TIMESTAMPTZ '2026-06-27T17:30:46.128487Z', 'Appointment has been cancelled', 'Cancelled', 4, TIMESTAMPTZ '2026-06-27T17:30:46.128487Z');
    INSERT INTO "AppointmentStatusLookups" ("Id", "CreatedAt", "Description", "Name", "Status", "UpdatedAt")
    VALUES ('00000000-0000-0000-0000-000000000005', TIMESTAMPTZ '2026-06-27T17:30:46.128487Z', 'Some services completed, others rescheduled', 'Partially Completed', 5, TIMESTAMPTZ '2026-06-27T17:30:46.128487Z');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    INSERT INTO "ServiceStatusLookups" ("Id", "CreatedAt", "Description", "Name", "Status", "UpdatedAt")
    VALUES ('00000000-0000-0000-0001-000000000001', TIMESTAMPTZ '2026-06-27T17:30:46.129067Z', 'Service scheduled but not started', 'Pending', 0, TIMESTAMPTZ '2026-06-27T17:30:46.129067Z');
    INSERT INTO "ServiceStatusLookups" ("Id", "CreatedAt", "Description", "Name", "Status", "UpdatedAt")
    VALUES ('00000000-0000-0000-0001-000000000002', TIMESTAMPTZ '2026-06-27T17:30:46.129068Z', 'Service is currently being performed', 'In Progress', 1, TIMESTAMPTZ '2026-06-27T17:30:46.129068Z');
    INSERT INTO "ServiceStatusLookups" ("Id", "CreatedAt", "Description", "Name", "Status", "UpdatedAt")
    VALUES ('00000000-0000-0000-0001-000000000003', TIMESTAMPTZ '2026-06-27T17:30:46.129068Z', 'Service has been completed successfully', 'Completed', 2, TIMESTAMPTZ '2026-06-27T17:30:46.129068Z');
    INSERT INTO "ServiceStatusLookups" ("Id", "CreatedAt", "Description", "Name", "Status", "UpdatedAt")
    VALUES ('00000000-0000-0000-0001-000000000004', TIMESTAMPTZ '2026-06-27T17:30:46.129068Z', 'Service was cancelled or declined', 'Skipped', 3, TIMESTAMPTZ '2026-06-27T17:30:46.129068Z');
    INSERT INTO "ServiceStatusLookups" ("Id", "CreatedAt", "Description", "Name", "Status", "UpdatedAt")
    VALUES ('00000000-0000-0000-0001-000000000005', TIMESTAMPTZ '2026-06-27T17:30:46.129069Z', 'Service moved to a different appointment', 'Rescheduled', 4, TIMESTAMPTZ '2026-06-27T17:30:46.129069Z');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    INSERT INTO "TimeSlots" ("Id", "CreatedAt", "IsActive", "SequenceOrder", "SlotEndTime", "SlotStartTime", "UpdatedAt")
    VALUES ('00000000-0000-0000-0000-000000000001', TIMESTAMPTZ '2026-06-27T17:30:46.132528Z', TRUE, 1, TIME '08:30:00', TIME '08:00:00', TIMESTAMPTZ '2026-06-27T17:30:46.132528Z');
    INSERT INTO "TimeSlots" ("Id", "CreatedAt", "IsActive", "SequenceOrder", "SlotEndTime", "SlotStartTime", "UpdatedAt")
    VALUES ('00000000-0000-0000-0000-000000000002', TIMESTAMPTZ '2026-06-27T17:30:46.132529Z', TRUE, 2, TIME '09:00:00', TIME '08:30:00', TIMESTAMPTZ '2026-06-27T17:30:46.132529Z');
    INSERT INTO "TimeSlots" ("Id", "CreatedAt", "IsActive", "SequenceOrder", "SlotEndTime", "SlotStartTime", "UpdatedAt")
    VALUES ('00000000-0000-0000-0000-000000000003', TIMESTAMPTZ '2026-06-27T17:30:46.13253Z', TRUE, 3, TIME '09:30:00', TIME '09:00:00', TIMESTAMPTZ '2026-06-27T17:30:46.13253Z');
    INSERT INTO "TimeSlots" ("Id", "CreatedAt", "IsActive", "SequenceOrder", "SlotEndTime", "SlotStartTime", "UpdatedAt")
    VALUES ('00000000-0000-0000-0000-000000000004', TIMESTAMPTZ '2026-06-27T17:30:46.132532Z', TRUE, 4, TIME '10:00:00', TIME '09:30:00', TIMESTAMPTZ '2026-06-27T17:30:46.132532Z');
    INSERT INTO "TimeSlots" ("Id", "CreatedAt", "IsActive", "SequenceOrder", "SlotEndTime", "SlotStartTime", "UpdatedAt")
    VALUES ('00000000-0000-0000-0000-000000000005', TIMESTAMPTZ '2026-06-27T17:30:46.132533Z', TRUE, 5, TIME '10:30:00', TIME '10:00:00', TIMESTAMPTZ '2026-06-27T17:30:46.132533Z');
    INSERT INTO "TimeSlots" ("Id", "CreatedAt", "IsActive", "SequenceOrder", "SlotEndTime", "SlotStartTime", "UpdatedAt")
    VALUES ('00000000-0000-0000-0000-000000000006', TIMESTAMPTZ '2026-06-27T17:30:46.132533Z', TRUE, 6, TIME '11:00:00', TIME '10:30:00', TIMESTAMPTZ '2026-06-27T17:30:46.132533Z');
    INSERT INTO "TimeSlots" ("Id", "CreatedAt", "IsActive", "SequenceOrder", "SlotEndTime", "SlotStartTime", "UpdatedAt")
    VALUES ('00000000-0000-0000-0000-000000000007', TIMESTAMPTZ '2026-06-27T17:30:46.132534Z', TRUE, 7, TIME '11:30:00', TIME '11:00:00', TIMESTAMPTZ '2026-06-27T17:30:46.132534Z');
    INSERT INTO "TimeSlots" ("Id", "CreatedAt", "IsActive", "SequenceOrder", "SlotEndTime", "SlotStartTime", "UpdatedAt")
    VALUES ('00000000-0000-0000-0000-000000000008', TIMESTAMPTZ '2026-06-27T17:30:46.132535Z', TRUE, 8, TIME '12:00:00', TIME '11:30:00', TIMESTAMPTZ '2026-06-27T17:30:46.132535Z');
    INSERT INTO "TimeSlots" ("Id", "CreatedAt", "IsActive", "SequenceOrder", "SlotEndTime", "SlotStartTime", "UpdatedAt")
    VALUES ('00000000-0000-0000-0000-000000000009', TIMESTAMPTZ '2026-06-27T17:30:46.132535Z', TRUE, 9, TIME '12:30:00', TIME '12:00:00', TIMESTAMPTZ '2026-06-27T17:30:46.132535Z');
    INSERT INTO "TimeSlots" ("Id", "CreatedAt", "IsActive", "SequenceOrder", "SlotEndTime", "SlotStartTime", "UpdatedAt")
    VALUES ('00000000-0000-0000-0000-000000000010', TIMESTAMPTZ '2026-06-27T17:30:46.132536Z', TRUE, 10, TIME '13:00:00', TIME '12:30:00', TIMESTAMPTZ '2026-06-27T17:30:46.132536Z');
    INSERT INTO "TimeSlots" ("Id", "CreatedAt", "IsActive", "SequenceOrder", "SlotEndTime", "SlotStartTime", "UpdatedAt")
    VALUES ('00000000-0000-0000-0000-000000000011', TIMESTAMPTZ '2026-06-27T17:30:46.132536Z', TRUE, 11, TIME '13:30:00', TIME '13:00:00', TIMESTAMPTZ '2026-06-27T17:30:46.132536Z');
    INSERT INTO "TimeSlots" ("Id", "CreatedAt", "IsActive", "SequenceOrder", "SlotEndTime", "SlotStartTime", "UpdatedAt")
    VALUES ('00000000-0000-0000-0000-000000000012', TIMESTAMPTZ '2026-06-27T17:30:46.132537Z', TRUE, 12, TIME '14:00:00', TIME '13:30:00', TIMESTAMPTZ '2026-06-27T17:30:46.132537Z');
    INSERT INTO "TimeSlots" ("Id", "CreatedAt", "IsActive", "SequenceOrder", "SlotEndTime", "SlotStartTime", "UpdatedAt")
    VALUES ('00000000-0000-0000-0000-000000000013', TIMESTAMPTZ '2026-06-27T17:30:46.132538Z', TRUE, 13, TIME '14:30:00', TIME '14:00:00', TIMESTAMPTZ '2026-06-27T17:30:46.132538Z');
    INSERT INTO "TimeSlots" ("Id", "CreatedAt", "IsActive", "SequenceOrder", "SlotEndTime", "SlotStartTime", "UpdatedAt")
    VALUES ('00000000-0000-0000-0000-000000000014', TIMESTAMPTZ '2026-06-27T17:30:46.132538Z', TRUE, 14, TIME '15:00:00', TIME '14:30:00', TIMESTAMPTZ '2026-06-27T17:30:46.132538Z');
    INSERT INTO "TimeSlots" ("Id", "CreatedAt", "IsActive", "SequenceOrder", "SlotEndTime", "SlotStartTime", "UpdatedAt")
    VALUES ('00000000-0000-0000-0000-000000000015', TIMESTAMPTZ '2026-06-27T17:30:46.132539Z', TRUE, 15, TIME '15:30:00', TIME '15:00:00', TIMESTAMPTZ '2026-06-27T17:30:46.132539Z');
    INSERT INTO "TimeSlots" ("Id", "CreatedAt", "IsActive", "SequenceOrder", "SlotEndTime", "SlotStartTime", "UpdatedAt")
    VALUES ('00000000-0000-0000-0000-000000000016', TIMESTAMPTZ '2026-06-27T17:30:46.13254Z', TRUE, 16, TIME '16:00:00', TIME '15:30:00', TIMESTAMPTZ '2026-06-27T17:30:46.13254Z');
    INSERT INTO "TimeSlots" ("Id", "CreatedAt", "IsActive", "SequenceOrder", "SlotEndTime", "SlotStartTime", "UpdatedAt")
    VALUES ('00000000-0000-0000-0000-000000000017', TIMESTAMPTZ '2026-06-27T17:30:46.13254Z', TRUE, 17, TIME '16:30:00', TIME '16:00:00', TIMESTAMPTZ '2026-06-27T17:30:46.13254Z');
    INSERT INTO "TimeSlots" ("Id", "CreatedAt", "IsActive", "SequenceOrder", "SlotEndTime", "SlotStartTime", "UpdatedAt")
    VALUES ('00000000-0000-0000-0000-000000000018', TIMESTAMPTZ '2026-06-27T17:30:46.132541Z', TRUE, 18, TIME '17:00:00', TIME '16:30:00', TIMESTAMPTZ '2026-06-27T17:30:46.132541Z');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    CREATE INDEX "IX_Appointments_CustomerId" ON "Appointments" ("CustomerId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    CREATE INDEX "IX_Appointments_DealershipId" ON "Appointments" ("DealershipId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    CREATE INDEX "IX_Appointments_StatusId" ON "Appointments" ("StatusId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    CREATE INDEX "IX_Appointments_VehicleId" ON "Appointments" ("VehicleId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_AppointmentStatusLookup_Status_Unique" ON "AppointmentStatusLookups" ("Status");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    CREATE INDEX "IX_BusinessHours_DealershipId" ON "BusinessHours" ("DealershipId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    CREATE INDEX "IX_ServiceBays_DealershipId" ON "ServiceBays" ("DealershipId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_Service_Unique_AppointmentServiceTypeSequence" ON "Services" ("AppointmentId", "ServiceTypeId", "SequenceOrder");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    CREATE INDEX "IX_Services_DealershipId" ON "Services" ("DealershipId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    CREATE INDEX "IX_Services_EstimatedEndTimeSlotId" ON "Services" ("EstimatedEndTimeSlotId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    CREATE INDEX "IX_Services_EstimatedStartTimeSlotId" ON "Services" ("EstimatedStartTimeSlotId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    CREATE INDEX "IX_Services_ServiceBayId" ON "Services" ("ServiceBayId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    CREATE INDEX "IX_Services_ServiceStatusId" ON "Services" ("ServiceStatusId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    CREATE INDEX "IX_Services_ServiceTypeId" ON "Services" ("ServiceTypeId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    CREATE INDEX "IX_Services_TechnicianId" ON "Services" ("TechnicianId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_ServiceStatusLookup_Status_Unique" ON "ServiceStatusLookups" ("Status");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    CREATE INDEX "IX_Technicians_DealershipId" ON "Technicians" ("DealershipId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    CREATE INDEX "IX_TechnicianSchedules_TechnicianId" ON "TechnicianSchedules" ("TechnicianId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_TechnicianSkill_Unique_TechnicianServiceType" ON "TechnicianSkills" ("TechnicianId", "ServiceTypeId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    CREATE INDEX "IX_TechnicianSkills_ServiceTypeId" ON "TechnicianSkills" ("ServiceTypeId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    CREATE UNIQUE INDEX "UK_TimeSlot_SequenceOrder" ON "TimeSlots" ("SequenceOrder");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    CREATE INDEX "IX_Vehicles_CustomerId" ON "Vehicles" ("CustomerId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627173046_InitialCreate') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260627173046_InitialCreate', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627190136_AddAvailabilityViews') THEN

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
        (occ."SequenceOrder" IS NULL) AS "IsAvailable"
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
       AND occ."SequenceOrder" = ts."SequenceOrder";

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
        (occ."SequenceOrder" IS NULL) AS "IsAvailable"
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
        TRUE AS "CanFitService"
    FROM required r
    INNER JOIN "TechnicianSkills" skill
        ON skill."ServiceTypeId" = r."ServiceTypeId"
    INNER JOIN "TechnicianAvailableSlots" tas
        ON tas."TechnicianId" = skill."TechnicianId"
       AND tas."IsAvailable" = TRUE
    INNER JOIN "ServiceBayAvailableSlots" sb_start
        ON sb_start."DealershipId" = tas."DealershipId"
       AND sb_start."QueryDate" = tas."QueryDate"
       AND sb_start."SequenceOrder" = tas."SequenceOrder"
       AND sb_start."IsAvailable" = TRUE
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

    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260627190136_AddAvailabilityViews') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260627190136_AddAvailabilityViews', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    ALTER TABLE "Vehicles" ADD "IsActive" boolean NOT NULL DEFAULT TRUE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    ALTER TABLE "TechnicianSkills" ADD "IsActive" boolean NOT NULL DEFAULT TRUE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    ALTER TABLE "TechnicianSchedules" ADD "IsActive" boolean NOT NULL DEFAULT TRUE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    ALTER TABLE "Technicians" ADD "IsActive" boolean NOT NULL DEFAULT TRUE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    ALTER TABLE "ServiceTypes" ADD "IsActive" boolean NOT NULL DEFAULT TRUE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    ALTER TABLE "ServiceStatusLookups" ADD "IsActive" boolean NOT NULL DEFAULT TRUE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    ALTER TABLE "Services" ADD "IsActive" boolean NOT NULL DEFAULT TRUE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    ALTER TABLE "Dealerships" ADD "IsActive" boolean NOT NULL DEFAULT TRUE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    ALTER TABLE "Customers" ADD "IsActive" boolean NOT NULL DEFAULT TRUE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    ALTER TABLE "BusinessHours" ADD "IsActive" boolean NOT NULL DEFAULT TRUE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    ALTER TABLE "AppointmentStatusLookups" ADD "IsActive" boolean NOT NULL DEFAULT TRUE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    ALTER TABLE "Appointments" ADD "IsActive" boolean NOT NULL DEFAULT TRUE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.130633Z', "IsActive" = TRUE, "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.130633Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.130634Z', "IsActive" = TRUE, "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.130634Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.130635Z', "IsActive" = TRUE, "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.130635Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.130635Z', "IsActive" = TRUE, "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.130635Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.130635Z', "IsActive" = TRUE, "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.130635Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.131046Z', "IsActive" = TRUE, "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.131046Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.131046Z', "IsActive" = TRUE, "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.131046Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.131046Z', "IsActive" = TRUE, "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.131046Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.131047Z', "IsActive" = TRUE, "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.131047Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.131047Z', "IsActive" = TRUE, "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.131047Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.134348Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.134348Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.13435Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.13435Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.13435Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.13435Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.134351Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.134351Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.134352Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.134352Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.134353Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.134353Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000006';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.134353Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.134353Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000007';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.134354Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.134354Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000008';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.134355Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.134355Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000009';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.134356Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.134356Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000010';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.134356Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.134356Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000011';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.134357Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.134357Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000012';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.134358Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.134358Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000013';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.134358Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.134358Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000014';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.134359Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.134359Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000015';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.134359Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.134359Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000016';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.13436Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.13436Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000017';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.134361Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:41:59.134361Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000018';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074159_AddIsActiveToBaseEntity') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260628074159_AddIsActiveToBaseEntity', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074752_AddServiceTypePrice') THEN
    ALTER TABLE "ServiceTypes" ADD "Price" numeric(10,2) NOT NULL DEFAULT 0.0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074752_AddServiceTypePrice') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.16327Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.16327Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074752_AddServiceTypePrice') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.16327Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.16327Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074752_AddServiceTypePrice') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.163271Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.163271Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074752_AddServiceTypePrice') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.163271Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.163271Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074752_AddServiceTypePrice') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.163272Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.163272Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074752_AddServiceTypePrice') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.163683Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.163683Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074752_AddServiceTypePrice') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.163683Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.163683Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074752_AddServiceTypePrice') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.163684Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.163684Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074752_AddServiceTypePrice') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.163684Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.163684Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074752_AddServiceTypePrice') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.163684Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.163684Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074752_AddServiceTypePrice') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.167267Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.167267Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074752_AddServiceTypePrice') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.16727Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.16727Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074752_AddServiceTypePrice') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.167271Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.167271Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074752_AddServiceTypePrice') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.167272Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.167272Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074752_AddServiceTypePrice') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.167272Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.167272Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074752_AddServiceTypePrice') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.167273Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.167273Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000006';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074752_AddServiceTypePrice') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.167274Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.167274Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000007';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074752_AddServiceTypePrice') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.167274Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.167274Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000008';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074752_AddServiceTypePrice') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.167275Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.167275Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000009';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074752_AddServiceTypePrice') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.167276Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.167276Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000010';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074752_AddServiceTypePrice') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.167276Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.167276Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000011';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074752_AddServiceTypePrice') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.167277Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.167277Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000012';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074752_AddServiceTypePrice') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.167277Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.167278Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000013';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074752_AddServiceTypePrice') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.167278Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.167278Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000014';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074752_AddServiceTypePrice') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.167279Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.167279Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000015';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074752_AddServiceTypePrice') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.167279Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.167279Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000016';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074752_AddServiceTypePrice') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.16728Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.16728Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000017';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074752_AddServiceTypePrice') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.167281Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:47:52.167281Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000018';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074752_AddServiceTypePrice') THEN
    ALTER TABLE "ServiceTypes" ADD CONSTRAINT "CK_ServiceType_Price_NonNegative" CHECK ("Price" >= 0);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628074752_AddServiceTypePrice') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260628074752_AddServiceTypePrice', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628075103_AddIsActiveToViews') THEN

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

    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628075103_AddIsActiveToViews') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.86168Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.86168Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628075103_AddIsActiveToViews') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.861681Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.861681Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628075103_AddIsActiveToViews') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.861682Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.861682Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628075103_AddIsActiveToViews') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.861682Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.861682Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628075103_AddIsActiveToViews') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.861682Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.861682Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628075103_AddIsActiveToViews') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.862114Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.862114Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628075103_AddIsActiveToViews') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.862114Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.862114Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628075103_AddIsActiveToViews') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.862115Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.862115Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628075103_AddIsActiveToViews') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.862115Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.862115Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628075103_AddIsActiveToViews') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.862115Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.862115Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628075103_AddIsActiveToViews') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865585Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865585Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628075103_AddIsActiveToViews') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865586Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865586Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628075103_AddIsActiveToViews') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865587Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865587Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628075103_AddIsActiveToViews') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865588Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865588Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628075103_AddIsActiveToViews') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865588Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865588Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628075103_AddIsActiveToViews') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865589Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865589Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000006';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628075103_AddIsActiveToViews') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.86559Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.86559Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000007';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628075103_AddIsActiveToViews') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.86559Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.86559Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000008';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628075103_AddIsActiveToViews') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865591Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865591Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000009';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628075103_AddIsActiveToViews') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865592Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865592Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000010';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628075103_AddIsActiveToViews') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865592Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865592Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000011';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628075103_AddIsActiveToViews') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865593Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865593Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000012';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628075103_AddIsActiveToViews') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865599Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865599Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000013';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628075103_AddIsActiveToViews') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.8656Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.8656Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000014';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628075103_AddIsActiveToViews') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.8656Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.8656Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000015';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628075103_AddIsActiveToViews') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865601Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865601Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000016';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628075103_AddIsActiveToViews') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865601Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865601Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000017';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628075103_AddIsActiveToViews') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865602Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T07:51:02.865602Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000018';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628075103_AddIsActiveToViews') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260628075103_AddIsActiveToViews', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    DROP INDEX "IX_Services_DealershipId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    DROP INDEX "IX_Services_ServiceBayId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    DROP INDEX "IX_Services_TechnicianId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    ALTER TABLE "Services" ADD "BookingDate" date;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    ALTER TABLE "Services" ADD "EstimatedEndSlotSequenceExclusive" integer;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    ALTER TABLE "Services" ADD "EstimatedStartSlotSequence" integer;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    CREATE EXTENSION IF NOT EXISTS btree_gist;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN

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
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN

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
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    ALTER TABLE "Services" ALTER COLUMN "BookingDate" SET NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    ALTER TABLE "Services" ALTER COLUMN "EstimatedStartSlotSequence" SET NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    ALTER TABLE "Services" ALTER COLUMN "EstimatedEndSlotSequenceExclusive" SET NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.756905Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.756905Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.756906Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.756906Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.756906Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.756906Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.756907Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.756907Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.756907Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.756907Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.757459Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.757459Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.75746Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.75746Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.757461Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.757461Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.757461Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.757461Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.757462Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.757462Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763222Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763222Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763223Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763223Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763224Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763224Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763225Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763225Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763226Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763226Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763227Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763227Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000006';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763228Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763228Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000007';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763231Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763231Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000008';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763232Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763232Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000009';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763233Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763233Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000010';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763234Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763234Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000011';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763235Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763235Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000012';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763236Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763236Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000013';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763237Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763237Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000014';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763238Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763238Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000015';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763239Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763239Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000016';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.76324Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.76324Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000017';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.76324Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T11:40:15.763241Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000018';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    CREATE INDEX "IX_Service_Dealership_BookingDate" ON "Services" ("DealershipId", "BookingDate");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    CREATE INDEX "IX_Service_ServiceBay_BookingDate" ON "Services" ("ServiceBayId", "BookingDate");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    CREATE INDEX "IX_Service_Technician_BookingDate" ON "Services" ("TechnicianId", "BookingDate");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN

                    ALTER TABLE "Services"
                    ADD CONSTRAINT "EXC_Service_Tech_Date_SeqRange_NoOverlap"
                    EXCLUDE USING gist (
                        "BookingDate" WITH =,
                        "TechnicianId" WITH =,
                        int4range("EstimatedStartSlotSequence", "EstimatedEndSlotSequenceExclusive", '[)') WITH &&
                    )
                    WHERE ("TechnicianId" IS NOT NULL AND "IsActive");
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN

                    ALTER TABLE "Services"
                    ADD CONSTRAINT "EXC_Service_Bay_Date_SeqRange_NoOverlap"
                    EXCLUDE USING gist (
                        "BookingDate" WITH =,
                        "ServiceBayId" WITH =,
                        int4range("EstimatedStartSlotSequence", "EstimatedEndSlotSequenceExclusive", '[)') WITH &&
                    )
                    WHERE ("ServiceBayId" IS NOT NULL AND "IsActive");
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628114016_AddServiceBookingConflictInvariant') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260628114016_AddServiceBookingConflictInvariant', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628131104_AddIdempotencyRequests') THEN
    CREATE TABLE "IdempotencyRequests" (
        "Id" uuid NOT NULL,
        "IdempotencyKey" character varying(100) NOT NULL,
        "RequestPath" character varying(200) NOT NULL,
        "RequestHash" character varying(128) NOT NULL,
        "Status" integer NOT NULL,
        "ResponseStatusCode" integer,
        "ResponseBody" text,
        "ExpiresAt" timestamp with time zone NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "IsActive" boolean NOT NULL DEFAULT TRUE,
        CONSTRAINT "PK_IdempotencyRequests" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628131104_AddIdempotencyRequests') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.465196Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.465196Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628131104_AddIdempotencyRequests') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.465197Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.465197Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628131104_AddIdempotencyRequests') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.465197Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.465197Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628131104_AddIdempotencyRequests') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.465198Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.465198Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628131104_AddIdempotencyRequests') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.465198Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.465198Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628131104_AddIdempotencyRequests') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.465653Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.465653Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628131104_AddIdempotencyRequests') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.465654Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.465654Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628131104_AddIdempotencyRequests') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.465654Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.465654Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628131104_AddIdempotencyRequests') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.465654Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.465654Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628131104_AddIdempotencyRequests') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.465655Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.465655Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628131104_AddIdempotencyRequests') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.469168Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.469168Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628131104_AddIdempotencyRequests') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.469169Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.469169Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628131104_AddIdempotencyRequests') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.469169Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.469169Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628131104_AddIdempotencyRequests') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.46917Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.46917Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628131104_AddIdempotencyRequests') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.469171Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.469171Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628131104_AddIdempotencyRequests') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.469171Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.469171Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000006';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628131104_AddIdempotencyRequests') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.469172Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.469172Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000007';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628131104_AddIdempotencyRequests') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.469173Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.469173Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000008';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628131104_AddIdempotencyRequests') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.469173Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.469173Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000009';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628131104_AddIdempotencyRequests') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.469174Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.469174Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000010';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628131104_AddIdempotencyRequests') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.469175Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.469175Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000011';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628131104_AddIdempotencyRequests') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.469175Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.469175Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000012';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628131104_AddIdempotencyRequests') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.469176Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.469176Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000013';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628131104_AddIdempotencyRequests') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.469176Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.469176Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000014';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628131104_AddIdempotencyRequests') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.469177Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.469177Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000015';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628131104_AddIdempotencyRequests') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.469178Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.469178Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000016';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628131104_AddIdempotencyRequests') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.469178Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.469178Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000017';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628131104_AddIdempotencyRequests') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.469179Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T13:11:04.469179Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000018';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628131104_AddIdempotencyRequests') THEN
    CREATE INDEX "IX_IdempotencyRequest_ExpiresAt" ON "IdempotencyRequests" ("ExpiresAt");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628131104_AddIdempotencyRequests') THEN
    CREATE UNIQUE INDEX "IX_IdempotencyRequest_Key_Path_Unique" ON "IdempotencyRequests" ("IdempotencyKey", "RequestPath");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628131104_AddIdempotencyRequests') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260628131104_AddIdempotencyRequests', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    CREATE TABLE "IdempotencyRequestStatusLookups" (
        "Id" uuid NOT NULL,
        "Status" integer NOT NULL,
        "Name" character varying(50) NOT NULL,
        "Description" character varying(200) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "IsActive" boolean NOT NULL DEFAULT TRUE,
        CONSTRAINT "PK_IdempotencyRequestStatusLookups" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    ALTER TABLE "IdempotencyRequests" ADD "StatusId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.309073Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.309073Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.309073Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.309073Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.309074Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.309074Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.309074Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.309074Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.309075Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.309075Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    INSERT INTO "IdempotencyRequestStatusLookups" ("Id", "CreatedAt", "Description", "IsActive", "Name", "Status", "UpdatedAt")
    VALUES ('00000000-0000-0000-0002-000000000001', TIMESTAMPTZ '2026-06-28T15:45:05.309491Z', 'Request processing started and not yet completed', TRUE, 'In Progress', 1, TIMESTAMPTZ '2026-06-28T15:45:05.309491Z');
    INSERT INTO "IdempotencyRequestStatusLookups" ("Id", "CreatedAt", "Description", "IsActive", "Name", "Status", "UpdatedAt")
    VALUES ('00000000-0000-0000-0002-000000000002', TIMESTAMPTZ '2026-06-28T15:45:05.309492Z', 'Request completed and response persisted for replay', TRUE, 'Completed', 2, TIMESTAMPTZ '2026-06-28T15:45:05.309492Z');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN

                    UPDATE "IdempotencyRequests"
                    SET "StatusId" = CASE "Status"
                        WHEN 2 THEN '00000000-0000-0000-0002-000000000002'::uuid
                        ELSE '00000000-0000-0000-0002-000000000001'::uuid
                    END;
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN

                    DO $$
                    BEGIN
                        IF EXISTS (SELECT 1 FROM "IdempotencyRequests" WHERE "StatusId" IS NULL) THEN
                            RAISE EXCEPTION 'IdempotencyRequests StatusId backfill failed';
                        END IF;
                    END
                    $$;
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    ALTER TABLE "IdempotencyRequests" ALTER COLUMN "StatusId" SET NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.309661Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.309661Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.309663Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.309663Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.309663Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.309663Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.309663Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.309663Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.309664Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.309664Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.314314Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.314315Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.314316Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.314316Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.314316Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.314316Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.314317Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.314317Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.314318Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.314318Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.314318Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.314318Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000006';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.314319Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.314319Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000007';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.314319Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.314319Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000008';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.31432Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.31432Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000009';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.314321Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.314321Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000010';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.314321Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.314322Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000011';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.314322Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.314322Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000012';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.314323Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.314323Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000013';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.314323Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.314323Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000014';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.314324Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.314324Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000015';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.314325Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.314325Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000016';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.314325Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.314325Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000017';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.314326Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T15:45:05.314326Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000018';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    CREATE INDEX "IX_IdempotencyRequests_StatusId" ON "IdempotencyRequests" ("StatusId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    CREATE UNIQUE INDEX "IX_IdempotencyRequestStatusLookup_Status_Unique" ON "IdempotencyRequestStatusLookups" ("Status");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    ALTER TABLE "IdempotencyRequests" ADD CONSTRAINT "FK_IdempotencyRequest_StatusLookup" FOREIGN KEY ("StatusId") REFERENCES "IdempotencyRequestStatusLookups" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    ALTER TABLE "IdempotencyRequests" DROP COLUMN "Status";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628154505_ConvertIdempotencyStatusToLookup') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260628154505_ConvertIdempotencyStatusToLookup', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628165905_SyncDbContextChanges') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.031864Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.031864Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628165905_SyncDbContextChanges') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.031865Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.031865Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628165905_SyncDbContextChanges') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.031865Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.031865Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628165905_SyncDbContextChanges') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.031865Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.031865Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628165905_SyncDbContextChanges') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.031866Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.031866Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628165905_SyncDbContextChanges') THEN
    UPDATE "IdempotencyRequestStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.032318Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.032318Z'
    WHERE "Id" = '00000000-0000-0000-0002-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628165905_SyncDbContextChanges') THEN
    UPDATE "IdempotencyRequestStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.032319Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.032319Z'
    WHERE "Id" = '00000000-0000-0000-0002-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628165905_SyncDbContextChanges') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.032494Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.032494Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628165905_SyncDbContextChanges') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.032495Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.032495Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628165905_SyncDbContextChanges') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.032496Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.032496Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628165905_SyncDbContextChanges') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.032496Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.032496Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628165905_SyncDbContextChanges') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.032497Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.032497Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628165905_SyncDbContextChanges') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.036231Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.036231Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628165905_SyncDbContextChanges') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.036232Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.036232Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628165905_SyncDbContextChanges') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.036232Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.036232Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628165905_SyncDbContextChanges') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.036233Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.036233Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628165905_SyncDbContextChanges') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.036234Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.036234Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628165905_SyncDbContextChanges') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.036234Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.036234Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000006';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628165905_SyncDbContextChanges') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.036235Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.036235Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000007';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628165905_SyncDbContextChanges') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.036236Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.036236Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000008';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628165905_SyncDbContextChanges') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.036236Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.036236Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000009';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628165905_SyncDbContextChanges') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.036237Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.036237Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000010';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628165905_SyncDbContextChanges') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.036238Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.036238Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000011';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628165905_SyncDbContextChanges') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.036238Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.036238Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000012';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628165905_SyncDbContextChanges') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.036239Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.036239Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000013';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628165905_SyncDbContextChanges') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.036239Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.036239Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000014';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628165905_SyncDbContextChanges') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.036242Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.036242Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000015';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628165905_SyncDbContextChanges') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.036243Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.036243Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000016';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628165905_SyncDbContextChanges') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.036243Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.036243Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000017';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628165905_SyncDbContextChanges') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.036244Z', "UpdatedAt" = TIMESTAMPTZ '2026-06-28T16:59:05.036244Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000018';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260628165905_SyncDbContextChanges') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260628165905_SyncDbContextChanges', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717025212_AddCustomerAuthUserId') THEN
    ALTER TABLE "Customers" ADD "AuthUserId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717025212_AddCustomerAuthUserId') THEN
    UPDATE "Customers" SET "AuthUserId" = "Id" WHERE "AuthUserId" IS NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717025212_AddCustomerAuthUserId') THEN
    ALTER TABLE "Customers" ALTER COLUMN "AuthUserId" SET NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717025212_AddCustomerAuthUserId') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.847117Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.847117Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717025212_AddCustomerAuthUserId') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.847118Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.847118Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717025212_AddCustomerAuthUserId') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.847118Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.847118Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717025212_AddCustomerAuthUserId') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.847118Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.847118Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717025212_AddCustomerAuthUserId') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.847119Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.847119Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717025212_AddCustomerAuthUserId') THEN
    UPDATE "IdempotencyRequestStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.847506Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.847506Z'
    WHERE "Id" = '00000000-0000-0000-0002-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717025212_AddCustomerAuthUserId') THEN
    UPDATE "IdempotencyRequestStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.847507Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.847507Z'
    WHERE "Id" = '00000000-0000-0000-0002-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717025212_AddCustomerAuthUserId') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.847669Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.847669Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717025212_AddCustomerAuthUserId') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.847669Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.847669Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717025212_AddCustomerAuthUserId') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.84767Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.84767Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717025212_AddCustomerAuthUserId') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.84767Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.84767Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717025212_AddCustomerAuthUserId') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.84767Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.84767Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717025212_AddCustomerAuthUserId') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.850962Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.850962Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717025212_AddCustomerAuthUserId') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.850963Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.850963Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717025212_AddCustomerAuthUserId') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.850964Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.850964Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717025212_AddCustomerAuthUserId') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.850965Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.850965Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717025212_AddCustomerAuthUserId') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.850965Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.850965Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717025212_AddCustomerAuthUserId') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.850966Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.850966Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000006';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717025212_AddCustomerAuthUserId') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.850967Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.850967Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000007';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717025212_AddCustomerAuthUserId') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.850967Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.850967Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000008';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717025212_AddCustomerAuthUserId') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.850968Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.850968Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000009';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717025212_AddCustomerAuthUserId') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.850968Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.850968Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000010';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717025212_AddCustomerAuthUserId') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.850969Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.850969Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000011';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717025212_AddCustomerAuthUserId') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.85097Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.85097Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000012';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717025212_AddCustomerAuthUserId') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.85097Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.85097Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000013';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717025212_AddCustomerAuthUserId') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.850971Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.850971Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000014';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717025212_AddCustomerAuthUserId') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.850971Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.850971Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000015';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717025212_AddCustomerAuthUserId') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.850972Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.850972Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000016';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717025212_AddCustomerAuthUserId') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.850972Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.850972Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000017';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717025212_AddCustomerAuthUserId') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.850973Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-17T02:52:11.850973Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000018';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717025212_AddCustomerAuthUserId') THEN
    CREATE UNIQUE INDEX "UX_Customers_AuthUserId" ON "Customers" ("AuthUserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717025212_AddCustomerAuthUserId') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260717025212_AddCustomerAuthUserId', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    CREATE TABLE "CurrencyLookups" (
        "Id" uuid NOT NULL,
        "Code" character varying(3) NOT NULL,
        "Name" character varying(50) NOT NULL,
        "Symbol" character varying(8) NOT NULL,
        "DecimalPlaces" integer NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "IsActive" boolean NOT NULL DEFAULT TRUE,
        CONSTRAINT "PK_CurrencyLookups" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    CREATE TABLE "Orders" (
        "Id" uuid NOT NULL,
        "OrderCode" character varying(50) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "IsActive" boolean NOT NULL DEFAULT TRUE,
        CONSTRAINT "PK_Orders" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    CREATE TABLE "PaymentProviderLookups" (
        "Id" uuid NOT NULL,
        "Provider" integer NOT NULL,
        "Name" character varying(50) NOT NULL,
        "Description" character varying(200) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "IsActive" boolean NOT NULL DEFAULT TRUE,
        CONSTRAINT "PK_PaymentProviderLookups" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    CREATE TABLE "PaymentTransactionStatusLookups" (
        "Id" uuid NOT NULL,
        "Status" integer NOT NULL,
        "Name" character varying(50) NOT NULL,
        "Description" character varying(200) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "IsActive" boolean NOT NULL DEFAULT TRUE,
        CONSTRAINT "PK_PaymentTransactionStatusLookups" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    CREATE TABLE "ServiceTypePrices" (
        "Id" uuid NOT NULL,
        "ServiceTypeId" uuid NOT NULL,
        "CurrencyId" uuid NOT NULL,
        "Price" numeric(10,2) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "IsActive" boolean NOT NULL DEFAULT TRUE,
        CONSTRAINT "PK_ServiceTypePrices" PRIMARY KEY ("Id"),
        CONSTRAINT "CK_ServiceTypePrice_Price_NonNegative" CHECK ("Price" >= 0),
        CONSTRAINT "FK_ServiceTypePrice_Currency" FOREIGN KEY ("CurrencyId") REFERENCES "CurrencyLookups" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_ServiceTypePrice_ServiceType" FOREIGN KEY ("ServiceTypeId") REFERENCES "ServiceTypes" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    CREATE TABLE "OrderAppointments" (
        "Id" uuid NOT NULL,
        "OrderId" uuid NOT NULL,
        "AppointmentId" uuid NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "IsActive" boolean NOT NULL DEFAULT TRUE,
        CONSTRAINT "PK_OrderAppointments" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_OrderAppointment_Appointment" FOREIGN KEY ("AppointmentId") REFERENCES "Appointments" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_OrderAppointment_Order" FOREIGN KEY ("OrderId") REFERENCES "Orders" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    CREATE TABLE "ServiceTypeOrders" (
        "Id" uuid NOT NULL,
        "OrderId" uuid NOT NULL,
        "ServiceTypeId" uuid NOT NULL,
        "Quantity" integer NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "IsActive" boolean NOT NULL DEFAULT TRUE,
        CONSTRAINT "PK_ServiceTypeOrders" PRIMARY KEY ("Id"),
        CONSTRAINT "CK_ServiceTypeOrder_Quantity_Positive" CHECK ("Quantity" > 0),
        CONSTRAINT "FK_ServiceTypeOrder_Order" FOREIGN KEY ("OrderId") REFERENCES "Orders" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_ServiceTypeOrder_ServiceType" FOREIGN KEY ("ServiceTypeId") REFERENCES "ServiceTypes" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    CREATE TABLE "PaymentTransactions" (
        "Id" uuid NOT NULL,
        "TransactionCode" character varying(50) NOT NULL,
        "FromAccount" character varying(120) NOT NULL,
        "ToAccount" character varying(120) NOT NULL,
        "Direction" integer NOT NULL,
        "Amount" numeric(18,2) NOT NULL,
        "CurrencyId" uuid NOT NULL,
        "StatusId" uuid NOT NULL,
        "TransactionAtUtc" timestamp with time zone NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "IsActive" boolean NOT NULL DEFAULT TRUE,
        CONSTRAINT "PK_PaymentTransactions" PRIMARY KEY ("Id"),
        CONSTRAINT "CK_PaymentTransaction_Amount_NonNegative" CHECK ("Amount" >= 0),
        CONSTRAINT "FK_PaymentTransaction_Currency" FOREIGN KEY ("CurrencyId") REFERENCES "CurrencyLookups" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_PaymentTransaction_StatusLookup" FOREIGN KEY ("StatusId") REFERENCES "PaymentTransactionStatusLookups" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    CREATE TABLE "PaymentOrders" (
        "Id" uuid NOT NULL,
        "OrderId" uuid NOT NULL,
        "TransactionId" uuid NOT NULL,
        "OrderedAtUtc" timestamp with time zone NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
        "IsActive" boolean NOT NULL DEFAULT TRUE,
        CONSTRAINT "PK_PaymentOrders" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_PaymentOrder_Order" FOREIGN KEY ("OrderId") REFERENCES "Orders" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_PaymentOrder_PaymentTransaction" FOREIGN KEY ("TransactionId") REFERENCES "PaymentTransactions" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.325978Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.325978Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.325979Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.325979Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.325979Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.325979Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.325979Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.325979Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.32598Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.32598Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    INSERT INTO "CurrencyLookups" ("Id", "Code", "CreatedAt", "DecimalPlaces", "IsActive", "Name", "Symbol", "UpdatedAt")
    VALUES ('00000000-0000-0000-0004-000000000001', 'VND', TIMESTAMPTZ '2026-07-18T06:01:48.327501Z', 0, TRUE, 'Vietnamese Dong', 'VND', TIMESTAMPTZ '2026-07-18T06:01:48.327501Z');
    INSERT INTO "CurrencyLookups" ("Id", "Code", "CreatedAt", "DecimalPlaces", "IsActive", "Name", "Symbol", "UpdatedAt")
    VALUES ('00000000-0000-0000-0004-000000000002', 'USD', TIMESTAMPTZ '2026-07-18T06:01:48.327501Z', 2, TRUE, 'US Dollar', 'USD', TIMESTAMPTZ '2026-07-18T06:01:48.327502Z');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    UPDATE "IdempotencyRequestStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.326384Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.326384Z'
    WHERE "Id" = '00000000-0000-0000-0002-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    UPDATE "IdempotencyRequestStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.326384Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.326384Z'
    WHERE "Id" = '00000000-0000-0000-0002-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    INSERT INTO "PaymentProviderLookups" ("Id", "CreatedAt", "Description", "IsActive", "Name", "Provider", "UpdatedAt")
    VALUES ('00000000-0000-0000-0003-000000000001', TIMESTAMPTZ '2026-07-18T06:01:48.327903Z', 'Zalo Pay e-wallet and gateway', TRUE, 'Zalo Pay', 1, TIMESTAMPTZ '2026-07-18T06:01:48.327903Z');
    INSERT INTO "PaymentProviderLookups" ("Id", "CreatedAt", "Description", "IsActive", "Name", "Provider", "UpdatedAt")
    VALUES ('00000000-0000-0000-0003-000000000002', TIMESTAMPTZ '2026-07-18T06:01:48.327904Z', 'Momo wallet payments', TRUE, 'Momo', 2, TIMESTAMPTZ '2026-07-18T06:01:48.327904Z');
    INSERT INTO "PaymentProviderLookups" ("Id", "CreatedAt", "Description", "IsActive", "Name", "Provider", "UpdatedAt")
    VALUES ('00000000-0000-0000-0003-000000000003', TIMESTAMPTZ '2026-07-18T06:01:48.327904Z', 'Apple Pay card tokenization gateway', TRUE, 'Apple Pay', 3, TIMESTAMPTZ '2026-07-18T06:01:48.327904Z');
    INSERT INTO "PaymentProviderLookups" ("Id", "CreatedAt", "Description", "IsActive", "Name", "Provider", "UpdatedAt")
    VALUES ('00000000-0000-0000-0003-000000000004', TIMESTAMPTZ '2026-07-18T06:01:48.327906Z', 'Visa card network', TRUE, 'Visa', 4, TIMESTAMPTZ '2026-07-18T06:01:48.327906Z');
    INSERT INTO "PaymentProviderLookups" ("Id", "CreatedAt", "Description", "IsActive", "Name", "Provider", "UpdatedAt")
    VALUES ('00000000-0000-0000-0003-000000000005', TIMESTAMPTZ '2026-07-18T06:01:48.327906Z', 'MasterCard card network', TRUE, 'Master Card', 5, TIMESTAMPTZ '2026-07-18T06:01:48.327906Z');
    INSERT INTO "PaymentProviderLookups" ("Id", "CreatedAt", "Description", "IsActive", "Name", "Provider", "UpdatedAt")
    VALUES ('00000000-0000-0000-0003-000000000006', TIMESTAMPTZ '2026-07-18T06:01:48.327907Z', 'VNPay online payment gateway', TRUE, 'VNPay', 6, TIMESTAMPTZ '2026-07-18T06:01:48.327907Z');
    INSERT INTO "PaymentProviderLookups" ("Id", "CreatedAt", "Description", "IsActive", "Name", "Provider", "UpdatedAt")
    VALUES ('00000000-0000-0000-0003-000000000007', TIMESTAMPTZ '2026-07-18T06:01:48.327907Z', 'Domestic ATM transfer', TRUE, 'ATM Transfer', 7, TIMESTAMPTZ '2026-07-18T06:01:48.327907Z');
    INSERT INTO "PaymentProviderLookups" ("Id", "CreatedAt", "Description", "IsActive", "Name", "Provider", "UpdatedAt")
    VALUES ('00000000-0000-0000-0003-000000000008', TIMESTAMPTZ '2026-07-18T06:01:48.327908Z', 'Internal system wallet balance', TRUE, 'Internal Wallet', 8, TIMESTAMPTZ '2026-07-18T06:01:48.327908Z');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    INSERT INTO "PaymentTransactionStatusLookups" ("Id", "CreatedAt", "Description", "IsActive", "Name", "Status", "UpdatedAt")
    VALUES ('00000000-0000-0000-0005-000000000001', TIMESTAMPTZ '2026-07-18T06:01:48.327943Z', 'Transaction is created and waiting for processing', TRUE, 'Pending', 1, TIMESTAMPTZ '2026-07-18T06:01:48.327943Z');
    INSERT INTO "PaymentTransactionStatusLookups" ("Id", "CreatedAt", "Description", "IsActive", "Name", "Status", "UpdatedAt")
    VALUES ('00000000-0000-0000-0005-000000000002', TIMESTAMPTZ '2026-07-18T06:01:48.327943Z', 'Transaction is being processed', TRUE, 'In Progress', 2, TIMESTAMPTZ '2026-07-18T06:01:48.327944Z');
    INSERT INTO "PaymentTransactionStatusLookups" ("Id", "CreatedAt", "Description", "IsActive", "Name", "Status", "UpdatedAt")
    VALUES ('00000000-0000-0000-0005-000000000003', TIMESTAMPTZ '2026-07-18T06:01:48.327944Z', 'Transaction has been completed successfully', TRUE, 'Completed', 3, TIMESTAMPTZ '2026-07-18T06:01:48.327944Z');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.326558Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.326558Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.326559Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.326559Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.326559Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.326559Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.326559Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.326559Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.32656Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.32656Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331066Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331066Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331067Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331067Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331068Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331068Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331069Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331069Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331069Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331069Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.33107Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.33107Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000006';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.33107Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.33107Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000007';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331071Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331071Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000008';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331072Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331072Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000009';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331072Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331072Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000010';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331073Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331073Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000011';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331074Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331074Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000012';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331074Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331074Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000013';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331075Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331075Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000014';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331076Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331076Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000015';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331076Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331076Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000016';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331077Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331077Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000017';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331077Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331077Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000018';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    CREATE UNIQUE INDEX "IX_CurrencyLookup_Code_Unique" ON "CurrencyLookups" ("Code");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    CREATE UNIQUE INDEX "IX_OrderAppointment_Appointment_Unique" ON "OrderAppointments" ("AppointmentId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    CREATE UNIQUE INDEX "IX_OrderAppointment_Unique_Order_Appointment" ON "OrderAppointments" ("OrderId", "AppointmentId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    CREATE UNIQUE INDEX "IX_Order_OrderCode_Unique" ON "Orders" ("OrderCode");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    CREATE UNIQUE INDEX "IX_PaymentOrder_OrderId" ON "PaymentOrders" ("OrderId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    CREATE UNIQUE INDEX "IX_PaymentOrder_TransactionId_Unique" ON "PaymentOrders" ("TransactionId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    CREATE UNIQUE INDEX "IX_PaymentProviderLookup_Provider_Unique" ON "PaymentProviderLookups" ("Provider");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    CREATE UNIQUE INDEX "IX_PaymentTransaction_TransactionCode_Unique" ON "PaymentTransactions" ("TransactionCode");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    CREATE INDEX "IX_PaymentTransactions_CurrencyId" ON "PaymentTransactions" ("CurrencyId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    CREATE INDEX "IX_PaymentTransactions_StatusId" ON "PaymentTransactions" ("StatusId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    CREATE UNIQUE INDEX "IX_PaymentTransactionStatusLookup_Status_Unique" ON "PaymentTransactionStatusLookups" ("Status");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    CREATE UNIQUE INDEX "IX_ServiceTypeOrder_Unique_Order_ServiceType" ON "ServiceTypeOrders" ("OrderId", "ServiceTypeId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    CREATE INDEX "IX_ServiceTypeOrders_ServiceTypeId" ON "ServiceTypeOrders" ("ServiceTypeId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    CREATE UNIQUE INDEX "IX_ServiceTypePrice_Unique_ServiceType_Currency" ON "ServiceTypePrices" ("ServiceTypeId", "CurrencyId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    CREATE INDEX "IX_ServiceTypePrices_CurrencyId" ON "ServiceTypePrices" ("CurrencyId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718060148_AddPaymentOrderAndTransactionSimplification') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260718060148_AddPaymentOrderAndTransactionSimplification', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    ALTER TABLE "PaymentOrders" RENAME COLUMN "TransactionId" TO "PaymentTransactionId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    ALTER TABLE "ServiceTypeOrders" ADD "LineTotal" numeric(18,2) NOT NULL DEFAULT 0.0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    ALTER TABLE "ServiceTypeOrders" ADD "UnitPrice" numeric(18,2) NOT NULL DEFAULT 0.0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    ALTER TABLE "Orders" ADD "CurrencyId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    ALTER TABLE "Orders" ADD "TotalAmount" numeric(18,2) NOT NULL DEFAULT 0.0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.107514Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.107514Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.107514Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.107514Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.107515Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.107515Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.107515Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.107515Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.107515Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.107515Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "CurrencyLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.11071Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.11071Z'
    WHERE "Id" = '00000000-0000-0000-0004-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "CurrencyLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.11071Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.11071Z'
    WHERE "Id" = '00000000-0000-0000-0004-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "IdempotencyRequestStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.108003Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.108003Z'
    WHERE "Id" = '00000000-0000-0000-0002-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "IdempotencyRequestStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.108003Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.108003Z'
    WHERE "Id" = '00000000-0000-0000-0002-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "PaymentProviderLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.111273Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.111273Z'
    WHERE "Id" = '00000000-0000-0000-0003-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "PaymentProviderLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.111274Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.111274Z'
    WHERE "Id" = '00000000-0000-0000-0003-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "PaymentProviderLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.111274Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.111275Z'
    WHERE "Id" = '00000000-0000-0000-0003-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "PaymentProviderLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.111276Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.111276Z'
    WHERE "Id" = '00000000-0000-0000-0003-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "PaymentProviderLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.111276Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.111276Z'
    WHERE "Id" = '00000000-0000-0000-0003-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "PaymentProviderLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.111277Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.111277Z'
    WHERE "Id" = '00000000-0000-0000-0003-000000000006';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "PaymentProviderLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.111277Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.111277Z'
    WHERE "Id" = '00000000-0000-0000-0003-000000000007';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "PaymentProviderLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.111278Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.111278Z'
    WHERE "Id" = '00000000-0000-0000-0003-000000000008';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "PaymentTransactionStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.111321Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.111321Z'
    WHERE "Id" = '00000000-0000-0000-0005-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "PaymentTransactionStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.111322Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.111322Z'
    WHERE "Id" = '00000000-0000-0000-0005-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "PaymentTransactionStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.111323Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.111323Z'
    WHERE "Id" = '00000000-0000-0000-0005-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.108193Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.108193Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.108194Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.108194Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.108194Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.108194Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.108194Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.108194Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.108195Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.108195Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.114755Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.114755Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.114756Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.114756Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.114757Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.114757Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.114758Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.114758Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.114758Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.114758Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.114759Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.114759Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000006';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.11476Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.11476Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000007';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.11476Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.11476Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000008';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.114761Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.114761Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000009';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.114762Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.114762Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000010';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.114762Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.114762Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000011';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.114763Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.114763Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000012';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.114764Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.114764Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000013';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.114764Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.114764Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000014';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.114765Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.114765Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000015';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.114765Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.114765Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000016';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.114766Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.114766Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000017';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.114767Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:52:34.114767Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000018';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    ALTER TABLE "ServiceTypeOrders" ADD CONSTRAINT "CK_ServiceTypeOrder_LineTotal_NonNegative" CHECK ("LineTotal" >= 0);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    ALTER TABLE "ServiceTypeOrders" ADD CONSTRAINT "CK_ServiceTypeOrder_UnitPrice_NonNegative" CHECK ("UnitPrice" >= 0);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    CREATE INDEX "IX_Orders_CurrencyId" ON "Orders" ("CurrencyId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    ALTER TABLE "Orders" ADD CONSTRAINT "CK_Order_TotalAmount_NonNegative" CHECK ("TotalAmount" >= 0);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    ALTER TABLE "Orders" ADD CONSTRAINT "FK_Order_Currency" FOREIGN KEY ("CurrencyId") REFERENCES "CurrencyLookups" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105234_AddOrderCurrencyAndSnapshotPricing') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260718105234_AddOrderCurrencyAndSnapshotPricing', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    ALTER TABLE "ServiceTypes" DROP CONSTRAINT "CK_ServiceType_Price_NonNegative";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    ALTER TABLE "ServiceTypes" DROP COLUMN "Price";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.894755Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.894755Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.894755Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.894755Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.894756Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.894756Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.894756Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.894756Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.894757Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.894757Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "IdempotencyRequestStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.895326Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.895326Z'
    WHERE "Id" = '00000000-0000-0000-0002-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "IdempotencyRequestStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.895327Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.895327Z'
    WHERE "Id" = '00000000-0000-0000-0002-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "PaymentProviderLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.897074Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.897074Z'
    WHERE "Id" = '00000000-0000-0000-0003-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "PaymentProviderLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.897075Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.897075Z'
    WHERE "Id" = '00000000-0000-0000-0003-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "PaymentProviderLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.897075Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.897075Z'
    WHERE "Id" = '00000000-0000-0000-0003-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "PaymentProviderLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.897075Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.897075Z'
    WHERE "Id" = '00000000-0000-0000-0003-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "PaymentProviderLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.897076Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.897076Z'
    WHERE "Id" = '00000000-0000-0000-0003-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "PaymentProviderLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.897076Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.897076Z'
    WHERE "Id" = '00000000-0000-0000-0003-000000000006';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "PaymentProviderLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.897077Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.897077Z'
    WHERE "Id" = '00000000-0000-0000-0003-000000000007';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "PaymentProviderLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.897077Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.897077Z'
    WHERE "Id" = '00000000-0000-0000-0003-000000000008';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "PaymentTransactionStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.897123Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.897123Z'
    WHERE "Id" = '00000000-0000-0000-0005-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "PaymentTransactionStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.897124Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.897124Z'
    WHERE "Id" = '00000000-0000-0000-0005-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "PaymentTransactionStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.897124Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.897124Z'
    WHERE "Id" = '00000000-0000-0000-0005-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.895519Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.895519Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.89552Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.89552Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.89552Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.89552Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.89552Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.89552Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.895521Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.895521Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    INSERT INTO "ServiceTypes" ("Id", "CreatedAt", "DurationMinutes", "IsActive", "Name", "UpdatedAt")
    VALUES
      ('11111111-1111-1111-1111-030000000001', TIMESTAMPTZ '2026-07-18T10:52:34.1107100Z', 60, TRUE, 'Oil Change', TIMESTAMPTZ '2026-07-18T10:52:34.1107100Z'),
      ('11111111-1111-1111-1111-030000000002', TIMESTAMPTZ '2026-07-18T10:52:34.1107100Z', 90, TRUE, 'Brake Inspection', TIMESTAMPTZ '2026-07-18T10:52:34.1107100Z'),
      ('11111111-1111-1111-1111-030000000003', TIMESTAMPTZ '2026-07-18T10:52:34.1107100Z', 240, TRUE, 'Major Service', TIMESTAMPTZ '2026-07-18T10:52:34.1107100Z')
    ON CONFLICT ("Id") DO NOTHING;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.900487Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.900487Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.900488Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.900488Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.900488Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.900488Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.900489Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.900489Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.90049Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.90049Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.90049Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.90049Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000006';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.900491Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.900491Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000007';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.900492Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.900492Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000008';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.900492Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.900492Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000009';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.900493Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.900493Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000010';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.900494Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.900494Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000011';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.900494Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.900494Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000012';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.900495Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.900495Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000013';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.900496Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.900496Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000014';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.900498Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.900498Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000015';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.900499Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.900499Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000016';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.900499Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.900499Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000017';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000018';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    INSERT INTO "ServiceTypePrices" ("Id", "CreatedAt", "CurrencyId", "IsActive", "Price", "ServiceTypeId", "UpdatedAt")
    VALUES
      ('11111111-1111-1111-1111-031000000001', TIMESTAMPTZ '2026-07-18T10:52:34.1107100Z', '00000000-0000-0000-0004-000000000001', TRUE, 500000, '11111111-1111-1111-1111-030000000001', TIMESTAMPTZ '2026-07-18T10:52:34.1107100Z'),
      ('11111111-1111-1111-1111-031000000002', TIMESTAMPTZ '2026-07-18T10:52:34.1107100Z', '00000000-0000-0000-0004-000000000001', TRUE, 750000, '11111111-1111-1111-1111-030000000002', TIMESTAMPTZ '2026-07-18T10:52:34.1107100Z'),
      ('11111111-1111-1111-1111-031000000003', TIMESTAMPTZ '2026-07-18T10:52:34.1107100Z', '00000000-0000-0000-0004-000000000001', TRUE, 2200000, '11111111-1111-1111-1111-030000000003', TIMESTAMPTZ '2026-07-18T10:52:34.1107100Z'),
      ('11111111-1111-1111-1111-031000000004', TIMESTAMPTZ '2026-07-18T10:52:34.1107100Z', '00000000-0000-0000-0004-000000000002', TRUE, 19.99, '11111111-1111-1111-1111-030000000001', TIMESTAMPTZ '2026-07-18T10:52:34.1107100Z'),
      ('11111111-1111-1111-1111-031000000005', TIMESTAMPTZ '2026-07-18T10:52:34.1107100Z', '00000000-0000-0000-0004-000000000002', TRUE, 29.99, '11111111-1111-1111-1111-030000000002', TIMESTAMPTZ '2026-07-18T10:52:34.1107100Z'),
      ('11111111-1111-1111-1111-031000000006', TIMESTAMPTZ '2026-07-18T10:52:34.1107100Z', '00000000-0000-0000-0004-000000000002', TRUE, 89.99, '11111111-1111-1111-1111-030000000003', TIMESTAMPTZ '2026-07-18T10:52:34.1107100Z')
    ON CONFLICT ("Id") DO NOTHING;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260718105933_RemoveServiceTypeBasePriceAndSeedServiceTypePrices', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T16:39:02.745146Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T16:39:02.745146Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T16:39:02.745147Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T16:39:02.745147Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T16:39:02.745148Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T16:39:02.745148Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T16:39:02.745148Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T16:39:02.745148Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T16:39:02.745151Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T16:39:02.745151Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "IdempotencyRequestStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T16:39:02.745751Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T16:39:02.745751Z'
    WHERE "Id" = '00000000-0000-0000-0002-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "IdempotencyRequestStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T16:39:02.745752Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T16:39:02.745752Z'
    WHERE "Id" = '00000000-0000-0000-0002-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "PaymentProviderLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z'
    WHERE "Id" = '00000000-0000-0000-0003-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "PaymentProviderLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z'
    WHERE "Id" = '00000000-0000-0000-0003-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "PaymentProviderLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z'
    WHERE "Id" = '00000000-0000-0000-0003-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "PaymentProviderLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z'
    WHERE "Id" = '00000000-0000-0000-0003-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "PaymentProviderLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z'
    WHERE "Id" = '00000000-0000-0000-0003-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "PaymentProviderLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z'
    WHERE "Id" = '00000000-0000-0000-0003-000000000006';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "PaymentProviderLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z'
    WHERE "Id" = '00000000-0000-0000-0003-000000000007';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "PaymentProviderLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z'
    WHERE "Id" = '00000000-0000-0000-0003-000000000008';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "PaymentTransactionStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z'
    WHERE "Id" = '00000000-0000-0000-0005-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "PaymentTransactionStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z'
    WHERE "Id" = '00000000-0000-0000-0005-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "PaymentTransactionStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z'
    WHERE "Id" = '00000000-0000-0000-0005-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T16:39:02.746062Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T16:39:02.746062Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T16:39:02.746063Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T16:39:02.746063Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T16:39:02.746063Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T16:39:02.746063Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T16:39:02.746064Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T16:39:02.746064Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T16:39:02.746064Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T16:39:02.746064Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000006';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000007';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000008';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000009';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000010';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000011';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000012';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000013';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000014';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000015';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000016';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.9005Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000017';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718163903_StabilizeBookingSeedTimestamps') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260718163903_StabilizeBookingSeedTimestamps', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718164216_StabilizeBookingLookupSeedAuditValues') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.894755Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.894755Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718164216_StabilizeBookingLookupSeedAuditValues') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.894755Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.894755Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718164216_StabilizeBookingLookupSeedAuditValues') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.894755Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.894755Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718164216_StabilizeBookingLookupSeedAuditValues') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.894755Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.894755Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718164216_StabilizeBookingLookupSeedAuditValues') THEN
    UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.894755Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.894755Z'
    WHERE "Id" = '00000000-0000-0000-0000-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718164216_StabilizeBookingLookupSeedAuditValues') THEN
    UPDATE "IdempotencyRequestStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.895326Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.895326Z'
    WHERE "Id" = '00000000-0000-0000-0002-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718164216_StabilizeBookingLookupSeedAuditValues') THEN
    UPDATE "IdempotencyRequestStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.895326Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.895326Z'
    WHERE "Id" = '00000000-0000-0000-0002-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718164216_StabilizeBookingLookupSeedAuditValues') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.895519Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.895519Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718164216_StabilizeBookingLookupSeedAuditValues') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.895519Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.895519Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718164216_StabilizeBookingLookupSeedAuditValues') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.895519Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.895519Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718164216_StabilizeBookingLookupSeedAuditValues') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.895519Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.895519Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718164216_StabilizeBookingLookupSeedAuditValues') THEN
    UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.895519Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T10:59:32.895519Z'
    WHERE "Id" = '00000000-0000-0000-0001-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260718164216_StabilizeBookingLookupSeedAuditValues') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260718164216_StabilizeBookingLookupSeedAuditValues', '8.0.0');
    END IF;
END $EF$;
COMMIT;

