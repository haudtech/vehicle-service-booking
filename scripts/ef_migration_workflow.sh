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
ALLOW_REMOTE="false"
YES_MODE="false"
DRY_RUN="false"
TARGET_CONNECTION=""

usage() {
  cat <<'EOF'
EF Core Migration Workflow Helper

Usage:
  ./scripts/ef_migration_workflow.sh [--connection "<conn-string>"] [--allow-remote] [--yes] [--dry-run] <command> [args]

Safety options:
  --allow-remote
    Allow execution against non-local database hosts. Without this flag, remote hosts are blocked.

  --yes
    Skip interactive confirmation prompts for commands that change the database/schema.

  --dry-run
    Print commands without executing them.

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

log_guidance() {
  printf '[GUIDE] %s\n' "$1"
}

is_true() {
  local value="${1:-}"
  [[ "$value" == "true" ]]
}

extract_connection_host() {
  local conn="${1:-}"
  local host=""
  local old_ifs="$IFS"
  IFS=';'
  read -ra parts <<< "$conn"
  IFS="$old_ifs"

  for part in "${parts[@]}"; do
    case "$part" in
      Host=*|host=*)
        host="${part#*=}"
        break
        ;;
    esac
  done

  printf '%s' "$host"
}

extract_connection_database() {
  local conn="${1:-}"
  local db=""
  local old_ifs="$IFS"
  IFS=';'
  read -ra parts <<< "$conn"
  IFS="$old_ifs"

  for part in "${parts[@]}"; do
    case "$part" in
      Database=*|database=*)
        db="${part#*=}"
        break
        ;;
    esac
  done

  printf '%s' "$db"
}

is_local_host() {
  local host="${1:-}"
  local normalized
  normalized="$(printf '%s' "$host" | tr '[:upper:]' '[:lower:]')"

  case "$normalized" in
    localhost|127.0.0.1|::1|0.0.0.0|host.docker.internal|postgres|db)
      return 0
      ;;
    *)
      return 1
      ;;
  esac
}

confirm_or_exit() {
  local message="$1"

  if is_true "$YES_MODE"; then
    return
  fi

  printf '[CONFIRM] %s Type YES to continue: ' "$message"
  local response
  read -r response
  if [[ "$response" != "YES" ]]; then
    echo "Cancelled."
    exit 1
  fi
}

ensure_safe_target() {
  local command="$1"

  local host
  host="$(extract_connection_host "$TARGET_CONNECTION")"
  local db
  db="$(extract_connection_database "$TARGET_CONNECTION")"

  if [[ -z "$host" ]]; then
    echo "ERROR: Could not parse database host from connection string."
    echo "Use --connection with a valid 'Host=' segment."
    exit 1
  fi

  local is_remote="false"
  if ! is_local_host "$host"; then
    is_remote="true"
  fi

  if is_true "$is_remote" && ! is_true "$ALLOW_REMOTE"; then
    echo "ERROR: Refusing to run '$command' against remote host '$host'."
    echo "Use --allow-remote only when you intentionally want to target a non-local database."
    exit 1
  fi

  case "$command" in
    add|update|remove-last)
      local scope="local"
      if is_true "$is_remote"; then
        scope="remote"
      fi
      confirm_or_exit "Command '$command' will modify the $scope database on host '$host'${db:+ (database '$db')}."

      # Add an extra explicit gate when the target appears production-like.
      local host_lc db_lc
      host_lc="$(printf '%s' "$host" | tr '[:upper:]' '[:lower:]')"
      db_lc="$(printf '%s' "$db" | tr '[:upper:]' '[:lower:]')"
      if [[ "$host_lc" == *prod* || "$host_lc" == *production* || "$db_lc" == *prod* || "$db_lc" == *production* ]]; then
        if ! is_true "$YES_MODE"; then
          printf '[CONFIRM] Production-like target detected. Type PRODUCTION to continue: '
          local prod_response
          read -r prod_response
          if [[ "$prod_response" != "PRODUCTION" ]]; then
            echo "Cancelled."
            exit 1
          fi
        fi
      fi
      ;;
  esac
}

print_command_guidance() {
  local command="$1"

  case "$command" in
    add)
      log_guidance "Use 'add' when entity/model/DbContext code changed and you need a new migration file."
      log_guidance "Recommended flow: 1) --dry-run, 2) add migration, 3) review generated migration code, 4) apply update."
      log_guidance "For production pipelines, prefer generating SQL first via 'script' and applying through controlled deployment."
      ;;
    update)
      log_guidance "Use 'update' to apply pending migrations to the target database."
      log_guidance "Before production update: run backups, ensure maintenance window, and confirm target connection string."
      log_guidance "Use '--allow-remote' for non-local hosts and keep interactive confirmations enabled unless CI requires '--yes'."
      ;;
    list)
      log_guidance "Use 'list' to check migration order and applied/pending state; this does not change schema."
      log_guidance "Run this first when validating environment state before update/remove operations."
      ;;
    remove-last)
      log_guidance "Use 'remove-last' only for the newest unshared migration during local development."
      log_guidance "Do not use it after migration is applied in shared/staging/production environments."
      log_guidance "If already deployed, create a new corrective migration instead of removing history."
      ;;
    script)
      log_guidance "Use 'script [from] [to]' to generate SQL for review, approvals, and production deployment pipelines."
      log_guidance "Default 'from=0' generates full history; use a bounded range for incremental rollout."
      log_guidance "Store generated SQL as deployment artifact and attach to change ticket/release notes."
      ;;
    status)
      log_guidance "Use 'status' for quick preflight: build validation plus migration list."
      log_guidance "Run this before 'update' when switching environments or branches."
      ;;
  esac
}

load_connection_string() {
  if [[ -n "$CONNECTION_OVERRIDE" ]]; then
    export CONNECTIONSTRINGS__DEFAULTCONNECTION="$CONNECTION_OVERRIDE"
    TARGET_CONNECTION="$CONNECTIONSTRINGS__DEFAULTCONNECTION"
    return
  fi

  if [[ -n "${CONNECTIONSTRINGS__DEFAULTCONNECTION:-}" ]]; then
    TARGET_CONNECTION="$CONNECTIONSTRINGS__DEFAULTCONNECTION"
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

  TARGET_CONNECTION="$CONNECTIONSTRINGS__DEFAULTCONNECTION"
}

run_dotnet_ef() {
  local ef_args=("$@")
  local cmd="dotnet ef ${ef_args[*]} --project $INFRA_PROJECT --startup-project $API_STARTUP_PROJECT --context $DB_CONTEXT"
  log_cmd "$cmd"
  if is_true "$DRY_RUN"; then
    return
  fi
  dotnet ef "${ef_args[@]}" \
    --project "$INFRA_PROJECT" \
    --startup-project "$API_STARTUP_PROJECT" \
    --context "$DB_CONTEXT"
}

build_solution() {
  log_step "Build the solution first so migration scaffolding starts from a valid compile state"
  log_cmd "dotnet build"
  if is_true "$DRY_RUN"; then
    return
  fi
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
      --allow-remote)
        ALLOW_REMOTE="true"
        shift
        ;;
      --yes)
        YES_MODE="true"
        shift
        ;;
      --dry-run)
        DRY_RUN="true"
        shift
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

  ensure_safe_target "$command"

  printf '[INFO] Using project=%s startup=%s context=%s\n' "$INFRA_PROJECT" "$API_STARTUP_PROJECT" "$DB_CONTEXT"
  if is_true "$DRY_RUN"; then
    echo "[INFO] Dry-run mode enabled: commands will be printed but not executed."
  fi

  print_command_guidance "$command"

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
