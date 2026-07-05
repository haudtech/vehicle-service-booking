# Auth/User Service Detailed Design

## Purpose
This document defines the detailed design of the Auth/User Service, including:
- database schema
- entity relationships
- endpoint request/response payloads
- authentication and token issuance details
- social login metadata model

This design is intended to support the Vehicle Scheduling Service and future downstream services.

---

## 1. Database schema

### 1.1 Users
Stores user accounts.

Columns:
- `Id` UUID PK
- `Email` varchar(256) NOT NULL UNIQUE
- `NormalizedEmail` varchar(256) NOT NULL UNIQUE
- `AccountName` varchar(50) NOT NULL UNIQUE
- `PasswordHash` text NULL
- `DisplayName` varchar(100) NULL
- `IsEmailConfirmed` bool NOT NULL DEFAULT false
- `IsActive` bool NOT NULL DEFAULT true
- `CreatedAt` timestamptz NOT NULL DEFAULT now()
- `UpdatedAt` timestamptz NOT NULL DEFAULT now()

Indexes:
- unique `Email`
- unique `NormalizedEmail`
- unique `AccountName`

Implementation note:
- Account name is normalized before persistence and stored in `AccountName`.

### 1.2 Roles
Defines named roles.

Columns:
- `Id` UUID PK
- `Name` varchar(100) NOT NULL UNIQUE
- `Description` varchar(256) NULL
- `IsActive` bool NOT NULL DEFAULT true
- `CreatedAt` timestamptz NOT NULL DEFAULT now()
- `UpdatedAt` timestamptz NOT NULL DEFAULT now()

Example values:
- `admin`
- `manager`
- `client`
- `technician`

### 1.3 Groups
Represents organizational membership.

Columns:
- `Id` UUID PK
- `Name` varchar(100) NOT NULL UNIQUE
- `Description` varchar(256) NULL
- `IsActive` bool NOT NULL DEFAULT true
- `CreatedAt` timestamptz NOT NULL DEFAULT now()
- `UpdatedAt` timestamptz NOT NULL DEFAULT now()

Example values:
- `dealership-north`
- `service-supervisors`

### 1.4 Permissions
Represents granular actions.

Columns:
- `Id` UUID PK
- `Name` varchar(150) NOT NULL UNIQUE
- `Description` varchar(256) NULL
- `IsActive` bool NOT NULL DEFAULT true
- `CreatedAt` timestamptz NOT NULL DEFAULT now()
- `UpdatedAt` timestamptz NOT NULL DEFAULT now()

Example values:
- `appointment:create`
- `appointment:complete`
- `appointment:view`
- `user:manage`

### 1.5 UserRoles
Maps users to roles.

Columns:
- `Id` UUID PK
- `UserId` UUID NOT NULL REFERENCES Users(Id)
- `RoleId` UUID NOT NULL REFERENCES Roles(Id)
- `CreatedAt` timestamptz NOT NULL DEFAULT now()
- `UpdatedAt` timestamptz NOT NULL DEFAULT now()

Indexes:
- unique `(UserId, RoleId)`

### 1.6 UserGroups
Maps users to groups.

Columns:
- `Id` UUID PK
- `UserId` UUID NOT NULL REFERENCES Users(Id)
- `GroupId` UUID NOT NULL REFERENCES Groups(Id)
- `CreatedAt` timestamptz NOT NULL DEFAULT now()
- `UpdatedAt` timestamptz NOT NULL DEFAULT now()

Indexes:
- unique `(UserId, GroupId)`

### 1.7 RolePermissions
Maps roles to permissions.

Columns:
- `Id` UUID PK
- `RoleId` UUID NOT NULL REFERENCES Roles(Id)
- `PermissionId` UUID NOT NULL REFERENCES Permissions(Id)
- `CreatedAt` timestamptz NOT NULL DEFAULT now()
- `UpdatedAt` timestamptz NOT NULL DEFAULT now()

Indexes:
- unique `(RoleId, PermissionId)`

### 1.8 RefreshTokens
Tracks refresh tokens for long-lived sessions.

Columns:
- `Id` UUID PK
- `UserId` UUID NOT NULL REFERENCES Users(Id)
- `TokenHash` text NOT NULL
- `CreatedByIp` varchar(45) NULL
- `RevokedByIp` varchar(45) NULL
- `ReplacedByTokenHash` text NULL
- `ReasonRevoked` varchar(256) NULL
- `IssuedAt` timestamptz NOT NULL
- `ExpiresAt` timestamptz NOT NULL
- `RevokedAt` timestamptz NULL
- `IsActive` bool NOT NULL DEFAULT true
- `CreatedAt` timestamptz NOT NULL DEFAULT now()
- `UpdatedAt` timestamptz NOT NULL DEFAULT now()

Indexes:
- unique `TokenHash`
- index `(UserId, IsActive)`

### 1.9 SocialProviders
Stores external provider link metadata.

Columns:
- `Id` UUID PK
- `UserId` UUID NOT NULL REFERENCES Users(Id)
- `ProviderName` varchar(50) NOT NULL
- `ProviderUserId` varchar(200) NOT NULL
- `Email` varchar(256) NULL
- `DisplayName` varchar(200) NULL
- `ProfilePictureUrl` varchar(512) NULL
- `CreatedAt` timestamptz NOT NULL DEFAULT now()
- `UpdatedAt` timestamptz NOT NULL DEFAULT now()

Indexes:
- unique `(ProviderName, ProviderUserId)`

### 1.10 UserLoginHistory (optional)
Tracks login events.

Columns:
- `Id` UUID PK
- `UserId` UUID NOT NULL REFERENCES Users(Id)
- `LoginType` varchar(50) NOT NULL
- `Success` bool NOT NULL
- `IpAddress` varchar(45) NULL
- `UserAgent` varchar(256) NULL
- `CreatedAt` timestamptz NOT NULL DEFAULT now()

---

## 2. Entity relationship diagram

```
Users
 ├─< UserRoles >─ Roles
 ├─< UserGroups >─ Groups
 ├─< SocialProviders >
 ├─< RefreshTokens >
 └─< UserLoginHistory >

Roles
 └─< RolePermissions >─ Permissions
```

---

## 3. Endpoint payload definitions

### 3.1 POST /api/v1/auth/signup

Request:
```json
{
  "email": "alice@example.com",
  "accountName": "alice.smith",
  "password": "P@ssw0rd!",
  "displayName": "Alice Smith"
}
```

Response:
```json
{
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "email": "alice@example.com",
  "status": "PendingVerification"
}
```

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

### 3.7 GET /api/v1/auth/google (Planned - Not Implemented Yet)

Behavior:
- redirects client to Google auth endpoint
- includes `client_id`, `redirect_uri`, `scope`, `state`

### 3.8 GET /api/v1/auth/google/callback (Planned - Not Implemented Yet)

Response:
```json
{
  "accessToken": "<jwt>",
  "refreshToken": "<refresh-token>",
  "expiresIn": 900
}
```

### 3.9 GET /api/v1/auth/github (Planned - Not Implemented Yet)

Behavior:
- redirects client to GitHub auth endpoint

### 3.10 GET /api/v1/auth/github/callback (Planned - Not Implemented Yet)

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
- `github`

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
- [ ] implement Google and GitHub OAuth callback flows
- [ ] implement role/permission seeding
- [ ] implement social provider metadata linking
- [ ] validate auth service contract with booking integration team
