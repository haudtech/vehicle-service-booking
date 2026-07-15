# Vehicle Service Booking Platform

A .NET 8 microservice platform for dealership scheduling, authentication, and asynchronous notifications.

This README is the entry point for:
- what the platform does
- which services own which responsibilities
- which stack is used
- how to run the system locally
- where to read deeper documentation

## 1) Platform Overview

The platform is built around three runtime services:
- Booking API: scheduling and appointment lifecycle
- Auth Service: identity, verification-first signup, login challenge, JWT issuance
- Notification Functions: queue-triggered email delivery

Core behavior:
- conflict-safe booking using database-level protection
- verification-first auth and challenge-based login
- JWT/JWKS trust between Auth and Booking
- async notification delivery through queue + function worker

## 2) Service Responsibilities

| Service | Main Responsibility | Key Endpoints/Behavior |
|---|---|---|
| VehicleServiceBooking.Api | Availability, appointments, booking authorization | /api/v1/availability, /api/v1/appointments |
| VehicleServiceBooking.Auth | Signup, verify-email, login challenge, token lifecycle, JWKS | /api/v1/auth/*, /api/v1/.well-known/jwks.json |
| VehicleServiceBooking.Notification.Functions | Queue message processing and email provider delivery | Queue trigger, retry, poison handling |

## 3) Technology Stack

| Area | Stack |
|---|---|
| Runtime | .NET 8, ASP.NET Core |
| Language | C# |
| Data | PostgreSQL, EF Core |
| Validation | FluentValidation |
| Auth | JWT, JWKS, OAuth (Google) |
| Observability | Serilog, OpenTelemetry |
| API Docs | Swagger/OpenAPI |
| Testing | xUnit, Moq, FluentAssertions |
| Functions | Azure Functions (.NET isolated) |

## 4) Quick Start (Local)

### Prerequisites
- .NET SDK 8+
- PostgreSQL 13+
- Azure Functions Core Tools (for notification worker)

### Clone and Build

```bash
git clone https://github.com/haudtech/vehicle-service-booking.git
cd vehicle-service-booking
dotnet restore
dotnet build
```

### Database Setup

Create local database:

```bash
createdb vehicle_service_booking
```

Set connection string (example):

```bash
export CONNECTIONSTRINGS__DEFAULTCONNECTION="Host=localhost;Port=5432;Database=vehicle_service_booking;Username=postgres;Password=your_password"
```

Apply migrations:

```bash
dotnet ef database update \
  --project src/VehicleServiceBooking.Infrastructure \
  --startup-project src/VehicleServiceBooking.Api
```

Optional debug seed:

```bash
PGPASSWORD='your_password' psql -h localhost -p 5432 -U postgres -d vehicle_service_booking -f scripts/migrations/populate_debug_test_data.sql
```

## 5) Start Services

Open separate terminals.

### Auth Service

```bash
dotnet run --project src/VehicleServiceBooking.Auth
```

Typical URL: http://localhost:5001

### Booking API

```bash
dotnet run --project src/VehicleServiceBooking.Api
```

Typical URL: http://localhost:5280
Swagger: http://localhost:5280/swagger

### Notification Functions (optional for full flow)

```bash
cd src/VehicleServiceBooking.Notification.Functions
func start --verbose
```

Typical URL: http://localhost:7071

## 6) Verify the Stack

JWKS from Auth:

```bash
curl -s http://localhost:5001/api/v1/.well-known/jwks.json | head
```

Booking swagger check:

```bash
curl -I http://localhost:5280/swagger
```

Functions host status:

```bash
curl -s http://localhost:7071/admin/host/status
```

## 7) Project Structure

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
  integration/
docs/
  ARCHITECTURE/
  FEATURES/
  GUIDANCES/
scripts/
```

## 8) Documentation Map

- Architecture hub: docs/ARCHITECTURE/README.md
- Auth feature docs: docs/FEATURES/AUTHENTICATION_INTEGRATION/README.md
- Notification feature docs: docs/FEATURES/NOTIFICATION/README.md
- Integration workflows: tests/integration/http/
- Operational scripts: scripts/

## 9) Documentation Standard (Recommended)

Use this rule consistently:

1. README (root): entry-point only
   - project overview
   - service responsibilities
   - stack summary
   - startup steps
   - doc navigation

2. Architecture docs: stable system views
   - boundaries, topology, trust model
   - no low-level request/response detail

3. Feature docs: exact runtime behavior
   - endpoint contracts
   - sequences and edge cases
   - operational behavior and troubleshooting

4. Archived docs: historical/planning only
   - clearly labeled as non-authoritative

5. Update policy
   - one source of truth per topic
   - if behavior changes in code, update the owning feature doc in the same PR

## 10) Current Status

- Runtime: .NET 8
- Databases: PostgreSQL
- Service model: Booking API + Auth + Notification Functions
- Auth model: verification-first + challenge login + JWT/JWKS
- Development state: active
