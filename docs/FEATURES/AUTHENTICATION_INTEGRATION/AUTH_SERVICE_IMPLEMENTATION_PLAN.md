# Auth/User Service Implementation Plan

## Purpose
This document defines a concrete, code-oriented plan to implement the Auth/User Service. It is intended to move from design to execution with clear tasks, component ownership, and implementation guidance.

---

## 1. Service structure and project setup

### 1.1 Project boundaries
- Create a separate service/repository for the Auth/User Service
- Keep it independent from the Vehicle Scheduling Service
- Use ASP.NET Core Web API
- Use EF Core for persistence

### 1.2 Recommended project structure
```
AuthService/
  src/
    AuthService.Api/
    AuthService.Application/
    AuthService.Domain/
    AuthService.Infrastructure/
  tests/
    AuthService.UnitTests/
    AuthService.IntegrationTests/
```

### 1.3 Configuration
- `appsettings.json`
- `appsettings.Development.json`
- environment variables for secrets and OAuth client IDs/secrets
- `.env` support for local development if desired

---

## 2. Data layer implementation

### 2.1 Entities and DbContext
Implement entities from the auth data model:
- `User`
- `Role`
- `Group`
- `Permission`
- `UserRole`
- `UserGroup`
- `RolePermission`
- `RefreshToken`
- `SocialProvider`
- `UserLoginHistory` (optional)

Create `AuthDbContext` with:
- DbSet<User>
- DbSet<Role>
- DbSet<Group>
- DbSet<Permission>
- DbSet<UserRole>
- DbSet<UserGroup>
- DbSet<RolePermission>
- DbSet<RefreshToken>
- DbSet<SocialProvider>
- DbSet<UserLoginHistory>

### 2.2 Migrations and schema
- add EF Core migrations for the auth schema
- include unique indexes and foreign keys
- seed default roles and permissions

### 2.3 Repositories
Create repository interfaces and implementations:
- `IUserRepository`
- `IRoleRepository`
- `IGroupRepository`
- `IPermissionRepository`
- `IRefreshTokenRepository`
- `ISocialProviderRepository`

Repository responsibilities:
- user lookup by email/id
- role/group assignment
- refresh token persistence
- social provider link lookup
- permission resolution

---

## 3. Application layer implementation

### 3.1 Services
Implement application services:
- `IUserService`
- `IAuthService`
- `ITokenService`
- `IRefreshTokenService`
- `ISocialAuthService`
- `IRolePermissionService`

Service responsibilities:
- user creation and validation
- credential verification
- refresh token lifecycle
- JWT creation and claim population
- social provider authentication mapping
- permission lookup for roles

### 3.2 DTOs and requests
Define request and response models:
- `SignUpRequest`, `SignUpResponse`
- `LoginRequest`, `LoginResponse`
- `RefreshTokenRequest`, `RefreshTokenResponse`
- `LogoutRequest`
- `UserProfileResponse`
- `SocialLoginCallbackResponse`

### 3.3 Validation
Use FluentValidation or equivalent for:
- email format and uniqueness
- password strength
- required fields
- refresh token format

---

## 4. API layer implementation

### 4.1 Controllers
Implement controllers:
- `AuthController`
- `MeController`
- `WellKnownController`

Planned controllers (next phases):
- `SocialAuthController`
- `Admin/UsersController` (optional)

Controller actions:
- `POST /api/v1/auth/signup`
- `POST /api/v1/auth/login`
- `POST /api/v1/auth/refresh`
- `POST /api/v1/auth/logout`
- `GET /api/v1/me`
- `GET /api/v1/.well-known/jwks.json`

Planned actions (not implemented yet):
- `GET /api/v1/auth/google`
- `GET /api/v1/auth/google/callback`
- `GET /api/v1/auth/github`
- `GET /api/v1/auth/github/callback`

### 4.2 Response formats
Use structured responses with standard error payloads.
Example error:
```json
{
  "code": "INVALID_CREDENTIALS",
  "message": "Invalid email or password"
}
```

---

## 5. JWT and security implementation

### 5.1 Token service
Implement `ITokenService` to:
- generate JWT access tokens
- sign with RSA or ECDSA private key
- include claims: `sub`, `iss`, `aud`, `roles`, `groups`, `scope`, `email`, `exp`, `iat`
- return refresh tokens separately

### 5.2 JWKS endpoint
Implement `.well-known/jwks.json` to expose public signing keys.
- support key rotation
- include `kid` values

### 5.3 Refresh token lifecycle
- issue refresh tokens at login and refresh
- store hashed token values in DB
- on refresh, revoke old refresh token and issue a new one
- support logout by revoking refresh token

### 5.4 Password hashing
- use secure hashing algorithm such as Argon2 or bcrypt
- do not store plaintext passwords

### 5.5 Token expiration
- access token: 15–30 minutes
- refresh token: 7–30 days configurable

---

## 6. Social login implementation

### 6.1 OAuth client setup
- configure Google and GitHub client IDs/secrets
- set redirect URIs to auth service callback endpoints

### 6.2 Social auth flow
- `GET /api/v1/auth/google` redirects to Google
- `GET /api/v1/auth/google/callback` receives `code`
- exchange code for user info
- map provider identity to internal user
- if user exists, sign in
- if not, create user and sign in
- issue JWT and refresh token

### 6.3 Social provider persistence
- store provider metadata in `SocialProvider`
- track `ProviderName`, `ProviderUserId`, `Email`, `DisplayName`, `ProfilePictureUrl`

### 6.4 Account linking rules
- if provider email matches existing user, link provider to user
- if provider email is absent, ask user to complete account data
- allow future linking via user profile management

---

## 7. Authorization and role/permission seeding

### 7.1 Seed data
Seed initial roles and permissions:
- `admin` → all permissions
- `manager` → `appointment:create`, `appointment:complete`, `appointment:view`
- `client` → `appointment:create`, `appointment:view`
- `technician` → `appointment:view`

Seed permissions:
- `appointment:create`
- `appointment:complete`
- `appointment:view`
- `user:manage`

### 7.2 Role assignment
- assign `client` by default to new sign-up users
- support admin and manager assignment through admin APIs

---

## 8. Testing and quality assurance

### 8.1 Unit tests
- user creation and validation
- login credential validation
- JWT creation and claim population
- refresh token issuance and revocation
- social login mapping logic

### 8.2 Integration tests
- sign-up and login end-to-end
- refresh token flow end-to-end
- social login callback flow
- JWKS endpoint consumer validation

### 8.3 Security tests
- verify password hashing
- verify access token expiry
- verify refresh token rotation
- verify token signature validation with JWKS

---

## 9. Deployment

### 9.1 Environment settings
- issuer URL
- JWT signing key location or secret
- OAuth client IDs/secrets
- refresh token lifetime
- token issuer audience
- database connection string

### 9.2 Logging and observability
- log auth successes and failures
- log refresh token usage and revocations
- log social login provider errors

### 9.3 Service readiness
- respond with health check endpoint
- validate config at startup
- expose API docs for auth endpoints

---

## 10. Implementation task list

### Phase A: Setup
- [ ] scaffold AuthService solution
- [ ] add `AuthService.Api`, `AuthService.Application`, `AuthService.Domain`, `AuthService.Infrastructure`
- [ ] configure EF Core and database connection

### Phase B: Data model and persistence
- [ ] implement entities and relationships
- [ ] create EF Core migrations
- [ ] seed roles and permissions

### Phase C: Auth APIs
- [ ] implement sign-up and login
- [ ] implement refresh and logout
- [ ] implement `GET /me`
- [ ] implement JWKS endpoint

### Phase D: Token security
- [ ] implement JWT token service
- [ ] implement refresh token persistence and rotation
- [ ] implement password hashing

### Phase E: Social login
- [ ] configure Google OAuth
- [ ] configure GitHub OAuth
- [ ] implement social login redirect and callback
- [ ] implement social provider link tracking

### Phase F: Authorization
- [ ] seed default roles/permissions
- [ ] implement user role and permission mapping
- [ ] implement admin role assignment APIs

### Phase G: Testing
- [ ] unit tests for auth logic
- [ ] integration tests for endpoints
- [ ] security tests for JWT/JWKS/refresh

### Phase H: Documentation
- [ ] finalize auth service API docs
- [ ] publish auth contract for booking service
- [ ] add deployment and config docs

---

## 11. Next step
Begin implementation by scaffolding the auth service solution and building the data model with EF Core migrations.
