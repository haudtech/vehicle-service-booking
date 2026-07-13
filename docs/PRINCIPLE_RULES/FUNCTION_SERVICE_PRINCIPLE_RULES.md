# Function Service Principle Rules

Document Version: 1.0
Last Updated: July 13, 2026
Status: Active
Audience: Developers implementing Azure Functions-based services

## Purpose
This document defines mandatory principles and recommended patterns for Function services, based on lessons from Notification service implementation.

## 1. Architectural Principles

### 1.1 Keep Function host thin
Rules:
1. Function entry methods should only orchestrate trigger input, validation, and service invocation.
2. Provider integration logic must be in dedicated services behind interfaces.
3. Do not place complex business logic directly in trigger method bodies.

Pattern:
1. Trigger -> Parse/Validate -> IProviderService.SendAsync -> Log -> Complete/Rethrow

### 1.2 Use provider abstraction for downstream integrations
Rules:
1. Define interface contract (for example IEmailSender).
2. Register implementation by configuration key (Provider) through DI.
3. Keep one implementation per downstream provider (Logging, SendGrid, Google, others).

Benefits:
1. Safe local fallback provider.
2. Clear isolation for provider-specific auth, payload, and error handling.
3. Easy extension for new providers without changing trigger flow.

### 1.3 Asynchronous decoupling by queue first
Rules:
1. User-facing services must enqueue notification work asynchronously.
2. Request/response path must not wait on provider latency.
3. Queue trigger function handles delivery and retries.

## 2. Reliability and Error Handling Rules

### 2.1 Separate retriable vs non-retriable failures
Rules:
1. Invalid payload/deserialization/required-field failures are non-retriable and should be logged then returned.
2. Downstream provider failures should throw to allow runtime retry and dead-letter behavior.

### 2.2 Poison queue is mandatory
Rules:
1. Configure poison queue and implement explicit poison handler function.
2. Poison handler must log enough metadata to support replay and root-cause analysis.

### 2.3 Idempotency-aware design
Rules:
1. Assume at-least-once queue delivery.
2. Include correlation id and event metadata in logs.
3. Avoid side effects that cannot tolerate duplicate execution unless dedupe strategy exists.

## 3. Configuration Principles

### 3.1 Canonical key normalization
Rules:
1. Normalize environment key styles (upper-case, section-style, local.settings) to canonical runtime keys before host build.
2. Keep one canonical key path for option binding.

### 3.2 Deterministic local config loading
Rules:
1. Support robust .env discovery across script-root and normal run modes.
2. Ensure local.settings and .env values are consistently loaded regardless of startup mode.

### 3.3 Secret handling
Rules:
1. Never log secrets or tokens.
2. Log boolean flags only (for example ApiKeyConfigured=true/false).
3. Store secrets in environment/config providers only.

## 4. Observability Rules

### 4.1 Structured logs required
Rules:
1. Use structured logging with key fields: EventType, CorrelationId, ToEmail, Provider.
2. Include provider response details on success and failure:
   - status code
   - reason phrase
   - provider message id (if available)
   - response body (with sensitive data redaction policy)

### 4.2 Startup diagnostics required
Rules:
1. Log effective queue name, poison queue name, and provider on startup.
2. Log credential-configuration flags without exposing secret values.

### 4.3 Execution outcome logs
Rules:
1. Log clear markers for start, processed success, and failure paths.
2. Ensure logs allow correlation between enqueue and function processing.

## 5. Debug and Build Consistency Rules

### 5.1 Script-root output consistency
Rules:
1. If using func start --script-root ./bin/output, always build artifacts to the same output folder before start.
2. Keep PDB and DLL in sync with active source to avoid unbound breakpoints.

### 5.2 Attach workflow for isolated worker
Rules:
1. Start host first, then attach debugger to the active dotnet worker process.
2. Prefer the newest matching process id for the Function worker when multiple appear.

## 6. Provider Integration Rules

### 6.1 OAuth providers
Rules:
1. Support refresh-token mode for automatic token renewal.
2. Allow optional direct access token override for diagnostics only.
3. Validate required OAuth configuration before API requests.

### 6.2 API response validation
Rules:
1. Treat non-success HTTP status as failure and throw retriable exception.
2. Parse provider response identifiers when available (message id/thread id).

## 7. Documentation Rules for Every Function Service

Required docs:
1. Architecture overview (components and boundaries).
2. Cross-service sequence diagram (producer to consumer).
3. Trigger workflow (success path and retry path).
4. Troubleshooting playbook with exact validation commands.
5. Configuration reference with required/optional keys.

## 8. Compliance Checklist

A Function service implementation is compliant only when all are true:
1. Trigger function contains orchestration only.
2. Provider abstraction exists and is DI-selected by config.
3. Non-retriable and retriable errors are separated correctly.
4. Poison queue handler exists.
5. Structured logs include correlation and provider response metadata.
6. Startup config diagnostics are present and safe.
7. Script-root build/debug workflow is deterministic.
8. Architecture and troubleshooting docs are published.
