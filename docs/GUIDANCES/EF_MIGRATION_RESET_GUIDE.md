# 🔄 EF Core Migration Reset Guide

**Date:** June 26, 2026  
**Purpose:** Reset migrations and recreate from current entity/configuration state  
**Status:** Ready to Execute  

---

## 📋 What This Does

Removes all existing migration files and database, then creates a fresh migration from the current DbContext state.

---

## 🎯 Step-by-Step Process

### **Step 1: Remove Existing Migrations**

```bash
# Remove old migration files from Infrastructure project
rm -rf src/VehicleServiceBooking.Infrastructure/Migrations/
mkdir -p src/VehicleServiceBooking.Infrastructure/Migrations
```

**What it removes:**
- ✅ All `.cs` migration files
- ✅ All `.Designer.cs` files  
- ✅ ModelSnapshot.cs
- ❌ Keeps the Migrations folder (EF needs it)

---

### **Step 2: Remove Database**

```bash
# For PostgreSQL (if database exists)
# Option A: Drop via connection string
dropdb vehicle_service_booking

# Option B: Manual via psql
# psql -U postgres
# DROP DATABASE vehicle_service_booking;

# For SQL Server
# sqlcmd -S localhost -U sa -P your_password
# DROP DATABASE VehicleServiceBooking;

# For SQLite (if using)
# rm -f vehicle_service_booking.db
```

**What it does:**
- ✅ Removes all tables, views, stored procedures
- ✅ Clears all data
- ✅ Resets database state

---

### **Step 3: Create Initial Migration**

From project root, run:

```bash
cd /Users/tech/dev/net/vehicle-service-booking

dotnet ef migrations add InitialCreate \
  --project src/VehicleServiceBooking.Infrastructure \
  --startup-project src/VehicleServiceBooking.Api \
  --context ApplicationDbContext \
  --output-dir Migrations
```

**What happens:**
- ✅ Scans current DbContext and entities
- ✅ Generates fresh `InitialCreate.cs` migration
- ✅ Generates `InitialCreate.Designer.cs`
- ✅ Updates `ApplicationDbContextModelSnapshot.cs`

---

### **Step 4: Apply Migration to Database**

```bash
dotnet ef database update \
  --project src/VehicleServiceBooking.Infrastructure \
  --startup-project src/VehicleServiceBooking.Api \
  --context ApplicationDbContext
```

**What happens:**
- ✅ Executes migration SQL
- ✅ Creates all tables
- ✅ Creates all views (TechnicianAvailableSlots, ServiceBayAvailableSlots, ServiceTypeAvailability)
- ✅ Creates indexes and constraints
- ✅ Updates `__EFMigrationsHistory` table

---

### **Step 5: Verify Migration**

```bash
# Check migration status
dotnet ef migrations list \
  --project src/VehicleServiceBooking.Infrastructure \
  --startup-project src/VehicleServiceBooking.Api \
  --context ApplicationDbContext

# Expected output:
# 20260626123456_InitialCreate (Applied)
```

---

### **Step 6: Seed Initial Data (Optional)**

If you have seed data, run:

```bash
dotnet ef database update \
  --project src/VehicleServiceBooking.Infrastructure \
  --startup-project src/VehicleServiceBooking.Api \
  --context ApplicationDbContext \
  --configuration Release
```

---

## ✅ Verification Checklist

After completing all steps, verify:

- [ ] Migrations folder exists but only has one `InitialCreate` migration
- [ ] Database is created with all tables
- [ ] All 3 views exist (TechnicianAvailableSlots, ServiceBayAvailableSlots, ServiceTypeAvailability)
- [ ] Build succeeds: `dotnet build`
- [ ] No migration pending: `dotnet ef migrations list` shows only InitialCreate

---

## 🔍 Verify Database Tables & Views

### **PostgreSQL**
```sql
-- Connect to database
psql -U postgres -d vehicle_service_booking

-- List all tables
\dt

-- List all views
\dv

-- Expected tables:
-- Appointments, Services, ServiceTypes, Technicians, TechnicianSchedules, 
-- TechnicianSkills, ServiceBays, BusinessHours, Customers, Dealerships, 
-- Vehicles, TimeSlots, AppointmentStatusLookups, ServiceStatusLookups

-- Expected views:
-- TechnicianAvailableSlots, ServiceBayAvailableSlots, ServiceTypeAvailability
```

### **SQL Server**
```sql
-- List tables
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'

-- List views
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'VIEW'
```

---

## ⚠️ Important Notes

1. **Data Loss:** This process deletes all data. Backup first if needed.

2. **Connection String:** Ensure `appsettings.json` has correct database connection:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=vehicle_service_booking;User Id=postgres;Password=your_password;"
     }
   }
   ```

3. **Entity Framework Tools:** Ensure installed:
   ```bash
   dotnet tool install --global dotnet-ef
   ```

4. **Build First:** Always build before migrations:
   ```bash
   dotnet build
   ```

---

## 🐛 Troubleshooting

### Issue: "No migrations applied"
```bash
# Solution: Check migration history table
dotnet ef migrations list --project src/VehicleServiceBooking.Infrastructure
```

### Issue: "Could not connect to database"
```bash
# Solution: Verify connection string in appsettings.json
# Check database is running
# For PostgreSQL: psql -U postgres -l
```

### Issue: "Migrations already exist"
```bash
# Solution: Delete the folder and retry
rm -rf src/VehicleServiceBooking.Infrastructure/Migrations/
mkdir -p src/VehicleServiceBooking.Infrastructure/Migrations
```

---

## 📝 Script: Automated Reset (Optional)

Create `reset-migrations.sh`:

```bash
#!/bin/bash

echo "🔄 Starting EF Core Migration Reset..."

# Step 1: Remove migrations
echo "1️⃣ Removing old migrations..."
rm -rf src/VehicleServiceBooking.Infrastructure/Migrations/
mkdir -p src/VehicleServiceBooking.Infrastructure/Migrations

# Step 2: Build
echo "2️⃣ Building project..."
dotnet build

# Step 3: Create migration
echo "3️⃣ Creating InitialCreate migration..."
dotnet ef migrations add InitialCreate \
  --project src/VehicleServiceBooking.Infrastructure \
  --startup-project src/VehicleServiceBooking.Api \
  --context ApplicationDbContext \
  --output-dir Migrations

# Step 4: Update database
echo "4️⃣ Applying migration to database..."
dotnet ef database update \
  --project src/VehicleServiceBooking.Infrastructure \
  --startup-project src/VehicleServiceBooking.Api \
  --context ApplicationDbContext

# Step 5: Verify
echo "5️⃣ Verifying migration..."
dotnet ef migrations list \
  --project src/VehicleServiceBooking.Infrastructure \
  --startup-project src/VehicleServiceBooking.Api \
  --context ApplicationDbContext

echo "✅ Migration reset complete!"
```

**Run it:**
```bash
chmod +x reset-migrations.sh
./reset-migrations.sh
```

---

## 🎯 Result

After completion, you'll have:

✅ Clean migration history (single InitialCreate)  
✅ Fresh database reflecting current entities  
✅ All views (materialized views from Phase 3)  
✅ All constraints and indexes  
✅ No stale migration files  

---

## 📊 Migration Comparison

| Before Reset | After Reset |
|--------------|-------------|
| Multiple migrations | 1 InitialCreate |
| Stale migration files | Clean migrations folder |
| Old database schema | Current schema |
| Mixed entity configs | Unified OnModelCreating |

---

**Status:** Ready to execute. Choose manual steps or use automated script.
