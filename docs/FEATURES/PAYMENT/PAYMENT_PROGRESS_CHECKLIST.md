# Payment Feature Progress Checklist

Use this checklist as the execution tracker. Update status immediately after each phase and verification pass.

## Overall Progress
- [x] Phase 1. Domain Refactor
- [x] Phase 2. EF Mapping and Seed Alignment
- [x] Phase 3. Migration, Backfill, and SQL Mirror
- [x] Phase 4. Application Payment Services
- [x] Phase 5. API Endpoints
- [x] Phase 6. Tests and Verification

## Phase 1. Domain Refactor
Status: Completed

Checklist:
- [x] Order payment state fields updated.
- [x] PaymentOrder intent fields and nullable transaction linkage updated.
- [x] PaymentTransaction provider/failure fields updated.
- [x] PaymentWebhookInbox entity introduced.
- [x] Status enums/lookups aligned to intent and transaction lifecycle.

## Phase 2. EF Mapping and Seed Alignment
Status: Completed

Checklist:
- [x] ApplicationDbContext mappings updated for payment entities.
- [x] Unique index on PaymentOrder.IntentCode.
- [x] Unique composite index on PaymentTransaction(PaymentProviderId, ProviderEventId).
- [x] Payment lookup tables configured and seeded.
- [x] Provider and method dimensions separated and seeded.

## Phase 3. Migration, Backfill, and SQL Mirror
Status: Completed

Checklist:
- [x] EF migration generated and reviewed.
- [x] Backfill logic added before FK constraints.
- [x] Database update applied successfully.
- [x] Orphan FK validation checks passed.
- [x] SQL mirror updated:
  - [x] per-migration SQL file
  - [x] ALL_MIGRATIONS_IDEMPOTENT.sql
  - [x] mirror README metadata

## Phase 4. Application Payment Services
Status: Completed

Step 1 execution checklist (intent creation only):
- [x] Add create-intent request/response DTOs.
- [x] Add PaymentIntent service interface and implementation.
- [x] Add provider adapter abstraction.
- [x] Add first development gateway implementation.
- [x] Add create-intent validator.
- [x] Wire new services into dependency injection.
- [x] Verify build after Step 1 implementation.

Step 1 verification:
- [x] `dotnet build src/VehicleServiceBooking.Api/VehicleServiceBooking.Api.csproj` passed after implementation.
- [x] Nullable warnings introduced by Step 1 were resolved and build re-verified.

Step 2 execution checklist (status query endpoint):
- [x] Add payment status response DTO.
- [x] Add PaymentStatusQueryService contract and implementation.
- [x] Wire status query service into dependency injection.
- [x] Add GET /api/v1/orders/{orderId}/payments/status endpoint.
- [x] Verify build after Step 2 implementation.

Step 2 verification:
- [x] `dotnet build src/VehicleServiceBooking.Api/VehicleServiceBooking.Api.csproj` passed after Step 2 changes.

Step 3 execution checklist (webhook ingest foundation):
- [x] Add webhook request/response DTOs.
- [x] Add PaymentWebhookService contract and implementation.
- [x] Add webhook repository contract and implementation.
- [x] Wire webhook service/repository into dependency injection.
- [x] Add POST /api/v1/payments/webhooks/{providerType} endpoint.
- [x] Verify build after Step 3 implementation.

Step 3 verification:
- [x] `dotnet build src/VehicleServiceBooking.Api/VehicleServiceBooking.Api.csproj` passed after Step 3 changes.

Checklist:
- [x] Define service interfaces for intent, webhook, and status query.
- [x] Implement PaymentIntentService with order-level idempotency.
- [x] Implement PaymentWebhookService with dedupe + transition logic (foundation).
- [x] Implement PaymentStatusQueryService.
- [x] Add provider adapter abstraction and first provider implementation.
- [x] Add transaction boundary and concurrency handling rules.

## Phase 5. API Endpoints
Status: Completed

Checklist:
- [x] POST /api/v1/orders/{orderId}/payments/intent
- [x] GET /api/v1/orders/{orderId}/payments/status
- [x] POST /api/v1/payments/webhooks/{providerType}
- [x] Development mock checkout flow:
  - [x] GET /api/v1/payments/mock/checkout/{providerType}/{intentCode}
  - [x] POST /api/v1/payments/mock/checkout/{providerType}/{intentCode}/complete
- [x] Optional GET /api/v1/payments/return
- [x] Input validation, auth, and error contracts documented.

## Phase 6. Tests and Verification
Status: Completed

Checklist:
- [x] Domain/service transition tests.
- [x] Intent creation integration tests.
- [x] Webhook dedupe/replay integration tests.
- [x] Payment status endpoint tests.
- [x] Migration consistency and data integrity tests.

## Update Log
- 2026-07-20: Initialized payment plan checklist with Phases 1-3 marked complete from implemented schema/migration work.
- 2026-07-20: Completed Phase 4 Step 1 (intent creation foundation): DTOs, validator, PaymentIntentService, provider gateway abstraction, DI wiring, and create-intent endpoint.
- 2026-07-20: Completed Phase 4 Step 2 (status query): payment status DTO, PaymentStatusQueryService, DI wiring, and GET payment status endpoint.
- 2026-07-20: Completed Phase 4/5 Step 3 (webhook ingest foundation): webhook DTOs, PaymentWebhookService, PaymentWebhookRepository, payments webhook endpoint, and DI wiring.
- 2026-07-21: Added development mock checkout callback flow and webhook integration tests for valid signature, invalid signature, duplicate event dedupe, and paid transition state updates.
- 2026-07-21: Added optional payment return endpoint and API controller tests for payment intent creation/reuse and payment status query routes.
- 2026-07-21: Added PaymentIntentService integration tests for create/reuse/expired/invalid/disallowed-state paths and documented payment API validation/auth/error contracts in PAYMENT_API_CONTRACTS.md.
- 2026-07-21: Aligned create payment intent POST with idempotency coordinator flow (Idempotency-Key, replay/conflict semantics), added controller tests for idempotency mapping, and updated integration HTTP workflow to include intent replay.
- 2026-07-21: Added payment migration/data-integrity integration tests validating payment lookup seed coverage and key aggregate relationship integrity (with explicit note on EF InMemory uniqueness limitations).
- 2026-07-21: Hardened PaymentWebhookService with explicit repository transaction boundary, race-safe duplicate handling on unique inbox collisions, and added PaymentWebhookServiceTransactionTests regression coverage.

## Next Gate
Before continuing, approve optional production-grade follow-up: relational-provider webhook race integration test and retry policy/telemetry enhancements for payment webhook processing.
