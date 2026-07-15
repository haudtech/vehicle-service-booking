# Verification Email Principle Rules

Document Version: 1.0
Last Updated: July 14, 2026
Status: Active
Audience: Developers, reviewers, and release owners

## Purpose
This document defines strict implementation and review rules for verification email and related auth verification flows.

These rules are non-optional for any change that touches:
1. Sign-up verification
2. Password login verification challenge
3. Google login verification trust checks
4. Auth-to-notification message contracts

## Rule 1: Schema-First Compatibility
Rules:
1. Do not deploy auth code that reads new columns before migrations are applied in the target environment.
2. Release checklist must include migration status verification before accepting traffic.
3. Rollback plan must include migration compatibility statement.

Code Review Gate:
1. Reviewer confirms migration exists for every new verification field.
2. Reviewer confirms migration has been applied in target environment checklist.

Primary references:
1. src/VehicleServiceBooking.Auth/Data/AuthDbContext.cs
2. src/VehicleServiceBooking.Auth/Migrations/20260714052508_AddEmailVerificationFlow.cs
3. src/VehicleServiceBooking.Auth/Migrations/20260714061732_AddLoginVerificationChallenge.cs

## Rule 2: Raw Secret Non-Persistence
Rules:
1. Never store raw verification tokens or raw verification codes.
2. Persist only hash, expiry, and consumed state.
3. Never log raw token/code in application logs.

Code Review Gate:
1. Reviewer verifies persistence models do not contain raw token fields.
2. Reviewer verifies hashing is used before save.

Primary references:
1. src/VehicleServiceBooking.Auth/Services/AuthService.cs
2. src/VehicleServiceBooking.Auth/common/Verification/VerificationUtils.cs

## Rule 3: Constant-Time Verification Compare
Rules:
1. Token/code hash comparison must use constant-time comparison.
2. Never use plain string equality for verification secrets.

Code Review Gate:
1. Reviewer verifies CryptographicOperations.FixedTimeEquals is used.

Primary references:
1. src/VehicleServiceBooking.Auth/Services/AuthService.cs

## Rule 4: Verification Gate Before Token Issuance
Rules:
1. Password login must not issue JWT before local email verification succeeds.
2. Password login must complete challenge verification before token issuance.
3. OAuth login must require verified-email evidence from trusted provider attributes.

Code Review Gate:
1. Reviewer confirms blocked path for unverified users.
2. Reviewer confirms token issuance happens only after verification gates pass.

Primary references:
1. src/VehicleServiceBooking.Auth/Services/AuthService.cs
2. src/VehicleServiceBooking.Auth/Controllers/AuthController.cs

## Rule 5: Multi-Source Claim Resilience for OAuth
Rules:
1. Do not rely on one claim name only for provider verification status.
2. OAuth callback logic must support claim fallback and token fallback where provider mapping differs.
3. Token persistence setting must support fallback source access when required.

Code Review Gate:
1. Reviewer confirms claim fallback list is implemented.
2. Reviewer confirms provider configuration persists required token data for fallback parsing.

Primary references:
1. src/VehicleServiceBooking.Auth/Controllers/AuthController.cs
2. src/VehicleServiceBooking.Auth/Configuration/ServiceCollectionExtensions.cs

## Rule 6: Graceful Business Error Mapping
Rules:
1. Expected business failures must return deterministic 4xx responses.
2. Controller must not leak business exceptions as unhandled 500 errors.

Code Review Gate:
1. Reviewer confirms InvalidOperationException paths are mapped to intentional status codes.

Primary references:
1. src/VehicleServiceBooking.Auth/Controllers/AuthController.cs

## Rule 7: Notification Decoupling and Bounded Publish
Rules:
1. Sign-up and login verification flows must remain functional if notification publish times out.
2. Notification publish must have bounded timeout and safe failure handling.
3. Email payload should include plain text fallback and optional HTML content for compatibility.

Code Review Gate:
1. Reviewer confirms timeout/fallback handling exists around publish.
2. Reviewer confirms plain-text content remains present when HTML is added.

Primary references:
1. src/VehicleServiceBooking.Auth/Controllers/AuthController.cs
2. src/VehicleServiceBooking.Notification.Functions/Services/SendGridEmailSender.cs
3. src/VehicleServiceBooking.Notification.Functions/Services/GoogleEmailSender.cs

## Rule 8: Producer-Consumer Contract Lockstep
Rules:
1. Any notification contract change must be applied in producer and consumer in the same PR.
2. Queue payload schema must remain backward-compatible or include coordinated rollout notes.

Code Review Gate:
1. Reviewer confirms matching contract fields exist in both services.
2. Reviewer confirms no orphan field changes across boundaries.

Primary references:
1. src/VehicleServiceBooking.Auth/Models/Notifications/NotificationMessage.cs
2. src/VehicleServiceBooking.Notification.Functions/Models/NotificationMessage.cs

## Rule 9: Concurrency and Unique Constraint Safety
Rules:
1. Pre-checks for duplicate users are required for UX-quality errors.
2. Save path must still handle DB unique-constraint races deterministically.

Code Review Gate:
1. Reviewer confirms business pre-checks exist.
2. Reviewer confirms DB unique-violation catch is mapped to stable business error.

Primary references:
1. src/VehicleServiceBooking.Auth/Services/AuthService.cs
2. src/VehicleServiceBooking.Auth/Data/AuthDbContext.cs

## Rule 10: Test and Workflow Lockstep
Rules:
1. Any verification flow change must update unit tests and integration HTTP workflows in same change set.
2. API examples must represent current status codes and sequence.

Code Review Gate:
1. Reviewer confirms tests updated with changed contract signatures and response semantics.
2. Reviewer confirms integration HTTP files reflect latest flow.

Primary references:
1. tests/VehicleServiceBooking.Tests/Auth/Controllers/AuthControllerGoogleTests.cs
2. tests/VehicleServiceBooking.Tests/Auth/Services/AuthServiceGoogleLoginTests.cs
3. tests/integration/http/user_workflow_signup_to_appointment_with_auth.http
4. tests/integration/http/user_workflow_google_login_to_appointment_with_auth.http

## Mandatory PR Checklist (Verification Features)
A PR touching verification is merge-ready only when all are true:
1. Migration compatibility is proven for target environment.
2. Raw verification secrets are never persisted or logged.
3. Verification gates are enforced before token issuance.
4. OAuth verification logic is resilient to provider claim variation.
5. Controller maps business failures to deterministic 4xx responses.
6. Notification publish is bounded and non-blocking to business completion.
7. Producer-consumer message contracts are updated in lockstep.
8. Concurrency-safe uniqueness handling exists at save boundary.
9. Unit tests and integration workflow files were updated and validated.
