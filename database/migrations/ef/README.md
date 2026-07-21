# EF Migration SQL Mirror

This folder mirrors EF Core migrations from:

- `src/VehicleServiceBooking.Infrastructure/Migrations`

Generated artifacts:

- `ALL_MIGRATIONS_IDEMPOTENT.sql`: consolidated idempotent script for all migrations.
- `<MigrationId>_<Name>.sql`: one step script per EF migration in chronological order.

Current status:

- EF migration files: 16
- Mirrored step SQL files: 16

## Regenerate

Run from repository root:

```bash
mkdir -p database/migrations/ef

dotnet ef migrations script --idempotent \
  --project src/VehicleServiceBooking.Infrastructure/VehicleServiceBooking.Infrastructure.csproj \
  --startup-project src/VehicleServiceBooking.Api/VehicleServiceBooking.Api.csproj \
  --output database/migrations/ef/ALL_MIGRATIONS_IDEMPOTENT.sql

migrations=($(ls src/VehicleServiceBooking.Infrastructure/Migrations/*.cs \
  | xargs -n1 basename \
  | grep -vE 'Designer|ModelSnapshot' \
  | sed 's/\.cs$//' \
  | sort))

for ((i=1; i<=${#migrations}; i++)); do
  to=${migrations[$i]}
  if (( i == 1 )); then from=0; else from=${migrations[$((i-1))]}; fi

  dotnet ef migrations script "$from" "$to" \
    --project src/VehicleServiceBooking.Infrastructure/VehicleServiceBooking.Infrastructure.csproj \
    --startup-project src/VehicleServiceBooking.Api/VehicleServiceBooking.Api.csproj \
    --output "database/migrations/ef/${to}.sql"
done
```
