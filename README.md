# Vehicle Service Booking Platform

Production-ready .NET 8 microservice platform for dealership scheduling with three core services:
- Booking API (`VehicleServiceBooking.Api`)
- Auth Service (`VehicleServiceBooking.Auth`)
- Notification Functions (`VehicleServiceBooking.Notification.Functions`)

The platform supports resource-aware booking, conflict prevention, verification-first authentication, two-step login, JWT/JWKS trust, and asynchronous notification delivery.

## Table of Contents

- [Why This Project](#why-this-project)
- [Prerequisites](#prerequisites)
- [Technology Stack](#technology-stack)
- [Quick Start](#quick-start)
- [Full Local Stack Bring-Up](#full-local-stack-bring-up)
- [Quick Reference: Migration Commands](#quick-reference-migration-commands)
- [Developer Workflow](#developer-workflow)
- [API Quick Reference](#api-quick-reference)
- [Service Topology](#service-topology)
- [Environment Configuration](#environment-configuration)
- [Project Structure](#project-structure)
- [Documentation Map](#documentation-map)
- [Troubleshooting (Quick)](#troubleshooting-quick)
- [Contribution Notes](#contribution-notes)
- [Status](#status)

## Why This Project

This platform solves operational scheduling and identity constraints:
- A technician must have the required skill
- A service bay must be available for the whole service window
- Overlapping bookings are prevented at database level
- Retries can be made safely with idempotency keys
- Sign-up is verification-first before local password login token issuance
- Login uses challenge verification (`/login` -> `/login/verify-code`)
- Notification dispatch is asynchronous through queue-triggered functions

## Prerequisites

- .NET SDK 8.0+
- PostgreSQL 13+
- Git
- Azure Functions Core Tools (for local Notification Functions execution)

Verify:

```bash
dotnet --version
psql --version
git --version
func --version
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
git clone https://github.com/haudtech/vehicle-service-booking.git
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
PGPASSWORD='your_password' psql -h localhost -p 5432 -U postgres -d vehicle_service_booking -f scripts/migrations/populate_debug_test_data.sql
```

### 6. Run Booking API

```bash
dotnet run --project src/VehicleServiceBooking.Api
```

Default local URL:
- http://localhost:5280
- Swagger: http://localhost:5280/swagger

### 7. Run Auth Service

```bash
dotnet run --project src/VehicleServiceBooking.Auth
```

Typical local URL:
- http://localhost:5001

### 8. Run Notification Functions (optional for end-to-end notification flow)

```bash
cd src/VehicleServiceBooking.Notification.Functions
func start --verbose
```

If `func start` is unavailable in your environment, install Azure Functions Core Tools first.

## Full Local Stack Bring-Up

Use this sequence when you want Booking + Auth + Notification running together.

### 1. Pre-flight checks

```bash
dotnet --version
psql --version
func --version
pg_isready -h localhost
```

### 2. Ensure required ports are free

```bash
lsof -iTCP:5280 -sTCP:LISTEN -n -P || true
lsof -iTCP:5001 -sTCP:LISTEN -n -P || true
lsof -iTCP:7071 -sTCP:LISTEN -n -P || true
```

If a stale process is holding a port, stop it before continuing.

### 3. Start services in recommended order

Terminal A (Auth first):

```bash
dotnet run --project src/VehicleServiceBooking.Auth
```

Terminal B (Booking API):

```bash
dotnet run --project src/VehicleServiceBooking.Api
```

Terminal C (Notification Functions):

```bash
cd src/VehicleServiceBooking.Notification.Functions
func start --verbose
```

### 4. Verify service health quickly

Auth JWKS:

```bash
curl -s http://localhost:5001/api/v1/.well-known/jwks.json | head
```

Booking Swagger:

```bash
curl -I http://localhost:5280/swagger
```

Functions host status:

```bash
curl -s http://localhost:7071/admin/host/status
```

### 5. Smoke-check protected flow

1. Sign up and verify email with auth endpoints.
2. Complete login challenge via `/api/v1/auth/login/verify-code` to get JWT.
3. Call booking endpoint with bearer token (for example, `/api/v1/availability`).

Reference HTTP workflows:
- `tests/integration/http/user_workflow_signup_to_appointment_with_auth.http`
- `tests/integration/http/user_workflow_google_login_to_appointment_with_auth.http`

### 6. Known local issues

- Exit code `134` on `dotnet run` usually indicates startup/port conflict in this workspace context.
- If Notification Functions fail at startup, verify `func` installation and queue/storage config before retrying.

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

Build specific services:

```bash
dotnet build src/VehicleServiceBooking.Api/VehicleServiceBooking.Api.csproj
dotnet build src/VehicleServiceBooking.Auth/VehicleServiceBooking.Auth.csproj
dotnet build src/VehicleServiceBooking.Notification.Functions/VehicleServiceBooking.Notification.Functions.csproj
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

### Booking API routes
- GET /api/v1/availability?dealershipId={guid}&serviceTypeId={guid}&date={yyyy-mm-dd}
- POST /api/v1/appointments
- GET /api/v1/appointments/{id}

### Auth Service routes
- POST /api/v1/auth/signup
- GET /api/v1/auth/verify-email?email={email}&token={token}
- POST /api/v1/auth/verify-email
- POST /api/v1/auth/login
- POST /api/v1/auth/login/verify-code
- POST /api/v1/auth/refresh
- POST /api/v1/auth/logout
- GET /api/v1/me
- GET /api/v1/.well-known/jwks.json
- GET /api/v1/auth/google/start
- GET /api/v1/auth/google/callback

### Notification runtime behavior
- Auth publishes notification events asynchronously to queue.
- Notification Functions consume queue messages and deliver through configured providers.

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

## Service Topology

### 1) Booking API (`VehicleServiceBooking.Api`)
- Owns availability and appointment lifecycle.
- Validates JWT locally via JWKS.
- Enforces booking authorization policies.

### 2) Auth Service (`VehicleServiceBooking.Auth`)
- Owns user identity, verification-first sign-up, challenge-based login, token lifecycle, and JWKS publishing.
- Supports Google sign-in with verified-email evidence checks.

### 3) Notification Functions (`VehicleServiceBooking.Notification.Functions`)
- Queue-triggered email delivery worker.
- Processes retriable failures and poison messages.

## Environment Configuration

At minimum for local development:

- Booking API
  - `CONNECTIONSTRINGS__DEFAULTCONNECTION`

- Auth Service
  - Auth database connection string
  - JWT issuer/audience/signing configuration
  - Optional Google OAuth settings for social login

- Notification Functions
  - Storage/queue configuration
  - Provider settings (SendGrid or Google sender)

Reference docs:
- `docs/ARCHITECTURE/README.md`
- `docs/FEATURES/AUTHENTICATION_INTEGRATION/README.md`
- `docs/FEATURES/NOTIFICATION/README.md`

## Project Structure

```text
src/
  VehicleServiceBooking.Api/
  VehicleServiceBooking.Auth/
  VehicleServiceBooking.Application/
  VehicleServiceBooking.Domain/
  VehicleServiceBooking.Infrastructure/
  VehicleServiceBooking.Notification.Functions/
tests/
  VehicleServiceBooking.Tests/
docs/
  ARCHITECTURE/
  FEATURES/
scripts/
```

## Documentation Map

- Architecture overview: `docs/ARCHITECTURE/README.md`
- Auth integration: `docs/FEATURES/AUTHENTICATION_INTEGRATION/README.md`
- Notification feature: `docs/FEATURES/NOTIFICATION/README.md`
- Integration HTTP flows: `tests/integration/http/`
- Migration and API test helpers: `scripts/`

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
- Primary DBs: PostgreSQL (booking + auth)
- Service model: Microservice-oriented (API, Auth, Notification)
- API docs: Swagger enabled on Booking API and Auth service
- Current state: Active development with auth + notification integration in place

