# Entity Relationship Diagram (ERD) Analysis
## Vehicle Service Booking System

**Date:** June 29, 2026  
**Database:** PostgreSQL  
**Status:** CURRENT - REFLECTS ACTUAL DATABASE SCHEMA ✅

---

## 📊 Current Database Schema (Updated After Refactoring)

### Architecture Overview

The system currently uses a **slot-based Service scheduling model** where:
- **Appointment** holds booking date (DateOnly) and overall status
- **Service** holds resource assignment plus slot-window persistence (`BookingDate`, `EstimatedStartSlotSequence`, `EstimatedEndSlotSequenceExclusive`)
- **TimeSlot** is the static scheduling grid used for start/end references
- PostgreSQL exclusion constraints enforce no-overlap for technician/service bay at database level

---

## 🏗️ Core Entities (16 Total)

### **1. Dealership** (Central Hub)
- **Purpose:** Represents a dealership/service center
- **Fields:**
  - `Id` (GUID, PK)
  - `Name` (string, required, max 150)
  - `Address` (string, required, max 500)
  - `IsActive` (bool, default true)
  - `CreatedAt`, `UpdatedAt`
- **Relationships:** 1:M with Technician, ServiceBay, BusinessHours, Appointment, Service

### **2. Customer**
- **Purpose:** Represents a customer who books services
- **Fields:**
  - `Id` (GUID, PK)
  - `FirstName` (string, max 100, required)
  - `LastName` (string, max 100, required)
  - `Email` (string, max 254, required)
  - `PhoneNumber` (string, max 20, required)
  - `IsActive` (bool, default true)
  - `CreatedAt`, `UpdatedAt`
- **Relationships:** 1:M with Vehicle, Appointment

### **3. Vehicle**
- **Purpose:** Represents a vehicle that receives service
- **Fields:**
  - `Id` (GUID, PK)
  - `CustomerId` (GUID, FK, required)
  - `Vin` (string, max 17, required)
  - `Make` (string, max 20, required)
  - `Model` (string, max 50, required)
  - `Year` (int?, optional, constrained)
  - `IsActive` (bool, default true)
  - `CreatedAt`, `UpdatedAt`
- **Relationships:** M:1 with Customer, 1:M with Appointment

### **4. Appointment** ✅ (REFACTORED)
- **Purpose:** Represents a service booking
- **Key Change:** Removed direct Start/End datetime storage from Appointment; scheduling is persisted on Service
- **Fields:**
  - `Id` (GUID, PK)
  - `DealershipId` (GUID, FK, required)
  - `CustomerId` (GUID, FK, required)
  - `VehicleId` (GUID, FK, required)
  - `AppointmentDate` (DateOnly, required)
  - `StatusId` (GUID, FK, required)
  - `Notes` (text, required)
  - `IsActive` (bool, default true)
  - `CreatedAt`, `UpdatedAt`
- **Relationships:**
  - M:1 with Dealership (Restrict on delete)
  - M:1 with Customer (Restrict on delete)
  - M:1 with Vehicle (Restrict on delete)
  - M:1 with AppointmentStatusLookup (Restrict on delete)
  - 1:M with Service (Cascade on delete)

### **5. Service** ✅ (JUNCTION TABLE WITH SLOT-RANGE PERSISTENCE)
- **Purpose:** Represents individual services within an appointment
- **Fields:**
  - `Id` (GUID, PK)
  - `AppointmentId` (GUID, FK, required)
  - `ServiceTypeId` (GUID, FK, required)
  - `TechnicianId` (GUID, FK, optional - SetNull on delete)
  - `ServiceBayId` (GUID, FK, optional - SetNull on delete)
  - `DealershipId` (GUID, FK, required) - Denormalized for efficiency
  - `ServiceStatusId` (GUID, FK, required)
  - `EstimatedStartTimeSlotId` (GUID, FK, optional)
  - `EstimatedEndTimeSlotId` (GUID, FK, optional)
  - `BookingDate` (DateOnly, required)
  - `EstimatedStartSlotSequence` (int, required)
  - `EstimatedEndSlotSequenceExclusive` (int, required)
  - `SequenceOrder` (int, required)
  - `ActualStartTime` (DateTime, optional)
  - `ActualEndTime` (DateTime, optional)
  - `Notes` (string, max 500, required)
  - `IsActive` (bool, default true)
  - `CreatedAt`, `UpdatedAt`
- **Constraints:**
  - Unique(AppointmentId, ServiceTypeId, SequenceOrder)
  - Exclusion constraint: `EXC_Service_Tech_Date_SeqRange_NoOverlap`
  - Exclusion constraint: `EXC_Service_Bay_Date_SeqRange_NoOverlap`
- **Relationships:**
  - M:1 with Appointment (Cascade on delete)
  - M:1 with ServiceType (Restrict on delete)
  - M:1 with Technician (optional, SetNull on delete)
  - M:1 with ServiceBay (optional, SetNull on delete)
  - M:1 with Dealership (Restrict on delete)
  - M:1 with ServiceStatusLookup (Restrict on delete)
  - M:1 with TimeSlot (start/end slot references, Restrict on delete)
- **Business Logic:**
  - Scheduling authority is slot-based persistence columns
  - Overlap protection is enforced in PostgreSQL, not only in service-layer checks

### **6. ServiceType**
- **Purpose:** Represents service categories
- **Fields:**
  - `Id` (GUID, PK)
  - `Name` (string, max 100, required)
  - `DurationMinutes` (int, required)
  - `Price` (decimal(10,2), required, default 0)
  - `IsActive` (bool, default true)
  - `CreatedAt`, `UpdatedAt`
- **Relationships:** 1:M with Service, N:M with Technician (via TechnicianSkill)

### **7. Technician**
- **Purpose:** Represents service technicians
- **Fields:**
  - `Id` (GUID, PK)
  - `DealershipId` (GUID, FK, required)
  - `FirstName` (string, required)
  - `LastName` (string, required)
  - `IsActive` (bool, default true)
  - `CreatedAt`, `UpdatedAt`
- **Relationships:**
  - M:1 with Dealership (Cascade on delete)
  - 1:M with TechnicianSchedule (Cascade on delete)
  - N:M with ServiceType (via TechnicianSkill)
  - 1:M with Service (SetNull on delete)

### **8. TechnicianSchedule**
- **Purpose:** Represents technician working hours per day of week
- **Fields:**
  - `Id` (GUID, PK)
  - `TechnicianId` (GUID, FK, required)
  - `DayOfWeek` (enum/int, required)
  - `StartTime` (TimeOnly, required)
  - `EndTime` (TimeOnly, required)
  - `IsActive` (bool, default true)
  - `CreatedAt`, `UpdatedAt`
- **Relationships:** M:1 with Technician (Cascade on delete)

### **9. TechnicianSkill** (N:M Junction Table)
- **Purpose:** Links Technicians to ServiceTypes they can perform
- **Fields:**
  - `Id` (GUID, PK)
  - `TechnicianId` (GUID, FK, required)
  - `ServiceTypeId` (GUID, FK, required)
  - `IsActive` (bool, default true)
  - `CreatedAt`, `UpdatedAt`
- **Constraints:**
  - Unique(TechnicianId, ServiceTypeId)
- **Relationships:**
  - M:1 with Technician (Cascade on delete)
  - M:1 with ServiceType (Cascade on delete)

### **10. ServiceBay**
- **Purpose:** Represents physical service bays/stations
- **Fields:**
  - `Id` (GUID, PK)
  - `DealershipId` (GUID, FK, required)
  - `Name` (string, max 50, required)
  - `IsActive` (bool, default true)
  - `CreatedAt`, `UpdatedAt`
- **Relationships:**
  - M:1 with Dealership (Cascade on delete)
  - 1:M with Service (SetNull on delete)

### **11. BusinessHours**
- **Purpose:** Dealership business hours per day
- **Fields:**
  - `Id` (GUID, PK)
  - `DealershipId` (GUID, FK, required)
  - `DayOfWeek` (enum/int, required)
  - `OpenTime` (TimeOnly, required)
  - `CloseTime` (TimeOnly, required)
  - `IsActive` (bool, default true)
  - `CreatedAt`, `UpdatedAt`
- **Relationships:** M:1 with Dealership (Cascade on delete)

### **12. TimeSlot** (Lookup/Grid Table)
- **Purpose:** Defines static slot grid used for scheduling and availability
- **Fields:**
  - `Id` (GUID, PK, seeded)
  - `SequenceOrder` (int, unique, required)
  - `SlotStartTime` (TimeOnly, required)
  - `SlotEndTime` (TimeOnly, required)
  - `IsActive` (bool, default true)
  - `CreatedAt`, `UpdatedAt`
- **Seeded Values:** 18 slots (08:00-17:00 at 30-minute intervals)
- **Relationships:** 1:M with Service (as EstimatedStartTimeSlot and EstimatedEndTimeSlot)

### **13. AppointmentStatusLookup** (Lookup Table)
- **Purpose:** Defines possible appointment statuses
- **Fields:**
  - `Id` (GUID, PK, seeded)
  - `Status` (enum/int, unique, required)
  - `Name` (string, max 50, required)
  - `Description` (string, max 200, required)
  - `IsActive` (bool, default true)
  - `CreatedAt`, `UpdatedAt`
- **Seeded Values:**
  - Booked (00000000-0000-0000-0000-000000000001)
  - InProgress (00000000-0000-0000-0000-000000000002)
  - Completed (00000000-0000-0000-0000-000000000003)
  - Cancelled (00000000-0000-0000-0000-000000000004)
  - PartiallyCompleted (00000000-0000-0000-0000-000000000005)
- **Relationships:** 1:M with Appointment (Restrict on delete)

### **14. ServiceStatusLookup** (Lookup Table)
- **Purpose:** Defines possible service statuses
- **Fields:**
  - `Id` (GUID, PK, seeded)
  - `Status` (enum/int, unique, required)
  - `Name` (string, max 50, required)
  - `Description` (string, max 200, required)
  - `IsActive` (bool, default true)
  - `CreatedAt`, `UpdatedAt`
- **Seeded Values:**
  - Pending (00000000-0000-0000-0001-000000000001)
  - InProgress (00000000-0000-0000-0001-000000000002)
  - Completed (00000000-0000-0000-0001-000000000003)
  - Skipped (00000000-0000-0000-0001-000000000004)
  - Rescheduled (00000000-0000-0000-0001-000000000005)
- **Relationships:** 1:M with Service (Restrict on delete)

### **15. IdempotencyRequestStatusLookup** (Lookup Table)
- **Purpose:** Defines possible idempotency request statuses
- **Fields:**
  - `Id` (GUID, PK, seeded)
  - `Status` (enum/int, unique, required)
  - `Name` (string, max 50, required)
  - `Description` (string, max 200, required)
  - `IsActive` (bool, default true)
  - `CreatedAt`, `UpdatedAt`
- **Seeded Values:**
  - InProgress (00000000-0000-0000-0002-000000000001)
  - Completed (00000000-0000-0000-0002-000000000002)
- **Relationships:** 1:M with IdempotencyRequest (Restrict on delete)

### **16. IdempotencyRequest**
- **Purpose:** Stores idempotency metadata and replay payloads for create-appointment requests
- **Fields:**
  - `Id` (GUID, PK)
  - `IdempotencyKey` (string, max 100, required)
  - `RequestPath` (string, max 200, required)
  - `RequestHash` (string, max 128, required)
  - `StatusId` (GUID, FK, required)
  - `ResponseStatusCode` (int?, optional)
  - `ResponseBody` (text, optional)
  - `ExpiresAt` (DateTime, required)
  - `IsActive` (bool, default true)
  - `CreatedAt`, `UpdatedAt`
- **Constraints:**
  - Unique(IdempotencyKey, RequestPath)
  - Index(ExpiresAt)
- **Relationships:** M:1 with IdempotencyRequestStatusLookup (Restrict on delete)

---

## 🔑 Key Constraints

### **Referential Integrity**
| Parent | Child | Delete Behavior | Reason |
|--------|-------|---|---|
| Dealership | Appointment | Restrict | Prevent orphaning bookings |
| Dealership | Service | Restrict | Prevent orphaning service rows |
| Dealership | BusinessHours | Cascade | Remove dependent schedule rows |
| Dealership | Technician | Cascade | Remove dependent technician rows |
| Dealership | ServiceBay | Cascade | Remove dependent bay rows |
| Customer | Appointment | Restrict | Prevent orphaning bookings |
| Customer | Vehicle | Cascade | Remove dependent vehicles |
| Vehicle | Appointment | Restrict | Prevent orphaning bookings |
| Appointment | Service | Cascade | Delete services with appointment |
| ServiceType | Service | Restrict | Preserve service type integrity |
| ServiceType | TechnicianSkill | Cascade | Remove dependent skills |
| Technician | Service | SetNull | Allow reassignment |
| Technician | TechnicianSchedule | Cascade | Remove dependent schedule rows |
| Technician | TechnicianSkill | Cascade | Remove dependent skills |
| ServiceBay | Service | SetNull | Allow reassignment |
| TimeSlot | Service(Start/End) | Restrict | Protect slot references |
| IdempotencyRequestStatusLookup | IdempotencyRequest | Restrict | Preserve status integrity |

### **Unique Constraints**
- `AppointmentStatusLookup.Status`: Unique enum values
- `ServiceStatusLookup.Status`: Unique enum values
- `IdempotencyRequestStatusLookup.Status`: Unique enum values
- `Service(AppointmentId, ServiceTypeId, SequenceOrder)`: No duplicate services in appointment
- `TechnicianSkill(TechnicianId, ServiceTypeId)`: No duplicate skills per technician
- `TimeSlot.SequenceOrder`: Unique slot sequence
- `IdempotencyRequest(IdempotencyKey, RequestPath)`: Unique idempotency key per route

### **Data Type / Check Constraints**
- `Vehicle.Year`: Range 1900-2100
- `Vehicle.Vin`: Exactly 17 characters
- `AppointmentDate`: DateOnly (no time component)
- `TechnicianSchedule.StartTime/EndTime`: TimeOnly
- `BusinessHours.OpenTime/CloseTime`: TimeOnly
- `ServiceType.DurationMinutes`: 30..480 (`CK_ServiceType_DurationMinutes_Range`)
- `ServiceType.Price`: >= 0 (`CK_ServiceType_Price_NonNegative`)

### **PostgreSQL Concurrency Constraints**
- `EXC_Service_Tech_Date_SeqRange_NoOverlap`: Prevents overlapping slot ranges per technician per date
- `EXC_Service_Bay_Date_SeqRange_NoOverlap`: Prevents overlapping slot ranges per service bay per date

---

## 📊 Relationship Summary

| From | To | Cardinality | Delete Behavior | Type |
|------|----|----|---|---|
| Dealership | Technician | 1:M | Cascade | Navigation |
| Dealership | ServiceBay | 1:M | Cascade | Navigation |
| Dealership | BusinessHours | 1:M | Cascade | Navigation |
| Dealership | Appointment | 1:M | Restrict | Navigation |
| Dealership | Service | 1:M | Restrict | Denormalized FK |
| Customer | Vehicle | 1:M | Cascade | Navigation |
| Customer | Appointment | 1:M | Restrict | Navigation |
| Vehicle | Appointment | 1:M | Restrict | Navigation |
| Appointment | Service | 1:M | **Cascade** | Navigation |
| Appointment | Status | M:1 | Restrict | Navigation |
| ServiceType | Service | 1:M | Restrict | Navigation |
| ServiceType | TechnicianSkill | 1:M | Cascade | Junction |
| Technician | Service | 1:M | **SetNull** | Navigation |
| Technician | Schedule | 1:M | Cascade | Navigation |
| Technician | Skills | 1:M | Cascade | Junction |
| ServiceBay | Service | 1:M | **SetNull** | Navigation |
| Service | Status | M:1 | Restrict | Navigation |
| Service | StartTimeSlot | M:1 | Restrict | Navigation |
| Service | EndTimeSlot | M:1 | Restrict | Navigation |
| IdempotencyRequest | IdempotencyStatus | M:1 | Restrict | Navigation |

---

## 🎯 Design Principles Applied

### **1. Separation of Concerns** ✅
- **Appointment** = Booking date and overall status
- **Service** = Assignment plus slot-range scheduling and execution status
- This enables:
  - Multiple services per appointment
  - Deterministic conflict handling at row level
  - Database-backed race protection

### **2. Denormalization for Performance** ✅
- **Service.DealershipId**: Denormalized for faster filtering
- **Service.BookingDate + slot sequences**: Persisted for efficient overlap constraints and conflict checks

### **3. Flexible Assignment** ✅
- **Service.TechnicianId**: Optional (can be assigned later)
- **Service.ServiceBayId**: Optional (can be assigned later)
- Both use `SetNull` on delete

### **4. Audit Trail** ✅
- All entities inherit from `BaseEntity`
- Include `CreatedAt` and `UpdatedAt`
- Database defaults use `CURRENT_TIMESTAMP`
- Common active-state filtering uses `IsActive`

### **5. Domain-Driven Status Modeling** ✅
- Lookup tables for statuses:
  - AppointmentStatusLookup
  - ServiceStatusLookup
  - IdempotencyRequestStatusLookup
- FK-based status references prevent invalid free-form state values

### **6. Type Safety and Grid Scheduling** ✅
- `AppointmentDate` uses `DateOnly`
- Schedules use `TimeOnly`
- Service planning uses `TimeSlot` references + slot sequence windows
- Actual execution uses nullable `DateTime` (`ActualStartTime`, `ActualEndTime`)

### **7. Idempotency for Safe Retries** ✅
- `IdempotencyRequest` stores request fingerprint and replay payload
- Uniqueness on `(IdempotencyKey, RequestPath)` prevents duplicate logical create operations

---

## 📝 Migration Notes

**Changes from Earlier Schema Revisions:**
1. **Service scheduling persistence model updated:**
   - Slot references retained (`EstimatedStartTimeSlotId`, `EstimatedEndTimeSlotId`)
   - Added persisted invariants:
     - `BookingDate`
     - `EstimatedStartSlotSequence`
     - `EstimatedEndSlotSequenceExclusive`

2. **PostgreSQL conflict protection added:**
   - Added `btree_gist`-based exclusion constraints for technician/bay overlap prevention

3. **Idempotency model evolved:**
   - Added `IdempotencyRequest`
   - Converted status from direct enum column to FK lookup via `IdempotencyRequestStatusLookup`

4. **ServiceType enhancements:**
   - Added `Price` with non-negative check constraint

5. **Base entity active-flag standardization:**
   - `IsActive` added and used broadly across entities and views

---

## ✅ Schema Validation Checklist

- ✅ All entities inherit from BaseEntity
- ✅ All PKs are GUIDs
- ✅ FK relationships align with current delete behaviors
- ✅ `CreatedAt`/`UpdatedAt` defaults exist on entities
- ✅ `IsActive` defaulting exists on entities
- ✅ Lookup tables are seeded and constrained
- ✅ TimeSlot grid is seeded and uniquely ordered
- ✅ Service overlap invariants enforced in PostgreSQL
- ✅ Idempotency storage normalized with status lookup FK
- ✅ Availability views support current scheduling model

---

## 🔍 Query Examples

### **Get Appointment with All Details**
```csharp
var appointment = await context.Appointments
    .Include(a => a.Dealership)
    .Include(a => a.Customer)
    .Include(a => a.Vehicle)
    .Include(a => a.Status)
    .Include(a => a.Services)
        .ThenInclude(s => s.ServiceType)
    .Include(a => a.Services)
        .ThenInclude(s => s.Technician)
    .Include(a => a.Services)
        .ThenInclude(s => s.ServiceBay)
    .Include(a => a.Services)
        .ThenInclude(s => s.ServiceStatus)
    .Include(a => a.Services)
        .ThenInclude(s => s.EstimatedStartTimeSlot)
    .Include(a => a.Services)
        .ThenInclude(s => s.EstimatedEndTimeSlot)
    .FirstOrDefaultAsync(a => a.Id == id);
```

### **Find Conflicting Services (Slot-Range Overlap)**
```csharp
var conflicts = await context.Services
    .Where(s => s.TechnicianId == technicianId
        && s.DealershipId == dealershipId
        && s.BookingDate == bookingDate
        && s.EstimatedStartSlotSequence < requestedEndSlotExclusive
        && s.EstimatedEndSlotSequenceExclusive > requestedStartSlot)
    .ToListAsync();
```

### **Get Technician Availability Basis (Schedule + Existing Services)**
```csharp
var schedule = await context.TechnicianSchedules
    .FirstOrDefaultAsync(ts =>
        ts.TechnicianId == technicianId
        && ts.DayOfWeek == (int)date.DayOfWeek);

var servicesOnDay = await context.Services
    .Where(s => s.TechnicianId == technicianId
        && s.BookingDate == DateOnly.FromDateTime(date))
    .OrderBy(s => s.EstimatedStartSlotSequence)
    .ToListAsync();
```

---

## 📈 Database Statistics

- **Total Entities:** 16
- **Base Relationships (FKs):** 20
- **Many-to-Many (via junction):** 1 (TechnicianSkill)
- **Lookup Tables:** 3
- **Exclusion Constraints:** 2 (Service overlap protection)
- **Key Unique Constraints:** 7+
- **Cascade Delete Paths:** Dealership→BusinessHours/ServiceBays/Technicians, Appointment→Service, Technician→Schedules/Skills, Customer→Vehicles, ServiceType→TechnicianSkills

---

## 🚀 Performance Considerations

### **Indexed Columns (Current Important Set)**
- `Services.AppointmentId` and composite unique `(AppointmentId, ServiceTypeId, SequenceOrder)`
- `Services(DealershipId, BookingDate)`
- `Services(TechnicianId, BookingDate)`
- `Services(ServiceBayId, BookingDate)`
- `IdempotencyRequests(ExpiresAt)`
- `IdempotencyRequests(IdempotencyKey, RequestPath)` unique
- `TimeSlots.SequenceOrder` unique

### **Denormalization Benefits**
- `Service.DealershipId`: Enables direct dealership filtering
- `Service.BookingDate + slot sequences`: Enables fast overlap checks and deterministic exclusion constraints

### **Query Patterns**
- Eager loading for appointment read models
- Availability queries via SQL views/materialized refresh flows
- Slot-range conflict checks before persistence; database constraints remain final guard

---

**Document Version:** 3.0  
**Status:** ✅ CURRENT - Reflects Actual Schema  
**Last Updated:** June 29, 2026  
**Database:** PostgreSQL  
**Architecture:** Clean Architecture with Domain-Driven Design
