# Notification Troubleshooting Playbook

## Purpose
This playbook provides fast diagnosis paths for notification delivery issues in local development and integration environments.

## Scope
- VehicleServiceBooking.Auth publishes notification events
- Azure Storage Queue transport
- VehicleServiceBooking.Notification.Functions processing
- Email provider adapters (Logging, SendGrid, Google)

## 1. Quick Triage Checklist

1. Confirm queue trigger host is running.
2. Confirm target queue and poison queue are reachable.
3. Confirm message is enqueued with expected payload shape.
4. Confirm function execution logs show processing start and completion.
5. Confirm provider response status and provider message id.
6. If provider succeeded but recipient does not see email, check mailbox classification (Spam/Promotions/filters).

## 2. Fast Validation Commands

Run from repository root unless noted.

### 2.1 Build Notification Functions
```bash
cd src/VehicleServiceBooking.Notification.Functions
dotnet build -c Debug -o ./bin/output
```

### 2.2 Start Functions host from script root
```bash
cd src/VehicleServiceBooking.Notification.Functions
cp local.settings.json ./bin/output/local.settings.json
func start --script-root ./bin/output --verbose
```

### 2.3 Check host port
```bash
lsof -nP -iTCP:7071 -sTCP:LISTEN
```

### 2.4 Send a manual queue message
```bash
TO_EMAIL='haud.fin@gmail.com' EVENT_TYPE='Debug.UserEmailCheck' CORRELATION_ID='debug-check-1' ./scripts/send_notification_queue_message.sh
```

### 2.5 Check function-related processes
```bash
ps -axo pid,command | grep -E 'func start|VehicleServiceBooking.Notification.Functions.dll' | grep -v grep
```

### 2.6 Stop stale host and worker processes
```bash
pkill -f 'func start --script-root ./bin/output' || true
pkill -f 'VehicleServiceBooking.Notification.Functions.dll' || true
```

## 3. Common Errors and Fixes

## 3.1 Queue connection refused
Symptom:
- Retry timeout or connection errors to 127.0.0.1:10001

Likely root cause:
- Azurite is not running or queue endpoint is wrong

Checks:
```bash
lsof -nP -iTCP:10001 -sTCP:LISTEN
```

Fix:
1. Start Azurite.
2. Verify AzureWebJobsStorage or NOTIFICATION__QUEUECONNECTIONSTRING points to the correct local endpoint.

## 3.2 Messages move to poison queue
Symptom:
- Message reaches MaxDequeueCount and is moved to poison queue

Likely root cause:
- Repeated provider exception or payload parsing failure

Checks:
1. Review SendNotificationEmail logs for exceptions.
2. Review poison handler logs.

Fix:
1. Correct provider configuration or payload shape.
2. Requeue manually only after root cause is fixed.

## 3.3 Message encoding mismatch
Symptom:
- Trigger cannot parse message correctly, or repeated failures despite valid-looking payload

Likely root cause:
- Producer sends plain JSON while queue extension expects base64

Fix:
1. Ensure host.json queue extension uses messageEncoding none.
2. Rebuild and restart host.

## 3.4 Unbound breakpoints in Notification Functions
Symptom:
- Breakpoints stay hollow/unbound in function source files

Likely root cause:
- Host running stale binaries from bin/output while debugger source maps to newer bin/Debug output

Fix:
1. Build directly to bin/output before func start.
2. Start host, then attach to the dotnet process running VehicleServiceBooking.Notification.Functions.dll.
3. Pick highest current PID matching that command line.

## 3.5 Function host startup timeout in debug
Symptom:
- Script host unhealthy, worker start timed out

Likely root cause:
- Debug flow waits for attach incorrectly or stale/duplicated worker state

Fix:
1. Stop all host/worker processes.
2. Start host cleanly from script-root output mode.
3. Attach debugger after host is up.

## 3.6 Provider selected but missing credentials
Symptom:
- SendGrid or Google sender throws missing credential exceptions

Likely root cause:
- Provider key loaded, but required secret key not loaded

Fix:
1. Verify canonical keys exist in local.settings and or .env.
2. Restart host after editing env values.
3. Validate startup diagnostics flags in logs.

## 3.7 Google OAuth redirect_uri_mismatch
Symptom:
- OAuth Playground login fails with redirect_uri_mismatch

Likely root cause:
- OAuth client missing Playground redirect URI

Fix:
1. Add authorized redirect URI:
   - https://developers.google.com/oauthplayground
2. Save and wait for propagation.
3. Retry OAuth flow.

## 3.8 Google API accessNotConfigured (403)
Symptom:
- Gmail API response says API not used in project or disabled

Likely root cause:
- Gmail API not enabled for selected project

Fix:
1. Enable Gmail API in the same Google Cloud project used by OAuth client.
2. Wait for propagation.
3. Retry send.

## 3.9 Google send success but recipient cannot find email
Symptom:
- Gmail API send response is success with SENT label but recipient does not see message in Inbox

Likely root cause:
- Gmail classification or filtering, not transport failure

Fix:
1. Check recipient Spam, Promotions, All Mail.
2. Mark not spam and create filter never send to spam.
3. Search by sender and recent time window.

## 4. Log Patterns to Look For

Success path:
1. Executing Functions.SendNotificationEmail
2. Provider success log with status and provider message id
3. Processed notification message
4. Executed Functions.SendNotificationEmail Succeeded

Failure path:
1. Notification delivery failed with provider exception
2. Repeated dequeue attempts
3. Eventual poison queue movement

## 5. Provider-Specific Validation

### Logging provider
Expected:
- EMAIL SEND provider Logging appears in function logs

### SendGrid provider
Expected:
- HTTP success status and SendGrid message id header when available

### Google provider
Expected:
- HTTP 200 and response body containing id/threadId/labelIds with SENT

## 6. Recommended Operational Practices

1. Keep notification publishing asynchronous and non-blocking for user-facing requests.
2. Keep provider errors retriable in function handler by rethrowing.
3. Keep invalid payload errors non-retriable by handling and returning.
4. Regularly inspect poison queue and document replay procedures.
5. Use stable sender identity and mailbox filtering rules during local testing.

## 7. Implementation Timeline (Issues Resolved)

1. Auth request path blocked by queue retries
- Symptom: user-facing Auth calls slowed or failed when queue endpoint was unavailable.
- Resolution: bounded publish timeout and safe fallback path so auth request can still complete.

2. Debug compound confusion and attach flow instability
- Symptom: unpredictable debug startup and noisy process picker behavior.
- Resolution: explicit host-start plus attach pattern for Notification function debugging.

3. Host startup instability in local runs
- Symptom: host exits with unstable startup behavior under mixed run modes.
- Resolution: standardized script-root startup workflow with cleanup and deterministic run sequence.

4. Queue encoding mismatch causing retries/poison
- Symptom: repeated trigger failures and poison movement despite valid payload.
- Resolution: aligned queue extension configuration to use plain JSON message encoding.

5. Weak poison diagnostics
- Symptom: difficult root-cause analysis for dead-lettered messages.
- Resolution: introduced poison queue processor with structured error logging.

6. Non-retriable payload errors retried unnecessarily
- Symptom: invalid payloads retried repeatedly.
- Resolution: split handling rules: payload/validation errors are skipped, provider errors are retried.

7. Configuration loading inconsistencies
- Symptom: provider selected but credential values missing at runtime.
- Resolution: robust `.env` discovery plus normalization of key styles into canonical runtime keys.

8. Unbound breakpoints due to stale output artifacts
- Symptom: breakpoints remained unbound in Notification source files.
- Resolution: ensured build output and running script-root binaries are synchronized before host start.

9. Provider response observability gaps
- Symptom: insufficient logs to confirm downstream provider behavior.
- Resolution: enriched success/failure logs with status, reason phrase, provider message identifiers, and response body.

10. Google OAuth and API onboarding blockers
- Symptom: redirect URI mismatch and access-not-configured provider errors.
- Resolution: corrected OAuth redirect settings, enabled Gmail API, and validated refresh-token flow.

11. Delivery succeeded but recipient did not see inbox mail
- Symptom: provider success logs present, but email not found in primary inbox.
- Resolution: verified mailbox classification behavior (spam/promotions/filters) and documented remediation.

12. Documentation coverage gap
- Symptom: no single source describing architecture, sequence, workflow, and runbook.
- Resolution: created Notification architecture guide and troubleshooting playbook.
