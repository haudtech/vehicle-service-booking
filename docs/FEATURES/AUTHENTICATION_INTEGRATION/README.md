# Authentication Integration Documentation Index

This folder contains the documentation for the planned authentication and scheduling service split.

## Purpose

This README provides a single entry point to the feature documents created for the auth service and booking service integration plan.

## Documents

- [AUTH_SCHEDULING_EXECUTIVE_SUMMARY.md](AUTH_SCHEDULING_EXECUTIVE_SUMMARY.md)
  - High-level summary of the auth and scheduling split, business goals, and architecture decisions.

- [AUTH_SCHEDULING_IMPLEMENTATION_ROADMAP.md](AUTH_SCHEDULING_IMPLEMENTATION_ROADMAP.md)
  - Step-by-step roadmap for implementing the auth service and booking service changes.

- [AUTH_SERVICE_SPEC.md](AUTH_SERVICE_SPEC.md)
  - Auth service requirements, API surface, JWT and token handling, and social login scope.

- [AUTH_SERVICE_DATA_MODEL.md](AUTH_SERVICE_DATA_MODEL.md)
  - Auth service database model and entity relationships for users, roles, groups, and permissions.

- [AUTH_SERVICE_DETAILED_DESIGN.md](AUTH_SERVICE_DETAILED_DESIGN.md)
  - Detailed design for auth service internals, token issuance, refresh flows, and JWKS.

- [AUTH_SERVICE_IMPLEMENTATION_PLAN.md](AUTH_SERVICE_IMPLEMENTATION_PLAN.md)
  - Concrete implementation plan with tasks and developer actions.

- [BOOKING_SERVICE_AUTH_INTEGRATION.md](BOOKING_SERVICE_AUTH_INTEGRATION.md)
  - Booking service integration guidance for JWT validation, authorization policies, and audience/issuer checks.

- [USER_AUTH_AND_SCHEDULING_SERVICE_SPLIT.md](USER_AUTH_AND_SCHEDULING_SERVICE_SPLIT.md)
  - Document describing the responsibilities split between the auth service and the booking service.

- [USER_AUTH_AND_SCHEDULING_CHECKLIST.md](USER_AUTH_AND_SCHEDULING_CHECKLIST.md)
  - Completion checklist for both auth service and booking service work.

- [USER_WORKFLOW_SIGNUP_TO_APPOINTMENT_WITH_AUTH.md](USER_WORKFLOW_SIGNUP_TO_APPOINTMENT_WITH_AUTH.md)
  - End-to-end user journey from sign-up to appointment creation with workflow/sequence diagrams and curl-by-step API execution.

## How to use this directory

1. Start with [AUTH_SCHEDULING_EXECUTIVE_SUMMARY.md](AUTH_SCHEDULING_EXECUTIVE_SUMMARY.md) to understand the overall architecture and goals.
2. Read [USER_WORKFLOW_SIGNUP_TO_APPOINTMENT_WITH_AUTH.md](USER_WORKFLOW_SIGNUP_TO_APPOINTMENT_WITH_AUTH.md) for the practical end-to-end operational flow.
3. Read [AUTH_SERVICE_SPEC.md](AUTH_SERVICE_SPEC.md) and [AUTH_SERVICE_DATA_MODEL.md](AUTH_SERVICE_DATA_MODEL.md) to validate auth service requirements and schema.
4. Use [BOOKING_SERVICE_AUTH_INTEGRATION.md](BOOKING_SERVICE_AUTH_INTEGRATION.md) for JWT validation and booking authorization details.
5. Follow [AUTH_SCHEDULING_IMPLEMENTATION_ROADMAP.md](AUTH_SCHEDULING_IMPLEMENTATION_ROADMAP.md) and [AUTH_SERVICE_IMPLEMENTATION_PLAN.md](AUTH_SERVICE_IMPLEMENTATION_PLAN.md) for implementation sequencing.
6. Review [USER_AUTH_AND_SCHEDULING_CHECKLIST.md](USER_AUTH_AND_SCHEDULING_CHECKLIST.md) before finalizing the split.

## Notes

- This directory is intended for current feature planning and implementation guidance.
- Auth login supports `identifier` (account name or email), while `email` login input remains backward compatible.
- Rename or move the folder into a more permanent location once the feature work is complete.
