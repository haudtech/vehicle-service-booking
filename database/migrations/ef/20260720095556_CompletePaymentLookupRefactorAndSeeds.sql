START TRANSACTION;

DELETE FROM "PaymentProviderLookups"
WHERE "Id" = '00000000-0000-0000-0003-000000000006';

DELETE FROM "PaymentProviderLookups"
WHERE "Id" = '00000000-0000-0000-0003-000000000007';

DELETE FROM "PaymentProviderLookups"
WHERE "Id" = '00000000-0000-0000-0003-000000000008';

ALTER TABLE "PaymentTransactions" ADD "CompletedAtUtc" timestamp with time zone;

ALTER TABLE "PaymentTransactions" ADD "FailedAtUtc" timestamp with time zone;

ALTER TABLE "PaymentTransactions" ADD "FailureCode" character varying(60);

ALTER TABLE "PaymentTransactions" ADD "FailureMessage" character varying(500);

ALTER TABLE "PaymentTransactions" ADD "PaymentMethodId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';

ALTER TABLE "PaymentTransactions" ADD "PaymentProviderId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';

ALTER TABLE "PaymentTransactions" ADD "ProviderEventId" character varying(120);

ALTER TABLE "PaymentTransactions" ADD "ProviderTransactionId" character varying(120);

ALTER TABLE "PaymentTransactions" ADD "RawProviderPayload" text;

ALTER TABLE "PaymentOrders" ALTER COLUMN "PaymentTransactionId" DROP NOT NULL;

ALTER TABLE "PaymentOrders" ADD "CheckoutUrl" text;

ALTER TABLE "PaymentOrders" ADD "ExpiresAtUtc" timestamp with time zone;

ALTER TABLE "PaymentOrders" ADD "IntentCode" character varying(80) NOT NULL DEFAULT '';

ALTER TABLE "PaymentOrders" ADD "LastProviderEventAtUtc" timestamp with time zone;

ALTER TABLE "PaymentOrders" ADD "LastProviderEventId" character varying(120);

ALTER TABLE "PaymentOrders" ADD "PaymentMethodId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';

ALTER TABLE "PaymentOrders" ADD "PaymentProviderId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';

ALTER TABLE "PaymentOrders" ADD "StatusId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';

ALTER TABLE "Orders" ADD "AmountPaid" numeric(18,2) NOT NULL DEFAULT 0.0;

ALTER TABLE "Orders" ADD "PaidAtUtc" timestamp with time zone;

ALTER TABLE "Orders" ADD "PaymentStatusId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';

CREATE TABLE "OrderPaymentStatusLookups" (
    "Id" uuid NOT NULL,
    "Status" integer NOT NULL,
    "Name" character varying(50) NOT NULL,
    "Description" character varying(200),
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "IsActive" boolean NOT NULL DEFAULT TRUE,
    CONSTRAINT "PK_OrderPaymentStatusLookups" PRIMARY KEY ("Id")
);

CREATE TABLE "PaymentIntentStatusLookups" (
    "Id" uuid NOT NULL,
    "Status" integer NOT NULL,
    "Name" character varying(50) NOT NULL,
    "Description" character varying(200),
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "IsActive" boolean NOT NULL DEFAULT TRUE,
    CONSTRAINT "PK_PaymentIntentStatusLookups" PRIMARY KEY ("Id")
);

CREATE TABLE "PaymentMethodLookups" (
    "Id" uuid NOT NULL,
    "Method" integer NOT NULL,
    "Name" character varying(50) NOT NULL,
    "Description" character varying(200) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "IsActive" boolean NOT NULL DEFAULT TRUE,
    CONSTRAINT "PK_PaymentMethodLookups" PRIMARY KEY ("Id")
);

CREATE TABLE "PaymentWebhookProcessStatusLookups" (
    "Id" uuid NOT NULL,
    "Status" integer NOT NULL,
    "Name" character varying(50) NOT NULL,
    "Description" character varying(200),
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "IsActive" boolean NOT NULL DEFAULT TRUE,
    CONSTRAINT "PK_PaymentWebhookProcessStatusLookups" PRIMARY KEY ("Id")
);

CREATE TABLE "PaymentWebhookInboxes" (
    "Id" uuid NOT NULL,
    "PaymentProviderId" uuid NOT NULL,
    "EventId" character varying(120) NOT NULL,
    "SignatureHash" character varying(128) NOT NULL,
    "Payload" text NOT NULL,
    "ProcessStatusId" uuid NOT NULL,
    "ReceivedAtUtc" timestamp with time zone NOT NULL,
    "ProcessedAtUtc" timestamp with time zone,
    "ErrorMessage" character varying(500),
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "IsActive" boolean NOT NULL DEFAULT TRUE,
    CONSTRAINT "PK_PaymentWebhookInboxes" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_PaymentWebhookInbox_PaymentProvider" FOREIGN KEY ("PaymentProviderId") REFERENCES "PaymentProviderLookups" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_PaymentWebhookInbox_ProcessStatusLookup" FOREIGN KEY ("ProcessStatusId") REFERENCES "PaymentWebhookProcessStatusLookups" ("Id") ON DELETE RESTRICT
);

INSERT INTO "OrderPaymentStatusLookups" ("Id", "CreatedAt", "Description", "IsActive", "Name", "Status", "UpdatedAt")
VALUES ('00000000-0000-0000-0003-000000000101', TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', 'Order payment has not been completed yet', TRUE, 'Pending', 1, TIMESTAMPTZ '2026-07-18T10:59:32.9005Z');
INSERT INTO "OrderPaymentStatusLookups" ("Id", "CreatedAt", "Description", "IsActive", "Name", "Status", "UpdatedAt")
VALUES ('00000000-0000-0000-0003-000000000102', TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', 'Order payment has been completed successfully', TRUE, 'Paid', 2, TIMESTAMPTZ '2026-07-18T10:59:32.9005Z');
INSERT INTO "OrderPaymentStatusLookups" ("Id", "CreatedAt", "Description", "IsActive", "Name", "Status", "UpdatedAt")
VALUES ('00000000-0000-0000-0003-000000000103', TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', 'Order payment attempt failed', TRUE, 'Failed', 3, TIMESTAMPTZ '2026-07-18T10:59:32.9005Z');
INSERT INTO "OrderPaymentStatusLookups" ("Id", "CreatedAt", "Description", "IsActive", "Name", "Status", "UpdatedAt")
VALUES ('00000000-0000-0000-0003-000000000104', TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', 'Order payment expired before completion', TRUE, 'Expired', 4, TIMESTAMPTZ '2026-07-18T10:59:32.9005Z');
INSERT INTO "OrderPaymentStatusLookups" ("Id", "CreatedAt", "Description", "IsActive", "Name", "Status", "UpdatedAt")
VALUES ('00000000-0000-0000-0003-000000000105', TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', 'Order payment was cancelled', TRUE, 'Cancelled', 5, TIMESTAMPTZ '2026-07-18T10:59:32.9005Z');
INSERT INTO "OrderPaymentStatusLookups" ("Id", "CreatedAt", "Description", "IsActive", "Name", "Status", "UpdatedAt")
VALUES ('00000000-0000-0000-0003-000000000106', TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', 'Order payment was refunded', TRUE, 'Refunded', 6, TIMESTAMPTZ '2026-07-18T10:59:32.9005Z');

INSERT INTO "PaymentIntentStatusLookups" ("Id", "CreatedAt", "Description", "IsActive", "Name", "Status", "UpdatedAt")
VALUES ('00000000-0000-0000-0006-000000000101', TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', 'Payment intent has been created', TRUE, 'Initiated', 1, TIMESTAMPTZ '2026-07-18T10:59:32.9005Z');
INSERT INTO "PaymentIntentStatusLookups" ("Id", "CreatedAt", "Description", "IsActive", "Name", "Status", "UpdatedAt")
VALUES ('00000000-0000-0000-0006-000000000102', TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', 'User has been redirected to payment provider', TRUE, 'Redirected', 2, TIMESTAMPTZ '2026-07-18T10:59:32.9005Z');
INSERT INTO "PaymentIntentStatusLookups" ("Id", "CreatedAt", "Description", "IsActive", "Name", "Status", "UpdatedAt")
VALUES ('00000000-0000-0000-0006-000000000103', TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', 'Payment intent completed successfully', TRUE, 'Paid', 3, TIMESTAMPTZ '2026-07-18T10:59:32.9005Z');
INSERT INTO "PaymentIntentStatusLookups" ("Id", "CreatedAt", "Description", "IsActive", "Name", "Status", "UpdatedAt")
VALUES ('00000000-0000-0000-0006-000000000104', TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', 'Payment intent failed', TRUE, 'Failed', 4, TIMESTAMPTZ '2026-07-18T10:59:32.9005Z');
INSERT INTO "PaymentIntentStatusLookups" ("Id", "CreatedAt", "Description", "IsActive", "Name", "Status", "UpdatedAt")
VALUES ('00000000-0000-0000-0006-000000000105', TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', 'Payment intent expired', TRUE, 'Expired', 5, TIMESTAMPTZ '2026-07-18T10:59:32.9005Z');
INSERT INTO "PaymentIntentStatusLookups" ("Id", "CreatedAt", "Description", "IsActive", "Name", "Status", "UpdatedAt")
VALUES ('00000000-0000-0000-0006-000000000106', TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', 'Payment intent was cancelled', TRUE, 'Cancelled', 6, TIMESTAMPTZ '2026-07-18T10:59:32.9005Z');

INSERT INTO "PaymentMethodLookups" ("Id", "CreatedAt", "Description", "IsActive", "Method", "Name", "UpdatedAt")
VALUES ('00000000-0000-0000-0008-000000000001', TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', 'Domestic ATM card payments', TRUE, 1, 'ATM Card (Domestic)', TIMESTAMPTZ '2026-07-18T10:59:32.9005Z');
INSERT INTO "PaymentMethodLookups" ("Id", "CreatedAt", "Description", "IsActive", "Method", "Name", "UpdatedAt")
VALUES ('00000000-0000-0000-0008-000000000002', TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', 'Credit/debit card payments', TRUE, 2, 'Credit Card', TIMESTAMPTZ '2026-07-18T10:59:32.9005Z');
INSERT INTO "PaymentMethodLookups" ("Id", "CreatedAt", "Description", "IsActive", "Method", "Name", "UpdatedAt")
VALUES ('00000000-0000-0000-0008-000000000003', TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', 'Wallet app payments', TRUE, 3, 'E-Wallet', TIMESTAMPTZ '2026-07-18T10:59:32.9005Z');
INSERT INTO "PaymentMethodLookups" ("Id", "CreatedAt", "Description", "IsActive", "Method", "Name", "UpdatedAt")
VALUES ('00000000-0000-0000-0008-000000000004', TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', 'Apple Pay tokenized payments', TRUE, 4, 'Apple Pay', TIMESTAMPTZ '2026-07-18T10:59:32.9005Z');
INSERT INTO "PaymentMethodLookups" ("Id", "CreatedAt", "Description", "IsActive", "Method", "Name", "UpdatedAt")
VALUES ('00000000-0000-0000-0008-000000000005', TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', 'Internal balance wallet', TRUE, 5, 'Internal Wallet', TIMESTAMPTZ '2026-07-18T10:59:32.9005Z');

UPDATE "PaymentProviderLookups" SET "Description" = 'ZaloPay external payment gateway', "Name" = 'ZaloPay'
WHERE "Id" = '00000000-0000-0000-0003-000000000001';

UPDATE "PaymentProviderLookups" SET "Description" = 'MoMo external payment gateway', "Name" = 'MoMo'
WHERE "Id" = '00000000-0000-0000-0003-000000000002';

UPDATE "PaymentProviderLookups" SET "Description" = 'VNPay external payment gateway', "Name" = 'VNPay'
WHERE "Id" = '00000000-0000-0000-0003-000000000003';

UPDATE "PaymentProviderLookups" SET "Description" = 'ShopeePay external payment gateway', "Name" = 'ShopeePay'
WHERE "Id" = '00000000-0000-0000-0003-000000000004';

UPDATE "PaymentProviderLookups" SET "Description" = 'OnePay external payment gateway', "Name" = 'OnePay'
WHERE "Id" = '00000000-0000-0000-0003-000000000005';

INSERT INTO "PaymentTransactionStatusLookups" ("Id", "CreatedAt", "Description", "IsActive", "Name", "Status", "UpdatedAt")
VALUES ('00000000-0000-0000-0005-000000000004', TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', 'Transaction failed during processing', TRUE, 'Failed', 4, TIMESTAMPTZ '2026-07-18T10:59:32.9005Z');
INSERT INTO "PaymentTransactionStatusLookups" ("Id", "CreatedAt", "Description", "IsActive", "Name", "Status", "UpdatedAt")
VALUES ('00000000-0000-0000-0005-000000000005', TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', 'Transaction expired before completion', TRUE, 'Expired', 5, TIMESTAMPTZ '2026-07-18T10:59:32.9005Z');
INSERT INTO "PaymentTransactionStatusLookups" ("Id", "CreatedAt", "Description", "IsActive", "Name", "Status", "UpdatedAt")
VALUES ('00000000-0000-0000-0005-000000000006', TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', 'Transaction was cancelled', TRUE, 'Cancelled', 6, TIMESTAMPTZ '2026-07-18T10:59:32.9005Z');
INSERT INTO "PaymentTransactionStatusLookups" ("Id", "CreatedAt", "Description", "IsActive", "Name", "Status", "UpdatedAt")
VALUES ('00000000-0000-0000-0005-000000000007', TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', 'Transaction amount has been refunded', TRUE, 'Refunded', 7, TIMESTAMPTZ '2026-07-18T10:59:32.9005Z');

INSERT INTO "PaymentWebhookProcessStatusLookups" ("Id", "CreatedAt", "Description", "IsActive", "Name", "Status", "UpdatedAt")
VALUES ('00000000-0000-0000-0007-000000000101', TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', 'Webhook event has been received', TRUE, 'Received', 1, TIMESTAMPTZ '2026-07-18T10:59:32.9005Z');
INSERT INTO "PaymentWebhookProcessStatusLookups" ("Id", "CreatedAt", "Description", "IsActive", "Name", "Status", "UpdatedAt")
VALUES ('00000000-0000-0000-0007-000000000102', TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', 'Webhook event has been processed successfully', TRUE, 'Processed', 2, TIMESTAMPTZ '2026-07-18T10:59:32.9005Z');
INSERT INTO "PaymentWebhookProcessStatusLookups" ("Id", "CreatedAt", "Description", "IsActive", "Name", "Status", "UpdatedAt")
VALUES ('00000000-0000-0000-0007-000000000103', TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', 'Webhook event was ignored as duplicate or irrelevant', TRUE, 'Ignored', 3, TIMESTAMPTZ '2026-07-18T10:59:32.9005Z');
INSERT INTO "PaymentWebhookProcessStatusLookups" ("Id", "CreatedAt", "Description", "IsActive", "Name", "Status", "UpdatedAt")
VALUES ('00000000-0000-0000-0007-000000000104', TIMESTAMPTZ '2026-07-18T10:59:32.9005Z', 'Webhook event processing failed', TRUE, 'Failed', 4, TIMESTAMPTZ '2026-07-18T10:59:32.9005Z');

UPDATE "Orders" SET "PaymentStatusId" = '00000000-0000-0000-0003-000000000101'::uuid WHERE "PaymentStatusId" = '00000000-0000-0000-0000-000000000000'::uuid;

UPDATE "PaymentOrders" SET "PaymentProviderId" = '00000000-0000-0000-0003-000000000001'::uuid WHERE "PaymentProviderId" = '00000000-0000-0000-0000-000000000000'::uuid;

UPDATE "PaymentOrders" SET "PaymentMethodId" = '00000000-0000-0000-0008-000000000001'::uuid WHERE "PaymentMethodId" = '00000000-0000-0000-0000-000000000000'::uuid;

UPDATE "PaymentOrders" SET "StatusId" = '00000000-0000-0000-0006-000000000101'::uuid WHERE "StatusId" = '00000000-0000-0000-0000-000000000000'::uuid;

UPDATE "PaymentTransactions" SET "PaymentProviderId" = '00000000-0000-0000-0003-000000000001'::uuid WHERE "PaymentProviderId" = '00000000-0000-0000-0000-000000000000'::uuid;

UPDATE "PaymentTransactions" SET "PaymentMethodId" = '00000000-0000-0000-0008-000000000001'::uuid WHERE "PaymentMethodId" = '00000000-0000-0000-0000-000000000000'::uuid;

CREATE INDEX "IX_PaymentTransaction_PaymentMethodId" ON "PaymentTransactions" ("PaymentMethodId");

CREATE INDEX "IX_PaymentTransaction_PaymentProviderId" ON "PaymentTransactions" ("PaymentProviderId");

CREATE UNIQUE INDEX "IX_PaymentTransaction_Provider_Event_Unique" ON "PaymentTransactions" ("PaymentProviderId", "ProviderEventId");

CREATE UNIQUE INDEX "IX_PaymentOrder_IntentCode_Unique" ON "PaymentOrders" ("IntentCode");

CREATE INDEX "IX_PaymentOrder_PaymentMethodId" ON "PaymentOrders" ("PaymentMethodId");

CREATE INDEX "IX_PaymentOrder_PaymentProviderId" ON "PaymentOrders" ("PaymentProviderId");

CREATE INDEX "IX_PaymentOrder_StatusId" ON "PaymentOrders" ("StatusId");

CREATE INDEX "IX_Order_PaymentStatusId" ON "Orders" ("PaymentStatusId");

ALTER TABLE "Orders" ADD CONSTRAINT "CK_Order_AmountPaid_Lte_TotalAmount" CHECK ("AmountPaid" <= "TotalAmount");

ALTER TABLE "Orders" ADD CONSTRAINT "CK_Order_AmountPaid_NonNegative" CHECK ("AmountPaid" >= 0);

CREATE UNIQUE INDEX "IX_OrderPaymentStatusLookup_Status_Unique" ON "OrderPaymentStatusLookups" ("Status");

CREATE UNIQUE INDEX "IX_PaymentIntentStatusLookup_Status_Unique" ON "PaymentIntentStatusLookups" ("Status");

CREATE UNIQUE INDEX "IX_PaymentMethodLookup_Method_Unique" ON "PaymentMethodLookups" ("Method");

CREATE INDEX "IX_PaymentWebhookInbox_ProcessStatusId" ON "PaymentWebhookInboxes" ("ProcessStatusId");

CREATE UNIQUE INDEX "IX_PaymentWebhookInbox_Provider_Event_Unique" ON "PaymentWebhookInboxes" ("PaymentProviderId", "EventId");

CREATE UNIQUE INDEX "IX_PaymentWebhookProcessStatusLookup_Status_Unique" ON "PaymentWebhookProcessStatusLookups" ("Status");

ALTER TABLE "Orders" ADD CONSTRAINT "FK_Order_PaymentStatusLookup" FOREIGN KEY ("PaymentStatusId") REFERENCES "OrderPaymentStatusLookups" ("Id") ON DELETE RESTRICT;

ALTER TABLE "PaymentOrders" ADD CONSTRAINT "FK_PaymentOrder_PaymentMethod" FOREIGN KEY ("PaymentMethodId") REFERENCES "PaymentMethodLookups" ("Id") ON DELETE RESTRICT;

ALTER TABLE "PaymentOrders" ADD CONSTRAINT "FK_PaymentOrder_PaymentProvider" FOREIGN KEY ("PaymentProviderId") REFERENCES "PaymentProviderLookups" ("Id") ON DELETE RESTRICT;

ALTER TABLE "PaymentOrders" ADD CONSTRAINT "FK_PaymentOrder_StatusLookup" FOREIGN KEY ("StatusId") REFERENCES "PaymentIntentStatusLookups" ("Id") ON DELETE RESTRICT;

ALTER TABLE "PaymentTransactions" ADD CONSTRAINT "FK_PaymentTransaction_PaymentMethod" FOREIGN KEY ("PaymentMethodId") REFERENCES "PaymentMethodLookups" ("Id") ON DELETE RESTRICT;

ALTER TABLE "PaymentTransactions" ADD CONSTRAINT "FK_PaymentTransaction_PaymentProvider" FOREIGN KEY ("PaymentProviderId") REFERENCES "PaymentProviderLookups" ("Id") ON DELETE RESTRICT;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260720095556_CompletePaymentLookupRefactorAndSeeds', '8.0.0');

COMMIT;

