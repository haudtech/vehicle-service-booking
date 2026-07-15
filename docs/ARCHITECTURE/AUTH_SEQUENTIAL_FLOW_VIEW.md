# Auth, Booking, and Notification Sequential Flows View

Purpose: Provide a compact runtime overview for the main auth, booking, and notification flows.
Status: CURRENT - High-level runtime summary only.

For the full behavioral contracts and step-by-step flows, use the feature docs in `docs/FEATURES/AUTHENTICATION_INTEGRATION`.

---

## 1. What This Doc Covers

This file is intentionally narrow:
- service-to-service runtime sequence at a high level
- request boundaries between Auth, Booking, and Notification
- where the reader should go for exact flow behavior

Detailed state transitions, payload shapes, validation branches, and operational edge cases belong in feature docs, not here.

---

## 2. High-Level Runtime Flows

### Sign-up and Login

```mermaid
sequenceDiagram
    autonumber
    participant Client
    participant AuthAPI as Auth Service API

    Client->>AuthAPI: Sign-up request
    AuthAPI-->>Client: Verification required

    Client->>AuthAPI: Verify email
    AuthAPI-->>Client: Email verified

    Client->>AuthAPI: Login challenge
    AuthAPI-->>Client: Challenge required

    Client->>AuthAPI: Verify challenge code
    AuthAPI-->>Client: Access + refresh tokens
```

Reference docs:
- [AUTH_SERVICE_SPEC.md](../FEATURES/AUTHENTICATION_INTEGRATION/AUTH_SERVICE_SPEC.md)
- [AUTH_EMAIL_VERIFICATION_METHODS_AND_SEQUENTIAL_FLOWS.md](../FEATURES/AUTHENTICATION_INTEGRATION/AUTH_EMAIL_VERIFICATION_METHODS_AND_SEQUENTIAL_FLOWS.md)
- [AUTH_OTP_METHODODOLOGY_AND_CHANNEL_STRATEGY.md](../FEATURES/AUTHENTICATION_INTEGRATION/AUTH_OTP_METHODODOLOGY_AND_CHANNEL_STRATEGY.md)

### Google Sign-In

```mermaid
sequenceDiagram
    autonumber
    participant Browser as Client Browser
    participant AuthAPI as Auth Service API
    participant Google as Google OAuth

    Browser->>AuthAPI: Start Google sign-in
    AuthAPI->>Google: Redirect to provider
    Google-->>Browser: Provider callback
    Browser->>AuthAPI: Auth callback
    AuthAPI-->>Browser: Local tokens
```

Reference docs:
- [AUTH_GOOGLE_OAUTH_FULL_FLOW.md](../FEATURES/AUTHENTICATION_INTEGRATION/AUTH_GOOGLE_OAUTH_FULL_FLOW.md)

### Booking Authorization

```mermaid
sequenceDiagram
    autonumber
    participant Client
    participant BookingAPI as Booking API
    participant AuthJWKS as Auth JWKS Endpoint

    Client->>BookingAPI: Protected booking request with Bearer token
    BookingAPI->>AuthJWKS: Resolve signing key by kid
    AuthJWKS-->>BookingAPI: Public key material
    BookingAPI-->>Client: Authorized booking response
```

Reference docs:
- [BOOKING_SERVICE_AUTH_INTEGRATION.md](../FEATURES/AUTHENTICATION_INTEGRATION/BOOKING_SERVICE_AUTH_INTEGRATION.md)
- [AUTH_JWT_JWKS_FULL_FLOW.md](../FEATURES/AUTHENTICATION_INTEGRATION/AUTH_JWT_JWKS_FULL_FLOW.md)

### Refresh and Logout

```mermaid
sequenceDiagram
    autonumber
    participant Client
    participant AuthAPI as Auth Service API

    Client->>AuthAPI: Refresh token request
    AuthAPI-->>Client: Rotated tokens

    Client->>AuthAPI: Logout request
    AuthAPI-->>Client: Session revoked
```

Reference docs:
- [AUTH_JWT_JWKS_FULL_FLOW.md](../FEATURES/AUTHENTICATION_INTEGRATION/AUTH_JWT_JWKS_FULL_FLOW.md)

### Async Notification Dispatch

```mermaid
sequenceDiagram
    autonumber
    participant AuthAPI as Auth Service API
    participant Queue as Azure Storage Queue
    participant Func as Notification Function

    AuthAPI->>Queue: Publish notification event
    Queue->>Func: Trigger delivery worker
    Func-->>Queue: Complete or retry message
```

Reference docs:
- [docs/FEATURES/NOTIFICATION/README.md](../FEATURES/NOTIFICATION/README.md)
- [docs/FEATURES/NOTIFICATION/TROUBLESHOOTING_PLAYBOOK.md](../FEATURES/NOTIFICATION/TROUBLESHOOTING_PLAYBOOK.md)

---

## 3. Architecture Summary

- Auth is the identity provider and token issuer.
- Booking validates JWTs locally and enforces scheduling authorization.
- Notification is asynchronous and side-effect driven.
- JWKS is the trust bridge between Auth and Booking.
- Feature docs hold the exact flow and behavior contracts.

---

## 4. Where to Read the Details

Use the following docs for step-by-step behavior:
- [AUTH_SERVICE_SPEC.md](../FEATURES/AUTHENTICATION_INTEGRATION/AUTH_SERVICE_SPEC.md)
- [AUTH_GOOGLE_OAUTH_FULL_FLOW.md](../FEATURES/AUTHENTICATION_INTEGRATION/AUTH_GOOGLE_OAUTH_FULL_FLOW.md)
- [AUTH_JWT_JWKS_FULL_FLOW.md](../FEATURES/AUTHENTICATION_INTEGRATION/AUTH_JWT_JWKS_FULL_FLOW.md)
- [AUTH_OTP_METHODODOLOGY_AND_CHANNEL_STRATEGY.md](../FEATURES/AUTHENTICATION_INTEGRATION/AUTH_OTP_METHODODOLOGY_AND_CHANNEL_STRATEGY.md)
- [AUTH_EMAIL_VERIFICATION_METHODS_AND_SEQUENTIAL_FLOWS.md](../FEATURES/AUTHENTICATION_INTEGRATION/AUTH_EMAIL_VERIFICATION_METHODS_AND_SEQUENTIAL_FLOWS.md)
- [USER_WORKFLOW_SIGNUP_TO_APPOINTMENT_WITH_AUTH.md](../FEATURES/AUTHENTICATION_INTEGRATION/USER_WORKFLOW_SIGNUP_TO_APPOINTMENT_WITH_AUTH.md)
- [BOOKING_SERVICE_AUTH_INTEGRATION.md](../FEATURES/AUTHENTICATION_INTEGRATION/BOOKING_SERVICE_AUTH_INTEGRATION.md)
- [docs/FEATURES/NOTIFICATION/README.md](../FEATURES/NOTIFICATION/README.md)

---

## 5. Rule of Thumb

If the document answers:
- exact request/response behavior
- token claims
- verification branch logic
- provider-specific steps
- retry and poison queue handling

then it belongs in a feature doc.

If the document answers:
- which service talks to which
- what trust boundary exists
- what the runtime topology looks like

then it belongs here.
