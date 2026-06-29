#!/usr/bin/env bash
set -euo pipefail

##############################################################################
# dotnet test report helper
#
# Purpose:
# - Run dotnet test.
# - Capture the console output.
# - Auto-generate a small Markdown report with a tabular summary.
#
# The generated report is meant to be easy to scan in the editor and in git.
##############################################################################

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"

REPORT_DATE="${REPORT_DATE:-$(date +%F)}"
REPORT_RUN_ID="${REPORT_RUN_ID:-$(date +%Y%m%d_%H%M%S)}"
REPORT_DIR="${ROOT_DIR}/src/VehicleServiceBooking.Api/logs/test-reports/${REPORT_DATE}/${REPORT_RUN_ID}"
RAW_REPORT_FILE="${REPORT_DIR}/dotnet-test.raw.log"
SUMMARY_REPORT_FILE="${REPORT_DIR}/dotnet-test.md"
TRX_REPORT_FILE="${REPORT_DIR}/dotnet-test.trx"

mkdir -p "$REPORT_DIR"

echo "Running dotnet test..."
if dotnet test --results-directory "$REPORT_DIR" --logger "trx;LogFileName=dotnet-test.trx" "$@" >"$RAW_REPORT_FILE" 2>&1; then
    test_exit_code=0
else
    test_exit_code=$?
fi

passed="unknown"
failed="unknown"
skipped="unknown"
total="unknown"
duration="unknown"
status="Failed"
notes="See raw output for details"
executed_count="unknown"

summary_line=$(grep -E 'Passed!  - Failed:' "$RAW_REPORT_FILE" | tail -n 1 || true)
if [[ -n "$summary_line" ]]; then
    passed=$(sed -n 's/.*Passed:[[:space:]]*\([0-9][0-9]*\).*/\1/p' <<< "$summary_line")
    failed=$(sed -n 's/.*Failed:[[:space:]]*\([0-9][0-9]*\).*/\1/p' <<< "$summary_line")
    skipped=$(sed -n 's/.*Skipped:[[:space:]]*\([0-9][0-9]*\).*/\1/p' <<< "$summary_line")
    total=$(sed -n 's/.*Total:[[:space:]]*\([0-9][0-9]*\).*/\1/p' <<< "$summary_line")
    duration=$(sed -n 's/.*Duration:[[:space:]]*\([^ -]*\).*/\1/p' <<< "$summary_line")

    if [[ "$failed" == "0" ]]; then
        status="Passed"
        notes="All tests passed"
    fi
fi

test_case_rows=""
if [[ -f "$TRX_REPORT_FILE" ]]; then
    # Parse TRX UnitTestResult elements to build per-test scenario and outcome rows.
    test_case_rows=$(awk 'BEGIN { RS="<UnitTestResult "; ORS="" }
        NR > 1 {
            block = $0
            test_name = ""
            outcome = ""

            if (match(block, /testName="[^"]+"/)) {
                test_name = substr(block, RSTART + 10, RLENGTH - 11)
            }
            if (match(block, /outcome="[^"]+"/)) {
                outcome = substr(block, RSTART + 9, RLENGTH - 10)
            }

            if (test_name != "" && outcome != "") {
                gsub(/\|/, "\\|", test_name)
                print "| " test_name " | " outcome " |\n"
            }
        }' "$TRX_REPORT_FILE")

    if [[ -n "$test_case_rows" ]]; then
        executed_count=$(printf '%s' "$test_case_rows" | grep -c '^| ' || true)
    fi
fi

{
    echo "# dotnet test report"
    echo
    echo "| Field | Value |"
    echo "| --- | --- |"
    echo "| Command | dotnet test $* |"
    echo "| Status | $status |"
    echo "| Passed | $passed |"
    echo "| Failed | $failed |"
    echo "| Skipped | $skipped |"
    echo "| Total | $total |"
    echo "| Executed Cases (from TRX) | $executed_count |"
    echo "| Duration | $duration |"
    echo "| Notes | $notes |"
    echo
    echo "## Raw Output Summary"
    echo
    echo "| Section | Details |"
    echo "| --- | --- |"
    echo "| Console report | [dotnet-test.raw.log](dotnet-test.raw.log) |"
    echo "| TRX report | [dotnet-test.trx](dotnet-test.trx) |"
    echo "| Generated at | $(date) |"
    echo
    echo "## Test Cases Executed"
    echo
    echo "| Scenario | Status |"
    echo "| --- | --- |"

    if [[ -n "$test_case_rows" ]]; then
        printf '%s' "$test_case_rows"
    else
        echo "| Not available | TRX parsing did not return test case rows |"
    fi
} > "$SUMMARY_REPORT_FILE"

echo "Report written to: $SUMMARY_REPORT_FILE"

exit "$test_exit_code"