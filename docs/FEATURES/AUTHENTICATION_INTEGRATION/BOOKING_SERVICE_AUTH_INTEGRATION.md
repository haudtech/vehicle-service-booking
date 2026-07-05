# Booking Service Auth Integration Design

## Purpose
This document defines how the Vehicle Scheduling Service integrates with the external Auth/User Service using local JWT validation and service-side authorization.

---

## 1. Integration overview

### 1.1 Responsibilities
- **Auth/User Service** issues JWT access tokens and refresh tokens
- **Vehicle Scheduling Service** consumes JWTs and enforces authorization

### 1.2 Key principles
- Local JWT validation for performance
- Use `aud = vehicle-booking-api`
- Use `roles`, `groups`, and `scope` claims
- Keep booking service user-agnostic beyond token validation
- Do not implement sign-up/login in booking service

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
- token includes required claims: `sub`, `roles`, `scope`

### 3.2 JWKS discovery
- Booking service fetches public keys from `https://auth.example.com/.well-known/jwks.json`
- cache JWKS locally
- refresh keys periodically or on signature error

### 3.3 Key rotation
- auth service rotates signing keys transparently
- booking service must support multiple active keys via `kid`
- fallback when key is missing: refresh JWKS and retry once

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
- requires `scope` contains `appointment:create`
- or `roles` contains `client`, `manager`, or `admin`

`AppointmentCompletePolicy`
- requires `scope` contains `appointment:complete`
- or `roles` contains `manager` or `admin`

`AppointmentReadPolicy`
- requires `scope` contains `appointment:view`
- or `roles` contains `manager` or `admin`

### 4.3 Claim mapping
- `roles` claim maps to broad authorization categories
- `scope` claim maps to precise actions
- `groups` claim can be used for dealership or region-specific access later

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
- request booking endpoint with a token missing `appointment:create` and verify `403`
- request booking endpoint with expired token and verify `401`

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
