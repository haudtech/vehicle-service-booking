# Auth Service + Booking JWT Integration - Staged Commit Summary

## Commit Title
Auth service foundation + Booking JWT integration + documentation aligned to implemented behavior

## Summary
This change set establishes the Auth service as the identity provider, integrates Booking API JWT validation via JWKS, and updates documentation to mirror currently implemented endpoints and behavior while keeping social/admin APIs marked as planned.

Canonical architecture locations for these diagrams:
- `docs/ARCHITECTURE/AUTH_COMPONENTS_VIEW.md`
- `docs/ARCHITECTURE/AUTH_SEQUENTIAL_FLOW_VIEW.md`

## Sequential Flow

### 1) Sign-up and Login Token Issuance
```mermaid
sequenceDiagram
    autonumber
    participant Client
    participant AuthAPI as Auth Service API
    participant AuthApp as Auth Application Service
    participant AuthDB as Auth Database
    participant Token as JWT/JWKS Service

    Client->>AuthAPI: POST /api/v1/auth/signup
    AuthAPI->>AuthApp: Validate + normalize account identity
    AuthApp->>AuthDB: Create active user + defaults
    AuthDB-->>AuthApp: User created
    AuthApp-->>AuthAPI: 201 Created
    AuthAPI-->>Client: Signup result

    Client->>AuthAPI: POST /api/v1/auth/login
    AuthAPI->>AuthApp: Validate credentials (accountName/email + password)
    AuthApp->>AuthDB: Read active user + role/group relations
    AuthDB-->>AuthApp: User + auth graph
    AuthApp->>Token: Issue access + refresh token
    Token-->>AuthApp: Signed JWT + refresh token
    AuthApp-->>AuthAPI: Auth response
    AuthAPI-->>Client: 200 OK with tokens
```

### 2) Booking Request Authorization with JWKS
```mermaid
sequenceDiagram
    autonumber
    participant Client
    participant BookingAPI as Booking API
    participant JwtMiddleware as JWT Auth Middleware
    participant JwksProvider as JWKS Signing Key Provider
    participant AuthJWKS as Auth JWKS Endpoint
    participant BookingApp as Booking Application
    participant BookingDB as Booking Database

    Client->>BookingAPI: POST /api/v1/appointments (Bearer JWT)
    BookingAPI->>JwtMiddleware: Authenticate token
    JwtMiddleware->>JwksProvider: Resolve signing key by kid
    JwksProvider->>AuthJWKS: GET /.well-known/jwks.json
    AuthJWKS-->>JwksProvider: Public keys
    JwksProvider-->>JwtMiddleware: Matching key
    JwtMiddleware-->>BookingAPI: Token valid + claims principal
    BookingAPI->>BookingApp: Authorize policy + process appointment
    BookingApp->>BookingDB: Persist appointment
    BookingDB-->>BookingApp: Created
    BookingApp-->>BookingAPI: Success result
    BookingAPI-->>Client: 201 Created
```

## Architecture Components

```mermaid
flowchart TB
    subgraph Clients
        Web[Web Client]
        Mobile[Mobile Client]
    end

    subgraph AuthService[Auth Service]
        AuthControllers[Presentation: Auth, Me, WellKnown Controllers]
        AuthApplication[Application: Auth workflows, validation, policies]
        AuthInfrastructure[Infrastructure: EF Core, repositories, migrations]
        AuthDomain[Domain: User, Role, Group, Permission aggregates]
        JwtIssuer[JWT Issuer]
        JwksEndpoint[JWKS Endpoint]
    end

    subgraph BookingService[Booking Service]
        BookingControllers[Presentation: Availability and Appointments APIs]
        BookingApplication[Application: Scheduling use-cases and policies]
        BookingInfrastructure[Infrastructure: Persistence and integrations]
        JwtValidation[JWT Validation Middleware]
        JwksKeyResolver[JWKS Signing Key Resolver]
    end

    subgraph DataStores
        AuthDb[(Auth PostgreSQL)]
        BookingDb[(Booking PostgreSQL)]
    end

    Web --> AuthControllers
    Mobile --> AuthControllers

    AuthControllers --> AuthApplication --> AuthInfrastructure --> AuthDb
    AuthApplication --> AuthDomain
    AuthApplication --> JwtIssuer
    AuthControllers --> JwksEndpoint

    Web --> BookingControllers
    Mobile --> BookingControllers

    BookingControllers --> JwtValidation --> JwksKeyResolver --> JwksEndpoint
    BookingControllers --> BookingApplication --> BookingInfrastructure --> BookingDb
```

## Key Implementation Outcomes
- Added a full Auth service foundation and solution integration.
- Implemented core Auth APIs: signup, login, refresh, logout, me, and JWKS.
- Added Auth persistence, service/repository layers, validation, middleware, and migrations.
- Integrated Booking API with JWT authentication and JWKS-based signing key resolution.
- Enforced authorization on protected Booking endpoints.
- Centralized active-record filtering in model configuration, including join entities.
- Updated architecture and authentication docs to match implemented routes and behavior.
- Kept social/admin endpoints documented as planned future scope.

## Scope (Staged)
- 87 files changed
- 8301 insertions
- 2 deletions

## Suggested Commit Message Body (Copy/Paste)
```text
Auth service foundation + Booking JWT integration + documentation aligned to implemented behavior

- Add new Auth service and wire it into the solution.
- Implement core Auth APIs: signup, login, refresh, logout, me, and JWKS endpoint.
- Add Auth persistence, repositories, services, validation, middleware, DI setup, and migrations.
- Add JWT issuance in Auth and JWKS publishing for downstream verification.
- Integrate Booking API with JWT authentication/authorization and JWKS signing-key resolution.
- Protect Booking availability and appointment endpoints with authorization policies.
- Align authentication docs with implemented routes and current behavior.
- Keep social/admin endpoints documented as planned (not implemented).
- Enforce active-only data access centrally via OnModelCreating query filters, including join entities.

Scope: 87 files changed, 8301 insertions(+), 2 deletions(-).
```
