# Authentication Integration Feature Guide

## Purpose
Feature-level guide for Auth/Booking integration details, contracts, implementation plans, and runbooks.

System-wide architecture and cross-service runtime views are centralized in:
1. `docs/ARCHITECTURE/README.md`
2. `docs/ARCHITECTURE/CONCEPTUAL_VIEW.md`
3. `docs/ARCHITECTURE/AUTH_COMPONENTS_VIEW.md`
4. `docs/ARCHITECTURE/AUTH_SEQUENTIAL_FLOW_VIEW.md`

## Documentation Ownership
1. Centralized architecture docs:
2. Cross-service topology and canonical runtime diagrams.
3. Feature docs in this folder:
4. Auth-specific contracts, implementation details, migration plans, and execution checklists.

## Core Feature Documents

### Specifications and Runtime Contracts
1. [AUTH_SERVICE_SPEC.md](AUTH_SERVICE_SPEC.md)
2. [AUTH_JWT_JWKS_FULL_FLOW.md](AUTH_JWT_JWKS_FULL_FLOW.md)
3. [AUTH_GOOGLE_OAUTH_FULL_FLOW.md](AUTH_GOOGLE_OAUTH_FULL_FLOW.md)
4. [AUTH_SERVICE_DATA_MODEL.md](AUTH_SERVICE_DATA_MODEL.md)
5. [BOOKING_SERVICE_AUTH_INTEGRATION.md](BOOKING_SERVICE_AUTH_INTEGRATION.md)
6. [AUTH_EMAIL_VERIFICATION_METHODS_AND_SEQUENTIAL_FLOWS.md](AUTH_EMAIL_VERIFICATION_METHODS_AND_SEQUENTIAL_FLOWS.md)
7. [AUTH_OTP_METHODODOLOGY_AND_CHANNEL_STRATEGY.md](AUTH_OTP_METHODODOLOGY_AND_CHANNEL_STRATEGY.md)

### Implementation and Planning
1. [AUTH_SCHEDULING_EXECUTIVE_SUMMARY.md](AUTH_SCHEDULING_EXECUTIVE_SUMMARY.md)
2. [AUTH_SCHEDULING_IMPLEMENTATION_ROADMAP.md](AUTH_SCHEDULING_IMPLEMENTATION_ROADMAP.md)
3. [AUTH_SERVICE_DETAILED_DESIGN.md](AUTH_SERVICE_DETAILED_DESIGN.md)
4. [AUTH_SERVICE_IMPLEMENTATION_PLAN.md](AUTH_SERVICE_IMPLEMENTATION_PLAN.md)

### Workflow and Verification
1. [USER_WORKFLOW_SIGNUP_TO_APPOINTMENT_WITH_AUTH.md](USER_WORKFLOW_SIGNUP_TO_APPOINTMENT_WITH_AUTH.md)
2. [USER_AUTH_AND_SCHEDULING_CHECKLIST.md](USER_AUTH_AND_SCHEDULING_CHECKLIST.md)
3. [USER_AUTH_AND_SCHEDULING_SERVICE_SPLIT.md](USER_AUTH_AND_SCHEDULING_SERVICE_SPLIT.md)

## Document Lifecycle Status

### Current (Primary References)
1. [AUTH_SERVICE_SPEC.md](AUTH_SERVICE_SPEC.md)
2. [AUTH_JWT_JWKS_FULL_FLOW.md](AUTH_JWT_JWKS_FULL_FLOW.md)
3. [AUTH_GOOGLE_OAUTH_FULL_FLOW.md](AUTH_GOOGLE_OAUTH_FULL_FLOW.md)
4. [AUTH_SERVICE_DATA_MODEL.md](AUTH_SERVICE_DATA_MODEL.md)
5. [BOOKING_SERVICE_AUTH_INTEGRATION.md](BOOKING_SERVICE_AUTH_INTEGRATION.md)
6. [AUTH_KEY_ROTATION_GUIDE.md](AUTH_KEY_ROTATION_GUIDE.md)
7. [USER_WORKFLOW_SIGNUP_TO_APPOINTMENT_WITH_AUTH.md](USER_WORKFLOW_SIGNUP_TO_APPOINTMENT_WITH_AUTH.md)
8. [AUTH_EMAIL_VERIFICATION_METHODS_AND_SEQUENTIAL_FLOWS.md](AUTH_EMAIL_VERIFICATION_METHODS_AND_SEQUENTIAL_FLOWS.md)
9. [AUTH_OTP_METHODODOLOGY_AND_CHANNEL_STRATEGY.md](AUTH_OTP_METHODODOLOGY_AND_CHANNEL_STRATEGY.md)

### Mixed (Current + Planned Sections)
1. [AUTH_SERVICE_DETAILED_DESIGN.md](AUTH_SERVICE_DETAILED_DESIGN.md)
2. [AUTH_SERVICE_IMPLEMENTATION_PLAN.md](AUTH_SERVICE_IMPLEMENTATION_PLAN.md)
3. [USER_AUTH_AND_SCHEDULING_CHECKLIST.md](USER_AUTH_AND_SCHEDULING_CHECKLIST.md)

### Historical / Planning Baseline (Use for context, not source of truth)
1. [AUTH_SCHEDULING_EXECUTIVE_SUMMARY.md](AUTH_SCHEDULING_EXECUTIVE_SUMMARY.md)
2. [AUTH_SCHEDULING_IMPLEMENTATION_ROADMAP.md](AUTH_SCHEDULING_IMPLEMENTATION_ROADMAP.md)
3. [USER_AUTH_AND_SCHEDULING_SERVICE_SPLIT.md](USER_AUTH_AND_SCHEDULING_SERVICE_SPLIT.md)

Notes:
1. If conflicts appear between documents, prioritize Current references first.
2. Architecture authority remains in `docs/ARCHITECTURE`.

## Recommended Reading Order
1. Start with [AUTH_SERVICE_SPEC.md](AUTH_SERVICE_SPEC.md) for the current API contract.
2. Read [AUTH_JWT_JWKS_FULL_FLOW.md](AUTH_JWT_JWKS_FULL_FLOW.md) and [AUTH_GOOGLE_OAUTH_FULL_FLOW.md](AUTH_GOOGLE_OAUTH_FULL_FLOW.md) for runtime auth flows.
3. Read [AUTH_EMAIL_VERIFICATION_METHODS_AND_SEQUENTIAL_FLOWS.md](AUTH_EMAIL_VERIFICATION_METHODS_AND_SEQUENTIAL_FLOWS.md) for verification strategy, sequential methods, and security controls.
4. Read [AUTH_OTP_METHODODOLOGY_AND_CHANNEL_STRATEGY.md](AUTH_OTP_METHODODOLOGY_AND_CHANNEL_STRATEGY.md) for OTP channel selection, challenge flow, and verification rules.
5. Read [BOOKING_SERVICE_AUTH_INTEGRATION.md](BOOKING_SERVICE_AUTH_INTEGRATION.md) for consumer-side JWT verification behavior.
6. Use [USER_WORKFLOW_SIGNUP_TO_APPOINTMENT_WITH_AUTH.md](USER_WORKFLOW_SIGNUP_TO_APPOINTMENT_WITH_AUTH.md) for operational path validation.
7. Use roadmap/plan/checklist docs for delivery tracking and release readiness.

## Notes
1. Sign-up is verification-first: `POST /api/v1/auth/signup` returns `202 Accepted` and requires verify-email before password login token issuance.
2. Password login is two-step: `POST /api/v1/auth/login` starts challenge, then `POST /api/v1/auth/login/verify-code` issues tokens.
3. Login challenge supports `otp_first`, `email_otp`, and `authenticator_app`; resolved `challengeChannel` is returned in login response.
4. Authenticator setup supports direct QR image retrieval via `GET /api/v1/auth/mfa/authenticator/setup/qr`.
5. Google callback enforces provider email verification evidence before local token issuance.
6. Auth login supports `identifier` (account name or email), while `email` input remains backward compatible.
7. Notification architecture is documented under `docs/FEATURES/NOTIFICATION` and centralized architecture docs.
