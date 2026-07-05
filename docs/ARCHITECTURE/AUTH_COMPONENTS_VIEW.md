# Auth and Booking Components View

Purpose: Present the runtime component topology for identity and scheduling collaboration.
Status: CURRENT - Reflects implemented Auth and Booking integration.

---

## 1. Component Architecture

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

## 2. Design Intent

- Auth service is the identity provider and token issuer.
- Booking service is a protected resource API and validates JWT locally.
- Signing keys are discovered through JWKS and reused for token verification.
- Persistence boundaries remain separate between identity and scheduling domains.

## 3. Responsibilities by Service

### Auth Service
- User sign-up, login, refresh, and logout flows.
- Me endpoint for current principal.
- JWKS publication for downstream trust.
- Role/group/permission graph management.

### Booking Service
- JWT authentication and authorization policy enforcement.
- Availability and appointment business workflows.
- Scheduling data consistency and persistence.
