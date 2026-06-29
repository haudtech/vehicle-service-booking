#!/usr/bin/env bash
set -euo pipefail

##############################################################################
# Vehicle Service Booking API - Automated Test Suite
#
# Developer note:
# - This script validates end-to-end API behavior against a real PostgreSQL DB.
# - It captures reproducible request/response evidence under src/VehicleServiceBooking.Api/logs/api-tests/.
# - It mixes deterministic static IDs (for basic checks) with dynamic DB lookup
#   (for concurrency/idempotency scenarios that need currently available slots).
##############################################################################

GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"

# API endpoint under test.
# Override example:
#   API_URL="http://localhost:5280" ./scripts/run_api_tests.sh
#   API_URL="http://localhost:5280/api/v1" ./scripts/run_api_tests.sh
#   ./scripts/run_api_tests.sh --port 5280
#   ./scripts/run_api_tests.sh --url http://localhost:5280
API_URL="${API_URL:-http://localhost:5280}"

usage() {
    cat <<'EOF'
Usage: ./scripts/run_api_tests.sh [options]

Options:
  -p, --port <port>   Use localhost with this port (for example: --port 5280)
  -u, --url  <url>    Base API URL (for example: --url http://localhost:5280)
  -h, --help          Show this help message

Notes:
  - If both API_URL env var and CLI options are provided, CLI options win.
  - /api/v1 is appended automatically when missing.
EOF
}

parse_args() {
    while [[ $# -gt 0 ]]; do
        case "$1" in
            -p|--port)
                if [[ -z "${2:-}" ]]; then
                    echo "Error: missing value for $1" >&2
                    usage
                    exit 2
                fi
                API_URL="http://localhost:$2"
                shift 2
                ;;
            -u|--url)
                if [[ -z "${2:-}" ]]; then
                    echo "Error: missing value for $1" >&2
                    usage
                    exit 2
                fi
                API_URL="$2"
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
}

parse_args "$@"

# Normalize value to always include /api/v1 once.
if [[ "$API_URL" != */api/v1 ]]; then
    API_URL="${API_URL%/}/api/v1"
fi

# Evidence output location (one folder per date/run id).
# You can override these env vars to replay into a known evidence folder.
EVIDENCE_DATE="${EVIDENCE_DATE:-$(date +%F)}"
EVIDENCE_RUN_ID="${EVIDENCE_RUN_ID:-$(date +%Y%m%d_%H%M%S)}"
EVIDENCE_DIR="${ROOT_DIR}/src/VehicleServiceBooking.Api/logs/api-tests/${EVIDENCE_DATE}/${EVIDENCE_RUN_ID}"

# Baseline IDs used for happy-path coverage. For some tests, these may be
# replaced by dynamic IDs selected from ServiceTypeAvailability view.
DEALERSHIP_ID="11111111-1111-1111-1111-000000000001"
SERVICE_TYPE_ID="11111111-1111-1111-1111-030000000001"
CUSTOMER_ID="11111111-1111-1111-1111-010000000003"
VEHICLE_ID="11111111-1111-1111-1111-020000000003"
TECHNICIAN_ID="11111111-1111-1111-1111-040000000001"
SERVICE_BAY_ID="11111111-1111-1111-1111-050000000001"
START_SLOT_ID="00000000-0000-0000-0000-000000000007"
END_SLOT_ID="00000000-0000-0000-0000-000000000008"

DB_HOST="${DB_HOST:-localhost}"
DB_PORT="${DB_PORT:-5432}"
DB_NAME="${DB_NAME:-vehicle_service_booking}"
DB_USER="${DB_USER:-haudo}"
DB_PASSWORD="${DB_PASSWORD:-<password>}"

load_db_config_from_env_file() {
    # Prefer explicit DB_* environment variables. If not provided, parse .env
    # connection string so this script stays aligned with real local settings.
    if [[ -n "${DB_HOST:-}" && -n "${DB_PORT:-}" && -n "${DB_NAME:-}" && -n "${DB_USER:-}" && -n "${DB_PASSWORD:-}" ]]; then
        return
    fi

    local env_file="$ROOT_DIR/.env"
    if [[ ! -f "$env_file" ]]; then
        return
    fi

    local conn_line conn
    conn_line=$(grep -E '^CONNECTIONSTRINGS__DEFAULTCONNECTION=' "$env_file" | head -1 || true)
    if [[ -z "$conn_line" ]]; then
        return
    fi

    conn="${conn_line#CONNECTIONSTRINGS__DEFAULTCONNECTION=}"

    local old_ifs="$IFS"
    IFS=';'
    read -ra parts <<< "$conn"
    IFS="$old_ifs"

    local part key value key_lc
    for part in "${parts[@]}"; do
        key="${part%%=*}"
        value="${part#*=}"
        key_lc="$(printf '%s' "$key" | tr '[:upper:]' '[:lower:]')"

        case "$key_lc" in
            host)
                DB_HOST="${DB_HOST:-$value}"
                ;;
            port)
                DB_PORT="${DB_PORT:-$value}"
                ;;
            database)
                DB_NAME="${DB_NAME:-$value}"
                ;;
            username|user\ id|userid|user)
                DB_USER="${DB_USER:-$value}"
                ;;
            password)
                DB_PASSWORD="${DB_PASSWORD:-$value}"
                ;;
        esac
    done
}

# Counters printed in final summary.
TESTS_PASSED=0
TESTS_FAILED=0
TESTS_SKIPPED=0
TARGET_DATE=""
APPOINTMENT_ID=""
HAS_AVAILABILITY_DATA="unknown"

print_header() {
    echo -e "\n${BLUE}============================================================${NC}"
    echo -e "${BLUE}$1${NC}"
    echo -e "${BLUE}============================================================${NC}\n"
}

print_test() {
    echo -e "${YELLOW}TEST:${NC} $1"
}

pass() {
    echo -e "${GREEN}PASS:${NC} $1"
    TESTS_PASSED=$((TESTS_PASSED + 1))
}

fail() {
    echo -e "${RED}FAIL:${NC} $1"
    TESTS_FAILED=$((TESTS_FAILED + 1))
}

skip() {
    echo -e "${YELLOW}SKIP:${NC} $1"
    TESTS_SKIPPED=$((TESTS_SKIPPED + 1))
}

info() {
    echo -e "${BLUE}INFO:${NC} $1"
}

http_get() {
    # Wrapper keeps curl style consistent and makes test steps easier to read.
    local url="$1"
    local body_file="$2"
    curl -sS -o "$body_file" -w "%{http_code}" "$url"
}

http_post_json() {
    # Wrapper for JSON POST calls with consistent output capture.
    local url="$1"
    local payload="$2"
    local body_file="$3"
    curl -sS -o "$body_file" -w "%{http_code}" -X POST "$url" -H 'Content-Type: application/json' -d "$payload"
}

resolve_target_date() {
    # Finds the nearest future date (within 30 days) where availability exists.
    # This reduces false failures caused by testing on a date without slots.
    if [[ -n "${TARGET_DATE:-}" ]]; then
        return
    fi

    local offset candidate_date code body
    local tmp_body="${EVIDENCE_DIR}/_target_date_probe.json"

    for offset in {1..30}; do
        candidate_date=$(date -v+"${offset}"d +%F)
        code=$(curl -sS -o "$tmp_body" -w "%{http_code}" "$API_URL/availability?dealershipId=$DEALERSHIP_ID&serviceTypeId=$SERVICE_TYPE_ID&date=$candidate_date") || true
        body=$(cat "$tmp_body" 2>/dev/null || true)

        # Availability endpoint can return 200 with empty array; require slot data.
        if [[ "$code" == "200" ]] && grep -q 'slotStart' <<< "$body"; then
            TARGET_DATE="$candidate_date"
            info "Using target date with availability: $TARGET_DATE"
            return
        fi
    done

    TARGET_DATE=$(date -v+1d +%F)
    info "No availability found in next 30 days; falling back to $TARGET_DATE"
}

fetch_booking_candidate_row() {
    # Returns one currently valid candidate booking row from ServiceTypeAvailability.
    # Each call re-queries the view so earlier test bookings naturally move the candidate forward.
    if ! command -v psql >/dev/null 2>&1; then
        return 1
    fi

    PGPASSWORD="$DB_PASSWORD" psql -P pager=off -At -F '|' -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" -c "
WITH candidate AS (
    SELECT
        sva.\"DealershipId\",
        sva.\"ServiceTypeId\",
        sva.\"TechnicianId\",
        sva.\"ServiceBayId\",
        sva.\"TimeSlotId\" AS start_slot_id,
        sva.\"RequiredSlots\",
        sva.\"SequenceOrder\",
        sva.\"QueryDate\"
    FROM \"ServiceTypeAvailability\" sva
    WHERE sva.\"CanFitService\" = TRUE
      AND sva.\"QueryDate\" >= CURRENT_DATE + INTERVAL '1 day'
    ORDER BY sva.\"QueryDate\", sva.\"SequenceOrder\"
    LIMIT 1
)
SELECT
    c.\"DealershipId\",
    c.\"ServiceTypeId\",
    c.\"TechnicianId\",
    c.\"ServiceBayId\",
    c.start_slot_id,
    ts_end.\"Id\" AS end_slot_id,
    c.\"QueryDate\"::text AS appointment_date,
    cust.\"Id\" AS customer_id,
    veh.\"Id\" AS vehicle_id
FROM candidate c
JOIN \"TimeSlots\" ts_end ON ts_end.\"SequenceOrder\" = c.\"SequenceOrder\" + c.\"RequiredSlots\" - 1
JOIN \"Customers\" cust ON TRUE
JOIN \"Vehicles\" veh ON veh.\"CustomerId\" = cust.\"Id\"
LIMIT 1;"
}

detect_availability_data() {
    # Data-dependent scenarios need at least one row in ServiceTypeAvailability.
    if ! command -v psql >/dev/null 2>&1; then
        HAS_AVAILABILITY_DATA="unknown"
        info "psql not available; cannot pre-check ServiceTypeAvailability"
        return
    fi

    local count
    count=$(PGPASSWORD="$DB_PASSWORD" psql -P pager=off -At -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" -c "SELECT COUNT(*) FROM \"ServiceTypeAvailability\";" 2>/dev/null || echo "0")

    if [[ "${count:-0}" =~ ^[0-9]+$ ]] && [[ "$count" -gt 0 ]]; then
        HAS_AVAILABILITY_DATA="true"
        info "ServiceTypeAvailability rows detected: $count"
    else
        HAS_AVAILABILITY_DATA="false"
        info "ServiceTypeAvailability has no rows; booking-flow tests will be skipped"
    fi
}

require_availability_data() {
    if [[ "$HAS_AVAILABILITY_DATA" != "true" ]]; then
        skip "$1 (ServiceTypeAvailability is empty in current DB)"
        return 1
    fi

    return 0
}

check_api_reachable() {
    # Quick fail-fast check so we don't run long tests when API is down.
    print_test "API Reachability"
    local code
    code=$(curl -sS -o /dev/null -w "%{http_code}" "$API_URL/availability") || true

    if [[ "$code" == "200" || "$code" == "400" ]]; then
        pass "API is reachable on $API_URL (status $code)"
    else
        fail "API not reachable on $API_URL (status $code)"
        echo "Start API with:"
        echo "cd src/VehicleServiceBooking.Api"
        echo "ASPNETCORE_URLS='http://localhost:5280' ConnectionStrings__DefaultConnection='Host=$DB_HOST;Port=$DB_PORT;Database=$DB_NAME;Username=$DB_USER;Password=<your-password>;' dotnet run --no-launch-profile"
        exit 1
    fi
}

test_happy_get_availability() {
    print_test "Happy Case 1 - GET /availability"

    if ! require_availability_data "Happy Case 1 - GET /availability"; then
        return
    fi

    local req_url code
    local req_file="${EVIDENCE_DIR}/01_get_availability.request.log"
    local status_file="${EVIDENCE_DIR}/01_get_availability.status.log"
    local body_file="${EVIDENCE_DIR}/01_get_availability.response.json"

    resolve_target_date
    req_url="$API_URL/availability?dealershipId=$DEALERSHIP_ID&serviceTypeId=$SERVICE_TYPE_ID&date=$TARGET_DATE"
    printf '%s\n' "$req_url" > "$req_file"

    code=$(http_get "$req_url" "$body_file")
    printf '%s\n' "$code" > "$status_file"

    if [[ "$code" != "200" ]]; then
        fail "Expected HTTP 200, got $code"
        info "Body: $(cat "$body_file")"
        return
    fi

    if grep -q 'slotStart' "$body_file" && grep -q 'technicianId' "$body_file"; then
        pass "Availability returned HTTP 200 with expected fields"
    else
        fail "Availability response missing expected fields"
        info "Body: $(cat "$body_file")"
    fi
}

test_happy_post_appointment() {
    print_test "Happy Case 2 - POST /appointments"

    if ! require_availability_data "Happy Case 2 - POST /appointments"; then
        APPOINTMENT_ID=""
        return
    fi

    local payload code
    local selected_service_type_id="$SERVICE_TYPE_ID"
    local selected_technician_id="$TECHNICIAN_ID"
    local selected_service_bay_id="$SERVICE_BAY_ID"
    local selected_start_slot_id="$START_SLOT_ID"
    local selected_end_slot_id="$END_SLOT_ID"
    local body_file="${EVIDENCE_DIR}/02_post_appointment.response.json"
    local req_file="${EVIDENCE_DIR}/02_post_appointment.request.json"
    local status_file="${EVIDENCE_DIR}/02_post_appointment.status.log"

    resolve_target_date

    # Prefer DB-driven candidate selection for the chosen date so this test stays
    # stable even when static slot IDs become unavailable over time.
    if command -v psql >/dev/null 2>&1; then
        local slot_row
        slot_row=$(PGPASSWORD="$DB_PASSWORD" psql -P pager=off -At -F '|' -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" -c "
SELECT
  sta.\"ServiceTypeId\",
  sta.\"TechnicianId\",
  sta.\"ServiceBayId\",
  sta.\"TimeSlotId\",
  ts_end.\"Id\" AS end_slot_id
FROM \"ServiceTypeAvailability\" sta
JOIN \"TimeSlots\" ts_end ON ts_end.\"SequenceOrder\" = sta.\"SequenceOrder\" + sta.\"RequiredSlots\" - 1
WHERE sta.\"DealershipId\" = '$DEALERSHIP_ID'
  AND sta.\"ServiceTypeId\" = '$SERVICE_TYPE_ID'
  AND sta.\"QueryDate\" = '$TARGET_DATE'::date
LIMIT 1;")

        if [[ -n "$slot_row" ]]; then
            IFS='|' read -r selected_service_type_id selected_technician_id selected_service_bay_id selected_start_slot_id selected_end_slot_id <<< "$slot_row"
            info "Using dynamic slot combo from DB view for POST happy case"
        else
            info "No dynamic slot found from DB view; falling back to static slot IDs"
        fi
    else
        info "psql not available; using static slot IDs"
    fi

    payload=$(cat <<JSON
{
    "dealershipId": "$DEALERSHIP_ID",
    "customerId": "$CUSTOMER_ID",
    "vehicleId": "$VEHICLE_ID",
    "appointmentDate": "$TARGET_DATE",
    "serviceTypeId": "$selected_service_type_id",
    "technicianId": "$selected_technician_id",
    "serviceBayId": "$selected_service_bay_id",
    "estimatedStartTimeSlotId": "$selected_start_slot_id",
    "estimatedEndTimeSlotId": "$selected_end_slot_id"
}
JSON
)
    printf '%s\n' "$payload" > "$req_file"

    code=$(http_post_json "$API_URL/appointments" "$payload" "$body_file")
    printf '%s\n' "$code" > "$status_file"

    if [[ "$code" != "201" ]]; then
        fail "Expected HTTP 201, got $code"
        info "Body: $(cat "$body_file")"
        APPOINTMENT_ID=""
        return
    fi

    APPOINTMENT_ID=$(sed -n 's/.*"appointmentId":"\([^"]*\)".*/\1/p' "$body_file")
    if [[ -n "${APPOINTMENT_ID:-}" ]]; then
        pass "Appointment created with HTTP 201 (appointmentId=$APPOINTMENT_ID)"
    else
        fail "HTTP 201 received but appointmentId was not found in response"
        info "Body: $(cat "$body_file")"
    fi
}

test_idempotency_missing_header_create_appointment() {
    print_test "Idempotency Case 7 - Missing Idempotency-Key should still create appointment"

    if ! require_availability_data "Idempotency Case 7 - Missing Idempotency-Key should still create appointment"; then
        return
    fi

    local req_file="${EVIDENCE_DIR}/07_idempotency_missing_header.request.json"
    local status_file="${EVIDENCE_DIR}/07_idempotency_missing_header.status.log"
    local body_file="${EVIDENCE_DIR}/07_idempotency_missing_header.response.json"

    if ! command -v psql >/dev/null 2>&1; then
        fail "psql not available; cannot run idempotency missing-header DB-backed scenario"
        return
    fi

    resolve_target_date

    local candidate_row
    candidate_row=$(fetch_booking_candidate_row)

    if [[ -z "$candidate_row" ]]; then
        fail "No candidate row found in ServiceTypeAvailability for idempotency missing-header scenario"
        return
    fi

    local dealership_id service_type_id technician_id service_bay_id start_slot_id end_slot_id appointment_date customer_id vehicle_id
    IFS='|' read -r dealership_id service_type_id technician_id service_bay_id start_slot_id end_slot_id appointment_date customer_id vehicle_id <<< "$candidate_row"

    local payload
    payload=$(cat <<JSON
{
    "dealershipId": "$dealership_id",
    "customerId": "$customer_id",
    "vehicleId": "$vehicle_id",
    "appointmentDate": "$appointment_date",
    "serviceTypeId": "$service_type_id",
    "technicianId": "$technician_id",
    "serviceBayId": "$service_bay_id",
    "estimatedStartTimeSlotId": "$start_slot_id",
    "estimatedEndTimeSlotId": "$end_slot_id"
}
JSON
)
    printf '%s\n' "$payload" > "$req_file"

    local code
    code=$(http_post_json "$API_URL/appointments" "$payload" "$body_file")
    printf '%s\n' "$code" > "$status_file"

    if [[ "$code" != "201" ]]; then
        fail "Expected HTTP 201 with missing Idempotency-Key, got $code"
        info "Body: $(cat "$body_file")"
        return
    fi

    local appointment_id
    appointment_id=$(sed -n 's/.*"appointmentId":"\([^"]*\)".*/\1/p' "$body_file")
    if [[ -n "$appointment_id" ]]; then
        pass "Appointment created successfully without Idempotency-Key (appointmentId=$appointment_id)"
    else
        fail "HTTP 201 received but appointmentId was not found for missing-header idempotency case"
        info "Body: $(cat "$body_file")"
    fi
}

test_idempotency_header_present_create_appointment() {
    print_test "Idempotency Case 8 - Present Idempotency-Key should create appointment"

    if ! require_availability_data "Idempotency Case 8 - Present Idempotency-Key should create appointment"; then
        return
    fi

    local req_file="${EVIDENCE_DIR}/08_idempotency_header_present.request.json"
    local status_file="${EVIDENCE_DIR}/08_idempotency_header_present.status.log"
    local body_file="${EVIDENCE_DIR}/08_idempotency_header_present.response.json"

    if ! command -v psql >/dev/null 2>&1; then
        fail "psql not available; cannot run idempotency header-present DB-backed scenario"
        return
    fi

    resolve_target_date

    local candidate_row
    candidate_row=$(fetch_booking_candidate_row)

    if [[ -z "$candidate_row" ]]; then
        fail "No candidate row found in ServiceTypeAvailability for idempotency header-present scenario"
        return
    fi

    local dealership_id service_type_id technician_id service_bay_id start_slot_id end_slot_id appointment_date customer_id vehicle_id
    IFS='|' read -r dealership_id service_type_id technician_id service_bay_id start_slot_id end_slot_id appointment_date customer_id vehicle_id <<< "$candidate_row"

    local payload idem_key
    idem_key="idem-${EVIDENCE_RUN_ID}-present-$(date +%s)"
    payload=$(cat <<JSON
{
    "dealershipId": "$dealership_id",
    "customerId": "$customer_id",
    "vehicleId": "$vehicle_id",
    "appointmentDate": "$appointment_date",
    "serviceTypeId": "$service_type_id",
    "technicianId": "$technician_id",
    "serviceBayId": "$service_bay_id",
    "estimatedStartTimeSlotId": "$start_slot_id",
    "estimatedEndTimeSlotId": "$end_slot_id"
}
JSON
)
    printf '%s\n' "$payload" > "$req_file"

    local code
    code=$(curl -sS -o "$body_file" -w "%{http_code}" -X POST "$API_URL/appointments" \
        -H 'Content-Type: application/json' \
        -H "Idempotency-Key: $idem_key" \
        -d "$payload")
    printf '%s\n' "$code" > "$status_file"

    if [[ "$code" != "201" ]]; then
        fail "Expected HTTP 201 with Idempotency-Key, got $code"
        info "Body: $(cat "$body_file")"
        return
    fi

    local appointment_id
    appointment_id=$(sed -n 's/.*"appointmentId":"\([^"]*\)".*/\1/p' "$body_file")
    if [[ -n "$appointment_id" ]]; then
        pass "Appointment created successfully with Idempotency-Key (appointmentId=$appointment_id)"
    else
        fail "HTTP 201 received but appointmentId was not found for header-present idempotency case"
        info "Body: $(cat "$body_file")"
    fi
}

test_happy_get_appointment_by_id() {
    print_test "Happy Case 3 - GET /appointments/{id}"

    if [[ -z "${APPOINTMENT_ID:-}" ]]; then
        skip "Happy Case 3 depends on POST success (no appointmentId available)"
        return
    fi

    local req_url code
    local req_file="${EVIDENCE_DIR}/03_get_appointment_by_id.request.log"
    local status_file="${EVIDENCE_DIR}/03_get_appointment_by_id.status.log"
    local body_file="${EVIDENCE_DIR}/03_get_appointment_by_id.response.json"

    req_url="$API_URL/appointments/$APPOINTMENT_ID"
    printf '%s\n' "$req_url" > "$req_file"

    code=$(http_get "$req_url" "$body_file")
    printf '%s\n' "$code" > "$status_file"

    if [[ "$code" != "200" ]]; then
        fail "Expected HTTP 200, got $code"
        info "Body: $(cat "$body_file")"
        return
    fi

    if grep -q "$APPOINTMENT_ID" "$body_file"; then
        pass "Appointment retrieved successfully by ID"
    else
        fail "GET by id returned 200 but appointmentId mismatch"
        info "Body: $(cat "$body_file")"
    fi
}

test_invalid_guid_validation() {
    print_test "Validation Case 4 - Invalid GUID"
    local req_url code
    local req_file="${EVIDENCE_DIR}/04_invalid_guid.request.log"
    local status_file="${EVIDENCE_DIR}/04_invalid_guid.status.log"
    local body_file="${EVIDENCE_DIR}/04_invalid_guid.response.json"

    req_url="$API_URL/availability?dealershipId=invalid&serviceTypeId=invalid&date=2026-01-01"
    printf '%s\n' "$req_url" > "$req_file"

    code=$(http_get "$req_url" "$body_file")
    printf '%s\n' "$code" > "$status_file"

    if [[ "$code" == "400" ]]; then
        pass "Invalid GUID request rejected with HTTP 400"
    else
        fail "Expected HTTP 400, got $code"
        info "Body: $(cat "$body_file")"
    fi
}

test_past_date_validation() {
    print_test "Validation Case 5 - Past Date"
    local req_url code
    local req_file="${EVIDENCE_DIR}/05_past_date.request.log"
    local status_file="${EVIDENCE_DIR}/05_past_date.status.log"
    local body_file="${EVIDENCE_DIR}/05_past_date.response.json"

    req_url="$API_URL/availability?dealershipId=$DEALERSHIP_ID&serviceTypeId=$SERVICE_TYPE_ID&date=2020-01-01"
    printf '%s\n' "$req_url" > "$req_file"

    code=$(http_get "$req_url" "$body_file")
    printf '%s\n' "$code" > "$status_file"

    if [[ "$code" == "400" ]]; then
        pass "Past date request rejected with HTTP 400"
    else
        fail "Expected HTTP 400, got $code"
        info "Body: $(cat "$body_file")"
    fi
}

test_concurrency_overlap_conflict() {
    print_test "Concurrency Case 6 - Parallel overlap should yield 201 + 409 BOOKING_CONFLICT"

    if ! require_availability_data "Concurrency Case 6 - Parallel overlap should yield 201 + 409 BOOKING_CONFLICT"; then
        return
    fi

    local req_file="${EVIDENCE_DIR}/06_concurrency_overlap.request.json"
    local status_file="${EVIDENCE_DIR}/06_concurrency_overlap.status.log"
    local body_a_file="${EVIDENCE_DIR}/06_concurrency_overlap.response_a.json"
    local body_b_file="${EVIDENCE_DIR}/06_concurrency_overlap.response_b.json"
    local code_a_file="${EVIDENCE_DIR}/06_concurrency_overlap.code_a.log"
    local code_b_file="${EVIDENCE_DIR}/06_concurrency_overlap.code_b.log"

    if ! command -v psql >/dev/null 2>&1; then
        fail "psql not available; cannot run concurrency overlap DB-backed scenario"
        return
    fi

    # Pick one currently valid candidate window from the DB view, then send two
    # parallel create requests with the exact same payload to simulate a race.
    local candidate_row
    candidate_row=$(PGPASSWORD="$DB_PASSWORD" psql -P pager=off -At -F '|' -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" -c "
WITH candidate AS (
    SELECT
        sva.\"DealershipId\",
        sva.\"ServiceTypeId\",
        sva.\"TechnicianId\",
        sva.\"ServiceBayId\",
        sva.\"TimeSlotId\" AS start_slot_id,
        sva.\"RequiredSlots\",
        sva.\"SequenceOrder\",
        sva.\"QueryDate\"
    FROM \"ServiceTypeAvailability\" sva
    WHERE sva.\"CanFitService\" = TRUE
      AND sva.\"QueryDate\" >= CURRENT_DATE + INTERVAL '1 day'
    ORDER BY sva.\"QueryDate\", sva.\"SequenceOrder\"
    LIMIT 1
)
SELECT
    c.\"DealershipId\",
    c.\"ServiceTypeId\",
    c.\"TechnicianId\",
    c.\"ServiceBayId\",
    c.start_slot_id,
    ts_end.\"Id\" AS end_slot_id,
    c.\"QueryDate\"::text AS appointment_date,
    cust.\"Id\" AS customer_id,
    veh.\"Id\" AS vehicle_id
FROM candidate c
JOIN \"TimeSlots\" ts_end ON ts_end.\"SequenceOrder\" = c.\"SequenceOrder\" + c.\"RequiredSlots\" - 1
JOIN \"Customers\" cust ON TRUE
JOIN \"Vehicles\" veh ON veh.\"CustomerId\" = cust.\"Id\"
LIMIT 1;")

    if [[ -z "$candidate_row" ]]; then
        fail "No candidate row found in ServiceTypeAvailability for concurrency scenario"
        return
    fi

    local dealership_id service_type_id technician_id service_bay_id start_slot_id end_slot_id appointment_date customer_id vehicle_id
    IFS='|' read -r dealership_id service_type_id technician_id service_bay_id start_slot_id end_slot_id appointment_date customer_id vehicle_id <<< "$candidate_row"

    local payload
    payload=$(cat <<JSON
{
    "dealershipId": "$dealership_id",
    "customerId": "$customer_id",
    "vehicleId": "$vehicle_id",
    "appointmentDate": "$appointment_date",
    "serviceTypeId": "$service_type_id",
    "technicianId": "$technician_id",
    "serviceBayId": "$service_bay_id",
    "estimatedStartTimeSlotId": "$start_slot_id",
    "estimatedEndTimeSlotId": "$end_slot_id"
}
JSON
)
    printf '%s\n' "$payload" > "$req_file"

    # Run both requests concurrently; one should win, one should conflict.
    (curl -sS -o "$body_a_file" -w "%{http_code}" -X POST "$API_URL/appointments" -H 'Content-Type: application/json' -d "$payload" > "$code_a_file") &
    (curl -sS -o "$body_b_file" -w "%{http_code}" -X POST "$API_URL/appointments" -H 'Content-Type: application/json' -d "$payload" > "$code_b_file") &
    wait

    local code_a code_b
    code_a=$(cat "$code_a_file")
    code_b=$(cat "$code_b_file")

    {
        echo "candidate=$candidate_row"
        echo "code_a=$code_a"
        echo "code_b=$code_b"
    } > "$status_file"

    local has_201_and_409="false"
    if [[ "$code_a" == "201" && "$code_b" == "409" ]]; then
        has_201_and_409="true"
    elif [[ "$code_a" == "409" && "$code_b" == "201" ]]; then
        has_201_and_409="true"
    fi

    if [[ "$has_201_and_409" != "true" ]]; then
        fail "Expected one 201 and one 409, got code_a=$code_a code_b=$code_b"
        info "Body A: $(cat "$body_a_file")"
        info "Body B: $(cat "$body_b_file")"
        return
    fi

    local conflict_body
    if [[ "$code_a" == "409" ]]; then
        conflict_body=$(cat "$body_a_file")
    else
        conflict_body=$(cat "$body_b_file")
    fi

    if grep -q '"errorCode":"BOOKING_CONFLICT"' <<< "$conflict_body"; then
        pass "Parallel overlap produced expected 201 + 409 BOOKING_CONFLICT"
    else
        fail "Received 409 but missing BOOKING_CONFLICT errorCode"
        info "Conflict body: $conflict_body"
    fi
}

test_idempotency_duplicate_retry() {
    print_test "Idempotency Case 9 - Same key and same payload should replay"

    if ! require_availability_data "Idempotency Case 9 - Same key and same payload should replay"; then
        return
    fi

    local req_file="${EVIDENCE_DIR}/09_idempotency_retry.request.json"
    local status_file="${EVIDENCE_DIR}/09_idempotency_retry.status.log"
    local body_a_file="${EVIDENCE_DIR}/09_idempotency_retry.response_a.json"
    local body_b_file="${EVIDENCE_DIR}/09_idempotency_retry.response_b.json"
    local code_a_file="${EVIDENCE_DIR}/09_idempotency_retry.code_a.log"
    local code_b_file="${EVIDENCE_DIR}/09_idempotency_retry.code_b.log"

    if ! command -v psql >/dev/null 2>&1; then
        fail "psql not available; cannot run idempotency retry DB-backed scenario"
        return
    fi

    # Candidate selection mirrors concurrency test, but execution is sequential:
    # request A creates, request B replays using the same Idempotency-Key.
    local candidate_row
    candidate_row=$(fetch_booking_candidate_row)

    if [[ -z "$candidate_row" ]]; then
        fail "No candidate row found in ServiceTypeAvailability for idempotency scenario"
        return
    fi

    local dealership_id service_type_id technician_id service_bay_id start_slot_id end_slot_id appointment_date customer_id vehicle_id
    IFS='|' read -r dealership_id service_type_id technician_id service_bay_id start_slot_id end_slot_id appointment_date customer_id vehicle_id <<< "$candidate_row"

    local payload
    payload=$(cat <<JSON
{
    "dealershipId": "$dealership_id",
    "customerId": "$customer_id",
    "vehicleId": "$vehicle_id",
    "appointmentDate": "$appointment_date",
    "serviceTypeId": "$service_type_id",
    "technicianId": "$technician_id",
    "serviceBayId": "$service_bay_id",
    "estimatedStartTimeSlotId": "$start_slot_id",
    "estimatedEndTimeSlotId": "$end_slot_id"
}
JSON
)
    printf '%s\n' "$payload" > "$req_file"

    # Unique per run to avoid collisions with previous evidence runs.
    local idem_key
    idem_key="idem-${EVIDENCE_RUN_ID}-$(date +%s)"

    curl -sS -o "$body_a_file" -w "%{http_code}" -X POST "$API_URL/appointments" \
        -H 'Content-Type: application/json' \
        -H "Idempotency-Key: $idem_key" \
        -d "$payload" > "$code_a_file"

    curl -sS -o "$body_b_file" -w "%{http_code}" -X POST "$API_URL/appointments" \
        -H 'Content-Type: application/json' \
        -H "Idempotency-Key: $idem_key" \
        -d "$payload" > "$code_b_file"

    local code_a code_b
    code_a=$(cat "$code_a_file")
    code_b=$(cat "$code_b_file")

    local appt_a appt_b
    appt_a=$(sed -n 's/.*"appointmentId":"\([^"]*\)".*/\1/p' "$body_a_file")
    appt_b=$(sed -n 's/.*"appointmentId":"\([^"]*\)".*/\1/p' "$body_b_file")

    {
        echo "candidate=$candidate_row"
        echo "idempotency_key=$idem_key"
        echo "code_a=$code_a"
        echo "code_b=$code_b"
        echo "appointment_a=$appt_a"
        echo "appointment_b=$appt_b"
    } > "$status_file"

    if [[ "$code_a" != "201" || "$code_b" != "201" ]]; then
        fail "Expected 201 + 201 replay for idempotency, got code_a=$code_a code_b=$code_b"
        info "Body A: $(cat "$body_a_file")"
        info "Body B: $(cat "$body_b_file")"
        return
    fi

    if [[ -z "$appt_a" || -z "$appt_b" || "$appt_a" != "$appt_b" ]]; then
        fail "Idempotency replay did not return the same appointmentId"
        info "Body A: $(cat "$body_a_file")"
        info "Body B: $(cat "$body_b_file")"
        return
    fi

    pass "Duplicate retry with same Idempotency-Key replayed same 201 response"
}

summary() {
    # Final exit code is intentionally tied to failed test count for CI compatibility.
    print_header "Test Execution Summary"
    echo -e "${GREEN}Passed:${NC} $TESTS_PASSED"
    echo -e "${RED}Failed:${NC} $TESTS_FAILED"
    echo -e "${YELLOW}Skipped:${NC} $TESTS_SKIPPED"
    echo

    if [[ $TESTS_FAILED -eq 0 ]]; then
        echo -e "${GREEN}All tests passed.${NC}"
        exit 0
    else
        echo -e "${RED}Some tests failed.${NC}"
        exit 1
    fi
}

main() {
    # Test order matters:
    # 1) basic reachability/validation,
    # 2) happy flow (create then read by id),
    # 3) advanced safety checks (idempotency missing/present header + concurrency + replay).
    cd "$ROOT_DIR"
    load_db_config_from_env_file
    detect_availability_data
    print_header "Vehicle Service Booking API - Automated Tests"
    info "Base URL: $API_URL"
    info "Date: $(date)"
    info "Evidence run id: $EVIDENCE_RUN_ID"
    mkdir -p "$EVIDENCE_DIR"
    info "Evidence directory: $EVIDENCE_DIR"

    check_api_reachable
    test_happy_get_availability
    test_happy_post_appointment
    test_happy_get_appointment_by_id
    test_invalid_guid_validation
    test_past_date_validation
    test_idempotency_missing_header_create_appointment
    test_idempotency_header_present_create_appointment
    test_concurrency_overlap_conflict
    test_idempotency_duplicate_retry
    summary
}

main "$@"
