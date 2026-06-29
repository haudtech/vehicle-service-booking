# Vehicle Service Booking API - Test Cases

## Overview
This document defines API test cases for the current implementation using real PostgreSQL data.

Base URL used in this guide:
- `http://localhost:5291/api/v1`

Captured real request/response evidence:
- [docs/evidence/api/2026-06-29/20260629_094443](docs/evidence/api/2026-06-29/20260629_094443)

## Short API Implementation Report

Status meaning:
- `Done`: implemented and available in current API test flow
- `Pending`: planned but not started in current cycle
- `New`: newly requested, not implemented yet

| API | Method | Path | Purpose | Implementation Status |
|---|---|---|---|---|
| Availability | GET | `/availability` | Query available slots for booking | `Done` |
| Appointments | POST | `/appointments` | Create a new appointment | `Done` |
| Appointments | GET | `/appointments/{id}` | Get appointment details by id | `Done` |
| Services | PATCH/PUT | `/services/{id}/status` | Update status of a service (Service table) | `New` |
| Appointments | PATCH/PUT | `/appointments/{id}/status` | Update status of an appointment (Appointment table) | `New` |

---

## Prerequisites

### 1. Database
Use PostgreSQL with database `vehicle_service_booking`.

### 2. Seed Debug Data
Run cleanup and seed scripts before executing happy-path tests.

```bash
PGPASSWORD='123456xX' psql -h localhost -p 5432 -U haudo -d vehicle_service_booking -f docs/sql/cleanup_debug_test_data.sql
PGPASSWORD='123456xX' psql -h localhost -p 5432 -U haudo -d vehicle_service_booking -f docs/sql/populate_debug_test_data.sql
```

### 3. Start API

```bash
cd src/VehicleServiceBooking.Api
ASPNETCORE_URLS='http://localhost:5291' \
ConnectionStrings__DefaultConnection='Host=localhost;Port=5432;Database=vehicle_service_booking;Username=haudo;Password=123456xX;' \
dotnet run --no-launch-profile
```

---

## Happy Path Test Cases (1 per API)

## API Test Matrix (Tabular Report)

| Case ID | API | Method | Path | Scenario | Test Type | Expected Status | Evidence File Pattern |
|---|---|---|---|---|---|---|---|
| TC-01 | Availability | GET | `/availability` | Get available slots with valid dealership/service/date | Happy path | `200` | `01_get_availability.*` |
| TC-02 | Appointments | POST | `/appointments` | Create appointment with valid payload and slot | Happy path | `201` | `02_post_appointment.*` |
| TC-03 | Appointments | GET | `/appointments/{id}` | Retrieve appointment by id created in TC-02 | Happy path | `200` | `03_get_appointment_by_id.*` |
| TC-04 | Availability | GET | `/availability` | Invalid GUID values for dealership and service type | Validation | `400` | `04_invalid_guid.*` |
| TC-05 | Availability | GET | `/availability` | Past date request | Validation | `400` | `05_past_date.*` |
| TC-06 | Appointments | POST (parallel x2) | `/appointments` | Concurrent overlap booking race on same payload | Concurrency | one `201` + one `409` (`BOOKING_CONFLICT`) | `06_concurrency_overlap.*` |
| TC-07 | Appointments | POST | `/appointments` | Create appointment when idempotency is enabled but the `Idempotency-Key` header is missing | Idempotency | `201` | `07_idempotency_missing_header.*` |
| TC-08 | Appointments | POST | `/appointments` | Create appointment when idempotency is enabled and the `Idempotency-Key` header is present | Idempotency | `201` | `08_idempotency_header_present.*` |
| TC-09 | Appointments | POST (parallel x2, same idempotency key) | `/appointments` | Duplicate retry with same key and payload replays the same logical booking | Idempotency | `201` + `201` (same `appointmentId`) | `09_idempotency_retry.*` |

## Latest Execution Report (Tabular)

Use this table to track each execution run from `scripts/run_api_tests.sh`.

| Run ID | Run DateTime | API Base URL | TC-01 | TC-02 | TC-03 | TC-04 | TC-05 | TC-06 | TC-07 | TC-08 | TC-09 | Passed | Failed | Evidence Directory |
|---|---|---|---|---|---|---|---|---|---|---|---|---:|---:|---|
| `20260628_152302` | `2026-06-28 15:23:02 +07` | `http://localhost:5291/api/v1` | PASS (`200`) | PASS (`201`) | PASS (`200`) | PASS (`400`) | PASS (`400`) | N/A | N/A | N/A | N/A | 6 | 0 | `docs/evidence/api/2026-06-28/20260628_152302` |
| `20260628_195544` | `2026-06-28 19:55:44 +07` | `http://localhost:5291/api/v1` | PASS (`200`) | PASS (`201`) | PASS (`200`) | PASS (`400`) | PASS (`400`) | PASS (`201` + `409 BOOKING_CONFLICT`) | N/A | N/A | N/A | 7 | 0 | `docs/evidence/api/2026-06-28/20260628_195544` |
| `20260628_221520` | `2026-06-28 22:15:20 +07` | `http://localhost:5291/api/v1` | PASS (`200`) | PASS (`201`) | PASS (`200`) | PASS (`400`) | PASS (`400`) | PASS (`201` + `409 BOOKING_CONFLICT`) | FAIL (`201` + `201`, mismatched response payload casing/shape) | N/A | N/A | 7 | 1 | `docs/evidence/api/2026-06-28/20260628_221520` |
| `20260628_221617` | `2026-06-28 22:16:17 +07` | `http://localhost:5291/api/v1` | PASS (`200`) | PASS (`201`) | PASS (`200`) | PASS (`400`) | PASS (`400`) | PASS (`201` + `409 BOOKING_CONFLICT`) | PASS (`201` + `201`, same `appointmentId`) | N/A | N/A | 8 | 0 | `docs/evidence/api/2026-06-28/20260628_221617` |
| `20260629_094443` | `2026-06-29 09:44:43 +07` | `http://localhost:5291/api/v1` | PASS (`200`) | PASS (`201`) | PASS (`200`) | PASS (`400`) | PASS (`400`) | PASS (`201` + `409 BOOKING_CONFLICT`) | PASS (`201`, missing `Idempotency-Key`) | PASS (`201`, present `Idempotency-Key`) | PASS (`201` + `201`, same `appointmentId`) | 10 | 0 | `docs/evidence/api/2026-06-29/20260629_094443` |

## Test Case 1 - Get Availability

Description:
- Verify one successful availability query using real data.

Endpoint:
- `GET /availability`

Request:
```bash
target_date=$(date -v+1d +%F)
curl -sS -i "http://localhost:5291/api/v1/availability?dealershipId=11111111-1111-1111-1111-000000000001&serviceTypeId=11111111-1111-1111-1111-030000000001&date=$target_date"
```

Expected:
- HTTP `200 OK`
- JSON array returned
- At least one item contains:
  - `slotStart`
  - `slotEnd`
  - `technicianId`
  - `serviceBayId`

---

## Test Case 2 - Create Appointment

Description:
- Verify one successful appointment creation based on an available slot.

Endpoint:
- `POST /appointments`

Request:
```bash
target_date=$(date -v+1d +%F)

curl -sS -i -X POST "http://localhost:5291/api/v1/appointments" \
  -H 'Content-Type: application/json' \
  -d "{
    \"dealershipId\": \"11111111-1111-1111-1111-000000000001\",
    \"customerId\": \"11111111-1111-1111-1111-010000000003\",
    \"vehicleId\": \"11111111-1111-1111-1111-020000000003\",
    \"appointmentDate\": \"$target_date\",
    \"serviceTypeId\": \"11111111-1111-1111-1111-030000000001\",
    \"technicianId\": \"11111111-1111-1111-1111-040000000001\",
    \"serviceBayId\": \"11111111-1111-1111-1111-050000000001\",
    \"estimatedStartTimeSlotId\": \"00000000-0000-0000-0000-000000000007\",
    \"estimatedEndTimeSlotId\": \"00000000-0000-0000-0000-000000000008\"
  }"
```

Expected:
- HTTP `201 Created`
- Response contains:
  - `appointmentId`
  - `slotStart`
  - `slotEnd`
  - `createdAt`

Notes:
- If slot IDs conflict in your environment, first call availability and choose a currently free slot window.

---

## Test Case 3 - Get Appointment By Id

Description:
- Verify one successful retrieval by using the `appointmentId` from Test Case 2.

Endpoint:
- `GET /appointments/{id}`

Request:
```bash
appointment_id='<paste-appointment-id-from-test-2>'
curl -sS -i "http://localhost:5291/api/v1/appointments/$appointment_id"
```

Expected:
- HTTP `200 OK`
- Response contains same `appointmentId`
- `slotStart` and `slotEnd` align with created appointment

---

## Validation Test Cases

## Test Case 4 - Invalid GUID Validation

Description:
- Verify model binding validation for malformed GUID values.

Request:
```bash
curl -sS -i "http://localhost:5291/api/v1/availability?dealershipId=invalid&serviceTypeId=invalid&date=2026-01-01"
```

Expected:
- HTTP `400 Bad Request`
- Validation-style response body with field errors

---

## Test Case 5 - Past Date Validation

Description:
- Verify business rule validation for a past date.

Request:
```bash
curl -sS -i "http://localhost:5291/api/v1/availability?dealershipId=11111111-1111-1111-1111-000000000001&serviceTypeId=11111111-1111-1111-1111-030000000001&date=2020-01-01"
```

Expected:
- HTTP `400 Bad Request`
- Error message indicates date must be today or in the future

---

## Test Case 6 - Concurrency Overlap Conflict

Description:
- Verify race-window safety by sending two parallel create requests with the same payload for one available slot.

Endpoint:
- `POST /appointments` (parallel x2)

Request:
```bash
DB="host=localhost port=5432 dbname=vehicle_service_booking user=haudo password=123456xX"

row=$(psql "$DB" -At -F '|' -c "WITH candidate AS (
  SELECT sva.\"DealershipId\", sva.\"ServiceTypeId\", sva.\"TechnicianId\", sva.\"ServiceBayId\", sva.\"TimeSlotId\" AS start_slot_id,
         sva.\"RequiredSlots\", sva.\"SequenceOrder\", sva.\"QueryDate\"
  FROM \"ServiceTypeAvailability\" sva
  WHERE sva.\"CanFitService\" = TRUE
    AND sva.\"QueryDate\" >= CURRENT_DATE + INTERVAL '1 day'
  ORDER BY sva.\"QueryDate\", sva.\"SequenceOrder\"
  LIMIT 1
)
SELECT c.\"DealershipId\", c.\"ServiceTypeId\", c.\"TechnicianId\", c.\"ServiceBayId\", c.start_slot_id,
       ts_end.\"Id\" AS end_slot_id, c.\"QueryDate\"::text AS appointment_date, cust.\"Id\" AS customer_id, veh.\"Id\" AS vehicle_id
FROM candidate c
JOIN \"TimeSlots\" ts_end ON ts_end.\"SequenceOrder\" = c.\"SequenceOrder\" + c.\"RequiredSlots\" - 1
JOIN \"Customers\" cust ON TRUE
JOIN \"Vehicles\" veh ON veh.\"CustomerId\" = cust.\"Id\"
LIMIT 1;")

IFS='|' read -r dealershipId serviceTypeId technicianId serviceBayId startSlotId endSlotId appointmentDate customerId vehicleId <<< "$row"

cat > /tmp/booking_req.json <<JSON
{
  "dealershipId": "$dealershipId",
  "customerId": "$customerId",
  "vehicleId": "$vehicleId",
  "appointmentDate": "$appointmentDate",
  "serviceTypeId": "$serviceTypeId",
  "technicianId": "$technicianId",
  "serviceBayId": "$serviceBayId",
  "estimatedStartTimeSlotId": "$startSlotId",
  "estimatedEndTimeSlotId": "$endSlotId"
}
JSON

(curl -s -o /tmp/resp_a.json -w "%{http_code}" -H "Content-Type: application/json" -d @/tmp/booking_req.json http://localhost:5291/api/v1/appointments > /tmp/code_a.txt) &
(curl -s -o /tmp/resp_b.json -w "%{http_code}" -H "Content-Type: application/json" -d @/tmp/booking_req.json http://localhost:5291/api/v1/appointments > /tmp/code_b.txt) &
wait

echo "A=$(cat /tmp/code_a.txt)"
echo "B=$(cat /tmp/code_b.txt)"
```

Expected:
- Exactly one request returns HTTP `201 Created`
- The other returns HTTP `409 Conflict`
- Conflict body contains `"errorCode":"BOOKING_CONFLICT"`

Evidence from latest run:
- `docs/evidence/api/2026-06-28/20260628_195544/06_concurrency_overlap.request.json`
- `docs/evidence/api/2026-06-28/20260628_195544/06_concurrency_overlap.response_a.json`
- `docs/evidence/api/2026-06-28/20260628_195544/06_concurrency_overlap.response_b.json`
- `docs/evidence/api/2026-06-28/20260628_195544/06_concurrency_overlap.status.txt`

---

## Test Case 7 - Idempotency Missing Header

Description:
- Verify the create appointment endpoint still succeeds when idempotency is enabled but the `Idempotency-Key` header is not sent and the configuration does not require it.

Endpoint:
- `POST /appointments`

Request:
```bash
# Same payload as the happy path, but without the Idempotency-Key header.
curl -sS -i -X POST "http://localhost:5291/api/v1/appointments" \
  -H 'Content-Type: application/json' \
  -d '{ ... create appointment payload ... }'
```

Expected:
- HTTP `201 Created`
- Appointment is created normally
- No idempotency replay response is needed

Evidence from latest run:
- `docs/evidence/api/2026-06-29/20260629_094443/07_idempotency_missing_header.request.json`
- `docs/evidence/api/2026-06-29/20260629_094443/07_idempotency_missing_header.response.json`
- `docs/evidence/api/2026-06-29/20260629_094443/07_idempotency_missing_header.status.txt`

---

## Test Case 8 - Idempotency Header Present

Description:
- Verify the create appointment endpoint accepts a client-supplied `Idempotency-Key` header and processes the request successfully.

Endpoint:
- `POST /appointments`

Request:
```bash
# Same payload as the happy path, with a unique Idempotency-Key header.
curl -sS -i -X POST "http://localhost:5291/api/v1/appointments" \
  -H 'Content-Type: application/json' \
  -H 'Idempotency-Key: create-appointment-test-key' \
  -d '{ ... create appointment payload ... }'
```

Expected:
- HTTP `201 Created`
- Appointment is created normally
- Idempotency record is tracked for future replay

Evidence from latest run:
- `docs/evidence/api/2026-06-29/20260629_094443/08_idempotency_header_present.request.json`
- `docs/evidence/api/2026-06-29/20260629_094443/08_idempotency_header_present.response.json`
- `docs/evidence/api/2026-06-29/20260629_094443/08_idempotency_header_present.status.txt`

---

## Test Case 9 - Idempotency Duplicate Retry

Description:
- Verify duplicate retry protection by submitting two parallel create requests with the same `Idempotency-Key` and same payload.

Endpoint:
- `POST /appointments` (parallel x2, identical payload and idempotency key)

Request:
```bash
DB="host=localhost port=5432 dbname=vehicle_service_booking user=haudo password=123456xX"

row=$(psql "$DB" -At -F '|' -c "WITH candidate AS (
  SELECT sva.\"DealershipId\", sva.\"ServiceTypeId\", sva.\"TechnicianId\", sva.\"ServiceBayId\", sva.\"TimeSlotId\" AS start_slot_id,
         sva.\"RequiredSlots\", sva.\"SequenceOrder\", sva.\"QueryDate\"
  FROM \"ServiceTypeAvailability\" sva
  WHERE sva.\"CanFitService\" = TRUE
    AND sva.\"QueryDate\" >= CURRENT_DATE + INTERVAL '1 day'
  ORDER BY sva.\"QueryDate\", sva.\"SequenceOrder\"
  LIMIT 1
)
SELECT c.\"DealershipId\", c.\"ServiceTypeId\", c.\"TechnicianId\", c.\"ServiceBayId\", c.start_slot_id,
       ts_end.\"Id\" AS end_slot_id, c.\"QueryDate\"::text AS appointment_date, cust.\"Id\" AS customer_id, veh.\"Id\" AS vehicle_id
FROM candidate c
JOIN \"TimeSlots\" ts_end ON ts_end.\"SequenceOrder\" = c.\"SequenceOrder\" + c.\"RequiredSlots\" - 1
JOIN \"Customers\" cust ON TRUE
JOIN \"Vehicles\" veh ON veh.\"CustomerId\" = cust.\"Id\"
LIMIT 1;")

IFS='|' read -r dealershipId serviceTypeId technicianId serviceBayId startSlotId endSlotId appointmentDate customerId vehicleId <<< "$row"
idempotency_key="idem-$(date +%Y%m%d_%H%M%S)-$RANDOM"

cat > /tmp/idem_req.json <<JSON
{
  "dealershipId": "$dealershipId",
  "customerId": "$customerId",
  "vehicleId": "$vehicleId",
  "appointmentDate": "$appointmentDate",
  "serviceTypeId": "$serviceTypeId",
  "technicianId": "$technicianId",
  "serviceBayId": "$serviceBayId",
  "estimatedStartTimeSlotId": "$startSlotId",
  "estimatedEndTimeSlotId": "$endSlotId"
}
JSON

(curl -s -o /tmp/idem_resp_a.json -w "%{http_code}" -H "Content-Type: application/json" -H "Idempotency-Key: $idempotency_key" -d @/tmp/idem_req.json http://localhost:5291/api/v1/appointments > /tmp/idem_code_a.txt) &
(curl -s -o /tmp/idem_resp_b.json -w "%{http_code}" -H "Content-Type: application/json" -H "Idempotency-Key: $idempotency_key" -d @/tmp/idem_req.json http://localhost:5291/api/v1/appointments > /tmp/idem_code_b.txt) &
wait

echo "A=$(cat /tmp/idem_code_a.txt)"
echo "B=$(cat /tmp/idem_code_b.txt)"
```

Expected:
- Both requests return HTTP `201 Created`
- Both responses contain the same `appointmentId`
- Replay response body matches API contract shape

Evidence from latest successful run:
- `docs/evidence/api/2026-06-29/20260629_094443/09_idempotency_retry.request.json`
- `docs/evidence/api/2026-06-29/20260629_094443/09_idempotency_retry.response_a.json`
- `docs/evidence/api/2026-06-29/20260629_094443/09_idempotency_retry.response_b.json`
- `docs/evidence/api/2026-06-29/20260629_094443/09_idempotency_retry.status.txt`

---

## Execution Checklist

- [ ] Seed scripts executed successfully
- [ ] API running on port `5291`
- [ ] Availability happy case passed (`200`)
- [ ] Create appointment happy case passed (`201`)
- [ ] Get appointment by id happy case passed (`200`)
- [ ] Invalid GUID validation passed (`400`)
- [ ] Past date validation passed (`400`)
- [ ] Idempotency missing-header case passed (`201`)
- [ ] Idempotency header-present case passed (`201`)
- [ ] Concurrency overlap race passed (one `201`, one `409 BOOKING_CONFLICT`)
- [ ] Idempotency duplicate retry passed (two `201` with same `appointmentId`)

---

## Recommendations

1. Keep `Idempotency-Key` optional by default for this phase, because the current implementation and test suite prove the endpoint works both with and without the header.
2. Document the header explicitly in client-facing API docs so retry-capable clients know they can opt into replay protection.
3. Keep the concurrency overlap test in the automated script, because it still protects the booking invariant even after idempotency was added.
4. If you later need strict client enforcement, change `Idempotency.RequireHeader` to `true` and update the script to expect `400` for the missing-header case.

---

## Troubleshooting

API port already in use:
```bash
lsof -iTCP:5291 -sTCP:LISTEN -n -P
```

If needed, stop the process and restart API:
```bash
kill <pid>
```

If availability is empty:
- Re-run seed scripts.
- Confirm `target_date` is tomorrow.
- Confirm IDs match seeded records.

If create appointment returns conflict/invalid operation:
- Use a currently available slot from the availability response.
- Ensure technician/serviceBay/slot IDs are a valid combination for that date.
