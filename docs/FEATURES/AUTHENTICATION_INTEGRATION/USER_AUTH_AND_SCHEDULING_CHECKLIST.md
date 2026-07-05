# User/Auth Service + Vehicle Scheduling Service Implementation Checklist

## Phase Overview

### Phase 1 - Completed Foundation
- [x] Auth core design, entities, and relationships implemented (Sections 1, 2)
- [x] Core auth endpoints implemented: signup, login, refresh, logout, me, jwks (Section 3)
- [x] Booking JWT/JWKS integration and authorization policies implemented (Section 4)
- [x] Token model baseline implemented for booking use case (Section 5)
- [x] Core security operations implemented (lifetimes, revocation, jwks publishing) (Section 7)
- [x] Cross-service compatibility baseline implemented (Section 8)
- [x] End-to-end validation passed: build, tests, and runtime smoke checks (Section 11)

### Phase 2 - In Progress Hardening
- [ ] Complete appointment creation happy-path test with auth (Section 10)
- [ ] Finalize architecture and sequence documentation for stakeholders (Section 9)
- [ ] Define shared role/group semantics across services (Section 8)

### Phase 3 - Next Capabilities
- [ ] Social login design and implementation (Google/GitHub + callback + provisioning) (Section 6)
- [ ] Admin endpoints for user/group/role management (Section 3)
- [ ] JWT key rotation strategy and validation coverage (Sections 7, 10)
- [ ] Optional introspection fallback strategy (Section 8)

## Detailed Checklist (By Domain)

## 1. Auth/User Service Design
- [x] Define auth service responsibilities
  - [x] user registration / sign-up
  - [x] user login
  - [ ] social login / OAuth2 integration
  - [ ] group membership management
  - [x] role management (seeded default role for booking access)
  - [x] permission mapping (role-permission mapping seeded)
  - [x] JWT issuance and refresh
  - [x] JWKS endpoint for public key discovery

## 2. Auth Service Data Model
- [x] Define User entity
- [x] Define Role entity
- [x] Define Group entity
- [x] Define Permission entity
- [x] Define UserRole relationship
- [x] Define UserGroup relationship
- [x] Define RolePermission relationship
- [x] Define RefreshToken entity
- [x] Add shared auth base entity fields (Id, CreatedAt, UpdatedAt, IsActive)

## 3. Auth Service Endpoints
- [x] POST /api/v1/auth/signup
- [x] POST /api/v1/auth/login
- [x] POST /api/v1/auth/refresh
- [x] POST /api/v1/auth/logout
- [x] GET /api/v1/.well-known/jwks.json
- [ ] GET /api/v1/auth/{provider}/callback (social login)
- [x] GET /api/v1/me
- [ ] Admin endpoints for user/group/role management

## 4. Booking Service Integration
- [x] Add JWT validation middleware
- [x] Configure issuer and audience
- [x] Configure JWKS/public key refresh
- [x] Define authorization policies
  - [x] AppointmentCreatePolicy
  - [x] AppointmentCompletePolicy
  - [x] AppointmentReadPolicy
- [x] Apply [Authorize] to booking controllers/actions
- [x] Map JWT claims to booking permissions

## 5. Token Model
- [x] Define standard JWT claims
  - [x] sub
  - [x] email
  - [x] iss
  - [x] aud
  - [x] roles
  - [ ] groups
  - [ ] scope
  - [x] exp
  - [x] iat
- [x] Define service-specific audience values
  - [x] vehicle-booking-api
  - [ ] inventory-api
  - [ ] reporting-api
- [x] Define permission naming conventions
  - [x] appointment:create
  - [x] appointment:complete
  - [x] appointment:view

## 6. Social Login
- [ ] Select providers to support
  - [ ] Google
  - [ ] GitHub
- [ ] Define OAuth2 callback flow
- [ ] Implement user provisioning from provider claims
- [ ] Map external identity to internal user record

## 7. Security and Operations
- [x] Set access token lifetime (15-30 minutes)
- [x] Set refresh token lifetime
- [x] Define token revocation strategy
- [ ] Define key rotation strategy
- [x] Publish JWKS endpoint
- [x] Ensure secure storage of signing keys (local development implementation)
- [x] Define error responses for auth failures

## 8. Multi-Service Compatibility
- [x] Publish auth service contract (implemented endpoints and claims in code)
- [x] Document audience/issuer values (appsettings for auth and booking)
- [ ] Define group/role semantics across services
- [x] Document shared permission model (seeded permissions + policies)
- [x] Confirm local JWT validation approach
- [ ] Confirm optional introspection fallback plan

## 9. Documentation
- [ ] Create conceptual architecture docs
- [ ] Create workflow and sequence diagrams
- [x] Add practical request/response examples
- [x] Add checklist for implementation tasks
- [ ] Review with stakeholders

## 10. Validation and Testing
- [x] Validate JWT locally in booking service
- [ ] Test successful appointment creation with auth
- [x] Test forbidden access for unauthorized roles
- [ ] Test social login flow end-to-end
- [x] Test refresh token flow
- [ ] Test JWT key rotation handling

## 11. Session Validation Snapshot (2026-07-04)
- [x] Full solution build passed: `dotnet build VehicleServiceBooking.slnx`
- [x] Test suite passed: 57/57 tests
- [x] Live integration smoke test passed
  - [x] No token to booking endpoint -> 401
  - [x] Valid auth-issued token to booking endpoint -> 200
