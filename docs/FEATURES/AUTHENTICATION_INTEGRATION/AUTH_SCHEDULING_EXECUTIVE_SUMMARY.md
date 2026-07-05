# Executive Summary: Auth/User Service and Vehicle Scheduling Split

## Purpose
This executive summary provides a concise overview of the planned architecture split between:
- the Auth/User Service
- the Vehicle Scheduling Service

It is intended for stakeholders, technical leads, and implementation planning.

---

## 1. Summary

The current Vehicle Scheduling Service is a standalone booking API with no authentication or user management.

The proposed architecture separates identity from scheduling:
- **Auth/User Service** handles users, sign-up, login, social login, roles, permissions, and JWT issuance
- **Vehicle Scheduling Service** validates JWTs locally and enforces booking authorization

This architecture supports future extension services by using the Auth/User Service as a central identity provider.

---

## 2. Why this split?

### Key benefits
- separates concerns clearly
- improves security and scalability
- enables social login and user sign-up without coupling scheduling logic
- allows multiple services to reuse the same auth provider
- keeps the booking API lightweight and focused

### What is not included in Vehicle Scheduling Service
- user registration
- login/authentication
- social provider integrations
- role/group management

---

## 3. What each service owns

### Auth/User Service
- user sign-up
- user login
- refresh tokens
- logout
- social login (Google, GitHub)
- groups and roles
- permissions
- JWT issuance
- JWKS discovery
- user profile endpoint
- admin management endpoints

### Vehicle Scheduling Service
- availability queries
- appointment creation
- appointment lifecycle management
- local JWT validation
- booking-specific authorization policies
- no user identity storage beyond token validation

---

## 4. Core architecture

### High-level flow
1. user signs up / logs in via Auth/User Service
2. Auth/User Service issues JWT
3. client calls Vehicle Scheduling Service with `Authorization: Bearer <token>`
4. Vehicle Scheduling Service validates JWT locally
5. Vehicle Scheduling Service executes booking workflows

### Token design
- access token: JWT
- claims include `sub`, `iss`, `aud`, `roles`, `groups`, `scope`, `exp`, `iat`
- audience: `vehicle-booking-api`
- token validation is local in booking service

---

## 5. Implementation status

### Completed documentation
- `docs/FEATURES_TEMP/USER_AUTH_AND_SCHEDULING_SERVICE_SPLIT.md`
- `docs/FEATURES_TEMP/USER_AUTH_AND_SCHEDULING_CHECKLIST.md`
- `docs/FEATURES_TEMP/AUTH_SERVICE_SPEC.md`
- `docs/FEATURES_TEMP/AUTH_SERVICE_DATA_MODEL.md`
- `docs/FEATURES_TEMP/BOOKING_SERVICE_AUTH_INTEGRATION.md`

### Pending next steps
- finalize auth service data model details if needed
- implement auth service API and persistence
- implement JWT validation and authorization in booking service
- create example service contract and deployment notes

---

## 6. Recommended next action
Proceed with building the Auth/User Service first, including:
- sign-up/login endpoints
- refresh token flow
- JWKS endpoint
- social login support
- role/permission mapping

Then integrate the Booking Service by:
- configuring JWT validation
- defining authorization policies
- protecting booking endpoints

---

## 7. Stakeholder impact

### For product owners
- provides a clear path to secure access control
- enables user self-service through auth service
- supports future multi-service expansion

### For developers
- reduces coupling between identity and scheduling code
- makes auth behavior reusable across services
- simplifies booking service implementation

### For operations
- simplifies key rotation via JWKS
- centralizes auth auditing and security controls
- allows separate scaling of auth and booking runtimes
