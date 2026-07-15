# User/Auth Service + Vehicle Scheduling Service Architecture

## Purpose
This document describes the conceptual architecture, workflow, and implementation plan for separating the vehicle scheduling API from a new user/auth service. It includes:
- architecture vision
- workflow and sequence diagrams
- token model and auth contract
- practical examples per task
- integration guidance for multiple downstream services

---

## 1. Architectural Overview

### 1.1 Current system
The current repository is a single vehicle scheduling service with:
- `GET /api/v1/availability`
- `POST /api/v1/appointments`
- slot-based scheduling model
- PostgreSQL exclusion constraints for concurrency
- optional idempotency support

This service currently does not implement authentication or user management.

### 1.2 Desired system
Split the system into two cooperating services:

1. **Auth/User Service**
   - manages users, sign-up, login, groups, roles, permissions
   - issues JWTs and refresh tokens
  - supports Google sign-in with verified-email evidence checks
   - central identity provider for multiple services

2. **Vehicle Scheduling Service**
   - remains focused on scheduling business logic
   - validates JWTs locally
   - enforces authorization policies for booking workflows
   - serves only booking-related APIs

### 1.3 Architecture diagram

```
+----------------------+          +------------------------+
|  Client Applications  |          |  External IdP / SSO    |
|                      |          |      (Google, ...)     |
| - Web UI            |          +-----------+------------+
| - Mobile App        |                      |
+----------+-----------+                      |
           | Authorization / Sign-in           |
           v                                     v
+----------------------+          +------------------------+
| Auth/User Service    |<-------->| External OAuth2 / OIDC|
|                      | token   | identity provider      |
| - user sign-up       |          +------------------------+
| - login             |
| - social login      |
| - groups / roles    |
| - permissions       |
| - JWT issuance      |
+----------+-----------+
           |
           | JWT
           v
+----------------------+          +------------------------+
| Vehicle Scheduling   |          | Future Service A       |
| Service              |          | (inventory, reporting) |
|                      |          +------------------------+
| - validate JWT      |
| - authorization     |
| - availability      |
| - appointments      |
+----------------------+          +------------------------+
```

### 1.4 Why split?
- keeps booking service focused on scheduling only
- centralizes identity and access rules
- makes auth reusable across services
- enables support for user sign-up and social login without coupling booking logic
- supports future extensions with a shared auth platform

---

## 2. Auth Service Responsibilities

### 2.1 Core features
- user registration / sign-up
- email verification before local sign-in
- login with local credentials (challenge start)
- login challenge code verification (token issuance)
- social login via OAuth2 provider (Google)
- group and role assignment
- permission mapping
- token issuance and refresh
- JWKS endpoint for public key discovery
- token metadata / claims

### 2.2 User model
At a minimum, auth service should manage:
- `User` (id, email, name, status)
- `Group` (organization or team membership)
- `Role` (admin, manager, client, technician, etc.)
- `Permission` (appointment:create, appointment:complete, appointment:view)
- `UserRole` and `UserGroup` associations

### 2.3 Token model
Auth service issues JWTs containing claims such as:
- `sub`: user id
- `email`
- `iss`: issuer
- `aud`: target service identifier (e.g. `vehicle-booking-api`)
- `roles`: array of role names
- `groups`: array of group names
- `scope`: permissions strings
- `exp` / `iat`

Optional token claim examples for multi-service support:
- `service`: `vehicle-scheduling`
- `client_id`: service or application id

---

## 3. Booking Service Responsibilities

### 3.1 Core features
- local JWT validation using auth service public keys
- enforce authorization policies in controller/actions
- map claims to service permissions
- continue to use existing scheduling workflows
- keep user management out of the booking service

### 3.2 Authorization policy examples
Define policies by action rather than by endpoint only.
Examples:
- `AppointmentCreatePolicy` → requires `appointment:create` or `role:client`
- `AppointmentCompletePolicy` → requires `appointment:complete` or `role:manager`
- `AppointmentReadPolicy` → requires `appointment:view` or `role:manager`

### 3.3 Token validation
Booking service should validate locally:
- JWT signature
- `iss` matches auth service
- `aud` includes `vehicle-booking-api`
- token is not expired
- required claims are present

---

## 4. Workflow and sequence diagram

### 4.1 User sign-up workflow

```
Client -> Auth/User Service: POST /api/v1/auth/signup
Auth/User Service -> Database: create pending user + verification token hash/expiry
Auth/User Service -> Client: 202 Accepted (verification required)
Client -> Auth/User Service: GET /api/v1/auth/verify-email?email=...&token=...
Auth/User Service -> Database: mark email verified + clear token fields
Auth/User Service -> Client: 200 OK
```

### 4.2 User login workflow

```
Client -> Auth/User Service: POST /api/v1/auth/login
Auth/User Service -> Database: validate credentials
Auth/User Service -> Database: persist challenge id + code hash + expiry
Auth/User Service -> Client: 202 Accepted (challenge required)
Client -> Auth/User Service: POST /api/v1/auth/login/verify-code
Auth/User Service -> Database: verify and consume challenge
Auth/User Service -> JWT service: create access token + refresh token
Auth/User Service -> Client: 200 OK (tokens)
```

### 4.3 Social login workflow

```
Client -> Auth/User Service: GET /api/v1/auth/google/start
Client -> Google: auth request
Google -> Client: auth callback
Client -> Auth/User Service: callback with code
Auth/User Service -> Google: exchange code for user info
Auth/User Service: verify provider email evidence
Auth/User Service -> DB: create or map user
Auth/User Service -> Client: issue access token + refresh token
```

### 4.4 Booking API flow with token validation

```
Client -> Vehicle Scheduling Service: POST /api/v1/appointments
Headers: Authorization: Bearer <JWT>

Vehicle Scheduling Service -> Local JWT validator: verify token
Vehicle Scheduling Service: check claims / policies
Vehicle Scheduling Service -> AppointmentService: validate availability
Vehicle Scheduling Service -> Database: create appointment
Vehicle Scheduling Service -> Client: 201 Created
```

### 4.5 Future service flow

```
Client -> Future Service: GET /api/v1/whatever
Headers: Authorization: Bearer <JWT>

Future Service -> Local JWT validator: verify token
Future Service: check service-specific claims/audience
Future Service -> business logic
Future Service -> Client: response
```

---

## 5. Detailed task breakdown

### Task 5.1: Define auth contract and claims
- decide auth service issuer and audience values
- define role/group/permission model
- define standard JWT claims for all downstream services
- define social login provider mapping

### Task 5.2: Create auth service design doc
- user sign-up endpoints
- login endpoint
- refresh token endpoint
- social login / OAuth2 callback flows
- group/role management endpoints
- JWT issuance and JWKS publishing

### Task 5.3: Update booking service behavior
- add local JWT validation middleware
- configure issuer, audience, and public keys
- add authorization policies for booking actions
- protect controllers/actions with authorization policies

### Task 5.4: Define multi-service compatibility
- publish auth service contract for downstream services
- decide how to represent service audience in JWT
- document permission naming conventions
- document the auth service as reusable identity provider

### Task 5.5: Document practical examples
- signup request/response
- login request/response
- social login flow example
- booking request with Authorization header
- sample JWT claims payload

---

## 6. Practical examples

### 6.1 Sign-up example

Request:
```http
POST /api/v1/auth/signup
Content-Type: application/json

{
  "email": "alice@example.com",
  "accountName": "alice.smith",
  "password": "P@ssw0rd!",
  "displayName": "Alice Smith"
}
```

Response:
```http
202 Accepted
{
  "message": "Sign-up successful. Please verify your email before signing in.",
  "email": "alice@example.com",
  "verificationLink": "https://auth.example.com/api/v1/auth/verify-email?email=alice%40example.com&token=...",
  "verificationTokenExpiresAtUtc": "2026-07-15T08:00:00Z"
}
```

### 6.2 Login example

Request:
```http
POST /api/v1/auth/login
Content-Type: application/json

{
  "identifier": "alice.smith",
  "password": "P@ssw0rd!"
}
```

Backward-compatible alternative:
```http
POST /api/v1/auth/login
Content-Type: application/json

{
  "email": "alice@example.com",
  "password": "P@ssw0rd!"
}
```

Response:
```http
202 Accepted
{
  "message": "Credentials accepted. Submit the verification code to complete login.",
  "email": "alice@example.com",
  "challengeId": "550e8400-e29b-41d4-a716-446655440000",
  "verificationCodeExpiresAtUtc": "2026-07-14T08:10:00Z"
}
```

Follow-up request:
```http
POST /api/v1/auth/login/verify-code
Content-Type: application/json

{
  "email": "alice@example.com",
  "challengeId": "550e8400-e29b-41d4-a716-446655440000",
  "code": "123456"
}
```

Follow-up response:
```http
200 OK
{
  "accessToken": "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "d2d62ce2-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "expiresIn": 900
}
```

### 6.3 Social login example

Request:
```http
GET /api/v1/auth/google/start
```

Callback flow:
1. Client receives redirect to Google
2. User signs in with Google
3. Google redirects back
4. Auth service exchanges code, validates provider email evidence, and issues local tokens

Response:
```http
200 OK
{
  "accessToken": "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "...",
  "expiresIn": 900
}
```

### 6.4 Booking request example

Request:
```http
POST /api/v1/appointments
Authorization: Bearer <accessToken>
Content-Type: application/json

{
  "dealershipId": "11111111-1111-1111-1111-000000000001",
  "customerId": "11111111-1111-1111-1111-000000000002",
  "vehicleId": "11111111-1111-1111-1111-000000000003",
  "appointmentDate": "2026-07-10",
  "serviceTypeId": "11111111-1111-1111-1111-030000000001",
  "technicianId": "11111111-1111-1111-1111-040000000001",
  "serviceBayId": "11111111-1111-1111-1111-050000000001",
  "estimatedStartTimeSlotId": "00000000-0000-0000-0000-000000000005",
  "estimatedEndTimeSlotId": "00000000-0000-0000-0000-000000000006"
}
```

Response:
```http
201 Created
{
  "appointmentId": "550e8400-e29b-41d4-a716-446655440010",
  "slotStart": "2026-07-10T10:00:00",
  "slotEnd": "2026-07-10T11:00:00",
  "createdAt": "2026-07-03T08:30:00Z"
}
```

### 6.5 Sample JWT claims

```json
{
  "sub": "550e8400-e29b-41d4-a716-446655440000",
  "email": "alice@example.com",
  "iss": "https://auth.example.com",
  "aud": ["vehicle-booking-api"],
  "roles": ["client"],
  "groups": ["dealership-north"],
  "scope": ["appointment:create","appointment:view"],
  "exp": 1710000000,
  "iat": 1709996400
}
```

---

## 7. Cross-service compatibility

### 7.1 Shared auth contract
The auth service should publish a clear contract for all downstream services, including:
- JWKS endpoint for verifying tokens
- token issuer and audience values
- role and permission naming conventions
- supported claims
- refresh token lifecycle

### 7.2 Service-specific audiences
Each downstream service should receive a token with its own audience.
Examples:
- `vehicle-booking-api`
- `inventory-api`
- `reporting-api`

### 7.3 Permission naming guidance
Use action-based permission names:
- `appointment:create`
- `appointment:complete`
- `appointment:read`
- `vehicle:view`
- `inventory:update`

This makes permissions reusable and easy to map to service behavior.

---

## 8. Operational considerations

### 8.1 Token expiry and refresh
- issue short-lived access tokens (15–30 minutes)
- issue refresh tokens for longer lived sessions
- refresh tokens should be stored securely and revocable

### 8.2 Revocation strategy
- local JWT validation means revocation is not immediate
- use short token lifetime and refresh token revocation for practical control
- if immediate revoke is required later, add a blacklist or introspection path

### 8.3 Key rotation
- publish a JWKS endpoint
- support rotating signing keys
- booking service should cache JWKS and refresh periodically

### 8.4 User sign-up flows
- implement local sign-up in auth service
- support verification if needed
- separate sign-up from booking operations

---

## 9. Summary of conceptual implementation

1. Build a new Auth/User Service
2. Keep the current repo as Vehicle Scheduling Service
3. Issue JWTs from auth service
4. Validate JWTs locally in booking service
5. Enforce authorization policies in booking service
6. Support social login via OAuth2 in auth service
7. Keep user sign-up and identity concerns out of booking service
8. Enable multiple downstream services to reuse the same auth service

---

## 10. Next steps
- create feature docs for auth service API and booking service integration
- define role/permission matrix in a separate specification
- design the auth service database schema and endpoints
- create a checklist to track implementation progress
