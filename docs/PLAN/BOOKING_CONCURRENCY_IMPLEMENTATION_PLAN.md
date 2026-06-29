# Booking Concurrency Implementation Plan

**Last Reconciled:** June 29, 2026  
**Purpose:** Detailed execution plan for safe booking under concurrent requests  
**Scope:** Keep service re-check, enforce PostgreSQL booking invariant, add idempotency, evaluate Redis only after load testing

## Progress Snapshot (As of 2026-06-29)

- Phase 1 complete: conflict exception and `409 BOOKING_CONFLICT` mapping are active.
- Phase 2 complete: PostgreSQL overlap invariant is enforced via exclusion constraints on `Services`.
- Phase 3 complete: idempotency key flow is active for create booking and replay behavior is validated.
- Latest API validation run: `docs/evidence/api/2026-06-29/20260629_094443` with `10 passed / 0 failed`.
- Latest automated test summary: `docs/evidence/test-reports/2026-06-29/20260629_100758/dotnet-test.md` with `52 passed / 0 failed`.
- Remaining planned workstreams are Phase 4 (load testing and decision evidence) and optional Phase 5 (Redis locking only if Phase 4 proves necessary).

### Workstream Status Matrix

| Workstream | Status | Notes |
| --- | --- | --- |
| A. Service re-check clarity | Complete | `BookingConflictException` flow and API conflict behavior are active. |
| B. PostgreSQL booking invariant | Complete | `btree_gist` + exclusion constraints and persisted invariant fields are in place. |
| C. Conflict translation | Complete | Database contention maps to stable `409 BOOKING_CONFLICT`. |
| D. Idempotency for retries | Complete | `Idempotency-Key` flow supports create, replay, payload mismatch, and in-progress handling. |
| E. Testing and verification | Complete (for planned API + automated coverage) | API script includes concurrency and idempotency scenarios; latest report is green. |
| F. Load testing before Redis | Pending | Load/performance evidence for hot-slot contention is still required. |
| G. Optional Redis locking | Not started (by design) | Deferred until Workstream F proves current layers are insufficient. |
| H. Explicit PostgreSQL transaction orchestration | Planned (deferred) | Documented for later implementation after approval and scheduling. |

---

## 1. Objective

The booking path must prevent double-booking even when two requests validate the same slot at nearly the same time.

For this codebase, the safe implementation order is:

1. Keep the current application-side re-check in `AppointmentService`.
2. Enforce the booking invariant in PostgreSQL.
3. Add idempotency support for repeated create requests.
4. Load test the final shape.
5. Add Redis locking only if measured contention justifies it.

This order matters because the service re-check improves user feedback, but the database invariant is the mechanism that actually closes the race window.

---

## 2. Current Race Window

Current behavior in `AppointmentService.CreateAppointmentAsync()`:

1. Query availability.
2. Validate technician/service bay combination.
3. Validate technician skill.
4. Validate vehicle conflicts.
5. Create `Appointment` and `Service`.
6. Persist.

Two concurrent requests can both pass steps 1-4 before either request reaches step 6. That means a single API instance is still vulnerable if PostgreSQL does not reject conflicting inserts.

---

## 3. Workstream A: Keep and Clarify the Service Re-Check

### Goal

Preserve the current service-side validation as a fast pre-check, but make it explicit that it is not the final concurrency guard.

### Files to update

- `src/VehicleServiceBooking.Application/Services/AppointmentService.cs`
- `src/VehicleServiceBooking.Application/Interfaces/Services/IAppointmentService.cs` only if new result/exception contracts are introduced
- `src/VehicleServiceBooking.Api/Controllers/AppointmentsController.cs`

### Changes

1. Keep availability validation exactly before persistence.
2. Keep technician skill and vehicle conflict checks.
3. Add a clearly named conflict exception for booking contention.
4. Catch database-backed booking conflicts separately from generic failures.
5. Return `409 Conflict` instead of surfacing a generic invalid operation when the slot was taken during the race window.

### Implementation notes

- Introduce a domain/application exception such as `BookingConflictException`.
- Do not remove the current `AvailabilityService` call.
- Treat the service re-check as an early rejection layer, not the final authority.

### Done criteria

- A database conflict is translated to a deterministic application exception.
- The controller returns `409` for booking contention.
- Generic server errors remain separate from contention errors.

---

## 4. Workstream B: Enforce the Booking Invariant in PostgreSQL

### Goal

Make PostgreSQL reject overlapping bookings for the same technician or service bay on the same appointment date.

### Why this must live on `Service`

The actual bookable resource assignment is stored on the `Service` row:

- `TechnicianId`
- `ServiceBayId`
- `EstimatedStartTimeSlotId`
- `EstimatedEndTimeSlotId`
- `DealershipId`
- `AppointmentId`

That makes `Service` the correct place for the invariant, not `Appointment`.

### Files to update

- `src/VehicleServiceBooking.Domain/Entities/Service.cs`
- `src/VehicleServiceBooking.Infrastructure/Persistence/ApplicationDbContext.cs`
- `src/VehicleServiceBooking.Infrastructure/Migrations/<new migration>.cs`
- `src/VehicleServiceBooking.Infrastructure/Migrations/ApplicationDbContextModelSnapshot.cs`

### Data model additions on `Service`

Add persisted fields that let PostgreSQL validate overlap without depending on a runtime computed property:

1. `AppointmentDate` snapshot on `Service`
2. `EstimatedStartSlotSequence` on `Service`
3. `EstimatedEndSlotSequence` on `Service`

Optional later improvement:

4. A persisted derived booking range representation if you prefer timestamp-range constraints instead of slot-sequence constraints.

### Migration shape

The migration should do the following:

1. Enable `btree_gist` if not already enabled.
2. Add the new persisted columns to `Services`.
3. Backfill those columns from:
   - `Appointments.AppointmentDate`
   - `TimeSlots.SequenceOrder`
4. Mark the new columns required once backfill succeeds.
5. Add supporting indexes for normal query paths.
6. Add exclusion constraints to reject overlap.

### Constraint strategy

Prefer exclusion constraints over plain unique indexes.

Reason:

- Unique indexes only reject exact duplicates.
- Exclusion constraints can reject overlapping slot ranges.

### Target invariants

1. On the same appointment date, the same technician cannot own overlapping slot ranges.
2. On the same appointment date, the same service bay cannot own overlapping slot ranges.

### Example conceptual constraint shape

The exact SQL may vary, but the invariant is conceptually:

- same `AppointmentDate`
- same `TechnicianId`
- overlapping slot range is forbidden

and similarly for `ServiceBayId`.

### Application write changes required

When creating a `Service` in `AppointmentService`, populate:

- `AppointmentDate`
- `EstimatedStartSlotSequence`
- `EstimatedEndSlotSequence`

Those values should come from the requested appointment date and the selected `TimeSlot` rows.

### Done criteria

- PostgreSQL rejects overlapping bookings for the same technician.
- PostgreSQL rejects overlapping bookings for the same service bay.
- Existing data is backfilled safely.
- The application populates the new persisted fields on insert.

---

## 5. Workstream C: Translate PostgreSQL Conflicts Cleanly

### Goal

Convert low-level database errors into stable, user-safe booking conflict responses.

### Files to update

- `src/VehicleServiceBooking.Application/Services/AppointmentService.cs`
- `src/VehicleServiceBooking.Infrastructure/Repositories/AppointmentRepository.cs` if repository-layer translation is preferred
- `src/VehicleServiceBooking.Api/Controllers/AppointmentsController.cs`
- Any shared exception or response types used by the API

### Changes

1. Catch provider-specific PostgreSQL constraint violations.
2. Detect exclusion-constraint or uniqueness violations that map to booking contention.
3. Throw `BookingConflictException` with a safe message.
4. Map that exception to `409 Conflict` in the controller.

### Response shape recommendation

- HTTP status: `409`
- Message: `The selected slot is no longer available. Please check availability again.`
- Optional machine-readable code: `BOOKING_CONFLICT`

### Done criteria

- No raw database exception leaks to clients.
- Booking conflicts return `409` consistently.
- Observability logs still retain the underlying exception for diagnostics.

---

## 6. Workstream D: Add Idempotency for Booking Retries

### Goal

Ensure duplicate retries do not create duplicate logical bookings.

### Problem solved

This does not solve contention between different users. It solves repeated submission of the same request because of:

- browser retry
- client retry policy
- timeout after success
- double-click submit

### Files to update

- `src/VehicleServiceBooking.Api/Controllers/AppointmentsController.cs`
- `src/VehicleServiceBooking.Application/Services/AppointmentService.cs` if orchestration is kept there
- `src/VehicleServiceBooking.Infrastructure/Persistence/ApplicationDbContext.cs`
- `src/VehicleServiceBooking.Infrastructure/Migrations/<new migration>.cs`
- New entity/repository/service files for idempotency storage

### New persistence component

Add an idempotency table, for example:

- `Id`
- `IdempotencyKey`
- `RequestPath`
- `RequestHash`
- `Status`
- `ResponseStatusCode`
- `ResponseBody`
- `CreatedAt`
- `ExpiresAt`

### New code components

Recommended additions:

- `IdempotencyRequest` entity
- `IIdempotencyRepository`
- `IdempotencyRepository`
- `IIdempotencyService`
- `IdempotencyService`

### Request flow

1. Client sends `Idempotency-Key` header on `POST /api/v1/appointments`.
2. Server hashes the meaningful request body.
3. Server checks the idempotency store.
4. Behavior branches:
   - same key + same request + completed response: return stored response
   - same key + different request: return `409`
   - same key + in-progress request: return retry/in-progress response
   - no record: process booking and persist response outcome

### Rollout recommendation

Start with idempotency only on appointment creation.

### Done criteria

- Duplicate retry with same key returns the same logical result.
- Reusing a key for a different payload is rejected.
- The idempotency store expires old records on a retention schedule.

---

## 7. Workstream E: Testing and Verification

### Goal

Prove the concurrency hardening works under real conflict conditions.

### Files to update

- `tests/VehicleServiceBooking.Tests/Application/Services/AppointmentServiceTests.cs`
- `tests/VehicleServiceBooking.Tests/Application/Services/AppointmentServiceIntegrationTests.cs`
- Additional repository or API integration tests as needed
- `scripts/run_api_tests.sh`

### Required tests

#### Unit tests

1. `AppointmentService` converts database conflict to `BookingConflictException`.
2. Idempotency service returns stored response for same key and same payload.
3. Idempotency service rejects same key with different payload.

#### Integration tests

1. Two near-simultaneous booking attempts for the same technician/slot pair.
2. Two near-simultaneous booking attempts for the same service-bay/slot pair.
3. Same request retried with identical idempotency key returns one logical booking.

#### API verification

Extend `scripts/run_api_tests.sh` with:

1. duplicate-submit scenario with same idempotency key
2. same slot concurrent-create scenario if a reliable harness is added
3. assertion that booking conflicts return `409`

### Done criteria

- One of two conflicting concurrent creates succeeds and one fails with `409`.
- Duplicate retry with same key does not create an extra booking row.
- Automated API script covers at least the idempotency retry path.

---

## 8. Workstream F: Load Testing Before Redis

### Goal

Decide whether Redis locking is necessary based on evidence, not assumption.

### What to measure

1. booking success rate under hot-slot contention
2. conflict response rate under hot-slot contention
3. median and tail latency for booking requests
4. whether PostgreSQL-backed conflict handling produces acceptable UX

### Test scenario

Simulate many users attempting to book the same service type, dealership, date, and slot band.

### Acceptance targets (SLA for Workstream F)

| Metric | Target | Why it matters |
| --- | --- | --- |
| Double-booking count | `0` | Correctness is non-negotiable. |
| Conflict response correctness | `100%` of losing contenders return `409 BOOKING_CONFLICT` | Conflicts must be explicit and deterministic. |
| Non-conflict server errors | `< 0.5%` (`5xx` excluding expected startup/transient setup errors) | Distinguishes contention from instability. |
| Booking create latency (p50) | `<= 200 ms` | Baseline user responsiveness for normal load. |
| Booking create latency (p95) | `<= 500 ms` | Tail latency under contention should remain acceptable. |
| Booking create latency (p99) | `<= 800 ms` | Prevent extreme outliers during hot-slot races. |
| Idempotency replay latency (p95) | `<= 250 ms` | Retry flow should be fast and predictable. |

### Load-test matrix

| Scenario ID | Scenario | Traffic shape | Expected result |
| --- | --- | --- | --- |
| LT-01 | Hot-slot contention: many users try the same slot | Burst: 50-200 virtual users, same dealership/date/serviceType/start-end slot | Exactly one logical winner per slot/resource pair; all losers return `409 BOOKING_CONFLICT`. |
| LT-02 | Mixed realistic load: multiple nearby slots and service types | Steady-state: 30-80 virtual users over 10-15 minutes | Stable success/conflict mix, no double-booking, latency within targets. |
| LT-03 | Retry storm with same idempotency key and same payload | Burst retries against one successful create | First create persists once; retries replay stored response, no duplicate rows. |
| LT-04 | Key reuse with different payload | Same key reused intentionally with modified payload | Request rejected deterministically (`409`), no data mutation from rejected attempt. |
| LT-05 | Control run (no contention) | Low concurrency baseline | Confirms healthy baseline latency and low error rate. |

### Execution plan

1. Prepare test data
   - Pick one dealership/date with known availability windows.
   - Capture fixed candidate IDs for repeatable hot-slot tests.
2. Run baseline control (`LT-05`)
   - Validate environment health and baseline latency before contention scenarios.
3. Run contention scenarios (`LT-01`, `LT-02`)
   - Execute at least 3 rounds per scenario (warm-up, measured run, repeatability run).
4. Run idempotency scenarios (`LT-03`, `LT-04`)
   - Confirm replay and mismatch behavior under parallel retries.
5. Collect artifacts
   - Raw load output, request/response samples, summary table, and DB verification query results.
6. Verify invariants
   - Confirm no overlapping persisted bookings for same technician or service bay on same date.
7. Publish summary
   - Add a dated markdown report under `docs/evidence/load/<date>/<run-id>/`.

### Mandatory evidence artifacts

| Artifact | Required content |
| --- | --- |
| `load-summary.md` | SLA results table, pass/fail per metric, final recommendation. |
| `raw-results.*` | Tool-native output (json/csv/txt) for reproducibility. |
| `booking-integrity-check.sql.txt` | Query output proving zero overlapping persisted bookings. |
| `api-error-breakdown.md` | Count/rate of `201`, `409 BOOKING_CONFLICT`, and `5xx`. |
| `latency-percentiles.md` | p50/p95/p99 for create and idempotency replay paths. |

### Redis decision gates (start Workstream G only if any gate fails)

1. Any confirmed double-booking during load tests.
2. Conflict signaling is inconsistent (losers not reliably returning `409 BOOKING_CONFLICT`).
3. Latency repeatedly misses targets after database/query tuning and test-environment normalization.
4. CPU/DB pressure indicates excessive wasted concurrent work that cannot be reduced by existing layers.

If all gates pass, mark Workstream F complete and keep Workstream G deferred.

### Decision rule

If the combination of:

- service re-check
- PostgreSQL invariant
- idempotency

already yields acceptable correctness and latency, do not add Redis.

### Done criteria

- Load-test evidence is documented.
- Redis decision is based on measured contention, not design preference.
- SLA table above is evaluated and attached to evidence artifacts.

---

## 9. Workstream G: Optional Redis Locking

### Goal

Reduce wasted concurrent work only if earlier steps prove insufficient.

### Important rule

Redis is not the source of truth. PostgreSQL remains the final authority.

### Files to add or update if adopted

- Redis client configuration in API/infrastructure
- `IBookingLockService`
- `BookingLockService`
- booking flow integration in `AppointmentService`
- configuration in `appsettings*.json`

### Lock shape

Potential lock keys:

- dealership + appointment date + technician + slot range
- dealership + appointment date + service bay + slot range

### Lock flow

1. Acquire lock.
2. Re-check availability.
3. Attempt booking.
4. Release lock.
5. Still rely on PostgreSQL to reject conflicts if lock coordination fails.

### Adoption condition

Do not start this work until load testing proves the earlier layers are insufficient.

---

## 10. Suggested Execution Order

### Phase 1

1. Add `BookingConflictException`.
2. Update `AppointmentService` and controller mapping for `409`.
3. Add unit tests for conflict translation.

### Phase 2

4. Add persisted booking-invariant columns on `Service`.
5. Add migration with backfill and PostgreSQL exclusion constraints.
6. Populate new fields during create flow.
7. Add integration tests for overlapping bookings.

### Phase 3

8. Add idempotency table and storage service.
9. Wire `Idempotency-Key` into appointment creation.
10. Add retry-path tests and API script coverage.

### Phase 4

11. Run load tests against the final PostgreSQL + idempotency shape.
12. Decide whether Redis adds enough value.

### Phase 5

13. If needed, add Redis locking as an optimization layer only.

---

## 11. Definition of Done

This concurrency hardening work is complete when:

1. Concurrent conflicting creates cannot both persist.
2. Clients receive `409` instead of generic failures for booking contention.
3. Duplicate retries with the same idempotency key do not create duplicate bookings.
4. Automated tests prove the above behavior.
5. Load testing confirms whether Redis is necessary.
6. Status docs and API verification scripts are updated to match the final implementation.

---

## 12. Canonical References

- `PROJECT_STATUS.md`
- `docs/SYSTEM_IMPLEMENTATION_ANALYSIS_RISKS_AND_ROADMAP.md`
- `docs/NOT_YET_IMPLEMENTED_CHECKLIST.md`
- `docs/IMPLEMENTATION_CHECKLIST.md`

---

## Appendix A: Deferred Plan for Explicit PostgreSQL Transaction Orchestration

### Objective

Add explicit transaction boundaries to the booking write path so application-level atomicity is visible and auditable, while keeping PostgreSQL exclusion constraints as the final concurrency guard.

### Status

- Planned only.
- No implementation in this phase.
- Execute later after explicit approval and scheduling.

### Planned scope

1. Add explicit `BeginTransactionAsync` / `CommitAsync` / `RollbackAsync` orchestration to the booking persistence path.
2. Preserve existing conflict translation (`DbUpdateException` to `BookingConflictException` for known constraint names).
3. Keep existing public contracts and API behavior unchanged.
4. Keep isolation level at `ReadCommitted` initially; evaluate stricter isolation only if load-test evidence requires it.

### Planned file changes

1. `src/VehicleServiceBooking.Infrastructure/Repositories/AppointmentRepository.cs`
   - Update booking create method to wrap insert + `SaveChangesAsync` in explicit transaction handling.
   - Roll back transaction before rethrow on both known and unknown failures.
2. `src/VehicleServiceBooking.Application/Services/AppointmentService.cs`
   - Keep orchestration unchanged.
   - Add method-summary clarification that persistence atomicity is enforced in repository transaction boundary.
3. `tests/VehicleServiceBooking.Tests/Application/Services/AppointmentServiceIntegrationTests.cs`
   - Add/adjust tests to verify no partial persistence on failure and unchanged conflict mapping behavior.
4. Repository integration test location under `tests/VehicleServiceBooking.Tests/Infrastructure/Repositories` (existing file or new focused test file)
   - Add targeted rollback behavior validation where practical.

### Planned implementation sequence

1. Repository transaction boundary
   - Begin transaction.
   - Insert appointment and services.
   - Save changes.
   - Commit on success.
2. Exception handling
   - On `DbUpdateException`: rollback, map known booking-conflict constraints, rethrow mapped exception.
   - On other exceptions: rollback and rethrow.
3. Validation run
   - Run targeted integration tests.
   - Run full test suite.
   - Optionally rerun `scripts/run_api_tests.sh` for behavior confirmation.

### Acceptance criteria for this deferred work

1. Explicit transaction methods are present in booking write path.
2. Existing conflict mapping behavior remains deterministic (`409 BOOKING_CONFLICT` at API boundary).
3. No partial persisted records when failures occur during booking create.
4. Full automated test suite remains green.

### Out of scope for this deferred item

1. Retry policies for transient database failures.
2. `TransactionScope` adoption.
3. Redis/advisory locking changes.
4. API contract changes.
