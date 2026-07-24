#!/usr/bin/env bash
set -euo pipefail

usage() {
  cat <<'EOF'
Usage: ./scripts/zalopay_compute_callback_mac.sh --data '<json>' --key2 '<secret>'
       ./scripts/zalopay_compute_callback_mac.sh --data-file path/to/callback-data.json --key2 '<secret>'

Options:
  --data       Raw ZaloPay callback data JSON string (the value of the callback envelope 'data' field)
  --data-file  File containing the raw callback data JSON string
  --key2       ZaloPay callback key2 secret
  -h, --help   Show this help message

Output:
  Prints the lowercase hex HMAC SHA256 signature that should be sent as the callback envelope 'mac' field.
EOF
}

DATA=""
DATA_FILE=""
KEY2="${ZALOPAY_KEY2:-}"

while [[ $# -gt 0 ]]; do
  case "$1" in
    --data)
      DATA="${2:-}"
      shift 2
      ;;
    --data-file)
      DATA_FILE="${2:-}"
      shift 2
      ;;
    --key2)
      KEY2="${2:-}"
      shift 2
      ;;
    -h|--help)
      usage
      exit 0
      ;;
    *)
      echo "Error: unknown argument '$1'" >&2
      usage
      exit 2
      ;;
  esac
 done

if [[ -n "$DATA_FILE" ]]; then
  if [[ ! -f "$DATA_FILE" ]]; then
    echo "Error: data file not found: $DATA_FILE" >&2
    exit 2
  fi
  DATA="$(cat "$DATA_FILE")"
fi

if [[ -z "$DATA" ]]; then
  echo "Error: missing --data or --data-file" >&2
  usage
  exit 2
fi

if [[ -z "$KEY2" ]]; then
  echo "Error: missing --key2 (or ZALOPAY_KEY2 env var)" >&2
  usage
  exit 2
fi

if ! command -v openssl >/dev/null 2>&1; then
  echo "Error: openssl is required but not found" >&2
  exit 1
fi

printf '%s' "$DATA" | openssl dgst -sha256 -hmac "$KEY2" -binary | xxd -p -c 256
