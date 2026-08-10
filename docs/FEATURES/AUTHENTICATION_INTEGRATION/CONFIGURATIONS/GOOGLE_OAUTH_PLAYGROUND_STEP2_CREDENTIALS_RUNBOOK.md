# Google OAuth 2.0 Playground Step 2 Runbook

Purpose: generate and safely populate the three environment variables required by the email sender integration.

Target outputs:
- EMAILSENDER__GOOGLECLIENTID
- EMAILSENDER__GOOGLECLIENTSECRET
- EMAILSENDER__GOOGLEREFRESHTOKEN

---

## 1. Required links

Google Cloud project selection:
- https://console.cloud.google.com/projectselector2/home/dashboard

APIs dashboard:
- https://console.cloud.google.com/apis/dashboard

Enable Gmail API:
- https://console.cloud.google.com/apis/library/gmail.googleapis.com

Credentials page:
- https://console.cloud.google.com/apis/credentials

OAuth consent screen (Google Auth Platform):
- https://console.cloud.google.com/apis/credentials/consent

OAuth Playground:
- https://developers.google.com/oauthplayground/

OAuth consent screen official guide:
- https://developers.google.com/workspace/guides/configure-oauth-consent

OAuth 2.0 offline access (refresh token behavior):
- https://developers.google.com/identity/protocols/oauth2/web-server#offline

Gmail API OAuth scopes reference:
- https://developers.google.com/workspace/gmail/api/auth/scopes

---

## 2. Preconditions and security notes

1. Use the same Google Cloud project that owns your OAuth app and Gmail API access.
2. Do not reuse previously leaked client secrets or refresh tokens.
3. Do not paste tokens into docs, chat, commits, issue comments, or screenshots.
4. Keep .env gitignored and local-only.
5. If any token appears in screenshot/history, rotate it again after validation.

---

## 3. Enable API permissions (Gmail API)

1. Open APIs dashboard:
   - https://console.cloud.google.com/apis/dashboard
2. Click Enable APIs and Services.
3. Open Gmail API entry:
   - https://console.cloud.google.com/apis/library/gmail.googleapis.com
4. Click Enable (or verify status is Enabled).

Validation:
- Gmail API appears in Enabled APIs list for your selected project.

---

## 4. Configure OAuth consent screen correctly

1. Open OAuth consent screen:
   - https://console.cloud.google.com/apis/credentials/consent
2. Choose User Type:
   - External (typical for personal Gmail testing) or Internal (Workspace org only).
3. Fill required app info:
   - App name
   - User support email
   - Developer contact email
4. Scopes step:
   - Add scope: https://www.googleapis.com/auth/gmail.send
   - This is least-privilege for send-only behavior.
5. Test users step (critical when app is in Testing status):
   - Add the Gmail account that will authorize in OAuth Playground.
6. Save all steps.

Validation:
- App is configured and the intended Gmail account is listed in Test users.

---

## 5. Create or rotate OAuth client credentials

1. Open Credentials page:
   - https://console.cloud.google.com/apis/credentials
2. Create Credentials -> OAuth client ID.
3. Application type:
   - Web application
4. Authorized redirect URIs:
   - Add: https://developers.google.com/oauthplayground
5. Create and copy values.

Populate these values:
- EMAILSENDER__GOOGLECLIENTID=<new-client-id>
- EMAILSENDER__GOOGLECLIENTSECRET=<new-client-secret>

If rotating existing client secret:
1. Regenerate/create new secret on the same OAuth client.
2. Update the env value immediately.
3. Revoke prior app access/tokens later in Section 8.

---

## 6. OAuth Playground Step 1 (scope selection)

1. Open OAuth Playground:
   - https://developers.google.com/oauthplayground/
2. Click gear icon (top-right settings).
3. Enable Use your own OAuth credentials.
4. Paste your new OAuth client ID and client secret.
5. In Step 1, paste scope:
   - https://www.googleapis.com/auth/gmail.send
6. Click Authorize APIs.
7. Sign in as the Gmail test user from consent screen.
8. Approve consent.

Validation:
- Authorization code is returned in Playground Step 2.

---

## 7. OAuth Playground Step 2 (token exchange)

1. In Step 2, click Exchange authorization code for tokens.
2. Confirm both tokens appear:
   - Access token
   - Refresh token
3. Copy refresh token only for env usage.

Populate this value:
- EMAILSENDER__GOOGLEREFRESHTOKEN=<new-refresh-token>

Important:
- If refresh token is missing, re-run Step 1 and ensure consent is forced for a fresh grant.
- If using Playground default credentials instead of your own, refresh token lifecycle may not match your app requirements.

---

## 8. Revoke old exposed credentials (required)

### 8.1 Revoke old refresh tokens / app grants

1. Open Google Account security page of the Gmail owner:
   - https://myaccount.google.com/security
2. Go to Third-party apps with account access.
3. Find your app grant and remove access.
4. Re-authorize via Sections 6-7 to issue a fresh refresh token.

### 8.2 Rotate OAuth client secret if previously exposed

1. Go to project credentials:
   - https://console.cloud.google.com/apis/credentials
2. Regenerate secret for the OAuth client or create a new client.
3. Update EMAILSENDER__GOOGLECLIENTSECRET.

---

## 9. Update local configuration

Update these in your local .env:

- EMAILSENDER__GOOGLECLIENTID=<new-client-id>
- EMAILSENDER__GOOGLECLIENTSECRET=<new-client-secret>
- EMAILSENDER__GOOGLEREFRESHTOKEN=<new-refresh-token>

Keep these unchanged unless your integration design changes:
- EMAILSENDER__GOOGLEAPIENDPOINT=https://gmail.googleapis.com/gmail/v1/users
- EMAILSENDER__GOOGLETOKENENDPOINT=https://oauth2.googleapis.com/token
- EMAILSENDER__GOOGLEUSERID=me

Then restart the service so environment variables are reloaded.

---

## 10. Verification checklist

1. Gmail API is enabled in correct project.
2. OAuth consent screen configured and test user added.
3. OAuth client uses redirect URI:
   - https://developers.google.com/oauthplayground
4. Playground uses your own OAuth credentials.
5. Scope is exactly:
   - https://www.googleapis.com/auth/gmail.send
6. New refresh token obtained in Step 2.
7. Old app grants/tokens revoked.
8. Service restart completed.
9. Real email send test succeeds.

---

## 11. Common failure patterns and fixes

Issue: invalid_grant during token refresh
- Cause: refresh token revoked/expired or OAuth client mismatch.
- Fix: re-run Sections 6-7 with correct client and account.

Issue: access_denied during consent
- Cause: account not in Test users while app is in Testing mode.
- Fix: add account in consent screen Test users and retry.

Issue: redirect_uri_mismatch
- Cause: OAuth client missing Playground redirect URI.
- Fix: add https://developers.google.com/oauthplayground to authorized redirects.

Issue: insufficient_scope or Gmail send fails
- Cause: wrong scope selected.
- Fix: authorize with https://www.googleapis.com/auth/gmail.send.

---

## 12. Security hardening after successful setup

1. Store production secrets in a secret manager (not plain .env).
2. Limit OAuth scope to send-only.
3. Rotate OAuth client secret and refresh token on incident suspicion.
4. Disable token/sensitive logging.
5. Add periodic credential rotation policy and owner.
