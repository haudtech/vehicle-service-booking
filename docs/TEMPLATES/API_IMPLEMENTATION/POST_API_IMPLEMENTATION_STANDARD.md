# POST API Implementation Standard

## Purpose
This document defines the default engineering standard for implementing any new POST endpoint in this repository.

It enforces consistency across:
- authorization
- validation
- idempotency
- status-code semantics
- error contracts
- observability
- test coverage

Use this as a mandatory checklist before merging a new POST API.

When a request changes domain semantics, pricing rules, ownership boundaries, or lifecycle expectations,
the implementation plan must first expand the decision space for the requester before coding.

---

## 1. API Contract Rules

1. Endpoint must define clear request/response DTOs in Application layer.
2. Request DTO should contain only the data required by the use case.
3. Response DTO should return canonical identifiers and server-resolved values.
4. For monetary/composite create flows, include server-computed aggregate values (for example, totalAmount) in response.
5. Request and response examples must be included in controller XML comments.

### Clarification Workflow For Ambiguous Domain Changes

Before coding a POST API that introduces or changes domain semantics, the implementation plan must:

1. Present at least 2 realistic scenarios or lifecycle variants when multiple valid interpretations exist.
2. Explain pros/cons of each option in concise engineering terms.
3. Recommend one option explicitly and explain why.
4. Ask clarifying questions one-by-one for unresolved architectural decisions.
5. Record the approved decisions in the implementation plan before code edits begin.

Sample clarification prompts:

- Currency source of truth:
	- Option A: store CurrencyId on Order.
		- Pros: clearer aggregate semantics, simpler reporting, easier invariants.
		- Cons: duplicates data already implied by child/payment rows.
	- Option B: do not store CurrencyId on Order.
		- Pros: less duplication.
		- Cons: weaker aggregate identity, harder consistency checks.
	- Recommended: Option A.

- Payment lifecycle timing:
	- Option A: create PaymentTransaction during order creation.
		- Pros: immediate payment bootstrap.
		- Cons: mixes order creation with payment execution lifecycle.
	- Option B: create payment entities only during payment execution.
		- Pros: cleaner separation of concerns.
		- Cons: requires later workflow to initialize payment records.
	- Recommended: choose based on whether order creation is also the payment-initiation boundary.

- Historical pricing snapshot:
	- Option A: store UnitPrice/LineTotal snapshot on order lines.
		- Pros: immutable commercial history, reliable reconciliation.
		- Cons: denormalized data.
	- Option B: always derive from current ServiceTypePrice.
		- Pros: less stored data.
		- Cons: historical orders change when price tables change.
	- Recommended: Option A.

---

## 2. Authorization Rules

1. Every new POST endpoint must use a dedicated authorization policy.
2. Every dedicated policy must map to a dedicated permission claim.
3. Do not reuse unrelated permissions as a shortcut.
4. Policy name convention:
- <Resource><Action>Policy
- Example: OrderCreatePolicy
5. Permission convention:
- <resource>:<action>
- Example: order:create

Implementation points:
- API policy registration: JwtAuthenticationExtensions
- Auth permission seed: AuthDbContext + migration

---

## 3. Validation Rules

1. Register a FluentValidation validator for each request DTO.
2. Validate format + required fields in validator.
3. Validate cross-field consistency in validator custom rules.
4. Validate domain existence/uniqueness in service layer (not controller).
5. Error codes must be deterministic and stable.

Controller behavior:
- Call ValidateAndThrowAsync(request, cancellationToken)
- Let ValidationException middleware produce standard 400 response

---

## 4. Idempotency Rules

1. All create-style POST endpoints must support idempotency coordinator flow.
2. Request path key format:
- POST:/api/v1/<resource>
3. Request hash must be computed from canonical request JSON.
4. Coordinator outcomes:
- proceed
- key reused with different payload -> 409
- in progress -> 409
- replay stored response -> return stored status/body
5. On success, persist replay response using CompleteRequestAsync.
6. On unexpected failure, release in-progress record using ReleaseRequestAsync.

Implementation points:
- IIdempotencyRequestCoordinator
- IdempotencyRequestCoordinator
- IIdempotencyService

---

## 5. Controller Execution Flow (Mandatory Sequence)

For POST create actions, follow this order:

1. Resolve context defaults if needed (claims-based enrichment).
2. Validate request DTO.
3. Run idempotency coordinator.
4. Return early response if coordinator indicates replay/conflict/invalid key.
5. Call application service create method.
6. Persist idempotent completion record on success.
7. Return 201 Created with standard response DTO.

Exception mapping pattern:
- ValidationException: rethrow to middleware (400)
- domain conflict exception: return 409 with stable code
- InvalidOperationException/business rule violation: return 400
- unhandled exception: release idempotency record and rethrow (500 middleware path)

---

## 6. Status Code Contract

Minimum response types for create endpoints:

1. 201 Created: entity created
2. 400 Bad Request: validation/business invalid operation
3. 401 Unauthorized: missing/invalid identity claims when required
4. 409 Conflict: domain or idempotency conflict
5. 500 Internal Server Error: unexpected failure

All error payloads must use ErrorResponse.

---

## 7. Service Layer Rules

1. Service owns business logic orchestration.
2. Service validates referenced entities exist and are active.
3. Service enforces uniqueness invariants before persistence.
4. Service persists aggregate atomically via repository boundary.
5. Service returns DTO-ready data to controller.

For create operations with optional client key/code (e.g., orderCode):
- allow client-supplied value
- normalize value
- reject duplicates deterministically
- generate fallback value only when contract explicitly allows missing value

---

## 8. Repository Rules

1. Keep repositories persistence-focused; no business policy logic.
2. Add aggregate-specific repository interfaces where needed.
3. Add explicit methods for existence and uniqueness checks.
4. Use transaction-safe create methods for multi-table writes.
5. Translate known DB constraint violations to domain-level exceptions when applicable.

---

## 9. Logging and Observability Rules

1. Log create intent with key business identifiers.
2. Log successful creation with created ID.
3. Log domain conflicts as Warning.
4. Log unexpected failures as Error.
5. Ensure correlation ID is available via middleware pipeline.

Do not log secrets or sensitive credential material.

---

## 10. Cancellation Token Rules

1. Public API actions must accept CancellationToken.
2. Pass token through validator, coordinator, service, repository, and db calls.
3. Do not create detached tasks that ignore request cancellation.

---

## 11. Test Coverage Rules

Minimum required tests for each new POST endpoint:

1. Controller tests
- happy path 201
- idempotency header present flow
- idempotency replay flow
- idempotency key reused conflict
- domain conflict -> 409 mapping
- invalid operation -> 400 mapping
- must include controller-level unit tests for idempotency and conflict mappings (mirror existing endpoint coverage patterns, e.g. AppointmentsController tests)

2. Service tests
- happy path aggregate creation
- missing referenced entities
- duplicate unique value
- domain invariant violations

3. Validator tests
- required fields
- format constraints
- custom cross-field rules

4. Build verification
- build API project
- build related service project(s)

5. Auth migration metadata verification (when auth seed/policy changes are included)
- generate migration artifacts through EF tooling (do not handcraft only .cs migration file)
- ensure migration file + designer + AuthDbContextModelSnapshot are in sync

---

## 12. Create Order Profile (Approved Decisions)

This section captures approved decisions for Order create implementation and is the reference profile for this endpoint.

1. Authorization
- Use Option A: dedicated permission now.
- Required policy: OrderCreatePolicy
- Required permission: order:create

2. orderCode behavior
- Accept client-supplied orderCode
- Validate uniqueness
- Return deterministic conflict on duplicate

3. Response detail
- Include computed totalAmount in create response

These decisions are now part of the implementation contract for Create Order.

### Create Order Sample Request and Response

Use the sample below as the baseline for documenting POST create behavior and expected response shape.

Sample request:

```json
{
	"OrderCode": "ORD-20260707_01",
	"CurrencyId": "00000000-0000-0000-0004-000000000001",
	"ServiceTypeItems": [
		{
			"ServiceTypeId": "11111111-1111-1111-1111-030000000001",
			"Quantity": 1
		}
	],
	"AppointmentIds": [
		"7f2dd016-861b-4283-b1c4-ebb4fba1f529"
	]
}
```

Sample success response (HTTP 201):

```json
{
	"orderId": "bba9725d-3d2a-406b-8ce9-8b5ff16f67c5",
	"orderCode": "ORD-20260707_01",
	"currencyId": "00000000-0000-0000-0004-000000000001",
	"totalAmount": 500000,
	"createdAt": "2026-07-19T05:12:11.731316Z",
	"serviceTypeItems": [
		{
			"serviceTypeId": "11111111-1111-1111-1111-030000000001",
			"quantity": 1,
			"unitPrice": 500000.00,
			"lineTotal": 500000.00
		}
	],
	"appointmentIds": [
		"7f2dd016-861b-4283-b1c4-ebb4fba1f529"
	]
}
```

---

## 13. File Touch Checklist for New POST Endpoints

At minimum, review and update as needed:

1. API controller
- add endpoint and response metadata

2. DTOs
- request/response models

3. Validator
- request validator + registration

4. Application services
- service interface + implementation

5. Repositories
- interface + implementation + DI registration

6. Idempotency coordinator
- endpoint-specific coordinator method

7. Authorization policy
- API policy registration
- Auth permission seeding/migration

8. Tests
- controller, service, validator

9. Documentation
- endpoint XML docs and integration examples

10. Integration HTTP workflow
- update the relevant HTTP workflow file under tests/integration/http to include the new POST endpoint request
- include Idempotency-Key usage in the HTTP example for create-style endpoints

11. Decision log
- if clarification was required, record approved domain decisions in the implementation plan or verification doc before coding proceeds

---

## 14. Mandatory Completion Gates (Implementation Plan)

A POST API implementation is not complete until all gates below are satisfied:

1. Controller-level unit tests for idempotency and conflict mappings are implemented and passing.
2. If authorization seed data changed, EF migration metadata is generated via tooling and includes migration + designer + snapshot updates.
3. Integration HTTP test workflow is updated with executable request examples for the new endpoint.

---

## 15. Authorization Expansion Checklist (When New Action Permissions Are Added)

When a POST API request introduces new action permissions for an existing resource,
all items below are mandatory and must be tracked in the implementation plan:

1. Booking API policy registry is updated for every new action.
- Example: order:view, order:edit, order:cancel, order:complete.

2. Booking API endpoint coverage is explicitly tracked.
- For each new action policy, either:
- a) endpoint usage is implemented now, or
- b) usage is deferred with explicit scope note and follow-up task.

3. Auth static seed data is updated and migrated.
- Include Role, Permission, and RolePermission records as needed.
- Ensure migration + designer + model snapshot stay synchronized.

4. Signup default role assignment is updated if authorization intent changes for new users.
- Update auth default role configuration (appsettings + options binding).

5. Existing-user authorization migration strategy is documented and executed when required.
- Define whether existing users must receive new role/permission relationships.
- If yes, provide and run a backfill approach (script, migration, or job) and record evidence.

6. Verification evidence is required.
- Build success for affected projects.
- Tests proving authorization behavior for new/updated policies.

### Create Order Verification Evidence

Use this as the concrete evidence checklist for the approved Create Order implementation:

1. Gate 1 evidence: controller-level tests for idempotency/conflict mappings
- [tests/VehicleServiceBooking.Tests/Api/Controllers/OrdersControllerTests.cs](../../../tests/VehicleServiceBooking.Tests/Api/Controllers/OrdersControllerTests.cs)

2. Gate 2 evidence: authorization migration metadata generated and synchronized
- [src/VehicleServiceBooking.Auth/Migrations/20260718085649_AddOrderCreatePermission.cs](../../../src/VehicleServiceBooking.Auth/Migrations/20260718085649_AddOrderCreatePermission.cs)
- [src/VehicleServiceBooking.Auth/Migrations/20260718085649_AddOrderCreatePermission.Designer.cs](../../../src/VehicleServiceBooking.Auth/Migrations/20260718085649_AddOrderCreatePermission.Designer.cs)
- [src/VehicleServiceBooking.Auth/Migrations/AuthDbContextModelSnapshot.cs](../../../src/VehicleServiceBooking.Auth/Migrations/AuthDbContextModelSnapshot.cs)

3. Gate 2 evidence: create-order schema migration generated and synchronized
- [src/VehicleServiceBooking.Infrastructure/Migrations/20260718105234_AddOrderCurrencyAndSnapshotPricing.cs](../../../src/VehicleServiceBooking.Infrastructure/Migrations/20260718105234_AddOrderCurrencyAndSnapshotPricing.cs)
- [src/VehicleServiceBooking.Infrastructure/Migrations/20260718105234_AddOrderCurrencyAndSnapshotPricing.Designer.cs](../../../src/VehicleServiceBooking.Infrastructure/Migrations/20260718105234_AddOrderCurrencyAndSnapshotPricing.Designer.cs)
- [src/VehicleServiceBooking.Infrastructure/Migrations/ApplicationDbContextModelSnapshot.cs](../../../src/VehicleServiceBooking.Infrastructure/Migrations/ApplicationDbContextModelSnapshot.cs)

4. Gate 3 evidence: integration HTTP workflow updated with Idempotency-Key and currency-aware payload
- [tests/integration/http/user_workflow_signup_to_appointment_with_auth.http](../../../tests/integration/http/user_workflow_signup_to_appointment_with_auth.http)

5. Supporting evidence: service and validator tests for pricing source, currency validation, and request validation
- [tests/VehicleServiceBooking.Tests/Application/Services/OrderServiceTests.cs](../../../tests/VehicleServiceBooking.Tests/Application/Services/OrderServiceTests.cs)
- [tests/VehicleServiceBooking.Tests/Application/Validators/CreateOrderRequestValidatorTests.cs](../../../tests/VehicleServiceBooking.Tests/Application/Validators/CreateOrderRequestValidatorTests.cs)
