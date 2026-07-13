# 🏗️ Conceptual Architecture View

**Purpose:** High-level blueprint of system structure, layer boundaries, service boundaries, and trust topology.
**Architectural Style:** Clean Architecture with Domain-Driven Design (DDD) principles
**Status:** CURRENT - REFLECTS ACTUAL CODEBASE IMPLEMENTATION ✅
**Document Version:** 3.1

---

## 🏛️ 1. LAYERED ARCHITECTURE (CLEAN ARCHITECTURE)

The application isolates core business invariants from external technical infrastructure, frameworks, and databases. Dependencies flow strictly inward toward the Domain Layer.

```
┌──────────────────────────────┐
│      PRESENTATION LAYER      │  Controllers, DTOs, OpenAPI
└──────────────┬───────────────┘
               │ HTTP
               ▼
┌──────────────┴───────────────┐
│      APPLICATION LAYER       │  Interfaces, Biz Services, Validation
└──────────────┬───────────────┘
               │ Implements
               ▼
┌──────────────┴───────────────┐
│     INFRASTRUCTURE LAYER     │  EF Core, Repositories, Observability
└──────────────┬───────────────┘
               │ Mapping
               ▼
┌──────────────┴───────────────┐
│         DOMAIN LAYER         │  Entities, Business Invariants
└──────────────────────────────┘
```

---

## 🔗 2. CROSS-SERVICE ARCHITECTURE (AUTH + BOOKING + NOTIFICATION)

The current production shape is a three-service collaboration model:

- Auth service acts as identity provider and JWT issuer.
- Booking service acts as protected resource API.
- Booking validates JWT locally using keys published by Auth via JWKS.
- Notification service (Azure Functions) acts as asynchronous delivery worker.
- Auth publishes notification events to queue; Notification Functions process delivery via provider adapters.

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

    subgraph NotificationPipeline[Notification Service]
        Publisher[Auth Notification Publisher]
        Queue[(Azure Queue user-notification-events)]
        Poison[(Azure Queue user-notification-events-poison)]
        NotificationFunc[SendNotificationEmail Function]
        PoisonFunc[ProcessNotificationPoison Function]
        EmailSender[IEmailSender Adapter]
        GoogleProvider[GoogleEmailSender]
        SendGridProvider[SendGridEmailSender]
        LoggingProvider[LoggingEmailSender]
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

    AuthApplication --> Publisher --> Queue
    Queue --> NotificationFunc --> EmailSender
    EmailSender --> GoogleProvider
    EmailSender --> SendGridProvider
    EmailSender --> LoggingProvider
    Queue -. retry exhausted .-> Poison --> PoisonFunc
```

---

## 🔄 3. RUNTIME FLOW VIEW LINKS

For detailed step-by-step runtime interactions, use:

- `AUTH_SEQUENTIAL_FLOW_VIEW.md` for request sequences.
- `AUTH_COMPONENTS_VIEW.md` for component-level boundaries.
- `../FEATURES/NOTIFICATION/README.md` for notification workflow details.