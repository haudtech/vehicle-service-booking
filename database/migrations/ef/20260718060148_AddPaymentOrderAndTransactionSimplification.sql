START TRANSACTION;

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

CREATE TABLE "Orders" (
    "Id" uuid NOT NULL,
    "OrderCode" character varying(50) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "IsActive" boolean NOT NULL DEFAULT TRUE,
    CONSTRAINT "PK_Orders" PRIMARY KEY ("Id")
);

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

UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.325978Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.325978Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000001';

UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.325979Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.325979Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000002';

UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.325979Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.325979Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000003';

UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.325979Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.325979Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000004';

UPDATE "AppointmentStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.32598Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.32598Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000005';

INSERT INTO "CurrencyLookups" ("Id", "Code", "CreatedAt", "DecimalPlaces", "IsActive", "Name", "Symbol", "UpdatedAt")
VALUES ('00000000-0000-0000-0004-000000000001', 'VND', TIMESTAMPTZ '2026-07-18T06:01:48.327501Z', 0, TRUE, 'Vietnamese Dong', 'VND', TIMESTAMPTZ '2026-07-18T06:01:48.327501Z');
INSERT INTO "CurrencyLookups" ("Id", "Code", "CreatedAt", "DecimalPlaces", "IsActive", "Name", "Symbol", "UpdatedAt")
VALUES ('00000000-0000-0000-0004-000000000002', 'USD', TIMESTAMPTZ '2026-07-18T06:01:48.327501Z', 2, TRUE, 'US Dollar', 'USD', TIMESTAMPTZ '2026-07-18T06:01:48.327502Z');

UPDATE "IdempotencyRequestStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.326384Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.326384Z'
WHERE "Id" = '00000000-0000-0000-0002-000000000001';

UPDATE "IdempotencyRequestStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.326384Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.326384Z'
WHERE "Id" = '00000000-0000-0000-0002-000000000002';

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

INSERT INTO "PaymentTransactionStatusLookups" ("Id", "CreatedAt", "Description", "IsActive", "Name", "Status", "UpdatedAt")
VALUES ('00000000-0000-0000-0005-000000000001', TIMESTAMPTZ '2026-07-18T06:01:48.327943Z', 'Transaction is created and waiting for processing', TRUE, 'Pending', 1, TIMESTAMPTZ '2026-07-18T06:01:48.327943Z');
INSERT INTO "PaymentTransactionStatusLookups" ("Id", "CreatedAt", "Description", "IsActive", "Name", "Status", "UpdatedAt")
VALUES ('00000000-0000-0000-0005-000000000002', TIMESTAMPTZ '2026-07-18T06:01:48.327943Z', 'Transaction is being processed', TRUE, 'In Progress', 2, TIMESTAMPTZ '2026-07-18T06:01:48.327944Z');
INSERT INTO "PaymentTransactionStatusLookups" ("Id", "CreatedAt", "Description", "IsActive", "Name", "Status", "UpdatedAt")
VALUES ('00000000-0000-0000-0005-000000000003', TIMESTAMPTZ '2026-07-18T06:01:48.327944Z', 'Transaction has been completed successfully', TRUE, 'Completed', 3, TIMESTAMPTZ '2026-07-18T06:01:48.327944Z');

UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.326558Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.326558Z'
WHERE "Id" = '00000000-0000-0000-0001-000000000001';

UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.326559Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.326559Z'
WHERE "Id" = '00000000-0000-0000-0001-000000000002';

UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.326559Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.326559Z'
WHERE "Id" = '00000000-0000-0000-0001-000000000003';

UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.326559Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.326559Z'
WHERE "Id" = '00000000-0000-0000-0001-000000000004';

UPDATE "ServiceStatusLookups" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.32656Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.32656Z'
WHERE "Id" = '00000000-0000-0000-0001-000000000005';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331066Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331066Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000001';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331067Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331067Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000002';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331068Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331068Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000003';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331069Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331069Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000004';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331069Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331069Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000005';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.33107Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.33107Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000006';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.33107Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.33107Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000007';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331071Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331071Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000008';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331072Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331072Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000009';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331072Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331072Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000010';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331073Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331073Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000011';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331074Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331074Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000012';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331074Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331074Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000013';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331075Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331075Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000014';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331076Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331076Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000015';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331076Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331076Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000016';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331077Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331077Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000017';

UPDATE "TimeSlots" SET "CreatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331077Z', "UpdatedAt" = TIMESTAMPTZ '2026-07-18T06:01:48.331077Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000018';

CREATE UNIQUE INDEX "IX_CurrencyLookup_Code_Unique" ON "CurrencyLookups" ("Code");

CREATE UNIQUE INDEX "IX_OrderAppointment_Appointment_Unique" ON "OrderAppointments" ("AppointmentId");

CREATE UNIQUE INDEX "IX_OrderAppointment_Unique_Order_Appointment" ON "OrderAppointments" ("OrderId", "AppointmentId");

CREATE UNIQUE INDEX "IX_Order_OrderCode_Unique" ON "Orders" ("OrderCode");

CREATE UNIQUE INDEX "IX_PaymentOrder_OrderId" ON "PaymentOrders" ("OrderId");

CREATE UNIQUE INDEX "IX_PaymentOrder_TransactionId_Unique" ON "PaymentOrders" ("TransactionId");

CREATE UNIQUE INDEX "IX_PaymentProviderLookup_Provider_Unique" ON "PaymentProviderLookups" ("Provider");

CREATE UNIQUE INDEX "IX_PaymentTransaction_TransactionCode_Unique" ON "PaymentTransactions" ("TransactionCode");

CREATE INDEX "IX_PaymentTransactions_CurrencyId" ON "PaymentTransactions" ("CurrencyId");

CREATE INDEX "IX_PaymentTransactions_StatusId" ON "PaymentTransactions" ("StatusId");

CREATE UNIQUE INDEX "IX_PaymentTransactionStatusLookup_Status_Unique" ON "PaymentTransactionStatusLookups" ("Status");

CREATE UNIQUE INDEX "IX_ServiceTypeOrder_Unique_Order_ServiceType" ON "ServiceTypeOrders" ("OrderId", "ServiceTypeId");

CREATE INDEX "IX_ServiceTypeOrders_ServiceTypeId" ON "ServiceTypeOrders" ("ServiceTypeId");

CREATE UNIQUE INDEX "IX_ServiceTypePrice_Unique_ServiceType_Currency" ON "ServiceTypePrices" ("ServiceTypeId", "CurrencyId");

CREATE INDEX "IX_ServiceTypePrices_CurrencyId" ON "ServiceTypePrices" ("CurrencyId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260718060148_AddPaymentOrderAndTransactionSimplification', '8.0.0');

COMMIT;

