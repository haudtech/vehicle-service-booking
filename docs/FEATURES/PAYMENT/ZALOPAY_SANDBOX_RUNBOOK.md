# ZaloPay Sandbox Runbook

## Purpose
This runbook provides a practical, copy-paste flow to integrate and validate ZaloPay sandbox with this service.

Use this when you want to:
1. Configure sandbox credentials.
2. Expose local callback endpoint over HTTPS.
3. Create payment intent and complete checkout.
4. Verify callback processing and payment state transitions.

## Preconditions
1. API project builds and runs locally.
2. Database has payment seed data.
3. You can access ZaloPay sandbox merchant center.

Merchant portal:
1. https://sbmc.zalopay.vn/
2. https://sbmc.zalopay.vn/homepage/index.html

## Step 1. Get Sandbox Credentials and Configure App
In sandbox merchant center:
1. Login.
2. Open menu: Hi, [Phone number] -> App integrated management.
3. Get values:
   - AppId
   - Key1
   - Key2
4. Set:
   - Callback URL
   - Redirect URL

Callback/Redirect guidance:
1. Callback URL should be your public API webhook endpoint:
   - https://<public-host>/api/v1/payments/webhooks/ZaloPay
2. Redirect URL can be your frontend result page or API return endpoint:
   - https://<public-host>/api/v1/payments/return

## Step 1A. Support-Confirmed Wallet Test Flow (Sandbox)
Based on ZaloPay support guidance for sandbox testing, complete these payer-side prerequisites:
1. Install sandbox wallet app for your device using the official test-wallet instructions page.
2. Register sandbox account (OTP default from support flow: 111111).
3. Set sandbox payment PIN (6 digits).
4. Complete identity verification flow in sandbox app (upload ID and selfie) if prompted.
5. Top up sandbox balance using the top-up instructions on the same test-wallet page.

Why this matters:
1. Even with a correct backend integration, checkout cannot complete unless the sandbox payer wallet is provisioned and funded.

## Step 2. Configure Service Settings
Update development config in [src/VehicleServiceBooking.Api/appsettings.Development.json](../../../src/VehicleServiceBooking.Api/appsettings.Development.json):

```json
"PaymentProviderGateway": {
  "UseDevelopmentGatewayFallback": false,
  "ZaloPay": {
    "Enabled": true,
    "BaseUrl": "https://sb-openapi.zalopay.vn",
    "CreateOrderPath": "/v2/create",
    "QueryOrderPath": "/v2/query",
    "AppId": 2553,
    "Key1": "<sandbox-key1>",
    "Key2": "<sandbox-key2>",
    "AppUserPrefix": "vsb",
    "CallbackUrl": "https://<public-host>/api/v1/payments/webhooks/ZaloPay",
    "RedirectUrl": "https://<public-host>/api/v1/payments/return",
    "IntentTtlMinutes": 15,
    "RequestTimeoutSeconds": 15
  }
}
```

Notes:
1. Set UseDevelopmentGatewayFallback to false while validating real provider path.
2. Keep Key1/Key2 outside committed files for shared environments.
3. If credentials were shared in email/chat, rotate sandbox keys before team-wide testing and store new keys in secure env storage.

## Step 3. Expose Local API via HTTPS Tunnel
ZaloPay callback cannot call localhost directly. Use one tunnel option.

### Option A: ngrok
Install:

```bash
brew install ngrok/ngrok/ngrok
```

Run tunnel (replace port with API port):

```bash
ngrok http 5280
```

### Option B: cloudflared
Install:

```bash
brew install cloudflared
```

Run tunnel:

```bash
cloudflared tunnel --url http://localhost:5280
```

Then update CallbackUrl and RedirectUrl in both:
1. ZaloPay merchant app settings.
2. Local appsettings.Development.json.

## Step 4. Run API
Run API from repo root:

```bash
dotnet run --project src/VehicleServiceBooking.Api/VehicleServiceBooking.Api.csproj
```

## Step 5. Create Business Order in System
You need a valid order before creating payment intent.

Use your existing order creation endpoint/workflow first.

If you need a reminder of payment API contracts, see:
1. [docs/FEATURES/PAYMENT/PAYMENT_API_CONTRACTS.md](./PAYMENT_API_CONTRACTS.md)

## Step 6. Create ZaloPay Payment Intent
Call:
1. POST /api/v1/orders/{orderId}/payments/intent

Reusable HTTP workflow script:
1. [tests/integration/http/payment_workflow_with_zalopay_gateway.http](../../../tests/integration/http/payment_workflow_with_zalopay_gateway.http)
2. [tests/integration/http/payment_workflow_with_zalopay_gateway_manual_callback.http](../../../tests/integration/http/payment_workflow_with_zalopay_gateway_manual_callback.http)

Request body template:

```json
{
  "paymentProviderId": "<zalopay-provider-guid>",
  "paymentMethodId": "<atm-domestic-method-guid>"
}
```

Known seed examples used in tests:
1. ZaloPay provider: 00000000-0000-0000-0003-000000000001
2. ATM domestic method: 00000000-0000-0000-0008-000000000001

If your environment differs, query lookup tables to confirm active IDs.

Expected response:
1. checkoutUrl returned.
2. checkoutQrPayload returned (current implementation mirrors checkoutUrl and is intended for client-side QR rendering).
3. intentCode returned.
4. New intent: HTTP 201, or reuse path: HTTP 200.

## Step 7. Render QR and Complete Checkout in ZaloPay Sandbox
1. Render checkoutQrPayload as QR code in client browser/app.
2. Scan QR with ZaloPay sandbox wallet, or open checkoutUrl directly.

Note:
1. Backend intentionally returns payment payload data only; QR image generation is handled on client side.

## Step 8. Complete Provider or Manual Callback
Preferred:
1. Let ZaloPay callback reach your webhook automatically.

Fallback:
1. Use manual callback workflow script when merchant callback routing is unavailable:
   - [tests/integration/http/payment_workflow_with_zalopay_gateway_manual_callback.http](../../../tests/integration/http/payment_workflow_with_zalopay_gateway_manual_callback.http)

Manual callback consistency rules:
1. Compute app_trans_id from the intent code: yyMMdd + '_' + first 24 chars of intentCode with '-' removed.
2. Keep intentCode in signed data and posted callback body identical.
3. Recompute MAC whenever app_time, amount, intentCode, or app_trans_id changes.

## Step 9. Verify Final State
Call:
1. GET /api/v1/orders/{orderId}/payments/status

Expected successful path:
1. PaymentTransactionStatus = Completed.
2. PaymentIntentStatus = Paid.
3. OrderPaymentStatus = Paid.

## Step 10. Verify Callback Acknowledgment Shape
ZaloPay callback endpoint responds with provider-style acknowledgment:
1. Success: return_code = 1
2. Duplicate: return_code = 2
3. Invalid mac: return_code = -1
4. Temporary/internal error: return_code = 0

For manual-callback testing, generate the mac with:
1. [scripts/zalopay_compute_callback_mac.sh](../../../scripts/zalopay_compute_callback_mac.sh)

## Step 11. Validated Scenario (Current)
Validated successfully in this workspace:
1. Full create-intent flow using real ZaloPay sandbox path.
2. Client-side QR rendering from checkoutQrPayload/checkoutUrl.
3. Manual webhook callback posting via:
   - [tests/integration/http/payment_workflow_with_zalopay_gateway_manual_callback.http](../../../tests/integration/http/payment_workflow_with_zalopay_gateway_manual_callback.http)
4. Final status reconciliation via payment status endpoint.

## Troubleshooting
1. Startup fails on options validation:
   - Check AppId, Key1, Key2, CallbackUrl, BaseUrl when ZaloPay is enabled.
2. No callback arrives:
   - Verify tunnel URL is active and HTTPS.
   - Recheck merchant Callback URL matches exactly.
3. Callback signature invalid:
   - Confirm Key2 value and no trailing spaces.
   - Ensure MAC is computed from the exact same `data` string that is posted.
   - Ensure `intentCode` in embed_data matches the callback payload field used for signing.
4. Query API failure:
   - Confirm BaseUrl and Key1 are sandbox values.
5. Intent still uses dev gateway:
   - Ensure UseDevelopmentGatewayFallback is false and ZaloPay.Enabled is true.

## Official References
1. Getting started: https://developers.zalopay.vn/en/v2/start/
2. API overview (create, callback, query): https://developers.zalopay.vn/en/v2/general/overview.html
3. Gateway API details: https://developers.zalopay.vn/v2/docs/gateway/api.html
4. App-to-App sandbox/demo downloads: https://developers.zalopay.vn/en/docs/apptoapp/demo.html#zalopay-sandbox
5. Sandbox test wallets and top-up instructions: https://docs.zalopay.vn/vi/docs/developer-tools/test-instructions/test-wallets
6. Integration guide intro: https://docs.zalopay.vn/vi/docs/guides/integration-guide/intro

## About the App-to-App Sandbox Link
The App-to-App sandbox page is useful for mobile App-to-App SDK testing (sandbox app downloads and demo SDK flows).

For this service's current implementation:
1. Primary flow is Website/Gateway API (`/v2/create`, callback, `/v2/query`).
2. App-to-App sandbox tools are optional and only needed if you also test mobile App-to-App model.

## Model Comparison (Quick View)
| Area | Gateway / Website (current service) | App-to-App sandbox tool |
|---|---|---|
| Primary use | Web checkout with server-side payment APIs | Native mobile SDK flow between merchant app and ZaloPay app |
| Current repo support | Implemented | Not implemented |
| Core APIs | `/v2/create`, callback (`data` + `mac`), `/v2/query` | App-to-App SDK and demo apps |
| Required for current backend validation | Yes | No |
| When to use | Backend payment integration testing in this service | Mobile app integration testing for App-to-App model |

## Current Out-of-Scope Follow-ups
These are tracked separately and can be added later:
1. Bank-list API integration (getlistmerchantbanks).
2. Redirect checksum validation on return endpoint.