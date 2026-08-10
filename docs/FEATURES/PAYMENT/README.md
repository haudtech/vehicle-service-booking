# Payment Feature Guide

## Purpose
Feature-level guide for payment implementation, execution order, and delivery tracking.

This folder is the source of truth for payment feature planning and progress.

## Current Status Snapshot
1. Data model and lookup refactor completed.
2. EF migration and SQL mirror alignment completed.
3. Application service flow, API endpoints, and payment tests are pending.

## Documentation Map
1. Detailed implementation plan: [PAYMENT_IMPLEMENTATION_PLAN.md](PAYMENT_IMPLEMENTATION_PLAN.md)
2. Phase tracking checklist: [PAYMENT_PROGRESS_CHECKLIST.md](PAYMENT_PROGRESS_CHECKLIST.md)
3. ZaloPay implementation and architecture workflow: [ZALOPAY_IMPLEMENTATION_ARCHITECTURE_WORKFLOW.md](ZALOPAY_IMPLEMENTATION_ARCHITECTURE_WORKFLOW.md)
4. OnePay implementation and architecture workflow: [ONEPAY_IMPLEMENTATION_ARCHITECTURE_WORKFLOW.md](ONEPAY_IMPLEMENTATION_ARCHITECTURE_WORKFLOW.md)
5. ZaloPay sandbox integration runbook: [ZALOPAY_SANDBOX_RUNBOOK.md](ZALOPAY_SANDBOX_RUNBOOK.md)

## Related Cross-Cutting Documents
1. Architecture entry point: [docs/ARCHITECTURE/README.md](../../ARCHITECTURE/README.md)
2. Conceptual system view: [docs/ARCHITECTURE/CONCEPTUAL_VIEW.md](../../ARCHITECTURE/CONCEPTUAL_VIEW.md)
3. Auth components and boundaries: [docs/ARCHITECTURE/AUTH_COMPONENTS_VIEW.md](../../ARCHITECTURE/AUTH_COMPONENTS_VIEW.md)
4. Sequential runtime flow reference: [docs/ARCHITECTURE/AUTH_SEQUENTIAL_FLOW_VIEW.md](../../ARCHITECTURE/AUTH_SEQUENTIAL_FLOW_VIEW.md)

## Migration and SQL Mirror References
1. EF migration: [src/VehicleServiceBooking.Infrastructure/Migrations/20260720095556_CompletePaymentLookupRefactorAndSeeds.cs](../../../src/VehicleServiceBooking.Infrastructure/Migrations/20260720095556_CompletePaymentLookupRefactorAndSeeds.cs)
2. Per-migration SQL mirror: [database/migrations/ef/20260720095556_CompletePaymentLookupRefactorAndSeeds.sql](../../../database/migrations/ef/20260720095556_CompletePaymentLookupRefactorAndSeeds.sql)
3. Consolidated idempotent SQL: [database/migrations/ef/ALL_MIGRATIONS_IDEMPOTENT.sql](../../../database/migrations/ef/ALL_MIGRATIONS_IDEMPOTENT.sql)

## Working Agreement
1. Implement phase by phase.
2. Stop after each phase and request approval before continuing.
3. Update checklist immediately after phase completion.

## Recommended Reading Order
1. Start with [PAYMENT_IMPLEMENTATION_PLAN.md](PAYMENT_IMPLEMENTATION_PLAN.md).
2. Use [PAYMENT_PROGRESS_CHECKLIST.md](PAYMENT_PROGRESS_CHECKLIST.md) as the execution tracker.
3. Follow linked architecture docs for service boundaries and system context.
