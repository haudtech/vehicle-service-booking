# Implementation Roadmap: Auth/User Service and Booking Service Split

## Purpose
This roadmap defines the implementation phases, tasks, and milestones for building the Auth/User Service and integrating it with the Vehicle Scheduling Service.

---

## 1. Phase 1: Validate architecture and contract

### 1.1 Confirm service boundaries
- Auth/User Service owns user identity, auth, roles, permissions, social login, and token issuance
- Vehicle Scheduling Service owns scheduling, availability, appointments, and local authorization enforcement

### 1.2 Confirm token contract
- `iss`: auth service issuer URI
- `aud`: `vehicle-booking-api`
- `sub`, `email`, `roles`, `groups`, `scope`, `exp`, `iat`
- use JWT access tokens and refresh tokens

### 1.3 Confirm initial provider support
- local email/password sign-up/login
- Google social login
- GitHub social login

---

## 2. Phase 2: Build Auth/User Service

### 2.1 Design and implement data model
- create `User`, `Role`, `Group`, `Permission`
- create `UserRole`, `UserGroup`, `RolePermission`
- create `RefreshToken`
- create `SocialProvider`

### 2.2 Build core authentication APIs
- `POST /api/v1/auth/signup`
- `POST /api/v1/auth/login`
- `POST /api/v1/auth/refresh`
- `POST /api/v1/auth/logout`
- `GET /api/v1/me`
- `GET /api/v1/.well-known/jwks.json`

### 2.3 Build social login support
- `GET /api/v1/auth/google`
- `GET /api/v1/auth/google/callback`
- `GET /api/v1/auth/github`
- `GET /api/v1/auth/github/callback`

### 2.4 Implement token issuance
- issue short-lived JWT access tokens
- issue refresh tokens with rotation and revocation
- sign JWTs with private key
- publish JWKS for validation

### 2.5 Implement role and permission management
- seed default roles and permissions
- assign default permissions to roles
- support user role assignment
- support user group assignment

---

## 3. Phase 3: Build Booking Service auth integration

### 3.1 Add JWT validation
- configure `JwtBearer` authentication
- set `Issuer` and `Audience`
- configure JWKS endpoint source
- add `app.UseAuthentication()` and `app.UseAuthorization()`

### 3.2 Define authorization policies
- `AppointmentCreatePolicy`
- `AppointmentCompletePolicy`
- `AppointmentReadPolicy`

### 3.3 Protect booking endpoints
- apply `[Authorize(Policy = "AppointmentCreate")]` to `POST /appointments`
- apply `[Authorize(Policy = "AppointmentRead")]` if `GET /availability` is not public
- apply policies for future protected actions

### 3.4 Map claims to policies
- support `scope` claim actions
- support `roles` fallback for broad role checks
- use `groups` for dealership-specific authorization later

---

## 4. Phase 4: Test and validate

### 4.1 Unit tests
- auth service: sign-up, login, refresh, revoke, social login mapping
- booking service: JWT validation and policy enforcement

### 4.2 Integration tests
- end-to-end auth service login flow
- booking service endpoints with valid and invalid tokens
- forbidden access for missing permissions

### 4.3 Security validation
- verify token expiry behavior
- verify refresh token rotation and revocation
- test JWKS key rotation support

---

## 5. Phase 5: Deploy and operate

### 5.1 Deployment considerations
- deploy auth service independently
- deploy booking service independently
- use environment configuration for issuer, JWKS URI, audience

### 5.2 Monitoring and observability
- log auth failures and token errors
- monitor JWKS refresh failures
- monitor booking service auth/forbidden events

### 5.3 Operational handoff
- document auth service contract for other teams
- publish environment variables and config values
- document token lifetimes and refresh behavior

---

## 6. Recommended sequencing

1. finalize auth data model and contract
2. build core auth service APIs
3. add JWT issuance and JWKS support
4. integrate booking service local JWT validation
5. protect booking endpoints with policies
6. add tests and verify security
7. deploy auth service, then booking service

---

## 7. Success criteria

- Auth service can issue valid JWTs and refresh tokens
- Booking service validates JWTs locally and rejects invalid tokens
- Booking endpoints enforce action-based permissions
- social login works for selected providers
- token refresh and revocation behave correctly
- documentation is available for auth contract and booking integration
