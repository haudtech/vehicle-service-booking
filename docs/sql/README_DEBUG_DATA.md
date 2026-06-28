# Debug SQL Dataset

This folder contains a reproducible SQL dataset for local debugging and integration-style manual testing.

## File

- `docs/sql/populate_debug_test_data.sql`
- `docs/sql/cleanup_debug_test_data.sql`

## Purpose

The script seeds enough realistic data for:

- Happy cases: valid appointment/service records
- Edge cases: restricted technician schedule, inactive service bay, long-duration service
- Failed cases: overlapping appointment conflict, missing technician skill, plus expected DB constraint failures

## Run

```bash
export DB_PASSWORD='<your-db-password>'

PGPASSWORD="$DB_PASSWORD" psql -h localhost -p 5432 -U haudo -d vehicle_service_booking \
  -f docs/sql/populate_debug_test_data.sql

# remove only rows created by debug seed script
PGPASSWORD="$DB_PASSWORD" psql -h localhost -p 5432 -U haudo -d vehicle_service_booking \
  -f docs/sql/cleanup_debug_test_data.sql
```

## Suggested Loop

```bash
# one-time in shell
export DB_PASSWORD='<your-db-password>'

# 1) reset previous debug dataset
PGPASSWORD="$DB_PASSWORD" psql -h localhost -p 5432 -U haudo -d vehicle_service_booking -f docs/sql/cleanup_debug_test_data.sql

# 2) seed fresh dataset
PGPASSWORD="$DB_PASSWORD" psql -h localhost -p 5432 -U haudo -d vehicle_service_booking -f docs/sql/populate_debug_test_data.sql

# 3) run tests / API checks
dotnet test tests/VehicleServiceBooking.Tests/VehicleServiceBooking.Tests.csproj --no-build
```

## Safety

- Deterministic IDs are used.
- Script is idempotent for its own rows (it deletes/reinserts only rows created by this script).
- It does not remove or modify unrelated business data.

## Notes

- Appointment and service lookup rows are expected from EF migration seeds.
- The script includes best-effort `REFRESH MATERIALIZED VIEW` calls. If objects are regular views or absent, the script continues.
- A small block intentionally attempts invalid inserts and catches exceptions to verify DB constraints without persisting bad rows.
- If output stops at `(END)`, `psql` is using a pager; press `q` to return to shell.

## Case Matrix

The seeded data is organized into practical scenario buckets referenced in the SQL comments (`A1` to `A5`).

| Case ID | Scenario | Real Debug Goal | Typical API Call | Expected Result |
|---|---|---|---|---|
| A3 | Happy-path valid booking (future date, valid skill/schedule/bay) | Verify normal create + retrieval flow | `POST /api/v1/appointments` then `GET /api/v1/appointments/{id}` | `201 Created` then `200 OK` |
| A1 | Same-vehicle overlapping booked appointment | Verify conflict detection for vehicle/time overlap | `POST /api/v1/appointments` with same vehicle/date/overlapping slots | `400 Bad Request` (`INVALID_OPERATION`) |
| A2 | Overlap exists but appointment is Cancelled | Verify cancelled appointments do not block booking | `POST /api/v1/appointments` overlapping cancelled row | `201 Created` |
| Skill-gap | Technician without required skill (Ned vs Oil Change) | Verify skill validation in service layer | `POST /api/v1/appointments` with technician `040...0003` + Oil Change | `400 Bad Request` (`INVALID_OPERATION`) |
| Schedule edge | Technician with restricted schedule (10:00-14:00) | Verify availability filtering by schedule window | `GET /api/v1/availability?...` | `200 OK` with slots only inside schedule |
| Inactive bay edge | Inactive service bay present | Verify non-active bay is not selected | `GET /api/v1/availability?...` | `200 OK` without inactive bay options |
| Long-duration edge | Major service spanning many slots | Verify duration fit / slot span behavior | `GET /api/v1/availability?...` for long service type | `200 OK` with fewer valid starts |
| Isolation edge | Other dealership occupancy in same slot pattern | Verify dealership isolation in queries | `GET /api/v1/availability?...` for main dealership | `200 OK` unaffected by other dealership |
| DB constraint probe | Invalid duration and invalid VIN insert attempts (caught in DO block) | Verify DB constraint enforcement safely | SQL script execution | Script prints expected notices; no bad rows persisted |

### About 409 Conflict

Current controller mapping returns `409 Conflict` only when the exception message contains `no longer available` (concurrency-style condition). Most seeded failure scenarios above intentionally produce `400 Bad Request` with `INVALID_OPERATION`.

### Advanced Tests (Later)

The `409 Conflict` path should be treated as an advanced test track and handled later.

Deferred scope for later phase:

- Parallel request simulation against the same slot (race/concurrency behavior)
- Deterministic reproduction harness for `no longer available` messaging path
- Load-level collision tests (multi-client booking contention)
- Dedicated assertions for `409` response payload and error code contract

## Coverage Gaps To Add Later

These scenarios are present in SQL seed data but are not yet explicitly asserted in current test files.

| Seed Scenario | Proposed Test Name | Suggested File |
|---|---|---|
| Cancelled overlap should not block booking | `CreateAppointmentAsync_WithCancelledOverlappingAppointment_ShouldSucceed` | `tests/VehicleServiceBooking.Tests/Application/Services/AppointmentServiceIntegrationTests.cs` |
| Inactive bay should not appear in availability | `GetAvailableSlotsAsync_WithInactiveServiceBay_ShouldExcludeInactiveBay` | `tests/VehicleServiceBooking.Tests/Application/Services/AvailabilityServiceIntegrationTests.cs` |
| Other dealership occupancy should not affect target dealership | `GetAvailableSlotsAsync_WithOtherDealershipConflicts_ShouldIgnoreOtherDealership` | `tests/VehicleServiceBooking.Tests/Application/Services/AvailabilityServiceIntegrationTests.cs` |
| Technician exists but lacks required skill | `CreateAppointmentAsync_WithTechnicianMissingRequiredSkill_ShouldThrowException` | `tests/VehicleServiceBooking.Tests/Application/Services/AppointmentServiceIntegrationTests.cs` |
| DB check constraints for invalid duration and VIN | `InsertInvalidServiceTypeDuration_ShouldFailConstraint` and `InsertInvalidVehicleVin_ShouldFailConstraint` | `tests/VehicleServiceBooking.Tests/Infrastructure/Persistence/*` |

Priority suggestion:

1. Add cancelled-overlap and missing-skill tests first (direct business behavior).
2. Add inactive-bay and dealership-isolation availability tests second.
3. Keep DB constraint tests and `409` concurrency path in advanced phase.
