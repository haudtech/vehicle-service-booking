# Auth and Booking Sequential Flows View

Purpose: Describe end-to-end runtime interactions between clients, Auth service, and Booking service.
Status: CURRENT - Reflects implemented flows.

---

## 1. Sign-up and Login Token Issuance

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

## 2. Booking Request Authorization with JWKS

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

## 3. Refresh and Logout Lifecycle

```mermaid
sequenceDiagram
    autonumber
    participant Client
    participant AuthAPI as Auth Service API
    participant AuthDB as Auth Database
    participant Token as JWT/JWKS Service

    Client->>AuthAPI: POST /api/v1/auth/refresh
    AuthAPI->>AuthDB: Validate refresh token status
    AuthDB-->>AuthAPI: Token is active
    AuthAPI->>Token: Rotate and issue new tokens
    Token-->>AuthAPI: New access + refresh token
    AuthAPI-->>Client: 200 OK

    Client->>AuthAPI: POST /api/v1/auth/logout
    AuthAPI->>AuthDB: Revoke refresh token/session
    AuthDB-->>AuthAPI: Revoked
    AuthAPI-->>Client: 204 No Content
```
