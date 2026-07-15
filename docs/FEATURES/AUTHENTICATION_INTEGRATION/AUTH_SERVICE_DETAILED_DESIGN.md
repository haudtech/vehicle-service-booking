# Auth/User Service Detailed Design

## Purpose
Current design reference for auth persistence, token handling, and social identity linking.

## Status
Current supporting reference. Historical planning detail was removed to keep this doc focused on the active model.

## 1. Current design summary

The active auth/user model centers on:

- user accounts with email, account name, password hash, confirmation state, and activity flag
- role, group, and permission relations for downstream authorization
- refresh token storage with hashed token values and revocation tracking
- social provider metadata for Google-first login and future provider expansion
- optional login history for auditing and troubleshooting

## 2. Entity set

### Core entities
- User
- Role
- Group
- Permission

### Relationship entities
- UserRole
- UserGroup
- RolePermission

### Auth-specific entities
- RefreshToken
- SocialProvider
- UserLoginHistory

## 3. Current behavioral notes

1. Password-based accounts keep a stored password hash.
2. Social-only accounts may omit a password hash.
3. Email confirmation is tracked separately from sign-in authorization.
4. Refresh tokens are stored as hashes and revocable independently of access tokens.
5. Social provider links are unique by provider name plus provider user id.

## 4. Relationship snapshot

```text
User
 ├─< UserRole >─ Role
 ├─< UserGroup >─ Group
 ├─< SocialProvider >
 ├─< RefreshToken >
 └─< UserLoginHistory >

Role
 └─< RolePermission >─ Permission
```

## 5. How to use this doc

Use this file when you need the current auth/user data model and persistence shape.
Use [AUTH_SERVICE_SPEC.md](AUTH_SERVICE_SPEC.md) for endpoint contracts and [AUTH_GOOGLE_OAUTH_FULL_FLOW.md](AUTH_GOOGLE_OAUTH_FULL_FLOW.md) for runtime login behavior.

### 3.2 POST /api/v1/auth/login

Request:
```json
{
  "identifier": "alice.smith",
  "password": "P@ssw0rd!"
}
```

Backward-compatible request body:
```json
{
  "email": "alice@example.com",
  "password": "P@ssw0rd!"
}
```

Response:
```json
{
  "accessToken": "<jwt>",
  "refreshToken": "<refresh-token>",
  "expiresIn": 900
}
```

### 3.3 POST /api/v1/auth/refresh

Request:
```json
{
  "refreshToken": "<refresh-token>"
}
```

Response:
```json
{
  "accessToken": "<jwt>",
  "refreshToken": "<refresh-token>",
  "expiresIn": 900
}
```

### 3.4 POST /api/v1/auth/logout

Request:
```json
{
  "refreshToken": "<refresh-token>"
}
```

Response:
- `204 No Content`

### 3.5 GET /api/v1/me

Response:
```json
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

### 3.6 GET /api/v1/.well-known/jwks.json

Response example:
```json
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

### 3.7 GET /api/v1/auth/google/start (Implemented)

Behavior:
- redirects client to Google auth endpoint
- includes `client_id`, `redirect_uri`, `scope`, `state`

### 3.8 GET /api/v1/auth/google/callback (Implemented)

Response:
```json
{
  "accessToken": "<jwt>",
  "refreshToken": "<refresh-token>",
  "expiresIn": 900
}
```

---

## 4. Detailed token design

### 4.1 Access token claims
Required claims:
- `sub`
- `iss`
- `aud`
- `exp`
- `iat`
- `email`
- `roles`
- `scope`
- `groups`

Optional claims:
- `given_name`
- `family_name`
- `preferred_username`

### 4.2 Example JWT payload
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

### 4.3 Refresh token handling
- store only `TokenHash`
- issue new refresh token on login and refresh
- revoke old refresh token when rotating
- allow invalidation by setting `RevokedAt` and `IsActive = false`

---

## 5. Social provider integration model

### 5.1 Provider metadata
Social provider links store:
- `ProviderName`
- `ProviderUserId`
- `Email`
- `DisplayName`
- `ProfilePictureUrl`

### 5.2 Account linking rules
- if provider email matches an existing user, link the social provider to that user
- if no match, create a new user record
- if provider returns no email, require user to provide an email before issuing JWT

### 5.3 Provider-specific login types
- `google`
- optional future: `github`

---

## 6. Admin management endpoints (optional)

### 6.1 GET /api/v1/admin/users

Response:
```json
[
  {
    "userId": "...",
    "email": "alice@example.com",
    "roles": ["client"],
    "groups": ["dealership-north"]
  }
]
```

### 6.2 POST /api/v1/admin/users/{id}/roles

Request:
```json
{
  "roleId": "..."
}
```

### 6.3 POST /api/v1/admin/users/{id}/groups

Request:
```json
{
  "groupId": "..."
}
```

---

## 7. Validation rules

### 7.1 Signup validation
- `email` must be valid and unique
- `accountName` must be required and unique
- `password` must meet strength requirements
- `displayName` is optional

### 7.2 Login validation
- `identifier` (preferred) or `email` (backward compatible) plus `password` required
- return `401 Unauthorized` on invalid credentials

### 7.3 Refresh validation
- `refreshToken` required
- verify token hash and active status
- verify not expired

---

## 8. Implementation notes

### 8.1 Key rotation and JWKS
- publish new signing key and include in JWKS
- keep old keys active until all downstream services refresh
- use `kid` to select the right public key

### 8.2 Refresh tokens
- use secure random values
- hash the token before storing in DB
- never return stored hash to client

### 8.3 Social login
- protect callback endpoints against CSRF using `state`
- validate provider tokens and scopes
- map provider profile fields to internal user fields

---

## 9. Next step checklist
- [ ] implement database migrations for auth schema
- [ ] implement signup and login endpoints
- [ ] implement refresh/logout endpoints
- [ ] implement JWKS publishing
- [ ] implement Google OAuth callback flow
- [ ] optional future: implement GitHub OAuth callback flow
- [ ] implement role/permission seeding
- [ ] implement social provider metadata linking
- [ ] validate auth service contract with booking integration team
