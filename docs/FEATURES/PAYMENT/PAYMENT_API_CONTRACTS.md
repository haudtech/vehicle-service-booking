# Payment API Contracts

## Purpose
This document defines input validation, authorization, and error response contracts for payment-related APIs.

## Error Response Shape
Unless otherwise noted, error responses follow this structure:

```json
{
  "message": "Human-readable explanation",
  "errorCode": "MACHINE_READABLE_CODE",
  "timestamp": "2026-07-21T10:00:00Z"
}
```

## 1) Create Payment Intent
Endpoint:
- POST /api/v1/orders/{orderId}/payments/intent

Authorization:
- Required.
- Policy: OrderEditPolicy
- Expected permission claim: order:edit

Idempotency:
- Supports Idempotency-Key header (recommended for all create attempts).
- Request path key scope: POST:/api/v1/orders/{orderId}/payments/intent
- Replay outcomes:
  - same key + same payload -> stored 200/201 response replay
  - same key + different payload -> 409 IDEMPOTENCY_KEY_REUSED
  - in-progress duplicate -> 409 IDEMPOTENCY_IN_PROGRESS

Request body:
```json
{
  "paymentProviderId": "00000000-0000-0000-0003-000000000001",
  "paymentMethodId": "00000000-0000-0000-0008-000000000001"
}
```

Validation rules:
- orderId route value must not be empty GUID.
- paymentProviderId is required and not empty.
- paymentMethodId is required and not empty.

Success responses:
- 201 Created when a new intent is created.
- 200 OK when an existing open non-expired intent is reused.

Error responses:
- 400 INVALID_ORDER_ID when orderId is empty.
- 400 IDEMPOTENCY_KEY_REQUIRED when idempotency header is required by config but missing.
- 400 IDEMPOTENCY_KEY_INVALID when idempotency key exceeds configured max length.
- 400 from request validator:
  - INVALID_PAYMENT_PROVIDER_ID
  - INVALID_PAYMENT_METHOD_ID
- 400 INVALID_OPERATION for domain violations, such as:
  - order not found
  - order in paid/cancelled/refunded state
  - provider or method not found/inactive
- 409 IDEMPOTENCY_KEY_REUSED when same key is used with different payload.
- 409 IDEMPOTENCY_IN_PROGRESS when same key request is currently being processed.

Notes:
- Response includes checkoutUrl and intentCode.
- Created Location points to payment status resource for the order.

## 2) Get Payment Status
Endpoint:
- GET /api/v1/orders/{orderId}/payments/status

Authorization:
- Required.
- Policy: OrderViewPolicy
- Expected permission claim: order:view

Validation rules:
- orderId route value must not be empty GUID.

Success response:
- 200 OK with payment snapshot for order, payment order, and transaction status.

Error responses:
- 400 INVALID_ORDER_ID when orderId is empty.
- 404 ORDER_NOT_FOUND when order is missing.
- 400 INVALID_OPERATION for data linkage violations.

## 3) Ingest Payment Webhook
Endpoint:
- POST /api/v1/payments/webhooks/{providerType}

Authorization:
- AllowAnonymous (provider callback endpoint).

Request body:
```json
{
  "eventId": "evt-123",
  "intentCode": "PI-20260721110000-xxxx",
  "transactionStatus": "Completed",
  "providerTransactionId": "provider-tx-001",
  "signatureHash": "HEX_SIGNATURE",
  "payload": "{\"raw\":\"provider-payload\"}",
  "occurredAtUtc": "2026-07-21T11:00:00Z"
}
```

Validation rules:
- eventId is required.
- intentCode is required.
- transactionStatus is required and must map to PaymentTransactionStatus enum.
- signatureHash is required by validator.

Security behavior:
- If PaymentWebhookSecurity.Enabled and RequireSignature are true, signature is verified via HMAC SHA256 canonical message.

Success response:
- 200 OK with ProcessPaymentWebhookResponse.

Error responses:
- 400 validator/domain failures:
  - INVALID_EVENT_ID
  - INVALID_INTENT_CODE
  - INVALID_TRANSACTION_STATUS
  - UNSUPPORTED_TRANSACTION_STATUS
  - INVALID_SIGNATURE_HASH
  - INVALID_OPERATION
- 401 INVALID_SIGNATURE when HMAC verification fails.

Notes:
- Endpoint is idempotent by provider + eventId dedupe through webhook inbox persistence.

## 4) Payment Return Endpoint
Endpoint:
- GET /api/v1/payments/return

Authorization:
- AllowAnonymous.

Query parameters (all optional):
- intentCode
- transactionStatus
- providerTransactionId
- eventId

Success response:
- 200 OK acknowledgment payload.

Notes:
- This endpoint is UX-oriented redirect handling only.
- Final payment state is determined by webhook processing, not by this endpoint.

## 5) Development Mock Checkout Endpoints
Endpoint A:
- GET /api/v1/payments/mock/checkout/{providerType}/{intentCode}

Authorization:
- AllowAnonymous.

Behavior:
- Returns HTML page for development simulation.

Endpoint B:
- POST /api/v1/payments/mock/checkout/{providerType}/{intentCode}/complete

Authorization:
- AllowAnonymous.

Request format:
- form field: transactionStatus

Validation rules:
- intentCode is required.
- transactionStatus is required.
- generated webhook payload still passes ProcessPaymentWebhookRequest validator.

Success response:
- 200 OK with ProcessPaymentWebhookResponse.

Error responses:
- 400 INVALID_INTENT_CODE
- 400 INVALID_TRANSACTION_STATUS
- 400 validation/domain INVALID_OPERATION

Notes:
- If webhook signature enforcement is enabled, endpoint computes signature using configured shared secret before dispatching to webhook service.
