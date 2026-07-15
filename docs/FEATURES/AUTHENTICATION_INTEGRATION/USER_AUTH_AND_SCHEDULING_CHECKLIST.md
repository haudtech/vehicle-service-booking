# User/Auth Service + Vehicle Scheduling Service Implementation Checklist

## Status
Current completion snapshot for the auth + booking integration work. Historical phase notes were removed from the active reading path.

## What is complete
- Auth core design, entities, and relationships are implemented.
- Signup, email verification, login challenge, token refresh, logout, `me`, and JWKS endpoints are implemented.
- Google OAuth flow is implemented and tested end to end.
- Booking JWT/JWKS validation and authorization policies are implemented.
- Key rotation handling and validation coverage are implemented.
- Cross-service auth and booking workflow docs are aligned with code.
- End-to-end HTTP workflow validation is in place.

## What remains
- Optional GitHub social login.
- Admin endpoints for user, group, and role management.
- Optional introspection fallback plan.
- Broader key-rotation runtime matrix coverage beyond current smoke scenarios.
- Stakeholder review of the final documentation set.

## Current domain coverage
1. Auth/user service responsibilities
   - user registration and sign-up
   - user login
   - Google social login
   - role and permission mapping
   - JWT issuance and refresh
   - JWKS endpoint

2. Auth service data model
   - User, Role, Group, Permission
   - UserRole, UserGroup, RolePermission
   - RefreshToken
   - SocialProvider

3. Auth service endpoints
   - `POST /api/v1/auth/signup`
   - `GET /api/v1/auth/verify-email`
   - `POST /api/v1/auth/verify-email`
   - `POST /api/v1/auth/login`
   - `POST /api/v1/auth/login/verify-code`
   - `POST /api/v1/auth/refresh`
   - `POST /api/v1/auth/logout`
   - `GET /api/v1/.well-known/jwks.json`
   - `GET /api/v1/auth/google/callback`
   - `GET /api/v1/me`

4. Booking service integration
   - JWT validation middleware
   - issuer and audience configuration
   - JWKS/public key refresh
   - authorization policies
   - claim-to-permission mapping

## Verification snapshot
- Full solution build passes.
- Auth and booking test suites pass.
- End-to-end auth-to-booking workflow passes.
- Google callback runtime flow passes with local tokens.
- JWKS key resolution and auth key-ring rotation behaviors are covered.

## How to use this file
Use this file as the quick current-state checklist for auth/booking integration.
Use [AUTH_SERVICE_SPEC.md](AUTH_SERVICE_SPEC.md) for the API contract and [BOOKING_SERVICE_AUTH_INTEGRATION.md](BOOKING_SERVICE_AUTH_INTEGRATION.md) for the consumer-side JWT behavior.
