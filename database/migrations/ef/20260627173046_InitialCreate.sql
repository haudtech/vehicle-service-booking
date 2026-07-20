CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

CREATE TABLE "AppointmentStatusLookups" (
    "Id" uuid NOT NULL,
    "Status" integer NOT NULL,
    "Name" character varying(50) NOT NULL,
    "Description" character varying(200),
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    CONSTRAINT "PK_AppointmentStatusLookups" PRIMARY KEY ("Id")
);

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

CREATE TABLE "Dealerships" (
    "Id" uuid NOT NULL,
    "Name" character varying(150) NOT NULL,
    "Address" character varying(500) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    CONSTRAINT "PK_Dealerships" PRIMARY KEY ("Id")
);

CREATE TABLE "ServiceStatusLookups" (
    "Id" uuid NOT NULL,
    "Status" integer NOT NULL,
    "Name" character varying(50) NOT NULL,
    "Description" character varying(200) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    CONSTRAINT "PK_ServiceStatusLookups" PRIMARY KEY ("Id")
);

CREATE TABLE "ServiceTypes" (
    "Id" uuid NOT NULL,
    "Name" character varying(100) NOT NULL,
    "DurationMinutes" integer NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    CONSTRAINT "PK_ServiceTypes" PRIMARY KEY ("Id"),
    CONSTRAINT "CK_ServiceType_DurationMinutes_Range" CHECK ("DurationMinutes" >= 30 AND "DurationMinutes" <= 480)
);

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

CREATE INDEX "IX_Appointments_CustomerId" ON "Appointments" ("CustomerId");

CREATE INDEX "IX_Appointments_DealershipId" ON "Appointments" ("DealershipId");

CREATE INDEX "IX_Appointments_StatusId" ON "Appointments" ("StatusId");

CREATE INDEX "IX_Appointments_VehicleId" ON "Appointments" ("VehicleId");

CREATE UNIQUE INDEX "IX_AppointmentStatusLookup_Status_Unique" ON "AppointmentStatusLookups" ("Status");

CREATE INDEX "IX_BusinessHours_DealershipId" ON "BusinessHours" ("DealershipId");

CREATE INDEX "IX_ServiceBays_DealershipId" ON "ServiceBays" ("DealershipId");

CREATE UNIQUE INDEX "IX_Service_Unique_AppointmentServiceTypeSequence" ON "Services" ("AppointmentId", "ServiceTypeId", "SequenceOrder");

CREATE INDEX "IX_Services_DealershipId" ON "Services" ("DealershipId");

CREATE INDEX "IX_Services_EstimatedEndTimeSlotId" ON "Services" ("EstimatedEndTimeSlotId");

CREATE INDEX "IX_Services_EstimatedStartTimeSlotId" ON "Services" ("EstimatedStartTimeSlotId");

CREATE INDEX "IX_Services_ServiceBayId" ON "Services" ("ServiceBayId");

CREATE INDEX "IX_Services_ServiceStatusId" ON "Services" ("ServiceStatusId");

CREATE INDEX "IX_Services_ServiceTypeId" ON "Services" ("ServiceTypeId");

CREATE INDEX "IX_Services_TechnicianId" ON "Services" ("TechnicianId");

CREATE UNIQUE INDEX "IX_ServiceStatusLookup_Status_Unique" ON "ServiceStatusLookups" ("Status");

CREATE INDEX "IX_Technicians_DealershipId" ON "Technicians" ("DealershipId");

CREATE INDEX "IX_TechnicianSchedules_TechnicianId" ON "TechnicianSchedules" ("TechnicianId");

CREATE UNIQUE INDEX "IX_TechnicianSkill_Unique_TechnicianServiceType" ON "TechnicianSkills" ("TechnicianId", "ServiceTypeId");

CREATE INDEX "IX_TechnicianSkills_ServiceTypeId" ON "TechnicianSkills" ("ServiceTypeId");

CREATE UNIQUE INDEX "UK_TimeSlot_SequenceOrder" ON "TimeSlots" ("SequenceOrder");

CREATE INDEX "IX_Vehicles_CustomerId" ON "Vehicles" ("CustomerId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260627173046_InitialCreate', '8.0.0');

COMMIT;

