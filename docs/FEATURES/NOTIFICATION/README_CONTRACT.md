# VehicleServiceBooking.Notification.Functions

Azure Functions (isolated worker) project for asynchronous notification delivery.

## Purpose
- Consume notification messages from Azure Storage Queue.
- Send email notifications via configured provider.

## Queue Contract
This project consumes JSON payload aligned with Auth producer contract:
- EventType
- ToEmail
- Subject
- Content
- CorrelationId
- Source
- OccurredAtUtc

## Local Configuration
1. Copy `local.settings.example.json` to `local.settings.json`.
2. Set `AzureWebJobsStorage`:
   - Use Azurite for local development (`UseDevelopmentStorage=true`), or
   - Use real storage connection string.
3. Set `NotificationQueueName` to the same queue configured in Auth service.
4. Email provider values can come from either:
   - repository root `.env` (loaded automatically at startup), or
   - `local.settings.json`

Queue configuration fallback:
- If root `.env` provides `NOTIFICATION__QUEUENAME`, startup maps it to the Function trigger setting `NotificationQueueName`.
- If root `.env` provides `NOTIFICATION__QUEUECONNECTIONSTRING`, startup maps it to `AzureWebJobsStorage` for the queue trigger connection.
- This allows Auth producer and Notification Function to share the same queue settings from one `.env` source.

## Runtime Path
1. Auth publishes message to queue (`user-notification-events` by default).
2. `SendNotificationEmail` function is triggered by queue message.
3. `IEmailSender` implementation sends/logs email.

## Email Provider Selection
- `EmailSender__Provider=Logging`
   - Uses `LoggingEmailSender` (safe for local development).

- `EmailSender__Provider=SendGrid`
   - Uses `SendGridEmailSender`.
   - Required settings:
      - `EmailSender__FromAddress`
      - `EmailSender__SendGridApiKey`
   - Optional settings:
      - `EmailSender__FromName`
      - `EmailSender__SendGridEndpoint` (defaults to SendGrid v3 mail send endpoint)

- `EmailSender__Provider=Google`
   - Uses `GoogleEmailSender` (Gmail API mode).
   - Required settings:
      - `EmailSender__FromAddress`
      - Either:
         - `EmailSender__GoogleAccessToken`, or
         - all of: `EmailSender__GoogleClientId`, `EmailSender__GoogleClientSecret`, `EmailSender__GoogleRefreshToken`
   - Optional settings:
      - `EmailSender__FromName`
      - `EmailSender__GoogleApiEndpoint` (defaults to Gmail users endpoint)
      - `EmailSender__GoogleTokenEndpoint` (defaults to Google OAuth token endpoint)
      - `EmailSender__GoogleUserId` (defaults to `me`)

## Next Hardening Steps
- Add retry policy and dead-letter handling strategy for persistent email provider failures.
- Add deduplication strategy if queue retries can cause duplicate emails.
