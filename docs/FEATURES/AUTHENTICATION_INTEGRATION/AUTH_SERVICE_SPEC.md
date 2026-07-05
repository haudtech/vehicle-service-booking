# Auth/User Service API Specification

## Purpose
This document defines the Auth/User Service API contract for:
- user sign-up
- login
- refresh tokens
- logout
- social login
- JWKS discovery
- user profile
- admin user/group/role management

This service is intended to be the central identity provider for the Vehicle Scheduling Service and other future services.

---

## 1. API Overview

### Base URL
`https://auth.example.com/api/v1`

### Content type
- `application/json`

### Authentication
- Public endpoints (implemented): sign-up, login, refresh, logout, JWKS
- Protected endpoints (implemented): user profile (`/me`)
- Planned endpoints (not implemented yet): social OAuth start/callback, admin endpoints
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
201 Created
Content-Type: application/json

{
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "email": "alice@example.com",
  "status": "PendingVerification"
}
```

Validation errors:
- `400 Bad Request` for invalid email, weak password, or missing fields
- `409 Conflict` if email already exists

### 2.2 POST /api/v1/auth/login
Authenticate user using local credentials (AccountName or Email).

Request:
```http
POST /api/v1/auth/login
Content-Type: application/json

{
  "identifier": "alice.smith",
  "password": "P@ssw0rd!"
}
```

Backward-compatible alternative request body:

```http
POST /api/v1/auth/login
Content-Type: application/json

{
  "email": "alice@example.com",
  "password": "P@ssw0rd!"
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
- `400 Bad Request` for missing fields
- `401 Unauthorized` for invalid credentials
- `403 Forbidden` for disabled or unverified accounts

### 2.3 POST /api/v1/auth/refresh
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

### 2.4 POST /api/v1/auth/logout
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

### 2.5 GET /api/v1/.well-known/jwks.json
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

### 2.6 GET /api/v1/auth/{provider} (Planned - Not Implemented Yet)
Begin social login flow.

Example:
```http
GET /api/v1/auth/google
```

Behavior:
- redirect to provider authorization URL
- include client_id, redirect_uri, scope, state

### 2.7 GET /api/v1/auth/{provider}/callback (Planned - Not Implemented Yet)
Handle OAuth2 callback from social provider.

Example:
```http
GET /api/v1/auth/google/callback?code=...&state=...
```

Behavior:
- exchange code for provider token
- retrieve user profile
- create or map internal user
- issue JWT and refresh token
- redirect or return token payload

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
- `401 Unauthorized` when provider exchange fails

### 2.8 GET /api/v1/me
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

### 2.9 Admin endpoints (optional)
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

### 3.1 Required JWT claims
- `sub`: user id
- `iss`: issuer URI
- `aud`: list of service audiences
- `exp`: expiration time
- `iat`: issued at time
- `email`
- `roles`
- `groups`
- `scope`

### 3.2 Example JWT payload
```json
{
  "sub": "550e8400-e29b-41d4-a716-446655440000",
  "email": "alice@example.com",
  "iss": "https://auth.example.com",
  "aud": ["vehicle-booking-api"],
  "roles": ["client"],
  "groups": ["dealership-north"],
  "scope": ["appointment:create","appointment:view"],
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
- GitHub

### 4.2 Provider flow
- redirect user to provider auth page
- provider returns authorization code
- auth service exchanges code for user info
- auth service maps or creates internal user
- auth service issues local JWT and refresh tokens

### 4.3 User mapping rules
- use provider email as primary identifier when available
- if email exists, link provider login to existing user
- if email is absent, require user to supply email
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
