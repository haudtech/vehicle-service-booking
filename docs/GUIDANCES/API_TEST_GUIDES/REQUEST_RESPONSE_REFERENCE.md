# Request/Response Reference

Detailed request and response behavior for booking and availability endpoints.

## 🚀 REQUEST/RESPONSE FLOW (Detailed)

```text
QUERY AVAILABILITY (Multi-Service Support)
===========================================

Request:
  GET /api/v1/availability?
      dealershipId=550e8400-e29b-41d4-a716-446655440000&
      serviceTypeId=550e8400-e29b-41d4-a716-446655440001&
      date=2026-06-25

Controller:
  AvailabilityController.GetAvailableSlots(
    [FromQuery] GetAvailabilityRequest request)
    Fields: DealershipId, ServiceTypeId (single), Date

Service:
  AvailabilityService.GetAvailableSlotsAsync(dealershipId, serviceTypeId, date)
    → Returns: List<AvailabilityOption>

Response (HTTP 200) — List<AvailabilityOptionResponse>:
  [
    {
      "slotStart": "2026-06-25T10:00:00Z",
      "slotEnd":   "2026-06-25T11:00:00Z",
      "technicianId": "550e8400-e29b-41d4-a716-446655440002",
      "serviceBayId": "550e8400-e29b-41d4-a716-446655440003"
    },
    {
      "slotStart": "2026-06-25T10:30:00Z",
      "slotEnd":   "2026-06-25T11:30:00Z",
      "technicianId": "550e8400-e29b-41d4-a716-446655440004",
      "serviceBayId": "550e8400-e29b-41d4-a716-446655440005"
    }
  ]
  (ServiceBayId IS returned — client uses it in CreateAppointmentRequest)


CREATE APPOINTMENT (With Client-Chosen Resources & Re-Validation)
=================================================================

Request (HTTP POST):
  POST /api/v1/appointments
  Headers:
    Idempotency-Key: <uuid>   (optional)
  Body (flat — one service per request, CreateAppointmentRequest):
  {
    "dealershipId": "550e8400-e29b-41d4-a716-446655440000",
    "customerId": "550e8400-e29b-41d4-a716-446655440010",
    "vehicleId": "550e8400-e29b-41d4-a716-446655440011",
    "appointmentDate": "2026-06-25",
    "serviceTypeId": "550e8400-e29b-41d4-a716-446655440001",
    "technicianId": "550e8400-e29b-41d4-a716-446655440002",
    "serviceBayId": "550e8400-e29b-41d4-a716-446655440003",
    "estimatedStartTimeSlotId": "00000000-0000-0000-0000-000000000005",
    "estimatedEndTimeSlotId":   "00000000-0000-0000-0000-000000000006"
  }

Controller:
  AppointmentsController.Create(
    CreateAppointmentRequest request)

Service:
  AppointmentService.CreateAsync(request)
  
    ├─ STEP 1: RE-VALIDATE Client-Chosen Resources
    │  │
    │  │ For each service in request:
    │  ├─ Query AvailabilityService to get current available options
    │  ├─ Verify (TechnicianId, ServiceBayId) pair is in current availability
    │  ├─ Verify time slots still available
    │  ├─ Check if no other conflicts exist
    │  │
    │  └─ If client-chosen resources NOT in availability: throw BookingConflictException
    │     (Client must re-query availability and choose again)
    │
    ├─ STEP 2: Verify Technician Has Required Skill
    │  │
    │  └─ Query database to confirm technician certified for service type
    │
    ├─ STEP 3: Create Appointment entity
    │  ├─ AppointmentDate (DateOnly)
    │  ├─ DealershipId, CustomerId, VehicleId
    │  └─ StatusId (default: Booked)
    │
    ├─ STEP 4: Create Service entities (using CLIENT-PROVIDED resources):
    │  ├─ EstimatedStartTimeSlotId / EndTimeSlotId (from request)
    │  ├─ BookingDate [snapshot from AppointmentDate]
    │  ├─ EstimatedStartSlotSequence / EndSlotSequenceExclusive [from TimeSlot lookup]
    │  ├─ ServiceTypeId [from request]
    │  ├─ TechnicianId [from request - client selected from availability]
    │  ├─ ServiceBayId [from request - client selected from availability]
    │  ├─ SequenceOrder [from request]
    │  └─ ServiceStatusId (default: Pending)
    │
    ├─ STEP 5: Final database-level validation
    │  └─ PostgreSQL constraints enforce:
    │     • No duplicate (AppointmentId, ServiceTypeId, SequenceOrder)
    │     • EXC_Service_Tech_Date_SeqRange_NoOverlap (technician overlap)
    │     • EXC_Service_Bay_Date_SeqRange_NoOverlap (bay overlap)
    │     • Foreign key integrity
    │
    └─ STEP 6: Transactional save
       ├─ _dbContext.Appointments.Add(appointment)
       ├─ _dbContext.Services.AddRange(services)
       └─ await _dbContext.SaveChangesAsync()
            │
            ▼ Atomic transaction
          Database Persisted (all-or-nothing)

Response (HTTP 201 Created):
  Location: /api/v1/appointments/550e8400-e29b-41d4-a716-446655440006
  Body (CreateAppointmentResponse):
  {
    "appointmentId": "550e8400-e29b-41d4-a716-446655440006",
    "slotStart": "2026-06-25T10:00:00Z",
    "slotEnd":   "2026-06-25T11:00:00Z",
    "createdAt": "2026-06-25T09:30:00Z"
  }

Error Responses:
  400 Bad Request - FluentValidation failures
  400 Bad Request - INVALID_OPERATION (e.g. slot mismatch, no available slots)
  400 Bad Request - IDEMPOTENCY_KEY_REQUIRED / IDEMPOTENCY_KEY_INVALID
  409 Conflict  - BOOKING_CONFLICT (constraint or pre-check)
  409 Conflict  - IDEMPOTENCY_CONFLICT (payload mismatch)
  500 Internal Server Error - Unexpected error


CRITICAL ARCHITECTURAL PATTERNS:
================================

1. TWO-STAGE VALIDATION (TOCTOU Prevention):
   Stage 1: AvailabilityService re-validates BEFORE modifying state
   Stage 2: PostgreSQL constraints validate DURING commit
   Result: Prevents race conditions and concurrent overbooking

2. SERVICEBAYID VISIBILITY & FLOW:
   Query Response:  ✓ Exposed (client needs it to book)
   Request Body:    ✓ Accepted (client chooses from availability)
   Appointment Response: ✓ Exposed (confirmation of what was booked)
   Result: Full transparency; client makes informed choice, server validates

3. RESOURCE SELECTION MODEL:
   Client queries availability and gets all valid (TechnicianId, ServiceBayId) combinations
   Client chooses one combination and includes it in create request
   AppointmentService pre-validates the chosen combo still exists in availability
   PostgreSQL constraints provide final race-condition guard
   Result: Client control + server-enforced safety

4. TWO-LAYER VALIDATION FOR RESOURCES:
   Layer 1 (AvailabilityService pre-check): Verify (Tech, Bay) pair is available
   Layer 2 (PostgreSQL exclusion constraints): Enforce no overlaps at DB level
   Both layers must pass for booking to succeed
```
