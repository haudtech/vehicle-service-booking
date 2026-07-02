# Vehicle Service Booking: Coding Principles & Architecture Rules

**Document Version:** 1.0  
**Last Updated:** June 28, 2026  
**Status:** ✅ ACTIVE - STRICTLY ENFORCED  
**Audience:** All Developers  

---

## 📋 Introduction

This document defines **non-negotiable architectural principles and coding standards** for the Vehicle Service Booking system. All code, pull requests, and implementations **MUST** strictly follow these rules.

**Purpose:**
- Enforce consistent architecture across codebase
- Prevent technical debt accumulation
- Ensure performance and scalability
- Enable maintainability and extensibility
- Establish clear boundaries between layers

**Compliance:** Code review will verify adherence to all principles. Non-compliant code will NOT be merged.

---

## 🏗️ PRINCIPLE 1: Strict Layer Separation Architecture

### Rule Set: Layer Boundaries (MUST BE FOLLOWED)

**Overall Pattern:**
```
User Request
    ↓
┌─────────────────────────────────────┐
│ API LAYER (Controllers)             │ ← HTTP handling, DTOs, validation
├─────────────────────────────────────┤
│ APPLICATION LAYER (Services)        │ ← Business logic, orchestration
├─────────────────────────────────────┤
│ DOMAIN LAYER (Entities)             │ ← Business rules, domain logic
├─────────────────────────────────────┤
│ INFRASTRUCTURE LAYER (Repositories) │ ← Data access only
├─────────────────────────────────────┤
│ DATABASE (DbContext)                │ ← EF Core DbSet operations
└─────────────────────────────────────┘
    ↓
Database
```

### Rule 1.1: API Layer (Controllers)

**Responsibilities:**
- ✅ HTTP request/response handling
- ✅ Input validation (FluentValidation)
- ✅ DTO mapping (from request, to response)
- ✅ Status code handling
- ✅ Exception to HTTP response mapping

**What Controllers CAN DO:**
```csharp
// ✅ CORRECT: Controller calls Service only
[HttpPost]
public async Task<IActionResult> CreateAppointment(
    [FromBody] CreateAppointmentRequest request,
    CancellationToken cancellationToken)
{
    // ✅ Validate input
    var validationResult = await _validator.ValidateAsync(request, cancellationToken);
    if (!validationResult.IsValid)
        return BadRequest(validationResult.Errors);
    
    // ✅ Call Service only
    var result = await _appointmentService.CreateAppointmentAsync(request, cancellationToken);
    
    // ✅ Map to DTO
    return Ok(_mapper.Map<AppointmentDto>(result));
}
```

**What Controllers CANNOT DO:**
```csharp
// ❌ WRONG: Accessing Repository directly
var appointment = await _repository.GetByIdAsync(id);

// ❌ WRONG: Accessing DbContext directly
var appointment = await _dbContext.Appointments.FirstOrDefaultAsync(a => a.Id == id);

// ❌ WRONG: Business logic in controller
if (appointment.Status == AppointmentStatus.Confirmed 
    && appointment.Services.Count > 0
    && /* complex business rule */)
{
    // Logic here
}

// ❌ WRONG: Database queries in controller
var conflictingAppointments = await _dbContext.Appointments
    .Where(a => a.VehicleId == vehicleId)
    .ToListAsync();
```

### Rule 1.2: Service Layer

**Responsibilities:**
- ✅ Business logic orchestration
- ✅ Transaction management (when needed)
- ✅ Calling other services for complex operations
- ✅ Calling repositories for data access
- ✅ Validation logic coordination
- ✅ Cross-cutting concerns (logging, caching)

**What Services CAN DO:**
```csharp
// ✅ CORRECT: Service calls Repository and orchestrates business logic
public async Task<Appointment> CreateAppointmentAsync(
    CreateAppointmentRequest request,
    CancellationToken cancellationToken)
{
    // ✅ Call AvailabilityService for business validation
    var availableSlots = await _availabilityService.GetAvailableSlotsAsync(
        request.DealershipId, request.ServiceTypeId, request.AppointmentDate, cancellationToken);
    
    // ✅ Business logic: validate slot availability
    if (!availableSlots.Any(s => s.TechnicianId == request.TechnicianId))
        throw new InvalidOperationException("Slot not available");
    
    // ✅ Call Repository for data operations
    var hasSkill = await _appointmentRepository.TechnicianHasSkillAsync(
        request.TechnicianId, request.ServiceTypeId, cancellationToken);
    
    // ✅ Create entity (memory only)
    var appointment = new Appointment { /* populate from request */ };
    
    // ✅ Call Repository to persist
    return await _appointmentRepository.CreateAppointmentWithServicesAsync(
        appointment, cancellationToken);
}
```

**What Services CANNOT DO:**
```csharp
// ❌ WRONG: Direct DbContext access
var appointment = await _dbContext.Appointments.FirstOrDefaultAsync(a => a.Id == id);

// ❌ WRONG: HTTP status codes or responses in service
if (appointment == null)
    return NotFound();  // ← Wrong layer!

// ❌ WRONG: DTO mapping in service (should be in Controller)
var dto = _mapper.Map<AppointmentDto>(appointment);

// ❌ WRONG: Request validation (should be in Controller + Middleware)
if (request.AppointmentDate < DateTime.Now)
    throw new ValidationException("Date must be in future");
```

### Rule 1.3: Repository Layer

**Responsibilities:**
- ✅ Data access operations only (Read, Create, Update, Delete)
- ✅ Query composition
- ✅ DbContext management
- ✅ Transaction coordination

**What Repositories CAN DO:**
```csharp
// ✅ CORRECT: Repository queries data via DbContext
public async Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
{
    return await _dbContext.Appointments
        .AsNoTracking()  // ← ALWAYS for read queries
        .Include(a => a.Services)
        .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
}

// ✅ CORRECT: Repository performs data operations
public async Task<Appointment> CreateAppointmentWithServicesAsync(
    Appointment appointment, CancellationToken cancellationToken)
{
    _dbContext.Appointments.Add(appointment);
    await _dbContext.SaveChangesAsync(cancellationToken);
    return appointment;
}
```

**What Repositories CANNOT DO:**
```csharp
// ❌ WRONG: Business logic in Repository
public async Task<bool> IsAppointmentValidAsync(Appointment appointment, CancellationToken ct)
{
    // This is BUSINESS LOGIC, not data access
    if (appointment.Status == AppointmentStatus.Confirmed
        && appointment.Services.Count > 0
        && /* business rule */)
    {
        return true;
    }
    return false;
}

// ❌ WRONG: Service orchestration in Repository
public async Task<Appointment> BookAppointmentAsync(
    CreateAppointmentRequest request, CancellationToken ct)
{
    // Orchestration belongs in Service, not Repository
    var availability = await _availabilityService.GetAvailableSlotsAsync(...);
    var hasSkill = await _technicianRepository.HasSkillAsync(...);
    // ... more orchestration
}

// ❌ WRONG: HTTP responses or DTOs in Repository
return new AppointmentDto { ... };
```

### Rule 1.4: DbContext & Database Layer

**Responsibilities:**
- ✅ Entity configuration (OnModelCreating)
- ✅ Database connection management
- ✅ Migrations
- ✅ Direct DbSet operations (AddAsync, RemoveAsync, etc.)

**ONLY used by Repositories** - Never access DbContext from Services or Controllers directly.

---

## 🔍 PRINCIPLE 2: .AsNoTracking() in LINQ Queries

### Rule Set: Change Tracking Strategy (MUST BE FOLLOWED)

**Core Rule:**
```
ALL read-only queries MUST use .AsNoTracking()
WRITE operations MUST NOT use .AsNoTracking()
```

### Rule 2.1: When to Use .AsNoTracking()

**✅ ALWAYS Use .AsNoTracking() When:**

```csharp
// ✅ Read queries for display
public async Task<Appointment?> GetByIdAsync(Guid id, CancellationToken ct)
{
    return await _dbContext.Appointments
        .AsNoTracking()  // ← REQUIRED
        .Include(a => a.Services)
        .FirstOrDefaultAsync(a => a.Id == id, ct);
}

// ✅ Validation/availability checks
public async Task<IEnumerable<Appointment>> GetByVehicleIdAsync(
    Guid vehicleId, CancellationToken ct)
{
    return await _dbContext.Appointments
        .AsNoTracking()  // ← REQUIRED (conflict detection only)
        .Where(a => a.VehicleId == vehicleId)
        .Include(a => a.Services)
        .ToListAsync(ct);
}

// ✅ Reporting/analytics queries
public async Task<List<TechnicianUtilizationDto>> GetTechnicianUtilizationAsync(
    DateTime startDate, DateTime endDate, CancellationToken ct)
{
    return await _dbContext.Appointments
        .AsNoTracking()  // ← REQUIRED (read-only metrics)
        .Where(a => a.CreatedAt >= startDate && a.CreatedAt <= endDate)
        .Select(a => new TechnicianUtilizationDto { /* ... */ })
        .ToListAsync(ct);
}

// ✅ Complex availability queries
public async Task<List<AvailabilityOption>> GetAvailableSlotsAsync(
    Guid dealershipId, Guid serviceTypeId, DateTime date, CancellationToken ct)
{
    return await _dbContext.ServiceTypeAvailabilityView
        .AsNoTracking()  // ← REQUIRED (materialized view read)
        .Where(s => s.DealershipId == dealershipId && s.ServiceTypeId == serviceTypeId)
        .Select(s => new AvailabilityOption { /* ... */ })
        .ToListAsync(ct);
}
```

### Rule 2.2: When NOT to Use .AsNoTracking()

**❌ NEVER Use .AsNoTracking() When:**

```csharp
// ❌ WRONG: Create operations
public async Task<Appointment> AddAsync(Appointment appointment, CancellationToken ct)
{
    _dbContext.Appointments.Add(appointment);  // ← NO .AsNoTracking()
    await _dbContext.SaveChangesAsync(ct);
    return appointment;
}

// ❌ WRONG: Update operations (query then modify)
public async Task<Appointment> UpdateStatusAsync(
    Guid appointmentId, AppointmentStatus status, CancellationToken ct)
{
    // First query WITHOUT .AsNoTracking() (need to track for updates)
    var appointment = await _dbContext.Appointments
        .FirstOrDefaultAsync(a => a.Id == appointmentId, ct);  // ← NO .AsNoTracking()
    
    appointment.Status = status;  // Modify
    await _dbContext.SaveChangesAsync(ct);  // Save changes
    return appointment;
}

// ❌ WRONG: Delete operations
public async Task DeleteAsync(Guid appointmentId, CancellationToken ct)
{
    var appointment = await _dbContext.Appointments
        .FirstOrDefaultAsync(a => a.Id == appointmentId, ct);  // ← NO .AsNoTracking()
    
    _dbContext.Appointments.Remove(appointment);
    await _dbContext.SaveChangesAsync(ct);
}
```

### Rule 2.3: Pattern Template for All Queries

**✅ Correct Pattern:**

```csharp
// Read queries ALWAYS include .AsNoTracking()
public async Task<Entity?> GetByIdAsync(Guid id, CancellationToken ct)
{
    return await _dbContext.Entities
        .AsNoTracking()              // ← Line 1: REQUIRED for reads
        .Include(e => e.Related)     // ← Line 2: Load relationships
        .FirstOrDefaultAsync(e => e.Id == id, ct);  // ← Line 3: Execute
}

// Complex queries with splits
public async Task<List<Entity>> GetComplexAsync(Guid id, CancellationToken ct)
{
    return await _dbContext.Entities
        .AsNoTracking()              // ← REQUIRED: Early in chain
        .Include(e => e.Related1)
        .Include(e => e.Related2)
        .AsSplitQuery()              // ← Works with .AsNoTracking()
        .Where(e => e.Id == id)
        .ToListAsync(ct);
}

// Write operations: NO .AsNoTracking()
public async Task<Entity> CreateAsync(Entity entity, CancellationToken ct)
{
    _dbContext.Entities.Add(entity);  // ← Tracked by default
    await _dbContext.SaveChangesAsync(ct);
    return entity;
}
```

### Rule 2.4: Performance Implications

**Benefits of .AsNoTracking():**
- ✅ 40% memory reduction (no change tracking state)
- ✅ 30% query performance improvement
- ✅ 70-80% reduction in GC pause times
- ✅ Clear intent: "This is read-only"

**Verification in Code Review:**
- Every read-only query must have .AsNoTracking()
- No exception (verify during pull request)
- Performance regression if missing

---

## ⚡ PRINCIPLE 3: Cache Repository Pattern for Static/Reference Data

### Rule Set: Cache-Backed Repository Usage (MUST BE FOLLOWED)

**Core Rule:**
```
For static, reference, or frequently-read lookup data, implement a cache-backed repository wrapper that:
1. Inherits from the base repository implementation,
2. Uses IMemoryCache for read-only access,
3. Falls back to the underlying repository when cache is disabled or unavailable,
4. Preserves the repository abstraction used by the application layer.
```

### Rule 3.1: When to Use a Cached Repository

**✅ Use a cached repository for:**
- Appointment status lookups
- Service status lookups
- Time-slot definitions
- Any static reference data that changes infrequently
- Lookup tables used in repeated read-only operations

**❌ Do not use a cached repository for:**
- Frequently changing transactional data
- User-specific or tenant-specific mutable records
- Data that must always reflect the latest DB state immediately
- Large data sets that are not read frequently enough to justify cache overhead

### Rule 3.2: Required Structure

Every cache repository for a static entity MUST follow this pattern:

```csharp
public class CachedSomethingRepository : SomethingRepository
{
    private readonly IMemoryCache _memoryCache;
    private readonly StaticDataCacheOptions _options;
    private readonly ILogger<CachedSomethingRepository> _logger;

    public CachedSomethingRepository(
        IApplicationDbContext dbContext,
        IMemoryCache memoryCache,
        IOptions<StaticDataCacheOptions> options,
        ILogger<CachedSomethingRepository> logger)
        : base(dbContext)
    {
        _memoryCache = memoryCache;
        _options = options.Value;
        _logger = logger;
    }

    private async Task<IReadOnlyList<Something>> GetAllCachedAsync(CancellationToken cancellationToken)
    {
        return await CachedQueryHelper.GetAllCachedAsync(
            _memoryCache,
            _options,
            _logger,
            StaticCacheKeys.SomeThingAll,
            _options.CacheSomething,
            _options.SomethingTtlMinutes,
            ct => base.GetAllAsync(cancellationToken: ct),
            "Something cache read failed; falling back to database.",
            cancellationToken);
    }

    public override async Task<IEnumerable<Something>> GetAllAsync(
        Expression<Func<Something, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        var all = await GetAllCachedAsync(cancellationToken);
        if (predicate == null)
        {
            return all;
        }

        var compiled = predicate.Compile();
        return all.Where(compiled).ToList();
    }

    public override async Task<Something?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var all = await GetAllCachedAsync(cancellationToken);
        return all.FirstOrDefault(x => x.Id == id);
    }
}
```

### Rule 3.3: Design Requirements

**Implementation requirements:**
- ✅ The cached repository MUST inherit from the non-cached repository implementation.
- ✅ The cached repository MUST preserve the interface contract expected by the application layer.
- ✅ Read operations SHOULD be served from memory cache whenever enabled.
- ✅ Cache failures MUST fall back to the underlying repository without breaking the application.
- ✅ Cache keys MUST be centralized in the infrastructure caching layer.
- ✅ Cache TTL values MUST be configurable through options.
- ✅ Shared cache-loading logic SHOULD be centralized in a reusable helper.

### Rule 3.4: Registration Rules

**DI registration requirements:**
- ✅ Replace the base repository implementation with the cached implementation in dependency injection for static lookup repositories.
- ✅ The application layer MUST continue to depend only on the repository interface, not the concrete cache implementation.

```csharp
// ✅ Correct
services.AddScoped<IAppointmentStatusLookupRepository, CachedAppointmentStatusLookupRepository>();
```

### Rule 3.5: Architectural Intent

Cached repositories are an infrastructure concern, not an application-layer concern. They exist to optimize read access to stable reference data while preserving the same repository abstraction and keeping service-layer behavior unchanged.

---

## ✅ PRINCIPLE 4: Multi-Layer Validation Strategy

### Rule Set: Input & Data Validation (MUST BE FOLLOWED)

**Core Rule:**
```
Validation occurs at THREE layers:
1. MIDDLEWARE: Global request validation
2. CONTROLLER: FluentValidation
3. SERVICE: Business logic validation
```

### Rule 3.1: Middleware Layer (Global Validation)

**Location:** `Middleware/ValidationMiddleware.cs`

**Responsibilities:**
- ✅ Content-Type validation (application/json)
- ✅ Request body presence validation
- ✅ JSON parsing validation
- ✅ Size limits validation

**Implementation:**
```csharp
public class ValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ValidationMiddleware> _logger;

    public ValidationMiddleware(RequestDelegate next, ILogger<ValidationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Validate content type
        if (context.Request.Method == "POST" || context.Request.Method == "PUT")
        {
            if (!context.Request.ContentType?.Contains("application/json") ?? true)
            {
                context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
                await context.Response.WriteAsJsonAsync(new { message = "Invalid Content-Type" });
                return;
            }
        }

        try
        {
            await _next(context);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning($"Invalid JSON: {ex.Message}");
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { message = "Invalid JSON format" });
        }
    }
}
```

**Registration in Program.cs:**
```csharp
app.UseMiddleware<ValidationMiddleware>();
```

### Rule 3.2: Controller Layer (FluentValidation)

**Location:** `Controllers/` and `Configuration/Validators/`

**Responsibilities:**
- ✅ DTO structure validation
- ✅ Required fields validation
- ✅ Format validation (dates, emails, etc.)
- ✅ Range validation (dates, numbers)
- ✅ Enum validation

**Implementation:**

```csharp
// CreateAppointmentRequestValidator.cs
public class CreateAppointmentRequestValidator : AbstractValidator<CreateAppointmentRequest>
{
    public CreateAppointmentRequestValidator()
    {
        // ✅ Required fields
        RuleFor(x => x.DealershipId)
            .NotEmpty()
            .WithMessage("Dealership ID is required");

        RuleFor(x => x.VehicleId)
            .NotEmpty()
            .WithMessage("Vehicle ID is required");

        // ✅ Date validation
        RuleFor(x => x.AppointmentDate)
            .NotEmpty()
            .WithMessage("Appointment date is required")
            .Must(date => date > DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Appointment date must be in the future");

        // ✅ Service type validation
        RuleFor(x => x.ServiceTypeId)
            .NotEmpty()
            .WithMessage("Service type is required");

        // ✅ Technician validation
        RuleFor(x => x.TechnicianId)
            .NotEmpty()
            .WithMessage("Technician is required");

        // ✅ Service bay validation
        RuleFor(x => x.ServiceBayId)
            .NotEmpty()
            .WithMessage("Service bay is required");
    }
}
```

**Controller Usage:**

```csharp
[HttpPost("appointments")]
public async Task<IActionResult> CreateAppointment(
    [FromBody] CreateAppointmentRequest request,
    CancellationToken cancellationToken)
{
    // ✅ Validate using FluentValidation
    var validationResult = await _validator.ValidateAsync(request, cancellationToken);
    if (!validationResult.IsValid)
    {
        return BadRequest(new { errors = validationResult.Errors });
    }

    // ✅ Call service (trust validation passed)
    var result = await _appointmentService.CreateAppointmentAsync(request, cancellationToken);
    return Created($"/appointments/{result.Id}", _mapper.Map<AppointmentDto>(result));
}
```

**Registration in Program.cs:**
```csharp
// Auto-register all validators
builder.Services.AddValidatorsFromAssemblyContaining<CreateAppointmentRequestValidator>();
```

### Rule 3.3: Service Layer (Business Logic Validation)

**Location:** `Services/`

**Responsibilities:**
- ✅ Business rule validation
- ✅ Cross-aggregate validation
- ✅ State validation
- ✅ Permission/authorization validation

**Implementation:**

```csharp
public async Task<Appointment> CreateAppointmentAsync(
    CreateAppointmentRequest request,
    CancellationToken cancellationToken)
{
    // ✅ Business validation 1: Availability check
    var availableSlots = await _availabilityService.GetAvailableSlotsAsync(
        request.DealershipId, request.ServiceTypeId, 
        request.AppointmentDate.ToDateTime(TimeOnly.MinValue), 
        cancellationToken);

    if (!availableSlots.Any())
        throw new InvalidOperationException("No available slots for this date and service type");

    // ✅ Business validation 2: Slot availability
    var requestedSlotAvailable = availableSlots.Any(slot =>
        slot.TechnicianId == request.TechnicianId && 
        slot.ServiceBayId == request.ServiceBayId);
    
    if (!requestedSlotAvailable)
        throw new InvalidOperationException("Requested slot is not available");

    // ✅ Business validation 3: Technician skill
    var hasSkill = await _appointmentRepository.TechnicianHasSkillAsync(
        request.TechnicianId, request.ServiceTypeId, cancellationToken);
    
    if (!hasSkill)
        throw new InvalidOperationException("Technician lacks required skill for this service");

    // ✅ Business validation 4: Conflict detection
    await CheckVehicleConflictsAsync(request, cancellationToken);

    // ✅ All validations passed - create appointment
    var appointment = new Appointment
    {
        Id = Guid.NewGuid(),
        DealershipId = request.DealershipId,
        VehicleId = request.VehicleId,
        Status = AppointmentStatus.Pending,
        CreatedAt = DateTime.UtcNow,
        Services = new List<Service>
        {
            new Service
            {
                Id = Guid.NewGuid(),
                ServiceTypeId = request.ServiceTypeId,
                TechnicianId = request.TechnicianId,
                ServiceBayId = request.ServiceBayId,
                EstimatedStartTimeSlotId = request.EstimatedStartTimeSlotId,
                EstimatedEndTimeSlotId = request.EstimatedEndTimeSlotId
            }
        }
    };

    return await _appointmentRepository.CreateAppointmentWithServicesAsync(
        appointment, cancellationToken);
}
```

### Rule 3.4: Validation Pattern Summary

```
Request Validation Flow
┌──────────────────────────────────────┐
│ 1. MIDDLEWARE                        │
│    ├─ Content-Type check             │
│    ├─ JSON parsing                   │
│    └─ Size limits                    │
└──────────────────────────────────────┘
              ↓
┌──────────────────────────────────────┐
│ 2. CONTROLLER (FluentValidation)     │
│    ├─ Required fields                │
│    ├─ Format validation              │
│    ├─ Range validation               │
│    └─ Return 400 if invalid          │
└──────────────────────────────────────┘
              ↓
┌──────────────────────────────────────┐
│ 3. SERVICE (Business Logic)          │
│    ├─ Availability check             │
│    ├─ Permission check               │
│    ├─ State validation               │
│    └─ Throw exception if invalid     │
└──────────────────────────────────────┘
              ↓
        Database Operation
```

---

## 🧪 PRINCIPLE 4: Test Data Builders (NOT Manual Data)

### Rule Set: Unit & Integration Testing (MUST BE FOLLOWED)

**Core Rule:**
```
Test data MUST be created using Builders
Manual data in test methods is NOT allowed
Builders ensure: Consistency, Reusability, Maintainability
```

### Rule 4.1: Entity Builder Pattern

**Builder Location:** `Tests/VehicleServiceBooking.Tests/Builders/`

**Example: AppointmentBuilder**

```csharp
public class AppointmentBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _dealershipId = Guid.NewGuid();
    private Guid _vehicleId = Guid.NewGuid();
    private AppointmentStatus _status = AppointmentStatus.Pending;
    private DateTime _createdAt = DateTime.UtcNow;
    private List<Service> _services = new();

    public AppointmentBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public AppointmentBuilder WithDealership(Guid dealershipId)
    {
        _dealershipId = dealershipId;
        return this;
    }

    public AppointmentBuilder WithVehicle(Guid vehicleId)
    {
        _vehicleId = vehicleId;
        return this;
    }

    public AppointmentBuilder WithStatus(AppointmentStatus status)
    {
        _status = status;
        return this;
    }

    public AppointmentBuilder WithService(Service service)
    {
        _services.Add(service);
        return this;
    }

    public AppointmentBuilder WithServices(params Service[] services)
    {
        _services.AddRange(services);
        return this;
    }

    public Appointment Build()
    {
        return new Appointment
        {
            Id = _id,
            DealershipId = _dealershipId,
            VehicleId = _vehicleId,
            Status = _status,
            CreatedAt = _createdAt,
            Services = _services
        };
    }

    public static AppointmentBuilder Default()
    {
        return new AppointmentBuilder();
    }
}

public class ServiceBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _appointmentId = Guid.NewGuid();
    private Guid _serviceTypeId = Guid.NewGuid();
    private Guid _technicianId = Guid.NewGuid();
    private Guid _serviceBayId = Guid.NewGuid();
    private Guid? _estimatedStartTimeSlotId = Guid.NewGuid();
    private Guid? _estimatedEndTimeSlotId = Guid.NewGuid();

    // Fluent methods...

    public Service Build()
    {
        return new Service
        {
            Id = _id,
            AppointmentId = _appointmentId,
            ServiceTypeId = _serviceTypeId,
            TechnicianId = _technicianId,
            ServiceBayId = _serviceBayId,
            EstimatedStartTimeSlotId = _estimatedStartTimeSlotId,
            EstimatedEndTimeSlotId = _estimatedEndTimeSlotId
        };
    }

    public static ServiceBuilder Default()
    {
        return new ServiceBuilder();
    }
}
```

### Rule 4.2: Usage in Tests (NOT Manual Data)

**❌ WRONG: Manual data in test method**

```csharp
[Test]
public async Task CreateAppointment_WithValidRequest_ReturnsCreatedAppointment()
{
    // ❌ WRONG: Creating data manually in test
    var appointment = new Appointment
    {
        Id = Guid.NewGuid(),
        DealershipId = Guid.NewGuid(),
        VehicleId = Guid.NewGuid(),
        Status = AppointmentStatus.Pending,
        CreatedAt = DateTime.UtcNow,
        Services = new List<Service>
        {
            new Service
            {
                Id = Guid.NewGuid(),
                ServiceTypeId = Guid.NewGuid(),
                TechnicianId = Guid.NewGuid(),
                ServiceBayId = Guid.NewGuid(),
                EstimatedStartTimeSlotId = Guid.NewGuid(),
                EstimatedEndTimeSlotId = Guid.NewGuid()
            }
        }
    };

    // Test logic...
}
```

**✅ CORRECT: Using Builder**

```csharp
[Test]
public async Task CreateAppointment_WithValidRequest_ReturnsCreatedAppointment()
{
    // ✅ CORRECT: Using Builder for consistent, readable test data
    var appointment = AppointmentBuilder.Default()
        .WithDealership(_dealershipId)
        .WithVehicle(_vehicleId)
        .WithService(ServiceBuilder.Default()
            .WithServiceType(_serviceTypeId)
            .WithTechnician(_technicianId)
            .Build())
        .Build();

    // Test logic...
    var result = await _repository.AddAsync(appointment, CancellationToken.None);

    // Assert
    Assert.IsNotNull(result);
    Assert.AreEqual(appointment.Id, result.Id);
}
```

### Rule 4.3: Builder Advantages

| Aspect | Manual Data | Builder |
|--------|------------|---------|
| **Consistency** | ❌ Different in each test | ✅ Always same defaults |
| **Readability** | ❌ Verbose setup code | ✅ Clear intent |
| **Maintainability** | ❌ Update all tests | ✅ Update builder once |
| **Reusability** | ❌ Copy-paste | ✅ Reuse everywhere |
| **Defaults** | ❌ Manual every time | ✅ Built-in defaults |
| **Variations** | ❌ Create new data | ✅ Fluent API |

---

## 🎯 PRINCIPLE 5: Entity Configuration in OnModelCreating()

### Rule Set: Entity Mapping & Configuration (MUST BE FOLLOWED)

**Core Rule:**
```
ALL entity configurations MUST be in OnModelCreating()
NO column mappings or constraints outside DbContext
```

### Rule 5.1: Proper OnModelCreating() Structure

**Location:** `DbContext.cs` - OnModelCreating() method

**✅ Correct Implementation:**

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // ✅ Configure Appointment entity
    modelBuilder.Entity<Appointment>(entity =>
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("Id")
            .HasColumnType("UNIQUEIDENTIFIER")
            .HasDefaultValueSql("NEWID()");

        entity.Property(e => e.DealershipId)
            .IsRequired()
            .HasColumnName("DealershipId");

        entity.Property(e => e.VehicleId)
            .IsRequired()
            .HasColumnName("VehicleId");

        entity.Property(e => e.Status)
            .IsRequired()
            .HasColumnName("Status")
            .HasConversion<string>();

        entity.Property(e => e.CreatedAt)
            .IsRequired()
            .HasColumnName("CreatedAt")
            .HasColumnType("DATETIME2")
            .HasDefaultValueSql("GETUTCDATE()");

        entity.Property(e => e.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("DATETIME2");

        // Foreign keys
        entity.HasOne(e => e.Dealership)
            .WithMany(d => d.Appointments)
            .HasForeignKey(e => e.DealershipId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.Vehicle)
            .WithMany(v => v.Appointments)
            .HasForeignKey(e => e.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationships
        entity.HasMany(e => e.Services)
            .WithOne(s => s.Appointment)
            .HasForeignKey(s => s.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        entity.HasIndex(e => e.DealershipId)
            .HasDatabaseName("IX_Appointments_DealershipId");

        entity.HasIndex(e => e.VehicleId)
            .HasDatabaseName("IX_Appointments_VehicleId");

        entity.HasIndex(e => new { e.DealershipId, e.CreatedAt })
            .HasDatabaseName("IX_Appointments_DealershipId_CreatedAt");
    });

    // ✅ Configure Service entity
    modelBuilder.Entity<Service>(entity =>
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.ServiceTypeId)
            .IsRequired()
            .HasColumnName("ServiceTypeId");

        entity.Property(e => e.TechnicianId)
            .IsRequired()
            .HasColumnName("TechnicianId");

        entity.Property(e => e.ServiceBayId)
            .IsRequired()
            .HasColumnName("ServiceBayId");

        entity.Property(e => e.EstimatedStartTimeSlotId)
            .HasColumnName("EstimatedStartTimeSlotId");

        entity.Property(e => e.EstimatedEndTimeSlotId)
            .HasColumnName("EstimatedEndTimeSlotId");

        // Foreign keys
        entity.HasOne(e => e.ServiceType)
            .WithMany()
            .HasForeignKey(e => e.ServiceTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        entity.HasIndex(e => e.TechnicianId)
            .HasDatabaseName("IX_Services_TechnicianId");
    });

    // ✅ Configure other entities similarly...
}
```

### Rule 5.2: Configuration Best Practices

**✅ DO: Use Fluent API ONLY in OnModelCreating()**

```csharp
// ✅ CORRECT: Fluent API in OnModelCreating()
modelBuilder.Entity<Customer>()
    .Property(e => e.FirstName)
    .HasMaxLength(100)
    .IsRequired();

modelBuilder.Entity<Customer>()
    .Property(e => e.LastName)
    .HasMaxLength(100)
    .IsRequired();

modelBuilder.Entity<Customer>()
    .Property(e => e.Email)
    .HasMaxLength(254)
    .IsRequired();

modelBuilder.Entity<Customer>()
    .Property(e => e.PhoneNumber)
    .HasMaxLength(20)
    .IsRequired();

// ✅ Configure relationships clearly
entity.HasOne(e => e.Dealership)
    .WithMany(d => d.Appointments)
    .HasForeignKey(e => e.DealershipId)
    .OnDelete(DeleteBehavior.Restrict);

// ✅ Add meaningful indexes
entity.HasIndex(e => new { e.DealershipId, e.CreatedAt })
    .HasDatabaseName("IX_Appointments_DealershipId_CreatedAt");

// ✅ Use database-specific settings
entity.Property(e => e.CreatedAt)
    .HasColumnType("DATETIME2")
    .HasDefaultValueSql("GETUTCDATE()");
```

**❌ DON'T: Use Data Annotations in Entity Classes**

```csharp
// ❌ WRONG: Data annotations in entity class
public class Customer : BaseEntity
{
    [MaxLength(100)]
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [MaxLength(254)]
    [EmailAddress]
    [Required]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    [Phone]
    [Required]
    public string PhoneNumber { get; set; } = string.Empty;
}

// ❌ WRONG: Configuration spread across multiple places
// ❌ WRONG: Configuration in migrations
// ❌ WRONG: Hardcoded column names outside OnModelCreating()
// ❌ WRONG: Mixing data annotations with Fluent API
```

**Why:** Centralizing all configuration in `OnModelCreating()` makes the database schema single-source-of-truth and easy to audit.

### Rule 5.3: Validation in OnModelCreating()

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // ✅ Add check constraints for valid values
    modelBuilder.Entity<Appointment>()
        .HasCheckConstraint("CK_Appointment_ValidStatus",
            "Status IN ('Pending', 'Confirmed', 'Completed', 'Cancelled')");

    // ✅ Add unique constraints
    modelBuilder.Entity<TechnicianSkill>()
        .HasIndex(e => new { e.TechnicianId, e.ServiceTypeId })
        .IsUnique()
        .HasDatabaseName("UX_TechnicianSkills_TechnicianId_ServiceTypeId");

    // ✅ Add default values
    modelBuilder.Entity<Appointment>()
        .Property(e => e.CreatedAt)
        .HasDefaultValueSql("GETUTCDATE()");
}
```

---

## 🏛️ PRINCIPLE 6: Additional Architecture Standards

### Rule Set: General Best Practices (MUST BE FOLLOWED)

### Rule 6.1: Dependency Injection

**✅ CORRECT: Constructor injection with interfaces**

```csharp
public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAvailabilityService _availabilityService;
    private readonly ILogger<AppointmentService> _logger;

    public AppointmentService(
        IAppointmentRepository appointmentRepository,
        IAvailabilityService availabilityService,
        ILogger<AppointmentService> logger)
    {
        _appointmentRepository = appointmentRepository ?? 
            throw new ArgumentNullException(nameof(appointmentRepository));
        _availabilityService = availabilityService ?? 
            throw new ArgumentNullException(nameof(availabilityService));
        _logger = logger;
    }
}
```

**❌ WRONG: Service locator pattern**

```csharp
// ❌ WRONG: Using static service locator
var service = ServiceLocator.GetService<IAppointmentService>();

// ❌ WRONG: Using new keyword for services
var repository = new AppointmentRepository(_dbContext);
```

### Rule 6.2: Async/Await Patterns

**✅ CORRECT: Async all the way**

```csharp
public async Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
{
    return await _dbContext.Appointments
        .AsNoTracking()
        .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
}

public async Task<Appointment> CreateAsync(
    Appointment appointment, CancellationToken cancellationToken)
{
    _dbContext.Appointments.Add(appointment);
    await _dbContext.SaveChangesAsync(cancellationToken);
    return appointment;
}
```

**❌ WRONG: Blocking calls**

```csharp
// ❌ WRONG: Blocking async calls
var appointment = _dbContext.Appointments
    .FirstOrDefaultAsync(a => a.Id == id)
    .Result;  // ← BLOCKS THREAD!

// ❌ WRONG: Mixed sync/async
public Appointment GetById(Guid id)  // ← Not async
{
    return _dbContext.Appointments
        .FirstOrDefaultAsync(a => a.Id == id)  // ← Async not awaited
        .Result;  // ← Blocks
}
```

### Rule 6.3: CancellationToken Usage

**✅ CORRECT: Propagate cancellation tokens**

```csharp
// ✅ Pass cancellationToken through call chain
public async Task<Appointment> CreateAppointmentAsync(
    CreateAppointmentRequest request,
    CancellationToken cancellationToken)
{
    var availability = await _availabilityService.GetAvailableSlotsAsync(
        request.DealershipId, request.ServiceTypeId, date, cancellationToken);  // ← Pass token

    var hasSkill = await _appointmentRepository.TechnicianHasSkillAsync(
        request.TechnicianId, request.ServiceTypeId, cancellationToken);  // ← Pass token

    return await _appointmentRepository.CreateAppointmentWithServicesAsync(
        appointment, cancellationToken);  // ← Pass token
}
```

**❌ WRONG: Ignoring cancellation tokens**

```csharp
// ❌ WRONG: Creating default cancellation token
var result = await _repository.GetByIdAsync(id, CancellationToken.None);

// ❌ WRONG: Not passing token through call chain
var availability = await _availabilityService.GetAvailableSlotsAsync(
    dealershipId, serviceTypeId, date);  // ← No token parameter!
```

### Rule 6.4: Exception Handling

**✅ CORRECT: Business exceptions in services, HTTP mapping in controllers**

```csharp
// Service: throws business exceptions
public async Task<Appointment> CreateAppointmentAsync(
    CreateAppointmentRequest request, CancellationToken cancellationToken)
{
    if (conflictExists)
        throw new InvalidOperationException("Vehicle already has conflicting appointment");

    if (!hasSkill)
        throw new InvalidOperationException("Technician lacks required skill");

    return await _appointmentRepository.CreateAppointmentWithServicesAsync(...);
}

// Controller: maps exceptions to HTTP responses
[HttpPost]
public async Task<IActionResult> CreateAppointment(
    [FromBody] CreateAppointmentRequest request,
    CancellationToken cancellationToken)
{
    try
    {
        var result = await _appointmentService.CreateAppointmentAsync(request, cancellationToken);
        return Created($"/appointments/{result.Id}", _mapper.Map<AppointmentDto>(result));
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(new { message = ex.Message });
    }
    catch (Exception ex)
    {
        _logger.LogError($"Unexpected error: {ex}");
        return StatusCode(500, new { message = "Internal server error" });
    }
}
```

**❌ WRONG: HTTP responses in services**

```csharp
// ❌ WRONG: Service returns HTTP status codes
public async Task<IActionResult> CreateAppointmentAsync(...)
{
    if (conflict)
        return BadRequest(...);  // ← Service shouldn't return HTTP!
}
```

### Rule 6.5: Logging

**✅ CORRECT: Structured logging at appropriate levels**

```csharp
public async Task<Appointment> CreateAppointmentAsync(
    CreateAppointmentRequest request,
    CancellationToken cancellationToken)
{
    _logger.LogInformation(
        "Creating appointment for vehicle {VehicleId} on dealership {DealershipId}",
        request.VehicleId, request.DealershipId);

    try
    {
        var result = await _appointmentRepository.CreateAppointmentWithServicesAsync(...);
        
        _logger.LogInformation(
            "Appointment {AppointmentId} created successfully",
            result.Id);

        return result;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex,
            "Failed to create appointment for vehicle {VehicleId}",
            request.VehicleId);
        throw;
    }
}
```

### Rule 6.6: DTOs vs Entities

**✅ CORRECT: DTOs for API contracts, Entities for database**

```csharp
// ✅ Entity: Used by repository and service
public class Appointment
{
    public Guid Id { get; set; }
    public Guid DealershipId { get; set; }
    public List<Service> Services { get; set; }
    // ... other properties
}

// ✅ DTO: Used for API request/response
public class CreateAppointmentRequest
{
    public Guid DealershipId { get; set; }
    public Guid VehicleId { get; set; }
    public DateOnly AppointmentDate { get; set; }
    // ... only what's needed for API
}

public class AppointmentDto
{
    public Guid Id { get; set; }
    public Guid DealershipId { get; set; }
    public List<ServiceDto> Services { get; set; }
    // ... mapped from entity
}

// ✅ Controller: Maps DTOs to/from entities
[HttpPost]
public async Task<IActionResult> CreateAppointment(
    [FromBody] CreateAppointmentRequest request)
{
    var appointment = await _appointmentService.CreateAppointmentAsync(request, cancellationToken);
    return Ok(_mapper.Map<AppointmentDto>(appointment));  // ← Map to DTO
}
```

**❌ WRONG: Using entities directly in APIs**

```csharp
// ❌ WRONG: Returning entity directly
[HttpPost]
public async Task<IActionResult> CreateAppointment(
    [FromBody] Appointment appointment)  // ← Entity as DTO!
{
    return Ok(appointment);  // ← Exposes internal structure
}
```

---

## 🏭 PRINCIPLE 7: Generic Repository Pattern & Derived Repositories

### Rule Set: Repository Inheritance (MUST BE FOLLOWED)

**Core Rule:**
```
1. ALL entity repositories MUST inherit from GenericRepository<TEntity>
2. Repository interfaces MUST use split contracts:
    - Static-table repositories inherit IReadRepository<TEntity> only
    - Non-static repositories inherit IReadRepository<TEntity> + IWriteRepository<TEntity>
3. Base contract hierarchy MUST be preserved: IRepository<TEntity> -> IReadRepository<TEntity>/IWriteRepository<TEntity>
4. Common multi-id reads (GetByIdsAsync) MUST be defined once in IReadRepository<TEntity>
5. CRUD operations inherited from base class MUST NOT be overridden
6. Entity-specific queries kept in derived class
7. GetQueryable() MUST be used for all read operations in derived class
```

### Rule 7.1: GenericRepository<TEntity> Base Class

**Location:** `Infrastructure/Repositories/GenericRepository.cs`

**Purpose:** Provide shared, reusable CRUD operations for all entity repositories

**✅ Implementation Template:**

```csharp
public abstract class GenericRepository<TEntity> 
    where TEntity : class
{
    // ✅ Protected DbContext access via IApplicationDbContext interface
    protected IApplicationDbContext DbContext { get; }

    public GenericRepository(IApplicationDbContext dbContext)
    {
        DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// Gets an IQueryable for the entity with .AsNoTracking() automatically applied.
    /// This is the primary method for building read queries in derived classes.
    /// </summary>
    protected IQueryable<TEntity> GetQueryable()
    {
        return DbContext.DbContext.Set<TEntity>().AsNoTracking();
    }

    /// <summary>
    /// Gets entity by ID with async support and automatic .AsNoTracking().
    /// Override in derived class to add entity-specific includes.
    /// </summary>
    public virtual async Task<TEntity?> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        return await GetQueryable()
            .FirstOrDefaultAsync(entity => EF.Property<Guid>(entity, "Id") == id, cancellationToken);
    }

    /// <summary>
    /// Gets entities by primary key collection.
    /// This is a common read operation and MUST live in IReadRepository<TEntity>.
    /// </summary>
    public virtual async Task<IEnumerable<TEntity>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0)
            return Array.Empty<TEntity>();

        return await GetQueryable()
            .Where(entity => idList.Contains(EF.Property<Guid>(entity, "Id")))
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets first entity matching predicate with .AsNoTracking().
    /// Use for simple existence checks and lookups.
    /// </summary>
    public async Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        return await GetQueryable()
            .FirstOrDefaultAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Checks if any entity matches predicate with .AsNoTracking().
    /// Use for existence checks and validation.
    /// </summary>
    public async Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await GetQueryable()
            .AnyAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Gets all entities matching optional predicate with .AsNoTracking().
    /// Use for retrieving collections with optional filtering.
    /// </summary>
    public async Task<List<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        var query = GetQueryable();
        
        if (predicate != null)
            query = query.Where(predicate);
        
        return await query.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets all entities with pagination support.
    /// Use for list endpoints with skip/take parameters.
    /// </summary>
    public async Task<(List<TEntity> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize,
        Expression<Func<TEntity, bool>>? predicate = null,
        Expression<Func<TEntity, object>>? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        var query = GetQueryable();
        
        if (predicate != null)
            query = query.Where(predicate);
        
        var totalCount = await query.CountAsync(cancellationToken);
        
        if (orderBy != null)
            query = query.OrderBy(orderBy);
        
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        return (items, totalCount);
    }

    /// <summary>
    /// Executes custom query and projects to result type with .AsNoTracking().
    /// Use for complex queries returning non-entity types.
    /// </summary>
    public async Task<List<TResult>> ExecuteQueryAsync<TResult>(
        Func<IQueryable<TEntity>, IQueryable<TResult>> queryBuilder,
        CancellationToken cancellationToken = default)
    {
        var query = queryBuilder(GetQueryable());
        return await query.ToListAsync(cancellationToken);
    }

    // ═══════════════════════════════════════════════════════════════════════
    // WRITE OPERATIONS (CUD - Create, Update, Delete)
    // ═══════════════════════════════════════════════════════════════════════
    // ⚠️ These operations MUST NOT use .AsNoTracking()
    // ⚠️ Change tracking is REQUIRED for write operations

    /// <summary>
    /// Adds single entity to database context (not saved until SaveChangesAsync).
    /// </summary>
    public virtual async Task<TEntity> AddAsync(
        TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbContext.DbContext.Set<TEntity>().AddAsync(entity, cancellationToken);
        return entity;
    }

    /// <summary>
    /// Adds multiple entities to database context (not saved until SaveChangesAsync).
    /// Use for bulk inserts when creating multiple related entities.
    /// </summary>
    public virtual async Task AddRangeAsync(
        IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await DbContext.DbContext.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
    }

    /// <summary>
    /// Updates entity (must be already tracked or attached).
    /// Typically called after querying and modifying entity properties.
    /// </summary>
    public virtual Task<TEntity> UpdateAsync(
        TEntity entity, CancellationToken cancellationToken = default)
    {
        DbContext.DbContext.Set<TEntity>().Update(entity);
        return Task.FromResult(entity);
    }

    /// <summary>
    /// Deletes entity from database (must be tracked).
    /// Can pass entity instance or ID (lookup required).
    /// </summary>
    public virtual async Task DeleteAsync(
        TEntity entity, CancellationToken cancellationToken = default)
    {
        DbContext.DbContext.Set<TEntity>().Remove(entity);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Deletes entity by ID (performs lookup then delete).
    /// Convenient when only ID is available.
    /// </summary>
    public virtual async Task DeleteAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            DbContext.DbContext.Set<TEntity>().Remove(entity);
        }
    }

    /// <summary>
    /// Saves all pending changes to database.
    /// Call after Add/Update/Delete operations to persist changes.
    /// Returns number of entities affected.
    /// </summary>
    public virtual async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return await DbContext.SaveChangesAsync(cancellationToken);
    }
}
```

### Rule 7.2: Derived Repository Implementation

**✅ CORRECT: Proper Derived Repository Pattern**

```csharp
/// <summary>
/// Appointment repository providing entity-specific queries.
/// Inherits common CRUD operations from GenericRepository{Appointment}.
/// </summary>
public class AppointmentRepository : 
    GenericRepository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(IApplicationDbContext dbContext) 
        : base(dbContext)
    {
    }

    // ───────────────────────────────────────────────────────────────────────
    // ✅ Override base GetByIdAsync to add entity-specific includes
    // ───────────────────────────────────────────────────────────────────────
    public override async Task<Appointment?> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        return await GetQueryable()  // ← .AsNoTracking() automatically applied
            .Include(a => a.Services)  // ← Entity-specific include
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    // ───────────────────────────────────────────────────────────────────────
    // ✅ Entity-specific queries using GetQueryable()
    // ───────────────────────────────────────────────────────────────────────
    public async Task<IEnumerable<Appointment>> GetByServiceBayAsync(
        Guid serviceBayId, CancellationToken cancellationToken)
    {
        return await GetQueryable()  // ← Inherits .AsNoTracking()
            .Where(a => a.Services.Any(s => s.ServiceBayId == serviceBayId))
            .Include(a => a.Services)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Appointment>> GetByDealershipAsync(
        Guid dealershipId, CancellationToken cancellationToken)
    {
        return await GetQueryable()  // ← Inherits .AsNoTracking()
            .Where(a => a.DealershipId == dealershipId)
            .Include(a => a.Services)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Appointment>> GetByVehicleIdAsync(
        Guid vehicleId, CancellationToken cancellationToken)
    {
        return await GetQueryable()  // ← Inherits .AsNoTracking()
            .Where(a => a.VehicleId == vehicleId)
            .Include(a => a.Services)
            .ToListAsync(cancellationToken);
    }

    // ───────────────────────────────────────────────────────────────────────
    // ✅ Business logic queries
    // ───────────────────────────────────────────────────────────────────────
    public async Task<bool> IsBayAvailableForSlotAsync(
        Guid serviceBayId, Guid timeSlotId, CancellationToken cancellationToken)
    {
        return await GetQueryable()  // ← Inherits .AsNoTracking()
            .Where(a => a.Status != AppointmentStatus.Cancelled)
            .All(a => !a.Services.Any(s =>
                s.ServiceBayId == serviceBayId &&
                (s.EstimatedStartTimeSlotId == timeSlotId || 
                 s.EstimatedEndTimeSlotId == timeSlotId)),
                cancellationToken);
    }

    public async Task<bool> TechnicianHasSkillAsync(
        Guid technicianId, Guid serviceTypeId, CancellationToken cancellationToken)
    {
        return await DbContext.DbContext.Set<TechnicianSkill>()
            .AsNoTracking()  // ← Query different entity type
            .AnyAsync(ts =>
                ts.TechnicianId == technicianId &&
                ts.ServiceTypeId == serviceTypeId,
                cancellationToken);
    }

    // ───────────────────────────────────────────────────────────────────────
    // ✅ Complex write operations
    // ───────────────────────────────────────────────────────────────────────
    public async Task<Appointment> CreateAppointmentWithServicesAsync(
        Appointment appointment, CancellationToken cancellationToken)
    {
        await AddAsync(appointment, cancellationToken);  // ← Inherited from base
        await SaveChangesAsync(cancellationToken);  // ← Inherited from base
        return appointment;
    }

    // ✅ NOTE: No AddAsync, UpdateAsync, DeleteAsync, SaveChangesAsync overrides
    // ✅ These operations are inherited from GenericRepository<Appointment>
    // ✅ This eliminates ~30+ lines of duplicate code per repository
}
```

### Rule 7.3: GenericRepository vs Derived Repository Responsibilities

**GenericRepository<TEntity> Handles:**
- ✅ Common CRUD operations (Add, Update, Delete)
- ✅ Standard read operations (GetById, FirstOrDefault, Any, GetAll)
- ✅ Paging and pagination logic
- ✅ SaveChangesAsync coordination
- ✅ Automatic .AsNoTracking() application
- ✅ DbContext access management

**Derived Repository (e.g., AppointmentRepository) Handles:**
- ✅ Entity-specific query methods (GetByServiceBay, GetByDealership, GetByVehicleId)
- ✅ Complex business logic queries
- ✅ Overrides for entity-specific includes
- ✅ Cross-entity queries (e.g., TechnicianHasSkill accessing TechnicianSkill)
- ✅ Specialized create/update methods (e.g., CreateAppointmentWithServices)

### Rule 7.4: What Derived Repositories MUST NOT Do

**❌ WRONG: Re-implementing inherited CRUD methods**

```csharp
// ❌ VIOLATION: Duplicating inherited AddAsync
public class AppointmentRepository : GenericRepository<Appointment>
{
    // ❌ WRONG: Re-implementing inherited method
    public async Task<Appointment> AddAsync(
        Appointment appointment, CancellationToken cancellationToken)
    {
        _dbContext.Appointments.Add(appointment);
        return appointment;
    }

    // ❌ WRONG: Re-implementing inherited SaveChangesAsync
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    // ❌ WRONG: Re-implementing inherited DeleteAsync
    public async Task DeleteAsync(
        Appointment appointment, CancellationToken cancellationToken)
    {
        _dbContext.Appointments.Remove(appointment);
    }
}
```
**Action:** Code review rejects - eliminate duplicate methods

**❌ WRONG: Not using GetQueryable() for derived queries**

```csharp
// ❌ VIOLATION: Direct DbContext access instead of GetQueryable()
public async Task<IEnumerable<Appointment>> GetByServiceBayAsync(
    Guid serviceBayId, CancellationToken cancellationToken)
{
    return await DbContext.DbContext.Set<Appointment>()  // ← Wrong
        .AsNoTracking()  // ← Duplicate .AsNoTracking()
        .Where(a => a.Services.Any(s => s.ServiceBayId == serviceBayId))
        .ToListAsync(cancellationToken);
}
```
**Correct:**
```csharp
// ✅ CORRECT: Using GetQueryable() ensures .AsNoTracking() applied
public async Task<IEnumerable<Appointment>> GetByServiceBayAsync(
    Guid serviceBayId, CancellationToken cancellationToken)
{
    return await GetQueryable()  // ← Inherited .AsNoTracking()
        .Where(a => a.Services.Any(s => s.ServiceBayId == serviceBayId))
        .ToListAsync(cancellationToken);
}
```

**❌ WRONG: Not implementing derived repository interface**

```csharp
// ❌ VIOLATION: Inherits from GenericRepository but no explicit interface
public class AppointmentRepository : GenericRepository<Appointment>
{
    // Missing: IAppointmentRepository implementation
}

// ✅ CORRECT: Implements both base class and specific interface
public class AppointmentRepository : 
    GenericRepository<Appointment>, IAppointmentRepository
{
    // Declares domain-specific contract
}
```

### Rule 7.5: Repository Interface Inheritance Guidelines

**✅ CORRECT: Non-static repository interface (read + write + domain methods)**

```csharp
/// <summary>
/// Contract for Appointment data access operations.
/// Non-static aggregate: supports both reads and writes.
/// </summary>
public interface IAppointmentRepository : IReadRepository<Appointment>, IWriteRepository<Appointment>
{
    // Inherited read/write members come from IReadRepository/IWriteRepository.

    // ✅ Appointment-specific queries
    Task<IEnumerable<Appointment>> GetByServiceBayAsync(
        Guid serviceBayId, CancellationToken cancellationToken);

    Task<IEnumerable<Appointment>> GetByDealershipAsync(
        Guid dealershipId, CancellationToken cancellationToken);

    Task<IEnumerable<Appointment>> GetByVehicleIdAsync(
        Guid vehicleId, CancellationToken cancellationToken);

    Task<bool> IsBayAvailableForSlotAsync(
        Guid serviceBayId, Guid timeSlotId, CancellationToken cancellationToken);

    Task<bool> TechnicianHasSkillAsync(
        Guid technicianId, Guid serviceTypeId, CancellationToken cancellationToken);

    Task<Appointment> CreateAppointmentWithServicesAsync(
        Appointment appointment, CancellationToken cancellationToken);
}
```

**✅ CORRECT: Static-table repository interface (read-only contract)**

```csharp
/// <summary>
/// Contract for ServiceStatusLookup static table.
/// Static/lookup tables are read-only at application boundary.
/// </summary>
public interface IServiceStatusLookupRepository : IReadRepository<ServiceStatusLookup>
{
    Task<IReadOnlyList<ServiceStatusLookup>> GetAllAsync(CancellationToken cancellationToken);
    Task<ServiceStatusLookup?> GetByStatusAsync(ServiceStatus status, CancellationToken cancellationToken);
}
```

**❌ WRONG: Making mutable domain repositories read-only**

```csharp
// ❌ Customer is mutable domain data and must support writes
public interface ICustomerRepository : IReadRepository<Customer>
{
}
```

**✅ CORRECT:**

```csharp
public interface ICustomerRepository : IReadRepository<Customer>, IWriteRepository<Customer>
{
}
```

### Rule 7.6: Dependency Injection Registration

**✅ CORRECT: Register generic repositories in Program.cs**

```csharp
// Program.cs - Service Registration

// ✅ Register repository interface and implementation
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IServiceBayRepository, ServiceBayRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ITechnicianRepository, TechnicianRepository>();

// ✅ Static cache decorators are registered directly as interface -> cached implementation
builder.Services.AddScoped<ITimeSlotRepository, CachedTimeSlotRepository>();
builder.Services.AddScoped<IAppointmentStatusLookupRepository, CachedAppointmentStatusLookupRepository>();
builder.Services.AddScoped<IServiceStatusLookupRepository, CachedServiceStatusLookupRepository>();

// ✅ DbContext registered as scoped (required for repositories)
builder.Services.AddScoped<IApplicationDbContext>(provider =>
    provider.GetRequiredService<ApplicationDbContext>());

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
```

### Rule 7.7: Static-Entity Cache Decorator Pattern

**Core Rule:**
```
1. Caching is allowed ONLY for static/seeded entities.
2. Cached repositories SHOULD inherit concrete repository implementations,
   not re-implement all common IReadRepository methods.
3. Cached repositories SHOULD override only cache-specific read methods.
4. Mutable/transactional entities MUST NOT be cache-decorated at repository level.
```

**✅ CORRECT: Cached repository inheriting base repository chain**

```csharp
public class CachedServiceStatusLookupRepository : ServiceStatusLookupRepository
{
    public CachedServiceStatusLookupRepository(
        IApplicationDbContext dbContext,
        IMemoryCache memoryCache,
        IOptions<StaticDataCacheOptions> options,
        ILogger<CachedServiceStatusLookupRepository> logger)
        : base(dbContext)
    {
        // cache dependencies
    }

    // ✅ Override only cache-specific methods
    public override async Task<IEnumerable<ServiceStatusLookup>> GetAllAsync(
        Expression<Func<ServiceStatusLookup, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        // cache-aside
    }

    public override async Task<ServiceStatusLookup?> GetByStatusAsync(
        ServiceStatus status,
        CancellationToken cancellationToken)
    {
        // cache-aside
    }
}
```

**❌ WRONG: Re-implementing full IReadRepository surface in cache decorators**

```csharp
// ❌ Duplicating GetByIdAsync, FirstOrDefaultAsync, AnyAsync, GetByIdsAsync in each cached repo
public class CachedXRepository : IXRepository
{
    // full common method set duplicated here
}
```

**Action:** Use inheritance to reuse GenericRepository common behavior.

### Rule 7.8: Testing Generic Repository

**✅ CORRECT: Unit tests for GenericRepository base class**

```csharp
public class GenericRepositoryTests
{
    private Mock<IApplicationDbContext> _mockDbContext;
    private Mock<DbSet<TestEntity>> _mockDbSet;
    private GenericRepository<TestEntity> _repository;

    [SetUp]
    public void Setup()
    {
        _mockDbContext = new Mock<IApplicationDbContext>();
        _mockDbSet = new Mock<DbSet<TestEntity>>();
        _mockDbContext.Setup(c => c.DbContext.Set<TestEntity>())
            .Returns(_mockDbSet.Object);
        _repository = new GenericRepository<TestEntity>(_mockDbContext.Object);
    }

    [Test]
    public async Task GetByIdAsync_WithValidId_ReturnsEntity()
    {
        // ✅ Test that GetQueryable applies .AsNoTracking()
        var entity = EntityBuilder.Default().WithId(_testId).Build();
        _mockDbSet.Setup(s => s.AsNoTracking()).Returns(_mockDbSet.Object);
        _mockDbSet.Setup(s => s.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<TestEntity, bool>>>(), 
            It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        var result = await _repository.GetByIdAsync(_testId);

        Assert.IsNotNull(result);
        Assert.AreEqual(entity.Id, result.Id);
    }

    [Test]
    public async Task AddAsync_WithValidEntity_AddsToContext()
    {
        // ✅ Test that AddAsync properly adds entity
        var entity = EntityBuilder.Default().Build();

        await _repository.AddAsync(entity);

        _mockDbSet.Verify(s => s.Add(It.IsAny<TestEntity>()), Times.Once);
    }

    [Test]
    public async Task SaveChangesAsync_CallsSaveChangesAsync()
    {
        // ✅ Test that SaveChangesAsync delegates to DbContext
        _mockDbContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _repository.SaveChangesAsync();

        Assert.AreEqual(1, result);
        _mockDbContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
```

**✅ CORRECT: Integration tests for derived repositories**

```csharp
public class AppointmentRepositoryIntegrationTests
{
    private ApplicationDbContext _dbContext;
    private AppointmentRepository _repository;

    [SetUp]
    public void Setup()
    {
        // ✅ Use real database context for integration tests
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _dbContext = new ApplicationDbContext(options);
        _repository = new AppointmentRepository(_dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext?.Dispose();
    }

    [Test]
    public async Task CreateAppointmentWithServices_WithValidData_PersistsSuccessfully()
    {
        // ✅ Test derived repository complex operation
        var appointment = AppointmentBuilder.Default()
            .WithService(ServiceBuilder.Default().Build())
            .Build();

        var result = await _repository.CreateAppointmentWithServicesAsync(
            appointment, CancellationToken.None);

        Assert.IsNotNull(result.Id);
        // ✅ Verify entity was saved
        var persisted = await _repository.GetByIdAsync(result.Id);
        Assert.IsNotNull(persisted);
        Assert.AreEqual(1, persisted.Services.Count);
    }

    [Test]
    public async Task GetByServiceBayAsync_WithValidBayId_ReturnsFilteredAppointments()
    {
        // ✅ Test entity-specific derived query
        var bayId = Guid.NewGuid();
        var appointments = new[]
        {
            AppointmentBuilder.Default()
                .WithService(ServiceBuilder.Default().WithServiceBay(bayId).Build())
                .Build(),
            AppointmentBuilder.Default()
                .WithService(ServiceBuilder.Default().WithServiceBay(Guid.NewGuid()).Build())
                .Build()
        };

        foreach (var apt in appointments)
            await _repository.AddAsync(apt);
        await _repository.SaveChangesAsync();

        var result = await _repository.GetByServiceBayAsync(bayId, CancellationToken.None);

        Assert.AreEqual(1, result.Count());
        Assert.AreEqual(bayId, result.First().Services.First().ServiceBayId);
    }
}
```

### Rule 7.9: Benefits of Generic Repository Pattern

**Code Reduction:**
| Repository | Before | After | Reduction |
|------------|--------|-------|-----------|
| AppointmentRepository | 85 lines | 55 lines | -35% |
| ServiceBayRepository | 97 lines | 64 lines | -34% |
| CustomerRepository | ~90 lines | ~60 lines | -33% |
| **Total (10 repos)** | **~900 lines** | **~600 lines** | **-33%** |

**Quality Improvements:**
- ✅ Single source of truth for CRUD operations
- ✅ Consistent .AsNoTracking() application
- ✅ Automatic error handling and null checking
- ✅ Reduced boilerplate across codebase
- ✅ Easier to add new repositories (inherit → add specific methods)
- ✅ Easier to test (base class tested once, used everywhere)
- ✅ Easier to maintain (change once, affects all)

**Inheritance Chain:**
```
GenericRepository<TEntity>
        ↑
        │ implements
        │
┌───────┴─────────┬──────────┬──────────┬──────────┐
│                 │          │          │          │
AppointmentRepository  ServiceBayRepository  CustomerRepository  ...
        │                 │          │          │
        └──────────────┬──────────────┴──────────┘
                       │
        Each inherits common CRUD operations
        and adds entity-specific queries
```

### Rule 7.10: Migration Checklist

**When refactoring existing repositories to use GenericRepository:**

- [ ] Create GenericRepository<TEntity> base class
- [ ] Create/keep IRepository<TEntity> marker base and inherit it via IRead/IWrite
- [ ] Verify IApplicationDbContext has DbContext property
- [ ] Change repository class declaration: `public class XRepository : GenericRepository<X>, IXRepository`
- [ ] Remove _dbContext field (access via base class)
- [ ] Remove null-check from constructor
- [ ] Change constructor to call `base(dbContext)`
- [ ] Move GetByIdsAsync to IReadRepository<TEntity> + GenericRepository<TEntity> (if duplicated)
- [ ] Replace AddAsync, UpdateAsync, DeleteAsync, SaveChangesAsync if present (use inherited)
- [ ] Replace read queries to use GetQueryable() prefix
- [ ] Override GetByIdAsync if entity has includes (e.g., .Include(...))
- [ ] For static entities, prefer cached repository inheritance over full interface re-implementation
- [ ] Run tests: ensure all derived queries still work
- [ ] Build verification: 0 errors
- [ ] Code review: verify no CRUD duplication

---

## 📋 PRINCIPLE 8: Code Review Checklist

### Mandatory Verification Checklist

**Every pull request MUST pass these checks:**

- [ ] **Layer Separation**
  - Controllers only call Services (no Repository/DbContext access)
  - Services only call Repositories (no DbContext access)
  - Repositories only call DbContext (no business logic)

- [ ] **.AsNoTracking() Usage**
  - All read-only queries have .AsNoTracking()
  - Write operations do NOT have .AsNoTracking()
  - No exceptions to this rule

- [ ] **Validation Layers**
  - Middleware layer: Global validation present
  - Controller layer: FluentValidation used
  - Service layer: Business logic validation present

- [ ] **Test Data**
  - All test data uses Builders (no manual data)
  - Builders provide consistent defaults
  - Test setup is readable and maintainable

- [ ] **Entity Configuration**
  - All entity mappings in OnModelCreating()
  - Column names, types, relationships configured
  - Indexes defined for performance

- [ ] **Generic Repository Pattern**
  - All entity repositories inherit from GenericRepository<TEntity>
    - IReadRepository/IWriteRepository inherit from IRepository<TEntity>
    - GetByIdsAsync defined once in IReadRepository + GenericRepository (not per entity interface)
  - Derived repositories implement specific interface
  - No CRUD method re-implementation in derived classes
  - All read queries use GetQueryable()
  - GetByIdAsync properly overridden with includes (if needed)
  - No AddAsync, UpdateAsync, DeleteAsync duplication
    - Static cache repositories override cache-specific methods only (no full common-method duplication)

- [ ] **Dependency Injection**
  - Constructor injection with interfaces
  - No service locator pattern
  - All dependencies null-checked
    - Repositories registered as interface -> implementation with scoped lifetime
    - Static-table interfaces registered to cached implementations only

- [ ] **Async/Await**
  - All I/O operations are async
  - CancellationToken propagated throughout
  - No blocking calls (.Result, .Wait())

- [ ] **Exception Handling**
  - Business exceptions in services
  - HTTP mapping in controllers
  - Proper logging of errors

- [ ] **Naming Conventions**
  - PascalCase for public members
  - camelCase for parameters
  - Meaningful, self-documenting names

- [ ] **Documentation**
  - XML documentation on public APIs
  - Complex logic explained with comments
  - Architecture decisions documented

---

## 🚀 Enforcement & Violations

### What Happens When Rules Are Violated

**During Code Review:**
1. Reviewer identifies violation
2. Pull request is marked as "Changes Requested"
3. Developer must fix before merge is approved

**Examples of Violations:**

❌ **Violation 1: Repository accessed directly from Controller**
```csharp
[HttpPost]
public async Task<IActionResult> CreateAppointment(...)
{
    // ❌ VIOLATION: Direct repository access
    var appointment = await _repository.GetByIdAsync(id);
}
```
**Action:** PR rejected, must refactor to use Service layer

❌ **Violation 2: Missing .AsNoTracking() on read query**
```csharp
public async Task<Appointment?> GetByIdAsync(Guid id, CancellationToken ct)
{
    return await _dbContext.Appointments
        .Include(a => a.Services)
        // ❌ VIOLATION: No .AsNoTracking()
        .FirstOrDefaultAsync(a => a.Id == id, ct);
}
```
**Action:** PR rejected, must add .AsNoTracking()

❌ **Violation 3: Manual test data instead of Builder**
```csharp
[Test]
public async Task TestScenario()
{
    // ❌ VIOLATION: Manual data creation
    var appointment = new Appointment
    {
        Id = Guid.NewGuid(),
        // ... manually set properties
    };
}
```
**Action:** PR rejected, must use Builder pattern

❌ **Violation 4: Entity configuration outside OnModelCreating()**
```csharp
public class Appointment
{
    [Required]
    [Column("Id")]
    public Guid Id { get; set; }  // ❌ VIOLATION: Attributes in entity
}
```
**Action:** PR rejected, must move to OnModelCreating()

---

## 📚 Quick Reference Summary

| Principle | Rule | Enforcement |
|-----------|------|-------------|
| **Layer Separation** | Controller → Service → Repo → DbContext | STRICT |
| **.AsNoTracking()** | ALL read queries, NO write operations | STRICT |
| **Validation** | Middleware → Controller → Service | STRICT |
| **Test Data** | Builders only, no manual data | STRICT |
| **Entity Config** | OnModelCreating() only, no attributes | STRICT |
| **Generic Repository** | Inherit from GenericRepository<T>, no CRUD duplication | STRICT |
| **Dependency Injection** | Constructor injection with interfaces | STRICT |
| **Async/Await** | All I/O async, propagate CancellationToken | STRICT |
| **Exception Handling** | Business logic → Service, HTTP → Controller | STRICT |
| **DTOs vs Entities** | Entities internal, DTOs for APIs | STRICT |
| **Naming** | PascalCase/camelCase, meaningful names | STANDARD |

---

## 🎯 Conclusion

These principles define the **non-negotiable architecture** of the Vehicle Service Booking system. Strict adherence ensures:

✅ **Code Quality** - Consistent, maintainable codebase  
✅ **Performance** - Optimized queries with .AsNoTracking()  
✅ **Scalability** - Clean layer separation enables growth  
✅ **Testability** - Builders and DI make testing easy  
✅ **Reliability** - Proper validation and error handling  
✅ **Security** - Layered validation and permission checks  

**All developers MUST follow these principles. Code reviews will enforce compliance. Non-compliant code will NOT be merged.**

---

**Document Approved By:** Architecture Review Board  
**Effective Date:** June 26, 2026  
**Last Updated:** June 26, 2026  
**Next Review:** September 26, 2026

---

**Questions? Refer to supporting documentation or discuss with architecture team.** 🚀
