# 📊 Data & Database Architecture View

**Purpose:** Single source of truth for physical storage design, slot-based scheduling mathematics, and database-level concurrency protection.
**Database Engine:** PostgreSQL 13+ (Relational with `btree_gist` extensions)
**Status:** CURRENT - REFLECTS PRODUCTION SCHEMA ✅

---

## 🏗 1. ENTITY RELATIONSHIP DIAGRAM (ERD)

The system utilizes a normalized schema consisting of **16 tables** designed around a slot-range persistence model. Scheduling authority is decoupled from the `Appointment` container and embedded directly within individual `Service` allocations.

---

## ⏱ 2. SLOT MATHEMATICS & SCHEDULING ALGORITHM

The system completely offloads slot validation, window building, and sequential verification to the database layer via regular SQL Views. The C# application layer executes zero complex loops for grid generation.

### Static Reference Grid (`TimeSlots` Seed Data)
The operating day is bound to an 18-slot static architecture from **08:00 to 17:00** at deterministic **30-minute intervals** (Seq 1-18).

### Mathematical Formula for Required Slots
To determine how many consecutive 30-minute slots a service requires, the system applies a simple ceiling division formula directly inside the database view:

Required Slots = Ceiling( Service Duration Minutes / 30 )

Here is exactly how this formula translates into real-world business scenarios:

*   ⚙️ **Example A (Oil Change - 60 Minutes):** 
    Calculation: 60 / 30 = 2.0 -> Requires exactly 2 consecutive slots.
    
*   🛑 **Example B (Brake Inspection - 90 Minutes):** 
    Calculation: 90 / 30 = 3.0 -> Requires exactly 3 consecutive slots.
    
*   🔍 **Example C (Advanced Diagnostic - 45 Minutes):** 
    Calculation: 45 / 30 = 1.5 -> Rounded up to 2 consecutive slots.


### Inside the `ServiceTypeAvailabilityView` (SQL Logic)
To determine if a block fits, a lateral series is generated across candidate start positions (`tas.SequenceOrder` to `tas.SequenceOrder + RequiredSlots - 1`) and grouped, ensuring non-fitting windows are dropped at the database level.

---

## 🔐 3. CONCURRENCY PROTECTION & RACE CONSTRAINTS

To stop **Time-Of-Check-Time-Of-Use (TOCTOU)** vulnerabilities, the architecture operates a strict **Two-Stage Validation Strategy**.

### Layer 1: Application Layer Pre-Validation
Before executing entity building blocks, `AppointmentService` performs early failures checks by calling the availability view.

### Layer 2: PostgreSQL Invariant Exclusion Constraints (The Ultimate Guard)
If two identical booking commands bypass Layer 1, the database acts as the single source of truth using a **generalized search tree (`gist`) index range constraint**. 

Applied via Migration `AddServiceBookingConflictInvariant`, these rules prevent any active row overlap on the same date and slot boundaries for Technicians and Service Bays.

### Exception Mapping Execution Flow
When a violation occurs during `_dbContext.SaveChangesAsync()`, the repository catches the standard state and bubbles it safely up as a `BookingConflictException`.

---

## 📈 4. PERFORMANCE & DATA INDEXING STRATEGY

By migrating from heavy C# loop checking to direct index-backed SQL database views, read transactions bypass memory allocations, drastically reducing latency.

### Core Performance Index Topology
1.  **Composite Unique Index:** `IX_Services_AppId_TypeId_Seq`
2.  **Technician Query Performance Index:** `IX_Services_Tech_Date`
3.  **Service Bay Query Performance Index:** `IX_Services_Bay_Date`
4.  **Idempotency Isolation Key:** `IX_Idempotency_Key_Path`

---

## 🔐 5. REFERENTIAL INTEGRITY MATRIX

The system operates under strict cascading structures configured via Fluent API, ensuring data consistency (e.g., Appointment cascading to Services, Technician setting Null).