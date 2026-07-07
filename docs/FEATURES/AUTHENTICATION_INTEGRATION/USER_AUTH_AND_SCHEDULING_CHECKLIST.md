# User/Auth Service + Vehicle Scheduling Service Implementation Checklist

## Phase Overview

### Phase 1 - Completed Foundation
- [x] Auth core design, entities, and relationships implemented (Sections 1, 2)
- [x] Core auth endpoints implemented: signup, login, refresh, logout, me, jwks (Section 3)
- [x] Booking JWT/JWKS integration and authorization policies implemented (Section 4)
- [x] Token model baseline implemented for booking use case (Section 5)
- [x] Core security operations implemented (lifetimes, revocation, jwks publishing) (Section 7)
- [x] Cross-service compatibility baseline implemented (Section 8)
- [x] Build and test validation baseline recorded (Section 11)

### Phase 2 - Completed Hardening
- [x] Complete appointment creation happy-path test with auth (Section 10)
- [x] Finalize architecture and sequence documentation baseline (Section 9)
- [x] Define shared role/group semantics across services (Section 8)

### Phase 3 - Next Capabilities
- [ ] Social login design and implementation (Google/GitHub + callback + provisioning) (Section 6)
- [ ] Admin endpoints for user/group/role management (Section 3)
- [ ] JWT key rotation runtime execution validation (Sections 7, 10)
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
- [x] Define key rotation strategy
- [x] Publish JWKS endpoint
- [x] Ensure secure storage of signing keys (local development implementation)
- [x] Define error responses for auth failures

Key rotation strategy references:
- [x] docs/FEATURES/AUTHENTICATION_INTEGRATION/AUTH_JWT_JWKS_FULL_FLOW.md (Section 11)
- [x] docs/FEATURES/AUTHENTICATION_INTEGRATION/BOOKING_SERVICE_AUTH_INTEGRATION.md (Section 3.3)

## 8. Multi-Service Compatibility
- [x] Publish auth service contract (implemented endpoints and claims in code)
- [x] Document audience/issuer values (appsettings for auth and booking)
- [x] Define group/role semantics across services
- [x] Document shared permission model (seeded permissions + policies)
- [x] Confirm local JWT validation approach
- [ ] Confirm optional introspection fallback plan

Semantics baseline references:
- [x] docs/FEATURES/AUTHENTICATION_INTEGRATION/AUTH_SERVICE_SPEC.md (Section 3.0)
- [x] docs/FEATURES/AUTHENTICATION_INTEGRATION/BOOKING_SERVICE_AUTH_INTEGRATION.md (Sections 1.3, 4.2, 4.3)

## 9. Documentation
- [x] Create conceptual architecture docs
- [x] Create workflow and sequence diagrams
- [x] Add practical request/response examples
- [x] Add checklist for implementation tasks
- [ ] Review with stakeholders

Documentation baseline references:
- [x] docs/ARCHITECTURE/CONCEPTUAL_VIEW.md updated with cross-service auth-booking view
- [x] docs/ARCHITECTURE/AUTH_COMPONENTS_VIEW.md added
- [x] docs/ARCHITECTURE/AUTH_SEQUENTIAL_FLOW_VIEW.md added
- [x] docs/ARCHITECTURE/DATA_DATABASE_VIEW.md updated with auth-db vs booking-db boundaries

## 10. Validation and Testing
- [x] Validate JWT locally in booking service
- [x] Test successful appointment creation with auth
- [x] Test forbidden access for unauthorized roles
- [ ] Test social login flow end-to-end
- [x] Test refresh token flow
- [x] Test JWT key rotation handling
- [x] Define JWT key rotation validation coverage plan
- [x] Add unit tests for JWKS kid resolution behavior (cache hit, refresh-on-miss, fail closed on unresolved kid)
- [x] Add deterministic Auth key-ring retirement tests (overlap active then retired-key rejection)

JWT key rotation validation coverage references:
- [x] docs/FEATURES/AUTHENTICATION_INTEGRATION/AUTH_JWT_JWKS_FULL_FLOW.md (Section 12)
- [x] docs/FEATURES/AUTHENTICATION_INTEGRATION/BOOKING_SERVICE_AUTH_INTEGRATION.md (Section 8.4)

Validation evidence:
- [x] End-to-end API workflow passed using tests/integration/http/user_workflow_signup_to_appointment_with_auth.http (signup -> login -> me -> availability -> create appointment -> refresh -> logout -> jwks)
- [x] JWKS provider unit tests passed: tests/VehicleServiceBooking.Tests/Api/Configuration/JwksSigningKeyProviderTests.cs (3/3)
- [x] Auth key-ring rotation tests passed: tests/VehicleServiceBooking.Tests/Auth/Services/RsaSigningKeyProviderTests.cs (3/3)

## 11. Session Validation Snapshot (2026-07-04)
- [x] Full solution build passed: `dotnet build VehicleServiceBooking.slnx`
- [x] Test suite passed: 57/57 tests
- [x] Live integration smoke test passed
  - [x] No token to booking endpoint -> 401
  - [x] Valid auth-issued token to booking endpoint -> 200

## 12. Session Validation Snapshot (2026-07-06)
- [x] Re-ran live booking runtime smoke check in current environment
  - [x] Exit 134 root cause identified as port binding conflict on launch-profile default `localhost:5280`
  - [x] Booking API starts successfully on `localhost:5290` with `--no-launch-profile`
  - [x] Protected availability endpoint returns `401` without token (service healthy + auth middleware active)
- [x] Implemented JWKS key-resolution hardening in booking service
  - [x] Unknown `kid` now triggers one forced JWKS refresh
  - [x] Request fails closed when `kid` is still unresolved
  - [x] Unit validation passed in focused suite (3 tests)
- [x] Implemented Auth in-memory key-ring baseline with rotation policy config
  - [x] Auth now publishes active + overlap verification keys in JWKS
  - [x] Added internal non-production key rotation trigger endpoint: `POST /api/v1/internal/keys/rotate`
  - [x] Runtime smoke evidence captured for active/previous/unknown key scenarios
    - [x] Pre-rotation token accepted by Booking auth layer (non-401 response)
    - [x] Post-rotation JWKS key count observed as `2` (active + overlap)
    - [x] Pre-rotation token still accepted during overlap (non-401 response)
    - [x] Token with unknown `kid` rejected with `401`
  - [x] Retired-key post-overlap rejection validated in deterministic unit tests (`AdjustableTimeProvider` path)

## 13. Remaining Backlog (Phase 3 Only)
- [ ] Social login end-to-end (providers, callback, provisioning)
- [ ] Admin user/group/role management endpoints
- [ ] Expand key rotation runtime matrix coverage beyond current smoke scenarios
- [ ] Decide and document optional introspection fallback plan
- [ ] Stakeholder review of finalized documentation set
