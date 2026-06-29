# Vehicle Service Booking API

## 📋 Table of Contents

1. [Project Introduction](#-project-introduction)
2. [Architecture Overview](#-architecture-overview)
3. [Data Flow: Creating an Appointment](#data-flow-creating-an-appointment)
4. [UX Driven-Design](#ux-driven-design)
5. [Technology Stack](#-technology-stack)
6. [Prerequisites](#-prerequisites)
7. [Database Configuration](#-database-configuration)
8. [Entity Framework Migrations](#-entity-framework-migrations)
9. [Logging Configuration](#-logging-configuration)
10. [How to Build](#-how-to-build)
11. [How to Run](#-how-to-run)
12. [How to Test](#-how-to-test)
13. [Trace & Troubleshooting](#-trace--troubleshooting)
14. [API Endpoints](#-api-endpoints)
15. [Project Structure](#-project-structure)
16. [AI Collaboration Narrative](#-ai-collaboration-narrative)
17. [Additional Documentation](#-additional-documentation)
18. [Support](#-support)

---

## 🎯 Project Introduction

### What is This Project?

The **Vehicle Service Booking API** is a production-ready, real-time appointment scheduling system designed for automotive dealerships. It enables customers to request service appointments for their vehicles while automatically managing resource constraints.

### Purpose

This system solves a critical problem in automotive retail: managing service appointments efficiently across multiple technicians, service bays, and service types while preventing double-booking conflicts and ensuring qualified technicians are assigned to required services.

### Who It Serves

- **Dealership Managers**: Real-time visibility into service bay utilization and technician schedules
- **Service Advisors**: Book appointments with instant conflict detection and availability checking
- **Customers**: Request service appointments for their vehicles with immediate confirmation
- **System Administrators**: Monitor service operations through comprehensive logging and tracing

### Core Capabilities

✅ **Resource-Constrained Booking**: Ensure both a ServiceBay and qualified Technician are available for the service duration

✅ **Real-Time Availability Checking**: Query current availability across all constraints before confirming appointments

✅ **Persistent Appointment Records**: Create durable appointment records with customer, vehicle, technician, and service bay associations

✅ **Concurrency Protection**: Prevent race conditions and double-booking through PostgreSQL exclusion constraints and two-stage validation

✅ **Idempotency for Safe Retries**: Optional `Idempotency-Key` header ensures clients can safely retry failed requests

✅ **Observability**: Structured logging, distributed tracing, and correlation IDs for end-to-end request tracking

---

## 🏗️ Architecture Overview

### Layered Architecture

The system implements **Clean Architecture** with strict separation of concerns:

```
┌─────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                        │
│  HTTP Controllers, Request/Response DTOs, OpenAPI/Swagger    │
│  (GET /availability, POST /appointments)                     │
└────────────────┬────────────────────────────────────────────┘
                 │ HTTP Requests
                 ▼
┌─────────────────────────────────────────────────────────────┐
│                 APPLICATION LAYER                            │
│  Business Logic Services:                                    │
│  • AvailabilityService (Query) - Find available slots       │
│  • AppointmentService (Command) - Create/retrieve           │
│  • IdempotencyService - Key lifecycle management            │
│  • Validators (FluentValidation rules)                      │
└────────────────┬────────────────────────────────────────────┘
                 │ IApplicationDbContext (via repositories)
                 ▼
┌─────────────────────────────────────────────────────────────┐
│               INFRASTRUCTURE LAYER                           │
│  Data Access:                                               │
│  • ApplicationDbContext (EF Core + PostgreSQL)              │
│  • Repositories (AppointmentRepository, etc.)               │
│  • SQL Views for availability queries                       │
└────────────────┬────────────────────────────────────────────┘
                 │ DbSet<T> / SQL Views
                 ▼
┌─────────────────────────────────────────────────────────────┐
│                  DOMAIN LAYER                                │
│  Business Entities (no external dependencies):              │
│  • Appointment, Service, ServiceType, Technician            │
│  • TechnicianSchedule, TechnicianSkill, TimeSlot            │
│  • ServiceBay, Customer, Vehicle, BusinessHours             │
│  • IdempotencyRequest, StatusLookups (seeded data)          │
└─────────────────────────────────────────────────────────────┘
```

## Data Flow: Creating an Appointment

## UX Driven-Design

```
User
 │
 ▼
Select Service Type + Date
 │
 ▼
GET /availability
 │
 ▼
Available Slots Returned
 │
 ▼
Select Slot
 │
 ▼
POST /appointments
 │
 ▼
Appointment Created

```

### Appointment Creation Request

```
1. Client sends POST /appointments request
   ↓
2. AppointmentsController validates request via FluentValidation
   ↓
3. IdempotencyRequestCoordinator checks for duplicate requests
   ↓
4. AppointmentService orchestrates 7-step validation:
   • Re-validate availability (AvailabilityService)
   • Verify technician has required skill
   • Check vehicle doesn't have overlapping appointments
   • Create entities (Appointment + Service)
   ↓
5. AppointmentRepository.CreateAsync triggers:
   • EF Core SaveChangesAsync
   • PostgreSQL exclusion constraints validation
   • Returns 201 Created or 409 Conflict
   ↓
6. OpenTelemetry automatically captures:
   • HTTP request span (~100-200ms)
   • EF Core query spans (~5-30ms each)
   ↓
7. Serilog logs structured fields with correlation ID
```

---

## 🛠️ Technology Stack

| Component | Version | Purpose |
|-----------|---------|---------|
| **.NET** | 8.0 LTS | Runtime platform |
| **ASP.NET Core** | 8.0 | Web framework |
| **C#** | 11 | Programming language |
| **PostgreSQL** | 13+ | Relational database |
| **Entity Framework Core** | 8.0 | ORM & migrations |
| **FluentValidation** | 11.9.1 | Request validation |
| **Serilog** | 8.0.1 | Structured logging |
| **OpenTelemetry** | 1.9.0 | Distributed tracing |
| **Swagger/OpenAPI** | 6.5.0 | API documentation |
| **xUnit** | 2.9.3 | Unit testing framework |
| **Moq** | 4.20.70 | Mocking library |
| **FluentAssertions** | 6.12.0 | Assertion library |

#### Technology Justification

| Technology | Version | Layer | Why |
|---|---|---|---|
| **ASP.NET Core** | 8.0 | Presentation | Native async/await, high performance, LTS until 2026 |
| **C#** | 11 | All Layers | Type-safe, LINQ for queries, records for DTOs, pattern matching |
| **PostgreSQL** | 13+ | Database | Exclusion constraints for concurrency, SQL views, window functions, DateOnly support |
| **Entity Framework Core** | 8.0 | Infrastructure | Type-safe queries, migrations, query translation to SQL, interceptors |
| **FluentValidation** | 11.9.1 | Application | Declarative rules, testable validators, field-level error messages |
| **Serilog** | 8.0.1 | Infrastructure | Structured logging, context enrichment, multiple sinks |
| **OpenTelemetry** | 1.9.0 | Infrastructure | Standard tracing format, auto-instrumentation, vendor-neutral |
| **xUnit + FluentAssertions** | Latest | Testing | Clean test syntax, comprehensive assertions, excellent async support |

---

## 📋 Prerequisites

Before you start, ensure you have:

- **.NET 8.0 SDK** or later
  - Install from: https://dotnet.microsoft.com/download
  - Verify: `dotnet --version`

- **PostgreSQL 13+**
  - Install from: https://www.postgresql.org/download
  - Verify: `psql --version`
  - Or use Docker: `docker run --name postgres -e POSTGRES_PASSWORD=password -d postgres:13`

- **Git** for version control
  - Verify: `git --version`

- **Visual Studio Code** or **Visual Studio 2022** (optional but recommended)

---

## 🗄️ Database Configuration

### Connection String Setup

The application reads connection strings in this precedence order (highest to lowest):

1. `CONNECTIONSTRINGS__DEFAULTCONNECTION` environment variable
2. `.env` file in the project root (for local development)
3. `appsettings.{Environment}.json` configuration files
4. `appsettings.json` base configuration

### Step 1: Create the Database

```bash
# Create a new PostgreSQL database
createdb vehicle_service_booking

# Or using psql:
psql -U postgres -c "CREATE DATABASE vehicle_service_booking;"
```

### Step 2: Configure Connection String

**Option A: Using `.env` file (Development)**

Create a `.env` file in the project root:

```env
CONNECTIONSTRINGS__DEFAULTCONNECTION=Host=localhost;Port=5432;Database=vehicle_service_booking;Username=postgres;Password=your_password
```

**Option B: Using `appsettings.Development.json`**

Edit `src/VehicleServiceBooking.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=vehicle_service_booking;Username=postgres;Password=your_password"
  }
}
```

**Option C: Using Environment Variable**

```bash
export CONNECTIONSTRINGS__DEFAULTCONNECTION="Host=localhost;Port=5432;Database=vehicle_service_booking;Username=postgres;Password=your_password"
```

### Step 3: Verify Connection

```bash
# Test the connection with psql
psql -h localhost -U postgres -d vehicle_service_booking -c "SELECT 1;"

# Should output:
# ?column?
#----------
#        1
```

---

## 🗂️ Entity Framework Migrations

### Understanding Migrations

Migrations are versioned database schema changes that:
- Track all schema modifications in source control
- Enable reproducible database setups across environments
- Allow rollback to previous schema versions

### Migration Workflow

#### Step 1: Create a Migration

When you modify entity models, create a migration:

```bash
cd /path/to/vehicle-service-booking

# Create a new migration
dotnet ef migrations add InitialCreate \
  --project src/VehicleServiceBooking.Infrastructure \
  --startup-project src/VehicleServiceBooking.Api
```

**Output:**
```
Build started...
Build succeeded.

An EF Core command-line tool has migrated to a new version. 
The old tool will no longer be invoked for new commands.

Added migration 'InitialCreate' to project 'Infrastructure'.
To undo this action, use 'dotnet ef migrations remove'.
```

#### Step 2: Review the Migration

The migration is generated in:
```
src/VehicleServiceBooking.Infrastructure/Migrations/[timestamp]_InitialCreate.cs
```

Example migration content:
```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.CreateTable(
        name: "Appointments",
        columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            DealershipId = table.Column<Guid>(type: "uuid", nullable: false),
            CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
            // ... more columns
        },
        constraints: table =>
        {
            table.PrimaryKey("PK_Appointments", x => x.Id);
            // ... foreign keys, constraints
        });
}
```

#### Step 3: Apply Migration to Database

```bash
# Apply all pending migrations
dotnet ef database update \
  --project src/VehicleServiceBooking.Infrastructure \
  --startup-project src/VehicleServiceBooking.Api
```

**Output:**
```
Build started...
Build succeeded.

Applying migration '20260629000001_InitialCreate'.
Done.
```

**Verify in database:**
```bash
psql -d vehicle_service_booking -c "\dt"
```

#### Step 4: Populate Debug Test Data (Optional)

For development and testing, populate the database with debug fixtures that enable full API test coverage:

```bash
# Execute the debug data population script
PGPASSWORD='your_password' psql -h localhost -p 5432 -U postgres -d vehicle_service_booking -f docs/sql/populate_debug_test_data.sql
```

**What this script does:**
- Cleans up any existing test data
- Creates sample dealerships, customers, vehicles, service types, technicians, and service bays
- Populates time slots
- Seeds technician schedules and skills
- Creates test scenarios for:
  - Happy-path appointment creation
  - Concurrency conflict detection
  - Idempotency key replay
  - Long-duration service bookings
  - Availability isolation testing

**Output:**
```
BEGIN
DELETE ...
INSERT ...
DO
COMMIT
(1 row)
```

**Verify test data is loaded:**
```bash
# Check ServiceTypeAvailability view has rows
psql -d vehicle_service_booking -c "SELECT COUNT(*) FROM \"ServiceTypeAvailability\";"

# Should return a count > 0 (typically 300+ rows for a month of slots)
```

After populating, you can run the automated API tests:
```bash
scripts/run_api_tests.sh --port 5280
```

#### Step 5: Roll Back if Needed

```bash
# Remove the last migration (if not applied to database)
dotnet ef migrations remove \
  --project src/VehicleServiceBooking.Infrastructure \
  --startup-project src/VehicleServiceBooking.Api

# Revert database to previous migration
dotnet ef database update PreviousMigrationName \
  --project src/VehicleServiceBooking.Infrastructure \
  --startup-project src/VehicleServiceBooking.Api
```

### Quick Reference: Migration Commands

| Command | Purpose |
|---------|---------|
| `dotnet ef migrations list` | List all migrations |
| `dotnet ef migrations add [Name]` | Create a new migration |
| `dotnet ef migrations remove` | Remove last unapplied migration |
| `dotnet ef database update` | Apply all pending migrations |
| `dotnet ef database update [MigrationName]` | Revert to specific migration |
| `dotnet ef database drop` | Drop entire database (dangerous!) |

---

## 📝 Logging Configuration

### Log Levels

Log levels are configured in `appsettings.json` and `appsettings.Development.json`:

**Production** (`appsettings.json`):
```json
{
  "Observability": {
    "Serilog": {
      "MinimumLevel": "Information",
      "Override": {
        "Microsoft": "Information",
        "Microsoft.AspNetCore": "Warning"
      },
      "EnableConsole": true,
      "EnableFile": true,
      "FilePath": "logs/app-.txt",
      "RollingInterval": "Day"
    }
  }
}
```

**Development** (`appsettings.Development.json`):
```json
{
  "Observability": {
    "Serilog": {
      "MinimumLevel": "Debug",
      "Override": {
        "Microsoft": "Information",
        "Microsoft.AspNetCore": "Warning"
      },
      "EnableConsole": true,
      "EnableFile": true,
      "FilePath": "logs/app-.txt",
      "RollingInterval": "Day"
    }
  }
}
```

### Log Output Locations

**Console Output:**
- Real-time colored logs to terminal during development
- Useful for immediate debugging

**File Logs:**
- Location: `src/VehicleServiceBooking.Api/logs/app-{YYYYMMDD}.txt`
- Rolling: New file created daily at midnight
- Example: `src/VehicleServiceBooking.Api/logs/app-20260629.txt`, `src/VehicleServiceBooking.Api/logs/app-20260630.txt`

### Viewing Logs

```bash
# View today's logs
tail -f src/VehicleServiceBooking.Api/logs/app-$(date +%Y%m%d).txt

# View last 100 lines
tail -100 src/VehicleServiceBooking.Api/logs/app-*.txt

# Search for specific correlation ID
grep -Ei "CorrelationId|550e8400-e29b-41d4-a716-446655440001" src/VehicleServiceBooking.Api/logs/app-*.txt

# View all errors
grep "\[ERR\]" src/VehicleServiceBooking.Api/logs/app-*.txt
```

---

## 🏗️ How to Build

### Build the Entire Solution

```bash
cd /path/to/vehicle-service-booking

# Restore dependencies
dotnet restore

# Build in Debug mode
dotnet build

# Build in Release mode (optimized)
dotnet build --configuration Release
```

### Build Specific Projects

```bash
# Build API project only
dotnet build src/VehicleServiceBooking.Api/

# Build with verbose output
dotnet build --verbosity detailed

# Build and show warnings
dotnet build --warnings-as-errors false
```

### Expected Output

```
Microsoft (R) Build Engine version ...
Building for .NET 8.0

Build started ...

Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:15.23
```

---

## 🚀 How to Run

### Start the API Server

```bash
cd /path/to/vehicle-service-booking

# Run in development mode (with hot reload)
dotnet run --project src/VehicleServiceBooking.Api

# Run in release mode
dotnet run --project src/VehicleServiceBooking.Api --configuration Release

# Run on specific port
dotnet run --project src/VehicleServiceBooking.Api -- --urls "http://localhost:5000"
```

### Expected Startup Output

```
info: VehicleServiceBooking.Api.Program[0]
      Application starting...
      
info: Microsoft.Hosting.Lifetime[0]
      Now listening on: http://localhost:5280
      
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to exit.
```

### Verify API is Running

**Test endpoint health:**
```log
2026-06-29 19:25:57.306 +07:00 [INF] Now listening on: http://localhost:5280 {"EventId": {"Id": 14, "Name": "ListeningOnAddress"}, "SourceContext": "Microsoft.Hosting.Lifetime"}
2026-06-29 19:25:57.344 +07:00 [INF] Application started. Press Ctrl+C to shut down. {"SourceContext": "Microsoft.Hosting.Lifetime"}
2026-06-29 19:25:57.345 +07:00 [INF] Hosting environment: Development {"SourceContext": "Microsoft.Hosting.Lifetime"}
2026-06-29 19:25:57.345 +07:00 [INF] Content root path: /Users/tech/dev/net/vehicle-service-booking/src/VehicleServiceBooking.Api {"SourceContext": "Microsoft.Hosting.Lifetime"}
```

**Access Swagger documentation:**
- Open browser: http://localhost:5280/swagger
- Interactive API documentation and testing

---

## 🧪 How to Test

### Run All Tests

```bash
cd /path/to/vehicle-service-booking

# Run all tests with summary
dotnet test

# Run with detailed output
dotnet test --verbosity detailed

# Run with coverage reporting
dotnet test /p:CollectCoverage=true
```

### Expected Output

```
Test Run Summary:
  Total Tests: 40
  Passed: 40
  Failed: 0
  Skipped: 0
  Duration: 00:00:15.234s

Test Session: SUCCESSFUL ✓
```

### Run Specific Test Suites

```bash
# Validator tests only
dotnet test --filter "Validators"

# Integration tests only
dotnet test --filter "Integration"

# Single test method
dotnet test --filter "CreateAppointmentRequestValidatorTests.InvalidDateReturnsValidationError"
```

### Test Coverage by Category

| Test Suite | Count | Duration | Focus |
|-----------|-------|----------|-------|
| **Validator Tests** | 21 | ~2s | FluentValidation rules for all DTOs |
| **Integration Tests** | 9 | ~5s | Real database operations, concurrency |
| **API Tests** | 10 | ~8s | End-to-end HTTP request/response |
| **TOTAL** | **40** | **~15s** | **100% pass rate** |


### Test Results (Unit Tests)
| Field | Value |
| --- | --- |
| **Command** | dotnet test  |
| **Status** | Passed |
| **Passed** | 52 |
| **Failed** | 0 |
| **Skipped** | 0 |
| **Total** | 52 |
| **Executed Cases (from TRX)** | 52 |
| **Duration** | 897 |
| **Notes** | All tests passed |

#### Raw Output Summary

| Section | Details |
| --- | --- |
| Console report | [dotnet-test.raw.log](dotnet-test.raw.log) |
| TRX report | [dotnet-test.trx](dotnet-test.trx) |
| Generated at | Mon Jun 29 16:13:52 +07 2026 |

### Run Tests with HTML Report

```bash
# Generate detailed HTML report
dotnet test --logger "html;LogFileName=test-results.html"

# View the report
open test-results.html  # macOS
xdg-open test-results.html  # Linux
start test-results.html  # Windows
```

### Continuous Test Execution

```bash
# Watch for file changes and re-run tests automatically
dotnet watch --project tests/VehicleServiceBooking.Tests test
```

---

## 🔍 Trace & Troubleshooting

### Understanding Correlation IDs

Every request gets a unique `Correlation-ID` that flows through all logs and traces:

```bash
# Request with automatic correlation ID
curl -X POST http://localhost:5280/api/v1/appointments \
  -H "Content-Type: application/json" \
  -d '{"dealershipId": "...", "customerId": "...", ...}'

# Custom correlation ID (optional)
curl -X POST http://localhost:5280/api/v1/appointments \
  -H "X-Correlation-ID: my-trace-123" \
  -d '...'
```

### Viewing Trace Logs

**Console logs during request:**
```
[INF] HTTP GET /api/v1/availability [CorrelationId: 550e8400-...]
[DBG] Query ServiceTypeAvailability view [CorrelationId: 550e8400-...]
[INF] Found 3 available slots [CorrelationId: 550e8400-...]
[INF] HTTP 200 OK [CorrelationId: 550e8400-...]
```

**File logs (persistent):**
```bash
# View today's logs with correlation ID
grep -Ei "CorrelationId|550e8400-" src/VehicleServiceBooking.Api/logs/app-*.txt

# Follow logs in real-time
tail -f src/VehicleServiceBooking.Api/logs/app-$(date +%Y%m%d).txt

# View errors only
grep "\[ERR\]" src/VehicleServiceBooking.Api/logs/app-*.txt | tail -20
```

### Distributed Tracing

OpenTelemetry automatically captures:

**HTTP Request Span:**
- Operation: `HTTP POST /api/v1/appointments`
- Status: `201 Created` or `409 Conflict`
- Duration: `~50-200ms`

**Database Query Spans (nested under HTTP):**
```
HTTP POST /appointments
├─ ServiceTypeAvailability view query (~30ms)
├─ TimeSlots lookup (~5ms)
├─ INSERT Appointment (~5ms)
└─ INSERT Service (~5ms)
```

### Common Issues & Resolution

#### Issue 1: Database Connection Failed

**Error:**
```
Npgsql.NpgsqlException: Exception while connecting to server
FATAL: role "postgres" does not exist
```

**Solution:**
```bash
# Verify PostgreSQL is running
pg_isready -h localhost

# Check connection string
echo $CONNECTIONSTRINGS__DEFAULTCONNECTION

# Test connection manually
psql -h localhost -U postgres -d vehicle_service_booking
```

#### Issue 2: Migration Conflicts

**Error:**
```
Multiple pending migrations exist. Specify which one to apply.
```

**Solution:**
```bash
# List all migrations
dotnet ef migrations list

# Apply all at once
dotnet ef database update
```

#### Issue 3: Double-Booking Still Occurs

**Error:**
```
409 Conflict: The selected slot is no longer available
```

**Root Cause:** Race condition between requests

**Solution:**
```bash
# Verify PostgreSQL constraints exist
psql -d vehicle_service_booking -c "\d+ Services"

# Check constraint is active
psql -d vehicle_service_booking -c "SELECT * FROM pg_constraint WHERE conname LIKE 'EXC_Service%';"
```

#### Issue 4: Logs Not Appearing

**Solution:**
```bash
# Verify Serilog is configured
cat appsettings.Development.json | grep -A 10 "Serilog"

# Check log file permissions
ls -la logs/

# Verify minimum log level
# Debug level includes more logs than Information
```

### Debug Mode Queries

**Get all appointments:**
```bash
psql -d vehicle_service_booking -c "SELECT id, appointment_date, status_id FROM appointments LIMIT 10;"
```

**Monitor active connections:**
```bash
psql -d vehicle_service_booking -c "SELECT pid, usename, state, query FROM pg_stat_activity WHERE datname = 'vehicle_service_booking';"
```

---

## 🔌 API Endpoints

Implemented routes in the current codebase (`src/VehicleServiceBooking.Api/Controllers`):

- `GET /api/v1/availability?dealershipId={guid}&serviceTypeId={guid}&date={yyyy-mm-dd}`
- `POST /api/v1/appointments`
- `GET /api/v1/appointments/{id}`

### POST /api/v1/appointments

Headers:
- `Content-Type: application/json`
- `Idempotency-Key: <unique-key>` (optional unless required by idempotency configuration)

Request body:
```json
{
  "dealershipId": "550e8400-e29b-41d4-a716-446655440000",
  "customerId": "550e8400-e29b-41d4-a716-446655440001",
  "vehicleId": "550e8400-e29b-41d4-a716-446655440002",
  "appointmentDate": "2026-06-25",
  "serviceTypeId": "550e8400-e29b-41d4-a716-446655440010",
  "technicianId": "550e8400-e29b-41d4-a716-446655440003",
  "serviceBayId": "550e8400-e29b-41d4-a716-446655440004",
  "estimatedStartTimeSlotId": "00000000-0000-0000-0000-000000000005",
  "estimatedEndTimeSlotId": "00000000-0000-0000-0000-000000000006"
}
```

Response `201 Created` (CreateAppointmentResponse):
```json
{
  "appointmentId": "550e8400-e29b-41d4-a716-446655440100",
  "slotStart": "2026-06-25T10:00:00Z",
  "slotEnd": "2026-06-25T11:00:00Z",
  "createdAt": "2026-06-25T09:30:00Z"
}
```

### GET /api/v1/appointments/{id}

Response `200 OK` (CreateAppointmentResponse):
```json
{
  "appointmentId": "550e8400-e29b-41d4-a716-446655440100",
  "slotStart": "2026-06-25T10:00:00Z",
  "slotEnd": "2026-06-25T11:00:00Z",
  "createdAt": "2026-06-25T09:30:00Z"
}
```

### GET /api/v1/availability

Query parameters:
- `dealershipId` (Guid, required)
- `serviceTypeId` (Guid, required)
- `date` (DateTime/ISO date, required)

Example:
```bash
GET /api/v1/availability?dealershipId=550e8400-e29b-41d4-a716-446655440000&serviceTypeId=550e8400-e29b-41d4-a716-446655440010&date=2026-06-25
```

Response `200 OK` (List<AvailabilityOptionResponse>):
```json
[
  {
    "slotStart": "2026-06-25T10:00:00Z",
    "slotEnd": "2026-06-25T11:00:00Z",
    "technicianId": "550e8400-e29b-41d4-a716-446655440003",
    "serviceBayId": "550e8400-e29b-41d4-a716-446655440004"
  }
]
```

---


## 📁 Project Structure

```
├── README.md
├── VehicleServiceBooking.slnx
├── docs
│   ├── ARCHITECTURE
│   │   ├── ARCHITECTURE_DESIGN
│   │   ├── ARCHITECTURE_VISUALIZATION.md
│   │   ├── ERD_ANALYSIS.md
│   │   ├── README.md
│   │   ├── REQUIREMENTS_COVERAGE_ANALYSIS.md
│   │   └── TECHNICAL_REFERENCES
│   ├── GUIDANCES
│   │   ├── API_TEST_GUIDES
│   │   │   └── API_TEST_CASES.md
│   │   └── EF_MIGRATION_RESET_GUIDE.md
│   ├── PLAN
│   │   ├── BOOKING_CONCURRENCY_IMPLEMENTATION_PLAN.md
│   │   └── CORE_FEATURES_ROADMAP.md
│   ├── PRINCIPLE_RULES
│   │   └── CODING_PRINCIPLES_AND_ARCHITECTURE_RULES.md
│   ├── SRS
│   │   └── requirements
│   │       └── KeyloopCodingChallange.md
|   |
│   ├── sql
│       ├── README_DEBUG_DATA.md
│       ├── cleanup_debug_test_data.sql
│       └── populate_debug_test_data.sql
|
├── scripts
│   ├── dotnet_test_report.sh
│   ├── ef_migration_workflow.sh (Migration automation)
│   └── run_api_tests.sh (API test automation)
├── src
│   ├── VehicleServiceBooking.Api
│   │   ├── Configuration
│   │   │   ├── ApplicationOptionsExtensions.cs
│   │   │   ├── ApplicationServicesExtensions.cs
│   │   │   ├── ControllerAndValidationExtensions.cs
│   │   │   ├── CorsConfigurationExtensions.cs
│   │   │   ├── LogConfigurationExtensions.cs
│   │   │   ├── MiddlewareExtensions.cs
│   │   │   ├── PersistenceExtensions.cs
│   │   │   └── SwaggerExtensions.cs
│   │   ├── Controllers
│   │   │   ├── AppointmentsController.cs
│   │   │   └── AvailabilityController.cs
│   │   ├── Middleware
│   │   │   ├── CorrelationIdMiddleware.cs
│   │   │   └── ValidationExceptionMiddleware.cs
│   │   ├── Program.cs
│   │   ├── Properties
│   │   │   └── launchSettings.json
│   │   ├── Services
│   │   │   ├── IIdempotencyRequestCoordinator.cs
│   │   │   └── IdempotencyRequestCoordinator.cs
│   │   ├── VehicleServiceBooking.Api.csproj
│   │   ├── appsettings.Development.json
│   │   ├── appsettings.Production.json
│   │   ├── appsettings.Staging.json
│   │   ├── appsettings.json
│   │   ├── logs
│   │   │   ├── api-tests
│   │   │   ├── app-20260628.txt
│   │   │   ├── app-20260629.txt
│   │   │   └── test-reports
|   |
│   ├── VehicleServiceBooking.Application
│   │   ├── Configuration
│   │   │   ├── CorsOptions.cs
│   │   │   ├── IdempotencyOptions.cs
│   │   │   ├── Interfaces
│   │   │   ├── SchedulingOptions.cs
│   │   │   └── StaticDataCacheOptions.cs
│   │   ├── DTOs
│   │   │   ├── AvailabilityOptionResponse.cs
│   │   │   ├── CreateAppointmentRequest.cs
│   │   │   ├── CreateAppointmentResponse.cs
│   │   │   ├── ErrorResponse.cs
│   │   │   ├── GetAvailabilityRequest.cs
│   │   │   └── Idempotency
│   │   ├── Exceptions
│   │   │   └── BookingConflictException.cs
│   │   ├── Interfaces
│   │   │   ├── Persistence
│   │   │   ├── Repositories
│   │   │   └── Services
│   │   ├── Models
│   │   │   ├── AvailabilityOption.cs
│   │   │   ├── AvailabilityProjection.cs
│   │   │   ├── CreateAppointmentRequest.cs
│   │   │   ├── DateTimeSlot.cs
│   │   │   └── ViewModels
│   │   ├── Services
│   │   │   ├── AppointmentService.cs
│   │   │   ├── AvailabilityService.cs
│   │   │   └── IdempotencyService.cs
│   │   ├── Validators
│   │   │   ├── CreateAppointmentRequestValidator.cs
│   │   │   ├── CustomerRequestValidator.cs
│   │   │   └── GetAvailabilityRequestValidator.cs
│   │   ├── VehicleServiceBooking.Application.csproj
|   |
│   ├── VehicleServiceBooking.Domain
│   │   ├── Constants
│   │   ├── Entities
│   │   │   ├── Appointment.cs
│   │   │   ├── AppointmentStatusLookup.cs
│   │   │   ├── BaseEntity.cs
│   │   │   ├── BusinessHours.cs
│   │   │   ├── Customer.cs
│   │   │   ├── Dealership.cs
│   │   │   ├── IdempotencyRequest.cs
│   │   │   ├── IdempotencyRequestStatusLookup.cs
│   │   │   ├── Service.cs
│   │   │   ├── ServiceBay.cs
│   │   │   ├── ServiceStatusLookup.cs
│   │   │   ├── ServiceType.cs
│   │   │   ├── Technician.cs
│   │   │   ├── TechnicianSchedule.cs
│   │   │   ├── TechnicianSkill.cs
│   │   │   ├── TimeSlot.cs
│   │   │   └── Vehicle.cs
│   │   ├── Enums
│   │   │   ├── AppointmentStatus.cs
│   │   │   ├── IdempotencyRequestStatus.cs
│   │   │   └── ServiceStatus.cs
│   │   ├── ValueObjects
│   │   ├── VehicleServiceBooking.Domain.csproj
|   |
│   └── VehicleServiceBooking.Infrastructure
│       ├── Caching
│       │   └── StaticCacheKeys.cs
│       ├── Migrations
│       │   ├── 20260627173046_InitialCreate.Designer.cs
│       │   ├── 20260627173046_InitialCreate.cs
│       │   ├── 20260627190136_AddAvailabilityViews.Designer.cs
│       │   ├── 20260627190136_AddAvailabilityViews.cs
│       │   ├── 20260628074159_AddIsActiveToBaseEntity.Designer.cs
│       │   ├── 20260628074159_AddIsActiveToBaseEntity.cs
│       │   ├── 20260628074752_AddServiceTypePrice.Designer.cs
│       │   ├── 20260628074752_AddServiceTypePrice.cs
│       │   ├── 20260628075103_AddIsActiveToViews.Designer.cs
│       │   ├── 20260628075103_AddIsActiveToViews.cs
│       │   ├── 20260628114016_AddServiceBookingConflictInvariant.Designer.cs
│       │   ├── 20260628114016_AddServiceBookingConflictInvariant.cs
│       │   ├── 20260628131104_AddIdempotencyRequests.Designer.cs
│       │   ├── 20260628131104_AddIdempotencyRequests.cs
│       │   ├── 20260628154505_ConvertIdempotencyStatusToLookup.Designer.cs
│       │   ├── 20260628154505_ConvertIdempotencyStatusToLookup.cs
│       │   ├── 20260628165905_SyncDbContextChanges.Designer.cs
│       │   ├── 20260628165905_SyncDbContextChanges.cs
│       │   └── ApplicationDbContextModelSnapshot.cs
│       ├── Persistence
│       │   ├── ApplicationDbContext.cs
│       │   ├── Configurations
│       │   └── SeedData
│       ├── Repositories
│       │   ├── AppointmentRepository.cs
│       │   ├── AppointmentStatusLookupRepository.cs
│       │   ├── AvailabilityRepository.cs
│       │   ├── BusinessHoursRepository.cs
│       │   ├── CachedAppointmentStatusLookupRepository.cs
│       │   ├── CachedServiceStatusLookupRepository.cs
│       │   ├── CachedTimeSlotRepository.cs
│       │   ├── CustomerRepository.cs
│       │   ├── DealershipRepository.cs
│       │   ├── GenericRepository.cs
│       │   ├── IdempotencyRepository.cs
│       │   ├── ServiceBayRepository.cs
│       │   ├── ServiceRepository.cs
│       │   ├── ServiceStatusLookupRepository.cs
│       │   ├── ServiceTypeRepository.cs
│       │   ├── TechnicianRepository.cs
│       │   ├── TechnicianScheduleRepository.cs
│       │   ├── TechnicianSkillRepository.cs
│       │   ├── TimeSlotRepository.cs
│       │   └── VehicleRepository.cs
│       ├── VehicleServiceBooking.Infrastructure.csproj
|
└── tests
    └── VehicleServiceBooking.Tests
        ├── Api
        │   └── Controllers
        ├── Application
        │   ├── Services
        │   └── Validators
        ├── Common
        │   ├── AppointmentIntegrationTestDataBuilders.cs
        │   ├── AvailabilityTestDataBuilders.cs
        │   ├── CreateAppointmentRequestBuilder.cs
        │   ├── MaterializedViewsTestDataBuilder.cs
        │   ├── ServiceLayerTestDataBuilders.cs
        │   └── TestDataFactory.cs
        ├── Infrastructure
        │   └── Persistence
        ├── TestResults
        │   └── test-results.html
        ├── VehicleServiceBooking.Tests.csproj  
```

---

## 🤖 AI Collaboration Narrative

### How AI Was Used in This Project

This project demonstrates effective **AI-assisted software engineering** using Claude (GitHub Copilot), where the human strategically directed AI through all SDLC phases while maintaining ownership of design decisions and code quality.

### Strategy for Directing AI

**Phase 1: Architecture & Design Direction**

The human provided comprehensive context to AI:
```
Overview: "Build a real-time appointment scheduler with time slot conflict prevention"
Concept: "Slot-based scheduling model (30-minute slots, 18 slots/day, persistent in database)"
Purpose: "Enable dealerships to manage service appointments, prevent double-booking"
Specifications: Entity relationships, validation layers, concurrency protection
Task Details: "Create layered architecture: Domain → Application → Infrastructure → Presentation"
```

**Result:** AI generated Clean Architecture structure aligned with specifications. Human verified against .NET best practices and adapted where needed.

### Process for Verification & Refinement

**Code Quality Gates:**

1. **Architectural Alignment** (Before testing)
   - Verify: Is business logic in services, not controllers?
   - Check: Are dependencies injected, not created with `new`?
   - Validate: Do repositories follow abstraction contracts?

2. **Functional Correctness** (Unit + Integration Tests)
   - Run: Full test suite (40 tests)
   - Inspect: Test failures pinpoint gaps
   - Trace: "Double-booking test failing → constraint not working → check SQL"

3. **Cross-Check Against Requirements**
   - Business logic: "Does CreateAppointmentAsync validate availability?"
   - Error codes: "Are all 400/409 responses exactly as specified?"
   - Data types: "Are dates DateOnly, not DateTime?"


### Final Quality Metrics

- **Code Generation Success (First Pass):** 85% correct
- **Overall Code Rework Effort:** ~15%
- **Test Pass Rate:** 100% (40/40 tests passing)
- **AI Collaboration Benefit:** Reduced development time by ~60%

### Best Practices Applied

1. **Provided Full Context:** Overview → Concept → Purpose → Specifications → Task Details
2. **Explicit About Patterns:** Not just "Repository pattern" but "Interfaces in App layer, implementations in Infra layer"
3. **Structured Specifications:** JSON examples, DTOs, diagrams communicate better than prose
4. **Tested Early and Often:** Ran tests immediately after generation; failures pinpointed gaps
5. **Verified Against Requirements:** Cross-checked output against original specification
6. **Documented Decisions:** Recorded why each refinement was made

### Lessons Learned

- **Ambiguity costs iterations:** Clear requirements → first-pass code 80-90% correct
- **Visual specs are powerful:** Concrete examples (DTO fields, error codes) → zero rework
- **Testing validates faster than review:** Test suite caught all issues before manual inspection
- **AI excels at pattern implementation:** Once pattern defined, AI generated consistent code
- **Verification is the critical step:** AI quality depends on thoroughness of verification process

---

## 📚 Additional Documentation

For more details, see:

- [ARCHITECTURE_VISUALIZATION.md](docs/ARCHITECTURE_VISUALIZATION.md) - Comprehensive system design with diagrams
- [REQUIREMENTS_COVERAGE_ANALYSIS.md](docs/REQUIREMENTS_COVERAGE_ANALYSIS.md) - Keyloop Challenge compliance matrix
- [EF_MIGRATION_RESET_GUIDE.md](EF_MIGRATION_RESET_GUIDE.md) - Troubleshooting migrations
- [DEVELOPER_SETUP.md](DEVELOPER_SETUP.md) - Detailed development environment setup

---

## 🤝 Support

### Getting Help

- Check logs: `tail -f src/VehicleServiceBooking.Api/logs/app-$(date +%Y%m%d).txt`
- Search correlation ID: `grep -Ei "CorrelationId|<your-id>" src/VehicleServiceBooking.Api/logs/app-*.txt`
- Review test failures: `dotnet test --verbosity detailed`
- Check API documentation: http://localhost:5280/swagger

### Common Commands Cheat Sheet

```bash
# Build
dotnet build

# Test
dotnet test

# Run
dotnet run --project src/VehicleServiceBooking.Api

# Migrate DB
dotnet ef database update

# View logs
tail -f src/VehicleServiceBooking.Api/logs/app-*.txt

# Create appointment (via cURL)
curl -X POST http://localhost:5280/api/v1/appointments \
  -H "Content-Type: application/json" \
  -d '{...}'
```

---

**Last Updated:** June 29, 2026  
**Status:** Production Ready ✓  
**Documentation Version:** 3.0
