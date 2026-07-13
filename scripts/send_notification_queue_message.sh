#!/usr/bin/env bash
set -euo pipefail

QUEUE_NAME="${QUEUE_NAME:-user-notification-events}"
TO_EMAIL="${TO_EMAIL:-haud.fin@gmail.com}"
SUBJECT="${SUBJECT:-Manual queue test}"
CONTENT="${CONTENT:-Manual message to debug Function trigger}"
EVENT_TYPE="${EVENT_TYPE:-Debug.Manual}"
SOURCE="${SOURCE:-manual-debug}"
CORRELATION_ID="${CORRELATION_ID:-manual-debug-$(date +%s)}"
RAW_PAYLOAD="${RAW_PAYLOAD:-}"

AZURITE_CONN_DEFAULT='DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;'
AZURE_STORAGE_CONNECTION_STRING="${AZURE_STORAGE_CONNECTION_STRING:-$AZURITE_CONN_DEFAULT}"

if ! command -v az >/dev/null 2>&1; then
  echo "az CLI is required but not found." >&2
  exit 1
fi

if [[ -z "$RAW_PAYLOAD" ]]; then
  if ! command -v jq >/dev/null 2>&1; then
    echo "jq is required when RAW_PAYLOAD is not provided." >&2
    exit 1
  fi

  PAYLOAD="$(jq -nc \
    --arg eventType "$EVENT_TYPE" \
    --arg toEmail "$TO_EMAIL" \
    --arg subject "$SUBJECT" \
    --arg content "$CONTENT" \
    --arg correlationId "$CORRELATION_ID" \
    --arg source "$SOURCE" \
    --arg occurredAtUtc "$(date -u +%Y-%m-%dT%H:%M:%SZ)" \
    '{EventType:$eventType,ToEmail:$toEmail,Subject:$subject,Content:$content,CorrelationId:$correlationId,Source:$source,OccurredAtUtc:$occurredAtUtc}')"
else
  PAYLOAD="$RAW_PAYLOAD"
fi

az storage queue create \
  --name "$QUEUE_NAME" \
  --connection-string "$AZURE_STORAGE_CONNECTION_STRING" \
  --only-show-errors \
  -o none

az storage message put \
  --queue-name "$QUEUE_NAME" \
  --content "$PAYLOAD" \
  --connection-string "$AZURE_STORAGE_CONNECTION_STRING" \
  --only-show-errors \
  -o json
