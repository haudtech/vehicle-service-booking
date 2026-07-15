# Auth/User Service Data Model

## Purpose
This document defines the Auth/User Service data model for:
- users
- groups
- roles
- permissions
- refresh tokens
- social provider metadata

The model is designed to support both local authentication and OAuth2/social login, while enabling role- and permission-based access for downstream services such as the Vehicle Scheduling Service.

---

## 1. Core entities

### 1.1 User
Represents the application user identity.

Fields:
- `Id` (GUID)
- `Email` (string, unique)
- `NormalizedEmail` (string)
- `AccountName` (string, unique)
- `PasswordHash` (string, nullable for social-only accounts)
- `DisplayName` (string)
- `IsEmailConfirmed` (bool)
- `IsActive` (bool)
- `CreatedAt` (DateTime)
- `UpdatedAt` (DateTime)

Notes:
- Password is stored as a secure hash.
- Social-only users may have `PasswordHash = null`.
- Email confirmation status supports account verification.
- Account name is normalized before persistence and stored in `AccountName`.

### 1.2 Role
Represents a named role with a human-friendly meaning.

Fields:
- `Id` (GUID)
- `Name` (string, unique)
- `Description` (string)
- `IsActive` (bool)
- `CreatedAt` (DateTime)
- `UpdatedAt` (DateTime)

Example roles:
- `admin`
- `manager`
- `client`
- `technician`

### 1.3 Group
Represents an organizational grouping or tenant-like collection.

Fields:
- `Id` (GUID)
- `Name` (string, unique or scoped)
- `Description` (string)
- `IsActive` (bool)
- `CreatedAt` (DateTime)
- `UpdatedAt` (DateTime)

Example groups:
- `dealership-north`
- `service-supervisors`
- `regional-management`

### 1.4 Permission
Represents a specific action or capability.

Fields:
- `Id` (GUID)
- `Name` (string, unique)
- `Description` (string)
- `IsActive` (bool)
- `CreatedAt` (DateTime)
- `UpdatedAt` (DateTime)

Example permissions:
- `appointment:create`
- `appointment:complete`
- `appointment:view`
- `user:manage`

---

## 2. Relationship entities

### 2.1 UserRole
Assigns a role to a user.

Fields:
- `Id` (GUID)
- `UserId` (GUID)
- `RoleId` (GUID)
- `CreatedAt` (DateTime)
- `UpdatedAt` (DateTime)

Index:
- unique on `(UserId, RoleId)`

### 2.2 UserGroup
Assigns a user to a group.

Fields:
- `Id` (GUID)
- `UserId` (GUID)
- `GroupId` (GUID)
- `CreatedAt` (DateTime)
- `UpdatedAt` (DateTime)

Index:
- unique on `(UserId, GroupId)`

### 2.3 RolePermission
Maps roles to permissions.

Fields:
- `Id` (GUID)
- `RoleId` (GUID)
- `PermissionId` (GUID)
- `CreatedAt` (DateTime)
- `UpdatedAt` (DateTime)

Index:
- unique on `(RoleId, PermissionId)`

---

## 3. Auth-specific entities

### 3.1 RefreshToken
Stores refresh tokens for long-lived sessions.

Fields:
- `Id` (GUID)
- `UserId` (GUID)
- `Token` (string, hashed)
- `IssuedAt` (DateTime)
- `ExpiresAt` (DateTime)
- `RevokedAt` (DateTime?, nullable)
- `ReplacedByToken` (string, nullable)
- `CreatedByIp` (string)
- `RevokedByIp` (string, nullable)
- `ReasonRevoked` (string, nullable)
- `IsActive` (bool)
- `CreatedAt` (DateTime)
- `UpdatedAt` (DateTime)

Features:
- store refresh token hashes only
- support revocation and rotation
- track client IP for auditing

### 3.2 SocialProvider
Stores metadata for social login providers linked to a user.

Fields:
- `Id` (GUID)
- `UserId` (GUID)
- `ProviderName` (string)
- `ProviderUserId` (string)
- `Email` (string, optional)
- `DisplayName` (string, optional)
- `ProfilePictureUrl` (string, optional)
- `CreatedAt` (DateTime)
- `UpdatedAt` (DateTime)

Example providers:
- `google`

Index:
- unique on `(ProviderName, ProviderUserId)`

### 3.3 UserLoginHistory (optional)
Optional auditing entity for login events.

Fields:
- `Id` (GUID)
- `UserId` (GUID)
- `LoginType` (string: `local`, `google`)
- `Success` (bool)
- `IpAddress` (string)
- `UserAgent` (string)
- `CreatedAt` (DateTime)

---

## 4. Entity relationships

### Summary diagram
```
User
 ├─< UserRole >─ Role
 ├─< UserGroup >─ Group
 ├─< SocialProvider >
 ├─< RefreshToken >
 └─< UserLoginHistory >

Role
 └─< RolePermission >─ Permission
```

### Notes
- A single user may have multiple roles and groups.
- Permissions are granted via roles.
- Social provider metadata is separate from the local account, enabling account linking.
- Refresh tokens are stored securely and revoked independently of access tokens.

---

## 5. Data model details

### 5.1 User
Use a secure hashing algorithm such as bcrypt or Argon2.

User sample:
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "email": "alice@example.com",
  "accountName": "alice.smith",
  "passwordHash": "...",
  "displayName": "Alice Smith",
  "isEmailConfirmed": true,
  "isActive": true,
  "createdAt": "2026-07-03T08:00:00Z",
  "updatedAt": "2026-07-03T08:00:00Z"
}
```

### 5.2 Role
Role sample:
```json
{
  "id": "550e8400-e29b-41d4-a716-000000000001",
  "name": "client",
  "description": "Basic customer role for appointment creation",
  "isActive": true
}
```

### 5.3 Permission
Permission sample:
```json
{
  "id": "550e8400-e29b-41d4-a716-000000000010",
  "name": "appointment:create",
  "description": "Allows creating new appointments"
}
```

### 5.4 RefreshToken
Refresh token sample:
```json
{
  "id": "550e8400-e29b-41d4-a716-000000000020",
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "token": "<hashed-refresh-token>",
  "issuedAt": "2026-07-03T08:00:00Z",
  "expiresAt": "2026-08-03T08:00:00Z",
  "revokedAt": null,
  "isActive": true
}
```

### 5.5 SocialProvider
Social provider sample:
```json
{
  "id": "550e8400-e29b-41d4-a716-000000000030",
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "providerName": "google",
  "providerUserId": "1234567890",
  "email": "alice@example.com",
  "displayName": "Alice Smith",
  "profilePictureUrl": "https://..."
}
```

---

## 6. Additional considerations

### 6.1 Role/permission model
- roles should map to broad user categories
- permissions should map to granular actions
- avoid placing business logic in roles alone
- use permissions for booking service authorization checks

### 6.2 Social login behavior
- if social user email matches an existing user, link the social provider to that user
- if email is new, create a new user record
- if provider returns no email, require user completion of missing info

### 6.3 Refresh token security
- store refresh token hashes only
- use rotating refresh tokens on each refresh call
- revoke old refresh token when issuing a new one
- track token creation and revocation metadata

### 6.4 Migration path
- start with local sign-up/login and basic roles
- add social provider metadata as social login is enabled
- keep the model extensible for future identity providers

---

## 7. Next steps
- implement the database schema for these entities in the auth service
- wire local account registration and login
- wire refresh token issuance and validation
- wire social provider metadata and account linking
- map roles to permissions for booking service authorization
