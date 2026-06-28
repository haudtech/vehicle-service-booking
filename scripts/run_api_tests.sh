#!/usr/bin/env bash
set -euo pipefail

##############################################################################
# Vehicle Service Booking API - Automated Test Suite
#
# Aligned with docs/API_TEST_CASES.md
# - Base URL: http://localhost:5291/api/v1
# - Happy cases: availability, create appointment, get appointment by id
# - Validation cases: invalid GUID, past date
##############################################################################

GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"

API_URL="http://localhost:5291/api/v1"
EVIDENCE_DATE="${EVIDENCE_DATE:-$(date +%F)}"
EVIDENCE_RUN_ID="${EVIDENCE_RUN_ID:-$(date +%Y%m%d_%H%M%S)}"
EVIDENCE_DIR="${ROOT_DIR}/docs/evidence/api/${EVIDENCE_DATE}/${EVIDENCE_RUN_ID}"

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
DB_PASSWORD="${DB_PASSWORD:-123456xX}"

TESTS_PASSED=0
TESTS_FAILED=0
TARGET_DATE=""

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

info() {
    echo -e "${BLUE}INFO:${NC} $1"
}

http_get() {
    local url="$1"
    local body_file="$2"
    curl -sS -o "$body_file" -w "%{http_code}" "$url"
}

http_post_json() {
    local url="$1"
    local payload="$2"
    local body_file="$3"
    curl -sS -o "$body_file" -w "%{http_code}" -X POST "$url" -H 'Content-Type: application/json' -d "$payload"
}

resolve_target_date() {
    if [[ -n "${TARGET_DATE:-}" ]]; then
        return
    fi

    local offset candidate_date code body
    local tmp_body="${EVIDENCE_DIR}/_target_date_probe.json"

    for offset in {1..30}; do
        candidate_date=$(date -v+"${offset}"d +%F)
        code=$(curl -sS -o "$tmp_body" -w "%{http_code}" "$API_URL/availability?dealershipId=$DEALERSHIP_ID&serviceTypeId=$SERVICE_TYPE_ID&date=$candidate_date") || true
        body=$(cat "$tmp_body" 2>/dev/null || true)

        if [[ "$code" == "200" ]] && grep -q 'slotStart' <<< "$body"; then
            TARGET_DATE="$candidate_date"
            info "Using target date with availability: $TARGET_DATE"
            return
        fi
    done

    TARGET_DATE=$(date -v+1d +%F)
    info "No availability found in next 30 days; falling back to $TARGET_DATE"
}

check_api_reachable() {
    print_test "API Reachability"
    local code
    code=$(curl -sS -o /dev/null -w "%{http_code}" "$API_URL/availability") || true
    if [[ "$code" == "200" || "$code" == "400" ]]; then
        pass "API is reachable on $API_URL (status $code)"
    else
        fail "API not reachable on $API_URL (status $code)"
        echo "Start API with:"
        echo "cd src/VehicleServiceBooking.Api"
        echo "ASPNETCORE_URLS='http://localhost:5291' ConnectionStrings__DefaultConnection='Host=localhost;Port=5432;Database=vehicle_service_booking;Username=haudo;Password=123456xX;' dotnet run --no-launch-profile"
        exit 1
    fi
}

test_happy_get_availability() {
    print_test "Happy Case 1 - GET /availability"
    local code req_url
    local req_file="${EVIDENCE_DIR}/01_get_availability.request.txt"
    local status_file="${EVIDENCE_DIR}/01_get_availability.status.txt"
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
    local payload code
    local selected_service_type_id="$SERVICE_TYPE_ID"
    local selected_technician_id="$TECHNICIAN_ID"
    local selected_service_bay_id="$SERVICE_BAY_ID"
    local selected_start_slot_id="$START_SLOT_ID"
    local selected_end_slot_id="$END_SLOT_ID"
    local body_file="${EVIDENCE_DIR}/02_post_appointment.response.json"
    local req_file="${EVIDENCE_DIR}/02_post_appointment.request.json"
    local status_file="${EVIDENCE_DIR}/02_post_appointment.status.txt"

    resolve_target_date

    # Try to pick a currently available slot combo dynamically for reliable reruns.
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

test_happy_get_appointment_by_id() {
    print_test "Happy Case 3 - GET /appointments/{id}"
    if [[ -z "${APPOINTMENT_ID:-}" ]]; then
        fail "Skipped: missing appointmentId from POST test"
        return
    fi

    local code req_url
    local req_file="${EVIDENCE_DIR}/03_get_appointment_by_id.request.txt"
    local status_file="${EVIDENCE_DIR}/03_get_appointment_by_id.status.txt"
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
    local code req_url
    local req_file="${EVIDENCE_DIR}/04_invalid_guid.request.txt"
    local status_file="${EVIDENCE_DIR}/04_invalid_guid.status.txt"
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
    local code req_url
    local req_file="${EVIDENCE_DIR}/05_past_date.request.txt"
    local status_file="${EVIDENCE_DIR}/05_past_date.status.txt"
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

summary() {
    print_header "Test Execution Summary"
    echo -e "${GREEN}Passed:${NC} $TESTS_PASSED"
    echo -e "${RED}Failed:${NC} $TESTS_FAILED"
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
    cd "$ROOT_DIR"
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
    summary
}

main "$@"
