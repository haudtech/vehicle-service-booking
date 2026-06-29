# Phase 3 Implementation Roadmap - Core Features

**Status:** 🚀 Starting  
**Date:** June 25, 2026  
**Phase Duration:** Estimated 2-3 weeks  
**Reference:** Continuation of Phase 2 (Repository Pattern + CORS)

---

## 🎯 Phase 3 Overview

Phase 3 focuses on **core application features** required for a production-ready vehicle service booking system:

### Phase 3 Goals
1. ✅ Set up Entity Framework Core with database migrations
2. ✅ Implement JWT authentication and authorization
3. ✅ Create comprehensive API controller endpoints
4. ✅ Implement business logic for appointment workflow
5. ✅ Add data validation and error handling
6. ✅ Prepare for deployment and scaling

### Current State
- ✅ Clean Architecture foundation: Domain → Application → Infrastructure → API
- ✅ Repository Pattern for data access
- ✅ CORS middleware configured
- ✅ Interface organization optimized
- ✅ Build: 0 errors, 37 tests passing, 12 pre-existing failures deferred
- ✅ Test infrastructure with fluent builders in place

---

## 📋 Phase 3 Task Breakdown

### **TASK 1: Entity Framework Core & Database Setup**

**Priority:** 🔴 HIGH  
**Estimated Effort:** 4-6 hours  
**Files to Create/Modify:** 8-10  
**Complexity:** High (Database schema design)

#### Task 1.1: Entity Framework Core Configuration
```
Files to Create/Modify:
  - DbContext configuration enhancements
  - Entity mapping configurations (Fluent API)
  - Database connection string in appsettings
  - Migration initial setup

Scope:
  - Configure all 10+ domain entities
  - Set up relationships (1-to-many, many-to-many)
  - Add shadow properties for soft deletes (if needed)
  - Configure value converters for enums
```

#### Task 1.2: Create Initial Database Migration
```
Files to Create:
  - CreateInitialSchema migration file
  - Seed data scripts (optional)

Scope:
  - Create all database tables with proper constraints
  - Set up foreign keys and indexes
  - Add check constraints for business rules
  - Document migration decisions
```

#### Task 1.3: Connection String Management
```
Configuration:
  - Development: Local SQL Server or SQLite
  - Staging: Cloud database (Azure SQL / AWS RDS)
  - Production: Cloud database with backups

Scope:
  - Add connection string to appsettings.{Environment}.json
  - Add to .env files with secure patterns
  - Test connection in Program.cs startup
```

---

### **TASK 2: Authentication & Authorization**

**Priority:** 🔴 HIGH  
**Estimated Effort:** 5-7 hours  
**Files to Create/Modify:** 12-15  
**Complexity:** Very High (Security-critical)

#### Task 2.1: JWT Authentication Setup
```
Files to Create:
  - JwtOptions.cs - JWT configuration class
  - IJwtTokenProvider.cs - Token generation interface
  - JwtTokenProvider.cs - Token generation implementation
  - AuthenticationMiddleware.cs - Extract token from headers
  - AuthenticationService.cs - Login/token validation

Scope:
  - Configure JWT issuer, audience, secret key
  - Create access tokens (short-lived, 15-30 min)
  - Create refresh tokens (long-lived, 7 days)
  - Token validation and expiration checks
  - Secure secret key management
```

#### Task 2.2: Authentication Controllers
```
Files to Create:
  - AuthController.cs with endpoints:
    • POST /api/v1/auth/login - Issue tokens
    • POST /api/v1/auth/refresh - Refresh access token
    • POST /api/v1/auth/logout - Revoke tokens (optional)

Models to Create:
  - LoginRequest (email, password)
  - TokenResponse (accessToken, refreshToken, expiresIn)
  - RefreshTokenRequest
  - RefreshTokenResponse

Scope:
  - User authentication via email/password
  - Password hashing and validation
  - Rate limiting on login attempts
  - Error responses with proper HTTP status codes
```

#### Task 2.3: Authorization Policies
```
Files to Create:
  - AuthorizationPolicies.cs - Define authorization rules
  - ClaimsTransformer.cs - Extract user claims

Scope:
  - Define role-based access control (RBAC)
    • Admin: Full access
    • Manager: Dealership management
    • Technician: View own schedule
    • Customer: View own appointments
  - Define claim-based policies
  - Set up [Authorize] attributes on endpoints
```

---

### **TASK 3: User Management & Identity**

**Priority:** 🟡 MEDIUM  
**Estimated Effort:** 4-5 hours  
**Files to Create/Modify:** 6-8  
**Complexity:** High (Business logic integration)

#### Task 3.1: User Entity & Repository
```
Files to Create:
  - User.cs - User entity (if not in Domain)
  - IUserRepository.cs - User data access interface
  - UserRepository.cs - User data access implementation

Entity Properties:
  - Id, Email, PasswordHash, FirstName, LastName
  - Role, Status (Active/Inactive)
  - CreatedAt, LastLoginAt
  - RefreshTokens (collection for token management)

Scope:
  - User creation with hashed passwords
  - User lookup by email
  - Password validation
  - Token revocation tracking
```

#### Task 3.2: User Service
```
Files to Create:
  - IUserService.cs - User business logic interface
  - UserService.cs - User business logic

Methods:
  - RegisterUserAsync() - Create new user
  - ValidateUserAsync() - Check credentials
  - UpdateUserAsync() - Modify user details
  - DeactivateUserAsync() - Soft delete users

Scope:
  - Password hashing with BCrypt/PBKDF2
  - Email uniqueness validation
  - User role assignment
  - Audit logging of user actions
```

---

### **TASK 4: API Controllers & Endpoints**

**Priority:** 🔴 HIGH  
**Estimated Effort:** 6-8 hours  
**Files to Create/Modify:** 8-10  
**Complexity:** High (Many endpoints)

#### Task 4.1: Appointment Controller Endpoints
```
Endpoints to Create:
  GET    /api/v1/appointments/{id}              - Get appointment by ID
  GET    /api/v1/appointments                   - List appointments (paginated)
  POST   /api/v1/appointments                   - Create appointment
  PUT    /api/v1/appointments/{id}              - Update appointment
  DELETE /api/v1/appointments/{id}              - Cancel appointment
  GET    /api/v1/appointments/{id}/details      - Get full details with related entities

Implementation:
  - Add request/response DTOs
  - Add fluent validation rules
  - Add error handling (404, 409, 422 responses)
  - Add pagination support (skip, take, sortBy)
  - Add authorization checks [Authorize]
```

#### Task 4.2: Availability Endpoints
```
Endpoints to Create:
  GET    /api/v1/availability/slots             - Get available slots
  GET    /api/v1/availability/dealerships/{id}  - Get dealership availability
  GET    /api/v1/availability/technicians/{id}  - Get technician availability

Query Parameters:
  - serviceBayId, serviceTypeId
  - date, startDate, endDate
  - dealershipId

Implementation:
  - Use AvailabilityService
  - Cache results (short-lived, 5-10 min)
  - Return formatted slot options
```

#### Task 4.3: Resource Controllers
```
Controllers to Create/Enhance:
  - DealershipController (GET, LIST, POST for admins)
  - CustomerController (GET, UPDATE for own profile)
  - TechnicianController (GET, LIST for managers)
  - ServiceTypeController (GET, LIST)
  - ServiceBayController (GET, LIST)

Implementation:
  - Standard CRUD operations
  - Role-based access control
  - Proper HTTP status codes
  - Comprehensive error messages
```

---

### **TASK 5: Business Logic & Workflow**

**Priority:** 🔴 HIGH  
**Estimated Effort:** 5-7 hours  
**Files to Create/Modify:** 6-8  
**Complexity:** Very High (Complex business rules)

#### Task 5.1: Appointment Workflow Service
```
Files to Create:
  - IAppointmentWorkflowService.cs - Workflow interface
  - AppointmentWorkflowService.cs - Workflow implementation

Workflow States:
  - PENDING_CONFIRMATION → Customer created, awaiting acceptance
  - CONFIRMED → Customer accepted, technician assigned
  - IN_PROGRESS → Work started
  - COMPLETED → Work finished
  - CANCELLED → Customer/system cancelled

Methods:
  - ScheduleAppointmentAsync() - Create new appointment
  - ConfirmAppointmentAsync() - Customer confirms
  - RescheduleAppointmentAsync() - Change date/time
  - StartAppointmentAsync() - Begin work
  - CompleteAppointmentAsync() - Finish work
  - CancelAppointmentAsync() - Cancel with reason

Validations:
  - Check slot availability before scheduling
  - Validate technician skills for service
  - Prevent double-booking
  - Validate state transitions
  - Check dealership business hours
```

#### Task 5.2: Notification Service (Foundation)
```
Files to Create:
  - INotificationService.cs - Notification interface
  - NotificationService.cs - Notification implementation

Events to Notify:
  - AppointmentScheduled
  - AppointmentConfirmed
  - AppointmentRescheduled
  - AppointmentCancelled
  - AppointmentReminder (24h before)

Implementation:
  - In-memory notification queue for Phase 3
  - Email integration in Phase 4
  - SMS integration in Phase 5

Scope:
  - Log notifications to database
  - Prepare for message queue integration
```

#### Task 5.3: Business Rules Engine
```
Files to Create:
  - AppointmentBusinessRules.cs - Appointment validation rules
  - TechnicianAvailabilityRules.cs - Technician availability rules
  - ServiceBayAvailabilityRules.cs - Bay availability rules

Rules to Implement:
  - Slot must be between 30 minutes and 4 hours
  - Technician must have required skills
  - Service bay must be available for duration
  - Dealership must be open during slot time
  - Customer can only book for future dates
  - Minimum lead time: 2 hours
  - Maximum advance booking: 60 days
```

---

### **TASK 6: Data Validation & Error Handling**

**Priority:** 🟡 MEDIUM  
**Estimated Effort:** 3-4 hours  
**Files to Create/Modify:** 10-12  
**Complexity:** Medium

#### Task 6.1: Comprehensive Validators
```
Files to Create:
  - CreateAppointmentRequestValidator.cs
  - UpdateAppointmentRequestValidator.cs
  - RescheduleAppointmentRequestValidator.cs
  - LoginRequestValidator.cs
  - RegisterUserRequestValidator.cs

Validation Rules:
  - Field presence and format
  - Business logic validation
  - Custom async validation (email uniqueness, slot availability)
  - Cross-field validation (start < end time)
```

#### Task 6.2: Error Handling Middleware
```
Files to Create/Modify:
  - ExceptionHandlingMiddleware.cs - Catch all exceptions
  - ProblemDetailsFactory.cs - RFC 7231 compliant errors

Error Types to Handle:
  - ValidationException → 422 Unprocessable Entity
  - NotFoundException → 404 Not Found
  - ConflictException → 409 Conflict (double-booking)
  - UnauthorizedException → 401 Unauthorized
  - ForbiddenException → 403 Forbidden
  - InternalException → 500 Internal Server Error

Response Format:
  {
    "type": "https://api.example.com/errors/validation",
    "title": "Validation Failed",
    "status": 422,
    "detail": "The request contains invalid data",
    "errors": {
      "slotStart": ["Must be in future"]
    }
  }
```

---

### **TASK 7: API Documentation & Testing**

**Priority:** 🟡 MEDIUM  
**Estimated Effort:** 2-3 hours  
**Files to Create/Modify:** 3-5  
**Complexity:** Low-Medium

#### Task 7.1: Swagger/OpenAPI Documentation
```
Configuration:
  - Enable Swagger UI in Development
  - Add XML documentation comments to all endpoints
  - Add example request/response bodies
  - Document security requirements
  - Add authorization configuration

Endpoint Documentation:
  - Clear descriptions
  - Parameter details (required, format, examples)
  - Response codes (200, 400, 401, 404, 409, 422, 500)
  - Authentication requirements [Authorize]
```

#### Task 7.2: Integration Tests for API
```
Files to Create:
  - AppointmentControllerIntegrationTests.cs
  - AuthenticationControllerIntegrationTests.cs
  - AvailabilityControllerIntegrationTests.cs

Test Scenarios:
  - Happy path tests
  - Validation error tests
  - Authorization/authentication tests
  - Conflict/double-booking tests
  - Edge case tests
```

---

## 🔄 Task Execution Order

### Week 1: Foundation (Database & Auth)
1. **Task 1: EF Core & Database** (4-6 hours)
   - Set up DbContext with all entities
   - Create migrations
   - Test database connectivity
   - Build verified: ✅

2. **Task 2: Authentication** (5-7 hours)
   - JWT setup and token generation
   - Login endpoint
   - Token refresh
   - Authorization policies
   - Build verified: ✅

### Week 2: APIs & Business Logic
3. **Task 3: User Management** (4-5 hours)
   - User entity and repository
   - User service with password hashing
   - Build verified: ✅

4. **Task 4: API Controllers** (6-8 hours)
   - Appointment controller endpoints
   - Availability endpoints
   - Resource controllers
   - Build verified: ✅

### Week 3: Business Logic & Validation
5. **Task 5: Business Logic** (5-7 hours)
   - Appointment workflow service
   - Notification service foundation
   - Business rules engine
   - Build verified: ✅

6. **Task 6: Validation & Error Handling** (3-4 hours)
   - Comprehensive validators
   - Exception handling middleware
   - Standard error responses
   - Build verified: ✅

### Week 4: Documentation & Testing
7. **Task 7: Documentation & Testing** (2-3 hours)
   - Swagger/OpenAPI setup
   - Integration tests
   - Build verified: ✅

---

## 📊 Phase 3 Summary

| Task | Priority | Effort | Status | Dependencies |
|------|----------|--------|--------|--------------|
| 1. EF Core & Database | 🔴 HIGH | 4-6h | ⏳ Ready | Phase 2 ✅ |
| 2. Authentication | 🔴 HIGH | 5-7h | ⏳ Ready | Task 1 |
| 3. User Management | 🟡 MEDIUM | 4-5h | ⏳ Ready | Tasks 1, 2 |
| 4. API Controllers | 🔴 HIGH | 6-8h | ⏳ Ready | Tasks 1, 2, 3 |
| 5. Business Logic | 🔴 HIGH | 5-7h | ⏳ Ready | Tasks 3, 4 |
| 6. Validation & Errors | 🟡 MEDIUM | 3-4h | ⏳ Ready | Tasks 4, 5 |
| 7. Documentation & Tests | 🟡 MEDIUM | 2-3h | ⏳ Ready | Tasks 4, 5, 6 |
| **Total Phase 3** | | **30-40h** | ⏳ Ready | |

---

## 🚀 Success Criteria

### Must Have
- ✅ Database schema created and migrated
- ✅ JWT authentication working end-to-end
- ✅ All appointment endpoints functional
- ✅ Authorization policies enforced
- ✅ Comprehensive error handling
- ✅ Business rules validated
- ✅ Build: 0 errors
- ✅ All new tests passing

### Nice to Have
- ✅ API documentation complete in Swagger
- ✅ Performance optimizations (caching, indexing)
- ✅ Audit logging of sensitive operations
- ✅ Rate limiting implemented
- ✅ Integration tests > 80% coverage

---

## 🔍 Key Technical Decisions

### Database Choice
- **Recommendation:** SQL Server (production) / SQLite (development)
- **Rationale:** Better than Entity Framework relationships, strong typing in EF, good tooling
- **Alternative:** PostgreSQL (excellent, but SQL Server standard in enterprise C#)

### Authentication Strategy
- **Recommendation:** JWT + Refresh Token pattern
- **Rationale:** Stateless, scalable, industry standard, works with SPAs
- **Security:** Secure secret key storage, HTTPS only, short-lived access tokens

### Password Hashing
- **Recommendation:** BCrypt or PBKDF2
- **Rationale:** Slow hashing with salt, resistant to brute force
- **Configuration:** Cost factor 10-12, realistic work factor for modern hardware

### API Versioning
- **Recommendation:** URL-based (/api/v1/, /api/v2/)
- **Rationale:** Clear, explicit, easy to route, developer-friendly

---

## ⚠️ Known Risks & Mitigation

### Risk 1: Database Migration Issues
**Risk:** Migrating with existing data could cause issues  
**Mitigation:** Test migrations in staging, use idempotent migrations, plan rollback strategy

### Risk 2: Authentication Performance
**Risk:** JWT validation on every request could impact performance  
**Mitigation:** Cache token claims, optimize middleware ordering, use async validation

### Risk 3: Business Logic Complexity
**Risk:** State transitions could have edge cases  
**Mitigation:** Comprehensive test coverage, document all valid transitions, use state machine pattern

### Risk 4: Security Vulnerabilities
**Risk:** Authentication/authorization flaws could expose data  
**Mitigation:** Security review, penetration testing, follow OWASP guidelines

---

## 📝 Starting Point - Task 1.1: EF Core Configuration

**Next Steps:**
1. Review current ApplicationDbContext in Infrastructure/Persistence/
2. Identify all entities and their relationships
3. Configure entity mappings using Fluent API
4. Add shadow properties and value converters
5. Create initial migration
6. Test database connectivity

**Reference Files:**
- Current: `src/VehicleServiceBooking.Infrastructure/Persistence/ApplicationDbContext.cs`
- Entities: `src/VehicleServiceBooking.Domain/Entities/*.cs`

---

## 📌 Document Navigation

- Phase 2 Completion: `docs/PHASE2_PROPOSED_CHECKLIST.md`
- Architecture Reference: `docs/SYSTEM_IMPLEMENTATION_ANALYSIS_RISKS_AND_ROADMAP.md`
- Clean Architecture: `docs/ARCHITECTURE_VISUALIZATION.md`
- Previous Phases: `docs/PHASE1_CLARIFICATIONS.md`

---

**Document Version:** 1.0  
**Last Updated:** June 25, 2026  
**Status:** Ready to proceed with Phase 3 implementation
