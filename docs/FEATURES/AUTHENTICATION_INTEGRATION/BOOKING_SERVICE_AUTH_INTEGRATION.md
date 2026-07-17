# Booking Service Auth Integration Design

## Purpose
This document defines how the Vehicle Scheduling Service integrates with the external Auth/User Service using local JWT validation and service-side authorization.

## Status
Current source of truth for Booking-side Auth integration behavior.

---

## 1. Integration overview

### 1.1 Responsibilities
- **Auth/User Service** issues JWT access tokens and refresh tokens
- **Vehicle Scheduling Service** consumes JWTs and enforces authorization

### 1.2 Key principles
- Local JWT validation for performance
- Use `aud = vehicle-booking-api`
- Use `permissions` claims as the primary authorization contract
- Keep `roles` for coarse identity context and compatibility
- Keep `groups` as optional business-partition context (future scoping)
- Keep booking service user-agnostic beyond token validation
- Do not implement sign-up/login in booking service

### 1.3 Shared role/group semantics (cross-service contract)
- `roles`: broad identity category labels, stable across services.
- `permissions`: action-level grants used by Booking API policies.
- `groups`: organizational/partition context, not direct permission grants.

Current Booking policy behavior:
- Authorization is permission-first using `permissions` claims.
- `roles` and `groups` are informational unless a policy explicitly opts in.

---

## 2. Authentication flow

### 2.1 Request flow
1. Client obtains JWT from Auth/User Service
2. Client calls booking endpoint with `Authorization: Bearer <token>`
3. Booking service validates JWT locally
4. Booking service checks required claims and policies
5. Booking service proceeds with scheduling logic

### 2.2 Example request
```http
POST /api/v1/appointments
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...
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

---

## 3. JWT validation design

### 3.1 Validation criteria
Booking service must validate:
- signature validity
- `iss` equals auth service issuer
- `aud` contains `vehicle-booking-api`
- token not expired (`exp`)
- token not used before `nbf` if present
- token includes required claims: `sub`, `permissions`

### 3.2 JWKS discovery
- Booking service fetches public keys from `https://auth.example.com/.well-known/jwks.json`
- cache JWKS locally
- refresh keys periodically or on signature error

### 3.3 Key rotation
- auth service rotates signing keys transparently
- booking service must support multiple active keys via `kid`
- fallback when key is missing: refresh JWKS and retry once

Defined baseline policy:
- rotation cadence: periodic (recommended 30 days)
- overlap window: at least max access token TTL + clock skew buffer
- key retirement: only after overlap window expires
- incident fallback: force JWKS refresh and deny on persistent kid miss

---

## 4. Authorization policy design

### 4.1 Policy concepts
Use action-based policies rather than role-only checks.

Policies:
- `AppointmentCreatePolicy`
- `AppointmentCompletePolicy`
- `AppointmentReadPolicy`

### 4.2 Policy requirements

`AppointmentCreatePolicy`
- requires `permissions` contains `appointment:create`

`AppointmentCompletePolicy`
- requires `permissions` contains `appointment:complete`

`AppointmentReadPolicy`
- requires `permissions` contains `appointment:view`

### 4.3 Claim mapping
- `roles` claim maps to broad authorization categories
- `permissions` claim maps to precise actions
- `groups` claim can be used for dealership or region-specific access when tenant-scoped policies are introduced

---

## 5. Booking controller protection

### 5.1 Protect endpoints
Add `[Authorize]` to controllers or actions.

Example:
```csharp
[Authorize(Policy = "AppointmentCreate")]
[HttpPost("appointments")]
public async Task<IActionResult> CreateAppointment(CreateAppointmentRequest request)
{
  ...
}
```

### 5.2 Public endpoints
- `GET /api/v1/availability` may remain public initially, or be protected depending on requirements
- if protected, use `AppointmentReadPolicy`

---

## 6. Implementation details

### 6.1 Configuration
Add options:
- `JwtOptions.Issuer`
- `JwtOptions.Audience` = `vehicle-booking-api`
- `JwtOptions.JwksUri`
- `JwtOptions.RequireHttpsMetadata` = true
- `JwtOptions.ClockSkew` = `TimeSpan.FromMinutes(2)`

### 6.2 Middleware changes
- Add `AddAuthentication()` with JWT Bearer
- Add `AddAuthorization()` policies
- Call `app.UseAuthentication()` before `app.UseAuthorization()`

### 6.3 Service integration
- booking service should not call auth service for each request
- only fetch JWKS and validate tokens locally
- optional: use auth service only for token refresh or user profile if needed

---

## 7. Error handling

### 7.1 Authentication errors
Return:
- `401 Unauthorized` for invalid or expired tokens
- `401 Unauthorized` for missing Authorization header

Example response:
```json
{
  "code": "UNAUTHORIZED",
  "message": "Invalid or missing access token"
}
```

### 7.2 Authorization errors
Return:
- `403 Forbidden` when token is valid but user lacks permissions

Example:
```json
{
  "code": "FORBIDDEN",
  "message": "You do not have permission to perform this action"
}
```

---

## 8. Testing

### 8.1 Unit tests
- validate JWT validation logic with valid and invalid tokens
- validate authorization policies for role/scope claims
- validate misconfigured audience or issuer

### 8.2 Integration tests
- request booking endpoint with a valid token and verify success
- request booking endpoint with a token missing `permissions=appointment:create` and verify `403`
- request booking endpoint with expired token and verify `401`

### 8.4 Key rotation validation matrix
- active key token -> `200/201` expected
- previous key token during overlap -> `200/201` expected
- token with unknown `kid` -> `401` expected
- key miss then JWKS refresh resolves `kid` -> request succeeds
- retired key token after overlap -> `401` expected

### 8.3 End-to-end tests
- sign in via auth service
- obtain JWT
- call booking endpoint with token
- validate booking creation and response

---

## 9. Security considerations

### 9.1 Token replay
- JWTs are bearer tokens and may be replayed if stolen
- mitigate with short token lifetimes and refresh tokens

### 9.2 Local validation trade-offs
- best performance and scalability
- revocation is not immediate
- can be mitigated by short-lived tokens and refresh token revocation

### 9.3 Token size and claims
- keep JWT claim payload minimal
- avoid embedding large user profile data
- include only necessary identity and authorization data

---

## 10. Next step checklist
- [ ] add JWT authentication configuration in booking service
- [ ] add JWKS validation support
- [ ] define and register authorization policies
- [ ] protect booking controller endpoints
- [ ] add tests for auth and authorization flows
- [ ] document integration in booking service README or architecture docs

---

## 11. Customer bootstrap via Auth profile lookup (updated direction)

This section defines the updated integration direction:
- Auth service does not push customer data to Booking API.
- Booking API lazily provisions `Customer` by querying Auth service when needed.
- Client may call customer endpoints proactively for UX, but Booking business APIs must remain self-sufficient.

### 11.1 Target outcomes
- Single source of truth for identity core fields remains Auth service.
- Booking API owns `Customer` lifecycle for booking domain data.
- No cross-database foreign keys are required.
- Provisioning is idempotent and safe under concurrent requests.

### 11.2 Data contract alignment

Booking `Customer` table should include:
- `AuthUserId` (`Guid`, required, unique)
- `Email` (copied snapshot from Auth profile)
- `DisplayName` (copied snapshot from Auth profile)
- `PhoneNumber` (copied snapshot from Auth profile, optional/nullable if Auth does not provide)
- Existing booking-specific profile fields (first name, last name, etc.)

Constraint requirements:
- Unique index: `UX_Customers_AuthUserId`
- Optional unique index on `Email` only if business allows strict 1:1 email identity in booking domain

### 11.3 Auth service API contract (internal)

Endpoint:
```http
GET /api/v1/internal/users/{authUserId}/core-profile
```

Purpose:
- Return minimal identity fields required by Booking API to bootstrap local `Customer`.

Authentication/authorization:
- Service-to-service authentication only (not end-user bearer token).
- Require internal scope/permission, for example: `internal:user.read`.
- Restrict caller to Booking API service identity.

Successful response (`200`):
```json
{
  "authUserId": "11111111-1111-1111-1111-111111111111",
  "email": "user@example.com",
  "displayName": "Jane Doe",
  "phoneNumber": "+84901234567",
  "isEmailVerified": true,
  "isActive": true,
  "updatedAtUtc": "2026-07-17T08:30:00Z"
}
```

Error responses:
- `404 Not Found`: user does not exist
- `409 Conflict`: user exists but not eligible for booking bootstrap (`isEmailVerified=false` or `isActive=false`)
- `401/403`: service identity invalid or unauthorized

### 11.4 Booking API contract (public + internal behavior)

Public endpoint for client UX and app bootstrap:
```http
GET /api/v1/customers/me
Authorization: Bearer <end-user-access-token>
```

Behavior:
1. Extract `authUserId` from token `sub` claim.
2. Try load customer by `AuthUserId`.
3. If found, return existing customer.
4. If not found, call Auth internal API (`core-profile`).
5. Create customer using returned core fields.
6. Return created customer.

Optional admin/internal lookup endpoints:
```http
GET /api/v1/customers/{customerId}
GET /api/v1/internal/customers/by-auth-user/{authUserId}
```

Successful response (`200`):
```json
{
  "customerId": "22222222-2222-2222-2222-222222222222",
  "authUserId": "11111111-1111-1111-1111-111111111111",
  "email": "user@example.com",
  "displayName": "Jane Doe",
  "phoneNumber": "+84901234567",
  "firstName": "Jane",
  "lastName": "Doe",
  "createdAtUtc": "2026-07-17T08:31:10Z",
  "updatedAtUtc": "2026-07-17T08:31:10Z"
}
```

### 11.5 Idempotency and race-condition handling

For concurrent first-time requests:
- Always enforce unique constraint on `AuthUserId` at DB level.
- Use create-then-handle-unique-violation fallback:
  - attempt insert
  - if unique violation occurs, re-query by `AuthUserId` and return existing row

This guarantees deterministic behavior without distributed locks.

### 11.6 Booking business API rule

For user-scoped business endpoints (for example appointment creation):
- Do not trust client-supplied `CustomerId` as identity source.
- Resolve customer from token `sub` (`AuthUserId`) through `GetOrCreateCustomerByAuthUserId`.
- If admin/staff workflows require acting on behalf of another customer, expose separate endpoints/policies.

### 11.7 Recommended sequence flow

```mermaid
sequenceDiagram
    autonumber
    participant Client
    participant BookingAPI as Booking API
    participant BookingDB as Booking DB
    participant AuthAPI as Auth Service

    Client->>BookingAPI: GET /api/v1/customers/me (Bearer token)
    BookingAPI->>BookingAPI: read sub claim as authUserId
    BookingAPI->>BookingDB: find customer by AuthUserId
    alt customer exists
        BookingDB-->>BookingAPI: customer row
        BookingAPI-->>Client: 200 customer payload
    else customer missing
        BookingAPI->>AuthAPI: GET /api/v1/internal/users/{authUserId}/core-profile
        AuthAPI-->>BookingAPI: core profile
        BookingAPI->>BookingDB: insert customer(AuthUserId, Email, DisplayName, Phone)
        BookingDB-->>BookingAPI: inserted row
        BookingAPI-->>Client: 200 customer payload
    end
```

### 11.8 Resilience requirements

Booking API -> Auth API client should apply:
- timeout (for example 1-2 seconds)
- bounded retry (for transient failures only)
- circuit breaker
- structured logs with correlation id

Fallback behavior:
- If Auth API is unavailable and customer is missing, return `503 Service Unavailable` with retryable error code (for example `CUSTOMER_BOOTSTRAP_UNAVAILABLE`).

### 11.9 Updated checklist for this direction
- [ ] add `AuthUserId` to booking `Customer` entity
- [ ] add unique index `UX_Customers_AuthUserId`
- [ ] add Auth internal endpoint for user core profile by id
- [ ] add Booking endpoint `GET /api/v1/customers/me`
- [ ] implement `GetOrCreateCustomerByAuthUserId` service in Booking
- [ ] integrate customer resolution into user-scoped booking flows
- [ ] add contract tests for Auth <-> Booking internal API
- [ ] add concurrency tests for first-time lazy provisioning

### 11.10 Deployment configuration examples (hostname-first)

Booking API now supports a hostname-first endpoint configuration for Auth profile lookup, with backward compatibility for `BaseUrl`.

Preferred keys:
- `AuthUserProfile:Scheme`
- `AuthUserProfile:Host`
- `AuthUserProfile:Port` (optional)
- `AuthUserProfile:BasePath` (optional)
- `AuthUserProfile:CoreProfilePathTemplate`
- `AuthUserProfile:TimeoutSeconds`

Fallback key (legacy):
- `AuthUserProfile:BaseUrl`

Production example (public DNS + HTTPS):
```json
{
  "AuthUserProfile": {
    "Scheme": "https",
    "Host": "auth.company.com",
    "CoreProfilePathTemplate": "/api/v1/internal/users/{authUserId}/core-profile",
    "TimeoutSeconds": 2
  }
}
```

Production example (internal service DNS + custom base path):
```json
{
  "AuthUserProfile": {
    "Scheme": "https",
    "Host": "auth.identity.svc.cluster.local",
    "Port": 8443,
    "BasePath": "/auth",
    "CoreProfilePathTemplate": "/api/v1/internal/users/{authUserId}/core-profile",
    "TimeoutSeconds": 2
  }
}
```

Environment variable mapping example:
```bash
AuthUserProfile__Scheme=https
AuthUserProfile__Host=auth.company.com
AuthUserProfile__CoreProfilePathTemplate=/api/v1/internal/users/{authUserId}/core-profile
AuthUserProfile__TimeoutSeconds=2
```

Notes:
- If `BaseUrl` is provided, it takes precedence over `Scheme`/`Host`/`Port`/`BasePath`.
- Startup validation should fail fast if neither a valid absolute `BaseUrl` nor valid `Host`-based settings are provided.
