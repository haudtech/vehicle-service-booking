# Authentication Knowledge Base: Email Verification Methods and Sequential Flows

Last updated: 2026-07-14
Scope: Practical guidance for sign-up verification, login assurance, and modern authentication methodology in service-oriented systems.

## Status
Supporting reference for verification strategy; not a primary runtime contract.

## 1. Purpose

This document captures reusable knowledge and decision methodology for authentication verification flows, especially:

- Email verification by callback link
- Email verification by explicit POST (code/token submission)
- Hybrid flow design used by modern services
- Sequential request/response behavior per flow
- Security controls and implementation checklist

This file is intended to be a quick engineering reference for future implementation and review work.

## 2. Terms and Context

- Verification: proving ownership of a communication channel (email).
- Authentication: proving user identity at sign-in (password/passkey/2FA/etc.).
- Authorization: proving user permissions to access resources.
- Pending account: user record created but not fully trusted until verification succeeds.

Important distinction:

- Email verification confirms channel ownership.
- It does not replace strong sign-in authentication controls (2FA/WebAuthn/risk checks).

## 3. Option A: Verification Link Callback (Email Link Click)

### 3.1 What it is

After sign-up, system sends an email with a clickable URL containing an opaque one-time token. User click triggers verification callback flow.

### 3.2 Sequential steps

1. User submits sign-up request.
2. Auth service creates user in pending verification state.
3. Auth generates one-time verification token with short expiry.
4. Auth stores token hash + expiry + state (never plaintext token).
5. Auth publishes notification event to send verification email.
6. Notification service sends email with verification URL.
7. User clicks verification URL.
8. Browser/app calls verification callback endpoint.
9. Auth validates token hash, expiry, and one-time usage state.
10. Auth marks email verified and invalidates token.
11. User can now complete login/sign-in.

### 3.3 Pros

1. Best UX (one click).
2. Highest verification completion in consumer apps.
3. Familiar pattern for most users.
4. Easy deep-link integration for mobile/web.

### 3.4 Cons

1. Token in URL can leak through logs/history/referrer if not controlled.
2. Email security scanners can pre-click links.
3. Requires strict replay prevention and short token TTL.
4. Extra care needed for cross-domain frontend/backend redirects.

## 4. Option B: Explicit Verify POST (Code/Token Submission)

### 4.1 What it is

After sign-up, user receives code/token. UI collects it and sends secure POST request to verify endpoint.

### 4.2 Sequential steps

1. User submits sign-up request.
2. Auth creates pending user.
3. Auth generates code/token and stores only hash + expiry.
4. Notification sends code/token to email.
5. UI shows verification form.
6. User enters code/token manually.
7. UI submits POST /verify-email with body payload.
8. Auth validates token hash, expiry, usage state.
9. Auth marks email verified and consumes token.
10. User proceeds to login/sign-in.

### 4.3 Pros

1. Better control over token transport (in body, not URL).
2. Less exposure to passive URL leakage.
3. More resilient to auto-link scanners.
4. Works well for app-driven controlled UX and anti-bot checks.

### 4.4 Cons

1. More user friction (copy/paste or switch context).
2. Lower completion/conversion compared to click link.
3. Extra UI state and retry/resend handling required.
4. Slightly longer onboarding path.

## 5. Modern Best Practice: Hybrid Strategy

Most modern services effectively use a hybrid model:

1. Primary path: verification link callback for UX.
2. Fallback path: manual POST verification with code/token.
3. Risk-aware controls: if scanner/pre-click risk is detected, require explicit final confirmation.

Why hybrid is best now:

- Gives conversion benefits of one-click flow.
- Preserves resilience with manual fallback.
- Handles real-world client/device/email-gateway variability.

## 6. Relationship to Real-World Platform Patterns

Typical pattern in major platforms:

1. Email ownership verification is generally link-based.
2. Sign-in hardening uses additional challenges (2FA/passkeys/risk checks).
3. Risk scenarios can require extra post-login proof.

Takeaway:

- Link callback is for email verification UX.
- POST/challenge flows are for stronger authentication assurance.

## 7. Security Controls Checklist (Must-Have)

### 7.1 Token design

1. Use high-entropy random token.
2. Store hash only (SHA-256 or better) in DB.
3. Set short TTL (e.g., 15-60 min typical; up to 24h by policy).
4. Enforce one-time use (consumed flag or clear hash on success).
5. Compare in constant time.

### 7.2 Endpoint and transport

1. HTTPS only.
2. Strict redirect allowlist.
3. Avoid exposing sensitive data in logs.
4. Rate limit verify and resend endpoints.
5. Return generic error messages for invalid/expired tokens.

### 7.3 Anti-abuse

1. Resend cooldown and max attempts.
2. Event auditing with correlation IDs.
3. Device/IP telemetry for anomaly detection.
4. Optional CAPTCHA/risk challenge for suspicious activity.

### 7.4 Data and consistency

1. Keep verification state transitions atomic.
2. Ensure idempotent verify behavior.
3. Avoid issuing final auth tokens before verification (for password sign-up path).
4. Publish notifications through reliable async path with retry strategy.

## 8. Recommended API Shape

### 8.1 Link callback path

- GET /api/v1/auth/verify-email?token=...&email=...
- or GET with opaque verification ID only, where server resolves state.

Best practice:

- Validate then redirect to frontend result page (success/failure).
- Avoid leaking sensitive details in URL after completion.

### 8.2 Manual fallback path

- POST /api/v1/auth/verify-email
- Body: email + token/code

Best practice:

- Keep this endpoint available even when link callback exists.

### 8.3 Optional support endpoints

- POST /api/v1/auth/resend-verification
- POST /api/v1/auth/verification/status (optional)

## 9. Notification Event Semantics

Prefer semantic events tied to state transitions:

1. Auth.EmailVerification.Requested
2. Auth.EmailVerification.Success
3. Auth.SignUp.Verified
4. Auth.SignIn.Success

Avoid sending final “welcome activated” semantics before verification is complete.

## 10. Implementation Decision Matrix

Use this to choose flow per product need.

| Condition | Prefer Link Callback | Prefer POST Manual | Hybrid |
|---|---|---|---|
| Consumer UX priority | Yes | No | Yes |
| Strict anti-leak posture | Moderate | Strong | Strong |
| High email scanner exposure | Risky alone | Strong | Strong |
| Mobile deep-link support | Strong | Moderate | Strong |
| Fast MVP delivery | Strong | Moderate | Strong |
| Long-term enterprise robustness | Moderate | Strong | Strong |

Default recommendation: Hybrid.

## 11. Integration Guidance for This Repository

For future Auth updates, align implementation with these principles:

1. Sign-up creates pending account.
2. Verification notification sends clickable URL.
3. Verify endpoint supports both link callback and manual POST fallback.
4. Login path blocks unverified accounts for password-based sign-up.
5. Notification events represent verification lifecycle, not premature success.

## 12. Review Checklist for Future PRs

1. Is token plaintext avoided in persistence?
2. Is token one-time and short-lived?
3. Are callback and POST fallback both supported?
4. Are verify/resend endpoints rate-limited?
5. Are logs free from sensitive token exposure?
6. Is account activation state transition atomic and idempotent?
7. Are notification event names semantically correct?
8. Is UX clear for success, expired token, and resend paths?

---

If architecture changes, update this document first, then implementation docs, then test guides to keep verification methodology consistent across teams.
