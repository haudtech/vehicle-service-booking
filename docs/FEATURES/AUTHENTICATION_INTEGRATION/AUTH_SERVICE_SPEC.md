# Auth/User Service API Specification

## Purpose
This document defines the Auth/User Service API contract for:
- user sign-up
- email verification
- login
- login challenge verification
- refresh tokens
- logout
- social login
- JWKS discovery
- user profile
- admin user/group/role management

This service is intended to be the central identity provider for the Vehicle Scheduling Service and other future services.

## Status
Current source of truth for implemented Auth contracts.

---

## 1. API Overview

### Base URL
`https://auth.example.com/api/v1`

### Content type
- `application/json`

### Authentication
- Public endpoints (implemented): sign-up, login, refresh, logout, JWKS
- Public endpoints (implemented): Google OAuth start/callback
- Protected endpoints (implemented): user profile (`/me`), MFA authenticator setup start/qr/verify
- Planned endpoints (not implemented yet): admin endpoints
- Access token format: JWT via `Authorization: Bearer <token>`

---

## 2. Core Endpoints

### 2.1 POST /api/v1/auth/signup
Create a new user account.

Request:
```http
POST /api/v1/auth/signup
Content-Type: application/json

{
  "email": "alice@example.com",
  "accountName": "alice.smith",
  "password": "P@ssw0rd!",
  "displayName": "Alice Smith"
}
```

Response:
```http
202 Accepted
Content-Type: application/json

{
  "message": "Sign-up successful. Please verify your email before signing in.",
  "email": "alice@example.com",
  "verificationLink": "https://auth.example.com/api/v1/auth/verify-email?email=alice%40example.com&token=...",
  "verificationTokenExpiresAtUtc": "2026-07-15T08:00:00Z"
}
```

Validation errors:
- `400 Bad Request` for invalid email, weak password, or missing fields
- `409 Conflict` when email/account already exists

### 2.2 GET /api/v1/auth/verify-email
Verify email using callback query parameters.

Request:
```http
GET /api/v1/auth/verify-email?email=alice@example.com&token=... 
```

Response:
```http
200 OK
Content-Type: application/json

{
  "message": "Email verified successfully."
}
```

Errors:
- `400 Bad Request` for invalid/expired token

### 2.3 POST /api/v1/auth/verify-email
Verify email via JSON body (non-callback client flow).

Request:
```http
POST /api/v1/auth/verify-email
Content-Type: application/json

{
  "email": "alice@example.com",
  "token": "..."
}
```

Response:
```http
204 No Content
```

Errors:
- `400 Bad Request` for invalid/expired token

### 2.4 POST /api/v1/auth/login
Authenticate user using local credentials (AccountName or Email).

Request:
```http
POST /api/v1/auth/login
Content-Type: application/json

{
  "identifier": "alice.smith",
  "challengeChannel": "otp_first",
  "password": "P@ssw0rd!"
}
```

Backward-compatible alternative request body:

```http
POST /api/v1/auth/login
Content-Type: application/json

{
  "email": "alice@example.com",
  "challengeChannel": "email_otp",
  "password": "P@ssw0rd!"
}
```

Response:
```http
202 Accepted
Content-Type: application/json

{
  "message": "Credentials accepted. Open your authenticator app and submit the current OTP code.",
  "email": "alice@example.com",
  "challengeId": "550e8400-e29b-41d4-a716-446655440000",
  "challengeChannel": "authenticator_app",
  "verificationCodeExpiresAtUtc": "2026-07-14T08:10:00Z"
}
```

Notes:
- `challengeChannel` in response is the resolved channel (`email_otp` or `authenticator_app`).
- `otp_first` is accepted on request and resolved server-side based on user MFA state.

Errors:
- `400 Bad Request` for missing fields
- `401 Unauthorized` for invalid credentials
- `401 Unauthorized` for disabled or unverified accounts

### 2.5 POST /api/v1/auth/login/verify-code
Complete login challenge and issue tokens.

Request:
```http
POST /api/v1/auth/login/verify-code
Content-Type: application/json

{
  "email": "alice@example.com",
  "challengeId": "550e8400-e29b-41d4-a716-446655440000",
  "code": "123456"
}
```

Response:
```http
200 OK
Content-Type: application/json

{
  "accessToken": "<jwt>",
  "refreshToken": "<refresh-token>",
  "expiresIn": 900
}
```

Errors:
- `401 Unauthorized` for invalid/expired challenge or code

### 2.6 POST /api/v1/auth/refresh
Exchange a refresh token for a new access token.

Request:
```http
POST /api/v1/auth/refresh
Content-Type: application/json

{
  "refreshToken": "<refresh-token>"
}
```

Response:
```http
200 OK
Content-Type: application/json

{
  "accessToken": "<jwt>",
  "refreshToken": "<refresh-token>",
  "expiresIn": 900
}
```

Errors:
- `400 Bad Request` for missing token
- `401 Unauthorized` for invalid or revoked refresh token

### 2.7 POST /api/v1/auth/logout
Revoke the refresh token and/or invalidate session.

Request:
```http
POST /api/v1/auth/logout
Content-Type: application/json

{
  "refreshToken": "<refresh-token>"
}
```

Response:
```http
204 No Content
```

Errors:
- `400 Bad Request` for missing request fields (validator/model binding)
- `204 No Content` is returned for accepted revoke requests

### 2.8 GET /api/v1/.well-known/jwks.json
Expose public keys for JWT validation.

Response:
```http
200 OK
Content-Type: application/json

{
  "keys": [
    {
      "kty": "RSA",
      "kid": "2026-07-03-key-1",
      "use": "sig",
      "alg": "RS256",
      "n": "...",
      "e": "AQAB"
    }
  ]
}
```

### 2.9 GET /api/v1/auth/google/start
Begin Google sign-in flow.

Example:
```http
GET /api/v1/auth/google/start
```

Behavior:
- redirect to provider authorization URL
- include client_id, redirect_uri, scope, state

### 2.10 GET /api/v1/auth/google/callback
Handle Google callback and issue local tokens.

Example:
```http
GET /api/v1/auth/google/callback?code=...&state=...
```

Behavior:
- exchange code for provider token
- retrieve user profile + verified-email evidence
- create or map internal user
- issue JWT and refresh token
- return token payload (`200 OK`)

Response example:
```http
200 OK
Content-Type: application/json

{
  "accessToken": "<jwt>",
  "refreshToken": "<refresh-token>",
  "expiresIn": 900
}
```

Errors:
- `400 Bad Request` when callback parameters are invalid
- `401 Unauthorized` when provider exchange fails or provider email is not verified

### 2.11 GET /api/v1/me
Get current user profile.

Request:
```http
GET /api/v1/me
Authorization: Bearer <accessToken>
```

Response:
```http
200 OK
Content-Type: application/json

{
  "claims": [
    { "type": "sub", "value": "550e8400-e29b-41d4-a716-446655440000" },
    { "type": "email", "value": "alice@example.com" },
    { "type": "roles", "value": "booking-user" },
    { "type": "permissions", "value": "appointment:create" },
    { "type": "permissions", "value": "appointment:view" }
  ]
}
```

Errors:
- `401 Unauthorized` for invalid token

### 2.12 POST /api/v1/auth/mfa/authenticator/setup/start
Purpose: initialize authenticator-app enrollment for the current authenticated user.

What this endpoint does:
- generates a new TOTP shared secret
- stores it for the user in pending setup state
- returns an `otpauth://` URI (also used as QR payload) so clients can scan it in Google Authenticator, Microsoft Authenticator, etc.

Request:
```http
POST /api/v1/auth/mfa/authenticator/setup/start
Authorization: Bearer <accessToken>
```

Response:
```http
200 OK
Content-Type: application/json

{
  "otpauthUri": "otpauth://totp/https%3A%2F%2Fauth.vehicle-service-booking.local%3Aalice%40example.com?secret=JBSWY3DPEHPK3PXP&issuer=https%3A%2F%2Fauth.vehicle-service-booking.local&algorithm=SHA1&digits=6&period=30",
  "qrPayload": "otpauth://totp/https%3A%2F%2Fauth.vehicle-service-booking.local%3Aalice%40example.com?secret=JBSWY3DPEHPK3PXP&issuer=https%3A%2F%2Fauth.vehicle-service-booking.local&algorithm=SHA1&digits=6&period=30",
  "qrImageUrl": "https://auth.example.com/api/v1/auth/mfa/authenticator/setup/qr",
  "secretMasked": "JBSW...3PXP",
  "purpose": "Scan QR in authenticator app, then verify with a current code."
}
```

Errors:
- `401 Unauthorized` when access token is missing/invalid
- `400 Bad Request` when account is not active or email not yet verified

### 2.12.1 GET /api/v1/auth/mfa/authenticator/setup/qr
Purpose: return a PNG QR image for current authenticated user's pending authenticator setup.

What this endpoint does:
- reads current user's pending authenticator setup secret
- builds `otpauth://` payload using configured JWT issuer and user email
- returns QR PNG bytes (`image/png`)

Request:
```http
GET /api/v1/auth/mfa/authenticator/setup/qr
Authorization: Bearer <accessToken>
Accept: image/png
```

Response:
```http
200 OK
Content-Type: image/png

<binary png>
```

Errors:
- `401 Unauthorized` when access token is missing/invalid
- `400 Bad Request` when setup was not started or setup secret is invalid

### 2.13 POST /api/v1/auth/mfa/authenticator/setup/verify
Purpose: finalize authenticator-app enrollment by proving possession of the TOTP secret.

What this endpoint does:
- validates the submitted TOTP code against the pending secret
- marks `isAuthenticatorAppEnabled = true`
- switches login challenge preference to `authenticator_app`

Request:
```http
POST /api/v1/auth/mfa/authenticator/setup/verify
Authorization: Bearer <accessToken>
Content-Type: application/json

{
  "code": "123456"
}
```

Response:
```http
200 OK
Content-Type: application/json

{
  "isAuthenticatorAppEnabled": true,
  "loginVerificationChannel": "authenticator_app",
  "purpose": "Authenticator app is verified and can now be used for login challenges."
}
```

Errors:
- `401 Unauthorized` when access token is missing/invalid
- `400 Bad Request` when setup was not started or TOTP code is invalid/expired

### 2.13.1 Production rollout note: authenticator secret encryption keys

Purpose: define operational behavior for Data Protection key persistence and rotation used to encrypt authenticator shared secrets at rest.

Current implementation summary:
- MFA setup start encrypts the generated authenticator shared secret before persistence.
- MFA setup verify and authenticator login verify decrypt the stored secret at runtime.
- Ciphertext is versioned at app level (`enc:v1:` prefix) and Data Protection handles underlying key metadata.

What key rotation does:
- new encrypt operations use the newest active Data Protection key
- existing encrypted secrets continue to decrypt via older retained keys
- users do not need to re-scan QR or re-enroll MFA during healthy rotation

What key rotation does not do:
- it does not rotate user TOTP secrets by itself
- it does not bulk re-encrypt all existing rows automatically

When users must re-enroll authenticator MFA:
- only if decrypt keys are unavailable for previously encrypted records
- common causes:
  - non-persistent key ring and service restart
  - key ring deleted/corrupted
  - multi-instance deployment with non-shared key stores
  - incompatible app isolation/key scope changes

Production requirements (must-have):
- persist Data Protection keys to durable storage
- share the same key ring across all auth service instances in one environment
- back up and restore key ring data as part of disaster recovery
- restrict key store access with least privilege

Deployment checklist:
1. Configure shared durable key ring for the environment.
2. Verify all instances can read/write the same key ring.
3. Deploy and run MFA smoke tests:
   - start setup returns QR payload
   - verify setup succeeds
   - login verify via authenticator succeeds
4. Confirm previously-encrypted secrets from before deployment still decrypt.
5. Monitor decrypt failure metrics/logs after rollout.

Incident checklist (decrypt failure spike):
1. Verify key ring availability and permissions.
2. Verify instances are using the same key store and app isolation scope.
3. Restore missing key ring backup if needed.
4. Re-enroll MFA only for users whose secrets remain undecryptable after recovery.

### 2.13.2 Configuration precedence for startup

Auth startup uses a strict precedence model to keep deploy/runtime overrides predictable:
- shell/host environment variables (highest)
- `.env` values as fallback only for keys not already present in process env
- `appsettings.{Environment}.json`
- `appsettings.json` (base)

Implementation note:
- DotNetEnv is loaded with `NoClobber`, so `.env` cannot override values already supplied by shell/host (for example `ASPNETCORE_ENVIRONMENT=Production`).

### 2.14 Admin endpoints (optional)
These endpoints are protected and available only to admin users.

Current status:
- Planned for a later phase; not implemented in the current Auth service code.

Examples:
- `GET /api/v1/admin/users`
- `GET /api/v1/admin/users/{id}`
- `POST /api/v1/admin/users/{id}/roles`
- `POST /api/v1/admin/users/{id}/groups`
- `GET /api/v1/admin/roles`
- `GET /api/v1/admin/permissions`

---

## 3. Claim and token contract

### 3.0 Canonical cross-service authorization semantics
- `roles`: coarse-grained identity categories (for example: `booking-user`, `manager`, `admin`). Roles are stable and human-readable.
- `permissions`: operation-level authorization primitives consumed directly by service policies (for example: `appointment:create`, `appointment:view`, `appointment:complete`).
- `groups`: business-partition metadata (for example dealership/region/tenant grouping). Groups are for data-partition rules and reporting contexts, not for direct permission grants.

Normative rules for current implementation:
- Booking API authorization is permission-first.
- Roles can be used as optional compatibility context but do not replace permission checks.
- Groups are reserved for future cross-tenant/dealership scoping and are currently optional in tokens.

### 3.1 Required JWT claims
- `sub`: user id
- `iss`: issuer URI
- `aud`: list of service audiences
- `exp`: expiration time
- `iat`: issued at time
- `email`
- `roles`
- `permissions`

Optional (planned) claims:
- `groups`
- `scope` (alias/interoperability claim, if enabled in future)

### 3.2 Example JWT payload
```json
{
  "sub": "550e8400-e29b-41d4-a716-446655440000",
  "email": "alice@example.com",
  "iss": "https://auth.example.com",
  "aud": ["vehicle-booking-api"],
  "roles": ["booking-user"],
  "permissions": ["appointment:create","appointment:view"],
  "groups": ["dealership-north"],
  "exp": 1710000000,
  "iat": 1709996400
}
```

### 3.3 Access token lifetime
- `accessToken` TTL: 15–30 minutes
- `refreshToken` TTL: longer, configurable (e.g. 7–30 days)

### 3.4 Audience strategy
Use service-specific audiences:
- `vehicle-booking-api`
- `inventory-api`
- `reporting-api`

Downstream service validates that its own audience is present.

---

## 4. Social login provider mapping

### 4.1 Supported providers
- Google

### 4.2 Provider flow
- redirect user to provider auth page
- provider returns authorization code
- auth service exchanges code for user info
- auth service verifies provider email evidence before local login
- auth service maps or creates internal user
- auth service issues local JWT and refresh tokens

### 4.3 User mapping rules
- use provider email as primary identifier when available
- if email exists, link provider login to existing user
- if email is absent or provider email is not verified, reject login
- create user record with provider metadata

---

## 5. Error responses

### Common error format
```json
{
  "code": "INVALID_REQUEST",
  "message": "Validation failed",
  "details": [
    { "field": "email", "message": "Email is required" }
  ]
}
```

### Standard errors
- `400 Bad Request`
- `401 Unauthorized`
- `403 Forbidden`
- `404 Not Found`
- `409 Conflict`

---

## 6. API versioning
- use `/api/v1/` path versioning
- preserve compatibility for future auth service improvements

---

## 7. Notes for booking service integration
- booking service consumes JWT from auth service
- booking service validates locally via JWKS
- booking service checks `aud` includes `vehicle-booking-api`
- booking service checks permission claims for each operation
- booking service does not perform login or sign-up

---

## 8. Next step checklist
- [ ] confirm endpoint names and request schema
- [ ] confirm token claim names and audience values
- [ ] confirm provider support list
- [ ] confirm refresh token model
- [ ] confirm admin endpoint requirements
