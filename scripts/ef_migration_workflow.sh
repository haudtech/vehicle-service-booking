#!/usr/bin/env bash
set -euo pipefail

# EF Core migration workflow helper for VehicleServiceBooking
#
# Why this script exists:
# - Run EF Core commands with the correct project/startup/context every time.
# - Load connection string from .env (or use an explicit override).
# - Print clear, meaningful descriptions before each command.
#
# Connection string priority:
# 1) --connection "..."
# 2) CONNECTIONSTRINGS__DEFAULTCONNECTION from environment
# 3) CONNECTIONSTRINGS__DEFAULTCONNECTION from ./.env

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"
INFRA_PROJECT="src/VehicleServiceBooking.Infrastructure"
API_STARTUP_PROJECT="src/VehicleServiceBooking.Api"
DB_CONTEXT="ApplicationDbContext"

CONNECTION_OVERRIDE=""

usage() {
  cat <<'EOF'
EF Core Migration Workflow Helper

Usage:
  ./scripts/ef_migration_workflow.sh [--connection "<conn-string>"] <command> [args]

Commands:
  add <MigrationName>
    Build solution -> Add migration -> Update database -> List migrations

  update
    Apply all pending migrations to database

  list
    List migrations and show applied/pending state

  remove-last
    Remove the most recent migration (only when safe, e.g. before sharing)

  script [from] [to]
    Generate SQL migration script (defaults from=0, to=latest)

  status
    Build and print migration list only (quick validation)

Examples:
  ./scripts/ef_migration_workflow.sh add AddCustomerPreferredLanguage
  ./scripts/ef_migration_workflow.sh update
  ./scripts/ef_migration_workflow.sh script 0
  ./scripts/ef_migration_workflow.sh --connection "Host=localhost;Port=5432;Database=vehicle_service_booking;Username=haudo;Password=123456xX;" add AddIndexesForAppointments
EOF
}

log_step() {
  printf '\n[%s] %s\n' "STEP" "$1"
}

log_cmd() {
  printf '[CMD ] %s\n' "$1"
}

load_connection_string() {
  if [[ -n "$CONNECTION_OVERRIDE" ]]; then
    export CONNECTIONSTRINGS__DEFAULTCONNECTION="$CONNECTION_OVERRIDE"
    return
  fi

  if [[ -n "${CONNECTIONSTRINGS__DEFAULTCONNECTION:-}" ]]; then
    return
  fi

  if [[ -f "$ROOT_DIR/.env" ]]; then
    # Read only the connection line to avoid side effects.
    local line
    line="$(grep -E '^CONNECTIONSTRINGS__DEFAULTCONNECTION=' "$ROOT_DIR/.env" || true)"
    if [[ -n "$line" ]]; then
      export CONNECTIONSTRINGS__DEFAULTCONNECTION="${line#CONNECTIONSTRINGS__DEFAULTCONNECTION=}"
    fi
  fi

  if [[ -z "${CONNECTIONSTRINGS__DEFAULTCONNECTION:-}" ]]; then
    echo "ERROR: CONNECTIONSTRINGS__DEFAULTCONNECTION is not set."
    echo "Provide it via --connection, environment variable, or .env file."
    exit 1
  fi
}

run_dotnet_ef() {
  local ef_args=("$@")
  local cmd="dotnet ef ${ef_args[*]} --project $INFRA_PROJECT --startup-project $API_STARTUP_PROJECT --context $DB_CONTEXT"
  log_cmd "$cmd"
  dotnet ef "${ef_args[@]}" \
    --project "$INFRA_PROJECT" \
    --startup-project "$API_STARTUP_PROJECT" \
    --context "$DB_CONTEXT"
}

build_solution() {
  log_step "Build the solution first so migration scaffolding starts from a valid compile state"
  log_cmd "dotnet build"
  dotnet build
}

cmd_add() {
  local migration_name="${1:-}"
  if [[ -z "$migration_name" ]]; then
    echo "ERROR: Missing migration name."
    echo "Usage: ./scripts/ef_migration_workflow.sh add <MigrationName>"
    exit 1
  fi

  build_solution

  log_step "Create a new migration from entity and DbContext changes"
  run_dotnet_ef migrations add "$migration_name" --output-dir Migrations

  log_step "Apply the new migration to local database for immediate validation"
  run_dotnet_ef database update

  log_step "List migrations so you can confirm the new migration appears and is applied"
  run_dotnet_ef migrations list
}

cmd_update() {
  build_solution

  log_step "Apply all pending migrations to synchronize database schema with code"
  run_dotnet_ef database update
}

cmd_list() {
  log_step "Show migration history and pending state"
  run_dotnet_ef migrations list
}

cmd_remove_last() {
  build_solution

  log_step "Remove the most recent migration (safe mainly before sharing/merging)"
  run_dotnet_ef migrations remove

  log_step "List migrations after removal"
  run_dotnet_ef migrations list
}

cmd_script() {
  local from="${1:-0}"
  local to="${2:-}"

  log_step "Generate SQL script from migration range for review/deployment"
  if [[ -n "$to" ]]; then
    run_dotnet_ef migrations script "$from" "$to"
  else
    run_dotnet_ef migrations script "$from"
  fi
}

cmd_status() {
  build_solution
  cmd_list
}

main() {
  cd "$ROOT_DIR"

  # Parse optional --connection
  while [[ $# -gt 0 ]]; do
    case "$1" in
      --connection)
        CONNECTION_OVERRIDE="${2:-}"
        if [[ -z "$CONNECTION_OVERRIDE" ]]; then
          echo "ERROR: --connection requires a value."
          exit 1
        fi
        shift 2
        ;;
      -h|--help)
        usage
        exit 0
        ;;
      *)
        break
        ;;
    esac
  done

  local command="${1:-}"
  if [[ -z "$command" ]]; then
    usage
    exit 1
  fi
  shift || true

  load_connection_string

  printf '[INFO] Using project=%s startup=%s context=%s\n' "$INFRA_PROJECT" "$API_STARTUP_PROJECT" "$DB_CONTEXT"

  case "$command" in
    add)
      cmd_add "$@"
      ;;
    update)
      cmd_update
      ;;
    list)
      cmd_list
      ;;
    remove-last)
      cmd_remove_last
      ;;
    script)
      cmd_script "$@"
      ;;
    status)
      cmd_status
      ;;
    *)
      echo "ERROR: Unknown command '$command'"
      usage
      exit 1
      ;;
  esac
}

main "$@"
