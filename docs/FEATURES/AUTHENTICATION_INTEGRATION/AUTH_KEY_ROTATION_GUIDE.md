# Auth Service JWT Key Rotation Guide

## Purpose
This document explains JWT signing key rotation in this repository:
- what key rotation is
- why the Auth service needs it
- how the implemented mechanism works
- what can go wrong without rotation
- advantages and disadvantages
- when key rotation should be applied

## 1. What Is Key Rotation?
Key rotation is the controlled replacement of the JWT signing key used by the Auth service.

In this architecture:
- Auth signs new access tokens with one active RSA private key.
- Auth publishes public verification keys through JWKS.
- Booking validates token signatures by matching JWT header `kid` to JWKS keys.

Rotation means:
1. A new key becomes active for signing new tokens.
2. The previous key remains temporarily valid for verification (overlap window).
3. The previous key is retired after overlap expires.

## 2. Why We Need It In Auth Service
Key rotation is required to reduce security and operational risk.

Primary reasons:
- Limits blast radius: if a key is compromised, exposure is bounded in time.
- Supports cryptographic hygiene: long-lived static signing keys are risky.
- Enables non-breaking transitions: clients with valid in-flight tokens still work during overlap.
- Supports operational control: emergency rollover is possible without full auth downtime.

Repository-specific reason:
- This system is split into issuer (Auth) and verifier (Booking) using JWKS. Rotation is the safe way to evolve signing keys while keeping cross-service validation stable.

## 3. Implemented Mechanism (Current Repo)
The implementation is in-memory key-ring rotation with overlap.

Configuration model:
- `EnableRotation`
- `OverlapMinutes`
- `MaxPublishedKeys`

Core behavior:
1. Auth initializes with one active key (`kid=A`).
2. Auth issues token T1 signed by key A.
3. Rotation is triggered (internal endpoint in non-production):
   - previous active key A is marked for retirement at now + overlap
   - new key B becomes active
4. Auth JWKS publishes both A and B during overlap.
5. Booking can validate:
   - old token T1 (`kid=A`) during overlap
   - new token T2 (`kid=B`) after rotation
6. After overlap expires, key A is removed from validation set and JWKS.
7. Tokens signed by A then fail authentication.

Code points:
- Key ring and lifecycle: `src/VehicleServiceBooking.Auth/Services/RsaSigningKeyProvider.cs`
- Rotation config: `src/VehicleServiceBooking.Auth/Configuration/KeyRotationOptions.cs`
- Non-prod rotate endpoint: `src/VehicleServiceBooking.Auth/Controllers/KeyManagementController.cs`
- Deterministic retirement tests: `tests/VehicleServiceBooking.Tests/Auth/Services/RsaSigningKeyProviderTests.cs`
- Booking unknown-kid fail-closed validation tests: `tests/VehicleServiceBooking.Tests/Api/Configuration/JwksSigningKeyProviderTests.cs`

## 4. Proven Scenario Example
This scenario is already validated in this repository.

### Runtime smoke (auth-layer signal)
Observed in session validation:
- pre-rotation token: non-401 in Booking (auth accepted)
- after rotate: JWKS key count became 2 (active + overlap)
- unknown `kid`: 401 (fail-closed)

Note:
- Some protected endpoint responses were `400` due local DB connection configuration, but authentication signal remained clear:
  - non-401 means JWT passed auth layer
  - 401 means JWT rejected by auth layer

### Deterministic unit proof
`RsaSigningKeyProviderTests` validates:
1. Overlap active -> old and new kids both present for validation/JWKS.
2. Overlap expired -> old kid removed from validation/JWKS.
3. Unknown kid lookup -> empty key set.

This directly proves retired-key rejection without wall-clock waiting.

## 5. What Happens If Key Rotation Is Missed?
If key rotation is not applied (or applied incorrectly), the system carries avoidable risk.

Security risks:
- Long-lived key compromise impact increases.
- Revocation/containment of leaked private key becomes slower and harder.

Operational risks:
- Emergency replacement becomes high-risk and rushed.
- Clients may experience broad auth failures if rollover is done without overlap planning.

Compliance/governance risks:
- Audit and policy requirements for periodic credential rotation may not be met.

## 6. Advantages and Disadvantages
### Advantages
- Improved security posture through shorter key lifetime.
- Safer rollover with overlap and `kid`-based deterministic verification.
- Better incident response for key compromise.
- Compatible with JWKS-based distributed validation.

### Disadvantages
- Additional operational complexity (schedule, monitoring, rollback handling).
- More test matrix cases (active/overlap/retired/unknown kid).
- Requires good observability for `kid` mismatch and validation failures.
- In-memory key-ring is process-local; full resilience may require persistent/HSM-backed key management in future phases.

## 7. When Should Key Rotation Be Applied?
Apply rotation in these scenarios:
1. Periodic policy rotation:
   - Example: every 30 days (or per security policy).
2. Security event response:
   - suspected private-key leak
   - unauthorized infrastructure access
3. Planned crypto maintenance:
   - algorithm/key-size migration windows
   - signer infrastructure migration
4. Before high-risk releases:
   - environment migration or major auth changes where clean key baseline is desired

Do not skip overlap planning:
- overlap should be at least max access-token TTL plus clock skew
- retire old key only after overlap completion

## 8. Recommended Operational Baseline For This Repo
- Keep `EnableRotation=true` outside restricted local experiments.
- Use overlap >= access-token lifetime in minutes.
- Monitor and alert on repeated unknown-`kid` validation failures in Booking.
- Keep deterministic rotation tests in CI:
  - `RsaSigningKeyProviderTests`
  - `JwksSigningKeyProviderTests`
- In Production, avoid exposing rotation endpoint publicly; use controlled internal automation.

### Choosing MaxPublishedKeys (2 or 3)
Use this rule:
- If you rotate at most once per overlap window, `MaxPublishedKeys=2` is enough (active + one previous).
- If a second rotation can happen before the first previous key expires, set `MaxPublishedKeys=3`.

Why:
- During consecutive rotations inside one overlap window, three non-retired keys can exist at once.
- Publishing only two keys can hide one still-valid previous key from JWKS, causing avoidable signature validation failures in downstream services.

Does mechanism/flow change when moving from 2 to 3:
- No core behavior changes.
- Auth still signs with one active key.
- Booking still validates by `kid` against JWKS.
- Overlap and retirement logic remain the same.
- Only JWKS publication capacity changes, allowing one additional still-valid previous key to be published.

## 9. Summary
Key rotation in this Auth service is an implemented security control, not just a design note. It uses active+overlap key-ring publication via JWKS, fail-closed unknown-`kid` validation in Booking, and deterministic retirement tests to prove behavior. This reduces compromise risk while preserving service continuity during key rollover.
