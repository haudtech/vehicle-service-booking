# Vehicle Service Booking API

Production-ready .NET 8 API for scheduling dealership vehicle services with resource-aware booking, conflict prevention, and idempotent appointment creation.

## Table of Contents

- [Why This Project](#why-this-project)
- [Prerequisites](#prerequisites)
- [Technology Stack](#technology-stack)
- [Quick Start](#quick-start)
- [Quick Reference: Migration Commands](#quick-reference-migration-commands)
- [Developer Workflow](#developer-workflow)
- [API Quick Reference](#api-quick-reference)
- [Project Structure](#project-structure)
- [Documentation Map](#documentation-map)
- [Troubleshooting (Quick)](#troubleshooting-quick)
- [Contribution Notes](#contribution-notes)
- [Status](#status)

## Why This Project

This API solves appointment scheduling with real operational constraints:
- A technician must have the required skill
- A service bay must be available for the whole service window
- Overlapping bookings are prevented at database level
- Retries can be made safely with idempotency keys

## Prerequisites

- .NET SDK 8.0+
- PostgreSQL 13+
- Git

Verify:

```bash
dotnet --version
psql --version
git --version
```

## Technology Stack

| Component | Version | Purpose |
|---|---|---|
| .NET | 8.0 LTS | Runtime platform |
| ASP.NET Core | 8.0 | Web API framework |
| C# | 11 | Programming language |
| PostgreSQL | 13+ | Relational database |
| Entity Framework Core | 8.0 | ORM and migrations |
| FluentValidation | 11.9.1 | Request validation |
| Serilog | 8.0.1 | Structured logging |
| OpenTelemetry | 1.9.0 | Distributed tracing |
| Swagger/OpenAPI | 6.5.0 | API documentation |
| xUnit | 2.9.3 | Unit testing |
| Moq | 4.20.70 | Mocking |
| FluentAssertions | 6.12.0 | Test assertions |

## Quick Start

### 1. Prerequisites
See [Prerequisites](#prerequisites).

### 2. Clone and Restore

```bash
git clone <your-repo-url>
cd vehicle-service-booking
dotnet restore
dotnet build
```

### 3. Configure Database

Create database:

```bash
createdb vehicle_service_booking
```

Set connection string (choose one):

Option A: environment variable

```bash
export CONNECTIONSTRINGS__DEFAULTCONNECTION="Host=localhost;Port=5432;Database=vehicle_service_booking;Username=postgres;Password=your_password"
```

Option B: `.env` file in repository root

Create `.env` at the project root:

```env
CONNECTIONSTRINGS__DEFAULTCONNECTION=Host=localhost;Port=5432;Database=vehicle_service_booking;Username=postgres;Password=your_password
```

Option C: `src/VehicleServiceBooking.Api/appsettings.Development.json`

### 4. Apply Migrations

```bash
dotnet ef database update \
  --project src/VehicleServiceBooking.Infrastructure \
  --startup-project src/VehicleServiceBooking.Api
```

### 5. Seed Debug Data (Optional)

```bash
PGPASSWORD='your_password' psql -h localhost -p 5432 -U postgres -d vehicle_service_booking -f docs/sql/populate_debug_test_data.sql
```

### 6. Run API

```bash
dotnet run --project src/VehicleServiceBooking.Api
```

Default local URL:
- http://localhost:5280
- Swagger: http://localhost:5280/swagger

## Quick Reference: Migration Commands

| Command | Purpose |
|---|---|
| `dotnet ef migrations list` | List all migrations |
| `dotnet ef migrations add [Name]` | Create a new migration |
| `dotnet ef migrations remove` | Remove last unapplied migration |
| `dotnet ef database update` | Apply all pending migrations |
| `dotnet ef database update [MigrationName]` | Revert to a specific migration |
| `dotnet ef database drop` | Drop the database (dangerous) |

## Developer Workflow

### Build

```bash
dotnet build
```

### Test

```bash
dotnet test
```

### Run focused tests

```bash
dotnet test --filter "Validators"
dotnet test --filter "Integration"
```

### Watch tests

```bash
dotnet watch --project tests/VehicleServiceBooking.Tests test
```

## API Quick Reference

Implemented routes:
- GET /api/v1/availability?dealershipId={guid}&serviceTypeId={guid}&date={yyyy-mm-dd}
- POST /api/v1/appointments
- GET /api/v1/appointments/{id}

### Create Appointment (example)

```bash
curl -X POST http://localhost:5280/api/v1/appointments \
  -H "Content-Type: application/json" \
  -H "Idempotency-Key: 4f7b1f0e-5f5e-4ec9-8f14-6e43ff7cd001" \
  -d '{
    "dealershipId": "550e8400-e29b-41d4-a716-446655440000",
    "customerId": "550e8400-e29b-41d4-a716-446655440001",
    "vehicleId": "550e8400-e29b-41d4-a716-446655440002",
    "appointmentDate": "2026-06-25",
    "serviceTypeId": "550e8400-e29b-41d4-a716-446655440010",
    "technicianId": "550e8400-e29b-41d4-a716-446655440003",
    "serviceBayId": "550e8400-e29b-41d4-a716-446655440004",
    "estimatedStartTimeSlotId": "00000000-0000-0000-0000-000000000005",
    "estimatedEndTimeSlotId": "00000000-0000-0000-0000-000000000006"
  }'
```

## Troubleshooting (Quick)

Database connection check:

```bash
pg_isready -h localhost
psql -h localhost -U postgres -d vehicle_service_booking -c "SELECT 1;"
```

Logs:

```bash
tail -f src/VehicleServiceBooking.Api/logs/app-$(date +%Y%m%d).txt
```

Migrations list:

```bash
dotnet ef migrations list \
  --project src/VehicleServiceBooking.Infrastructure \
  --startup-project src/VehicleServiceBooking.Api
```

## Contribution Notes

- Keep business rules in Application services, not controllers
- Keep Infrastructure concerns in repositories and DbContext configs
- Add/adjust tests with every behavior change
- Update architecture docs when domain invariants or data flow changes

## Status

- Runtime: .NET 8
- Primary DB: PostgreSQL
- API docs: Swagger enabled
- Current state: Active development

