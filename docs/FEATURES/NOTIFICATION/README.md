# Notification Feature Guide

## Purpose
Feature-level guide for notification implementation details, configuration, and operations.

System-wide architecture and cross-service sequence diagrams are centralized in:
1. `docs/ARCHITECTURE/README.md`
2. `docs/ARCHITECTURE/CONCEPTUAL_VIEW.md`
3. `docs/ARCHITECTURE/AUTH_COMPONENTS_VIEW.md`
4. `docs/ARCHITECTURE/AUTH_SEQUENTIAL_FLOW_VIEW.md`

## Feature Scope
This feature covers:
1. Auth-side async notification publish.
2. Queue-triggered processing in Notification Functions.
3. Provider abstraction with pluggable implementations.
4. Retry/dead-letter behavior for downstream delivery failures.

## Runtime Responsibilities (Feature-Owned)

### Auth Publisher
1. Publishes notification events asynchronously to `NotificationQueueName`.
2. Keeps request path resilient with safe publish behavior.

### Notification Functions
1. `SendNotificationEmail`:
2. Deserialize and validate payload.
3. Invoke provider via `IEmailSender`.
4. Complete message on success.
5. Rethrow provider failures for retry semantics.
6. `ProcessNotificationPoison`:
7. Consume poison queue messages and log diagnostics.

### Provider Adapter (`IEmailSender`)
1. `LoggingEmailSender` for local-safe diagnostics.
2. `SendGridEmailSender` for SendGrid API delivery.
3. `GoogleEmailSender` for Gmail API delivery with OAuth token flow.

## Configuration Reference (Feature-Owned)

### Queue keys
1. `NotificationQueueName`
2. `NotificationPoisonQueueName`
3. `AzureWebJobsStorage`

### Provider selection
1. `EmailSender__Provider` (`Logging`, `SendGrid`, `Google`)

### SendGrid keys
1. `EmailSender__SendGridApiKey`
2. `EmailSender__SendGridEndpoint`
3. `EmailSender__FromAddress`
4. `EmailSender__FromName`

### Google keys
1. `EmailSender__GoogleClientId`
2. `EmailSender__GoogleClientSecret`
3. `EmailSender__GoogleRefreshToken`
4. `EmailSender__GoogleAccessToken` (optional override)
5. `EmailSender__GoogleApiEndpoint`
6. `EmailSender__GoogleTokenEndpoint`
7. `EmailSender__GoogleUserId`
8. `EmailSender__FromAddress`
9. `EmailSender__FromName`

## Local Operations
1. Build and start function host from project root mode for standard config loading.
2. Send deterministic test events using script:
3. `scripts/send_notification_queue_message.sh`
4. Validate logs for:
5. Trigger execution and processed message logs.
6. Provider response status and provider message id.

## Feature Contracts and Runbooks
1. Notification payload contract: `docs/FEATURES/NOTIFICATION/README_CONTRACT.md`
2. Troubleshooting playbook: `docs/FEATURES/NOTIFICATION/TROUBLESHOOTING_PLAYBOOK.md`

## Success Criteria
1. Auth request path returns without waiting for provider latency.
2. Notification message is consumed by function trigger.
3. Provider reports success with traceable response metadata.
4. No poison movement for successful deliveries.
