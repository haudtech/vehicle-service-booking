# Auth, Booking, and Notification Sequential Flows View

Purpose: Describe end-to-end runtime interactions between clients, Auth service, Booking service, and Notification service.
Status: CURRENT - Reflects implemented flows.

---

## 1. Sign-up Verification and Two-Step Password Login

```mermaid
sequenceDiagram
    autonumber
    participant Client
    participant AuthAPI as Auth Service API
    participant AuthApp as Auth Application Service
    participant AuthDB as Auth Database
    participant Notify as Notification Queue Publisher
    participant Token as JWT/JWKS Service

    Client->>AuthAPI: POST /api/v1/auth/signup
    AuthAPI->>AuthApp: Validate + normalize account identity
    AuthApp->>AuthDB: Create pending user + verification token hash + expiry
    AuthDB-->>AuthApp: User created
    AuthAPI->>Notify: Publish Auth.EmailVerification.Requested
    AuthApp-->>AuthAPI: Verification metadata
    AuthAPI-->>Client: 202 Accepted (verification required)

    Client->>AuthAPI: GET /api/v1/auth/verify-email?email=...&token=...
    AuthAPI->>AuthApp: VerifyEmailAsync(email, token)
    AuthApp->>AuthDB: Validate token hash + expiry; mark verified; clear token fields
    AuthDB-->>AuthApp: Verification completed
    AuthAPI-->>Client: 200 OK (email verified)

    Client->>AuthAPI: POST /api/v1/auth/login
    AuthAPI->>AuthApp: Validate credentials (accountName/email + password)
    AuthApp->>AuthDB: Read active user + role/group relations
    AuthDB-->>AuthApp: User + auth graph
    AuthApp->>AuthDB: Persist login challenge id + code hash + expiry + attempts
    AuthDB-->>AuthApp: Challenge persisted
    AuthAPI-->>Client: 202 Accepted (challenge required)

    Client->>AuthAPI: POST /api/v1/auth/login/verify-code
    AuthAPI->>AuthApp: Verify challenge id + code + expiry + attempts
    AuthApp->>AuthDB: Consume challenge on success
    AuthDB-->>AuthApp: Challenge consumed
    AuthApp->>Token: Issue access + refresh token
    Token-->>AuthApp: Signed JWT + refresh token
    AuthApp-->>AuthAPI: Auth response
    AuthAPI-->>Client: 200 OK with tokens
```

## 2. Google Sign-In to Local Token Issuance

```mermaid
sequenceDiagram
    autonumber
    participant Browser as Client Browser
    participant AuthAPI as Auth Service API
    participant Google as Google OAuth
    participant GoogleMW as Google Middleware (/signin-google)
    participant AuthApp as Auth Application Service
    participant AuthDB as Auth Database
    participant Token as JWT/JWKS Service

    Browser->>AuthAPI: GET /api/v1/auth/google/start
    AuthAPI-->>Browser: 302 Challenge Google

    Browser->>Google: Authenticate + consent
    Google-->>Browser: 302 /signin-google?code=...&state=...

    Browser->>GoogleMW: GET /signin-google
    GoogleMW->>Google: Exchange auth code for identity claims
    Google-->>GoogleMW: email + sub + profile claims
    GoogleMW-->>Browser: External auth cookie + 302 /api/v1/auth/google/callback

    Browser->>AuthAPI: GET /api/v1/auth/google/callback
    AuthAPI->>AuthAPI: Resolve verified-email evidence from claims and id_token fallback
    AuthAPI->>AuthApp: LoginWithGoogleAsync(email, displayName, providerSubject, emailVerified, ip)

    alt Email not verified by provider evidence
        AuthApp-->>AuthAPI: InvalidOperationException
        AuthAPI-->>Browser: 401 Unauthorized
    else Email verified by provider evidence

        alt Existing user by normalized email
            AuthApp->>AuthDB: Load active user + auth graph
            AuthDB-->>AuthApp: Existing user
        else First successful Google login
            AuthApp->>AuthDB: Create verified user + assign default role
            AuthDB-->>AuthApp: User provisioned
        end

        AuthApp->>Token: Issue access + refresh token
        Token-->>AuthApp: Signed JWT + refresh token
        AuthApp-->>AuthAPI: Auth response
        AuthAPI-->>Browser: 200 OK with local tokens
    end
```

## 3. Booking Request Authorization with JWKS

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

## 4. Refresh and Logout Lifecycle

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

## 5. Async Notification Dispatch and Delivery

```mermaid
sequenceDiagram
    autonumber
    participant Client
    participant API as VehicleServiceBooking.Api
    participant AuthAPI as Auth Service API
    participant AuthApp as Auth Application Service
    participant Queue as Azure Storage Queue
    participant Func as SendNotificationEmail Function
    participant Sender as IEmailSender Provider
    participant Provider as Email Provider API

    Client->>API: Signup/Login-related request
    API->>AuthAPI: Forward request
    AuthAPI->>AuthApp: Execute auth workflow
    AuthApp->>Queue: Enqueue NotificationMessage (async)
    AuthApp-->>AuthAPI: Business success response
    AuthAPI-->>API: Auth result
    API-->>Client: HTTP success

    Queue-->>Func: Trigger with queued message
    Func->>Func: Deserialize + validate payload
    Func->>Sender: SendAsync(message)
    Sender->>Provider: HTTP send request
    Provider-->>Sender: Success/Failure response

    alt Provider success
        Sender-->>Func: Success
        Func-->>Queue: Complete message
    else Provider failure
        Sender-->>Func: Exception
        Func-->>Queue: Rethrow for retry/dead-letter
    end
```

## 6. Notification Retry and Poison Lifecycle

```mermaid
sequenceDiagram
    autonumber
    participant Queue as Notification Queue
    participant Func as SendNotificationEmail Function
    participant Runtime as Functions Runtime
    participant Poison as Notification Poison Queue
    participant PoisonFunc as ProcessNotificationPoison Function

    Queue-->>Func: Deliver message
    Func-->>Runtime: Throw on retriable delivery failure
    Runtime-->>Queue: Increment dequeue count

    alt DequeueCount below threshold
        Runtime-->>Queue: Message visible for retry
    else MaxDequeueCount reached
        Runtime-->>Poison: Move message to poison queue
        Poison-->>PoisonFunc: Trigger poison handler
        PoisonFunc->>PoisonFunc: Log payload + diagnostics
    end
```
