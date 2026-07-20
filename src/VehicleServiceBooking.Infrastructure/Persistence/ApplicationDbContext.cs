using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using VehicleServiceBooking.Application.Interfaces.Persistence;
using VehicleServiceBooking.Application.Models.ViewModels;
using VehicleServiceBooking.Domain.Entities;
using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbContext DbContext => this;

    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<AppointmentStatusLookup> AppointmentStatusLookups => Set<AppointmentStatusLookup>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<ServiceStatusLookup> ServiceStatusLookups => Set<ServiceStatusLookup>();
    public DbSet<ServiceType> ServiceTypes => Set<ServiceType>();
    public DbSet<Technician> Technicians => Set<Technician>();
    public DbSet<TechnicianSchedule> TechnicianSchedules => Set<TechnicianSchedule>();
    public DbSet<TechnicianSkill> TechnicianSkills => Set<TechnicianSkill>();
    public DbSet<ServiceBay> ServiceBays => Set<ServiceBay>();
    public DbSet<BusinessHours> BusinessHours => Set<BusinessHours>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Dealership> Dealerships => Set<Dealership>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<IdempotencyRequest> IdempotencyRequests => Set<IdempotencyRequest>();
    public DbSet<IdempotencyRequestStatusLookup> IdempotencyRequestStatusLookups => Set<IdempotencyRequestStatusLookup>();
    public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<ServiceTypeOrder> ServiceTypeOrders => Set<ServiceTypeOrder>();
    public DbSet<CurrencyLookup> CurrencyLookups => Set<CurrencyLookup>();
    public DbSet<ServiceTypePrice> ServiceTypePrices => Set<ServiceTypePrice>();
    public DbSet<PaymentProviderLookup> PaymentProviderLookups => Set<PaymentProviderLookup>();
    public DbSet<PaymentMethodLookup> PaymentMethodLookups => Set<PaymentMethodLookup>();
    public DbSet<OrderPaymentStatusLookup> OrderPaymentStatusLookups => Set<OrderPaymentStatusLookup>();
    public DbSet<PaymentIntentStatusLookup> PaymentIntentStatusLookups => Set<PaymentIntentStatusLookup>();
    public DbSet<PaymentWebhookProcessStatusLookup> PaymentWebhookProcessStatusLookups => Set<PaymentWebhookProcessStatusLookup>();
    public DbSet<PaymentTransactionStatusLookup> PaymentTransactionStatusLookups => Set<PaymentTransactionStatusLookup>();
    public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();
    public DbSet<PaymentOrder> PaymentOrders => Set<PaymentOrder>();
    public DbSet<PaymentWebhookInbox> PaymentWebhookInboxes => Set<PaymentWebhookInbox>();
    public DbSet<OrderAppointment> OrderAppointments => Set<OrderAppointment>();

    // ==================== VIEW DbSETS (READ-ONLY) ====================
    
    /// <summary>
    /// Query-only DbSet for TechnicianAvailableSlots view.
    /// Returns pre-computed available time slots for each technician on the query date.
    /// 
    /// Usage: 
    /// var availability = await _dbContext.TechnicianAvailableSlotsView
    ///     .Where(x => x.TechnicianId == techId && x.DealershipId == dealershipId)
    ///     .ToListAsync();
    /// </summary>
    public DbSet<TechnicianAvailableSlotsView> TechnicianAvailableSlotsView => Set<TechnicianAvailableSlotsView>();

    /// <summary>
    /// Query-only DbSet for ServiceBayAvailableSlots view.
    /// Returns pre-computed available time slots for each service bay on the query date.
    /// 
    /// Usage:
    /// var availability = await _dbContext.ServiceBayAvailableSlotsView
    ///     .Where(x => x.ServiceBayId == bayId && x.DealershipId == dealershipId)
    ///     .ToListAsync();
    /// </summary>
    public DbSet<ServiceBayAvailableSlotsView> ServiceBayAvailableSlotsView => Set<ServiceBayAvailableSlotsView>();

    /// <summary>
    /// Query-only DbSet for ServiceTypeAvailability master view.
    /// Returns pre-computed availability combining service requirements with technician and bay availability.
    /// 
    /// Usage:
    /// var availability = await _dbContext.ServiceTypeAvailabilityView
    ///     .Where(x => x.ServiceTypeId == serviceTypeId && x.DealershipId == dealershipId && x.CanFitService)
    ///     .ToListAsync();
    /// </summary>
    public DbSet<ServiceTypeAvailabilityView> ServiceTypeAvailabilityView => Set<ServiceTypeAvailabilityView>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ==================== GLOBAL BASE ENTITY CONFIGURATION ====================
        // Configure common audit fields for all BaseEntity-derived entities
        
        // Configure Id as primary key for all BaseEntity types
        var baseEntityType = typeof(BaseEntity);
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (baseEntityType.IsAssignableFrom(entityType.ClrType))
            {
                // Configure CreatedAt and UpdatedAt with default value factories
                modelBuilder.Entity(entityType.ClrType)
                    .Property("CreatedAt")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .IsRequired();

                modelBuilder.Entity(entityType.ClrType)
                    .Property("UpdatedAt")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .IsRequired();

                modelBuilder.Entity(entityType.ClrType)
                    .Property("IsActive")
                    .HasDefaultValue(true)
                    .IsRequired();

                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(CreateIsActiveFilter(entityType.ClrType));
            }
        }

        // ==================== APPOINTMENT STATUS LOOKUP CONFIGURATION ====================
        
        // Configure AppointmentStatusLookup primary key
        modelBuilder.Entity<AppointmentStatusLookup>()
            .HasKey(x => x.Id)
            .HasName("PK_AppointmentStatusLookups");

        // AppointmentStatusLookup property constraints
        modelBuilder.Entity<AppointmentStatusLookup>()
            .Property(x => x.Status)
            .IsRequired();
        modelBuilder.Entity<AppointmentStatusLookup>()
            .Property(x => x.Name)
            .HasMaxLength(50)
            .IsRequired();
        modelBuilder.Entity<AppointmentStatusLookup>()
            .Property(x => x.Description)
            .HasMaxLength(200);

        // Unique constraint on Status enum value
        modelBuilder.Entity<AppointmentStatusLookup>()
            .HasIndex(x => x.Status)
            .IsUnique()
            .HasDatabaseName("IX_AppointmentStatusLookup_Status_Unique");

        // Seed AppointmentStatusLookup data
        var bookedId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var inProgressId = Guid.Parse("00000000-0000-0000-0000-000000000002");
        var completedId = Guid.Parse("00000000-0000-0000-0000-000000000003");
        var cancelledId = Guid.Parse("00000000-0000-0000-0000-000000000004");
        var partiallyCompletedId = Guid.Parse("00000000-0000-0000-0000-000000000005");
        var appointmentStatusSeedTimestamp = new DateTime(2026, 7, 18, 10, 59, 32, 894, DateTimeKind.Utc).AddTicks(7550);

        modelBuilder.Entity<AppointmentStatusLookup>().HasData(
            new AppointmentStatusLookup { Id = bookedId, Status = AppointmentStatus.Booked, Name = "Booked", Description = "Appointment is scheduled", IsActive = true, CreatedAt = appointmentStatusSeedTimestamp, UpdatedAt = appointmentStatusSeedTimestamp },
            new AppointmentStatusLookup { Id = inProgressId, Status = AppointmentStatus.InProgress, Name = "In Progress", Description = "Service is currently being performed", IsActive = true, CreatedAt = appointmentStatusSeedTimestamp, UpdatedAt = appointmentStatusSeedTimestamp },
            new AppointmentStatusLookup { Id = completedId, Status = AppointmentStatus.Completed, Name = "Completed", Description = "Service has been completed", IsActive = true, CreatedAt = appointmentStatusSeedTimestamp, UpdatedAt = appointmentStatusSeedTimestamp },
            new AppointmentStatusLookup { Id = cancelledId, Status = AppointmentStatus.Cancelled, Name = "Cancelled", Description = "Appointment has been cancelled", IsActive = true, CreatedAt = appointmentStatusSeedTimestamp, UpdatedAt = appointmentStatusSeedTimestamp },
            new AppointmentStatusLookup { Id = partiallyCompletedId, Status = AppointmentStatus.PartiallyCompleted, Name = "Partially Completed", Description = "Some services completed, others rescheduled", IsActive = true, CreatedAt = appointmentStatusSeedTimestamp, UpdatedAt = appointmentStatusSeedTimestamp }
        );

        // ==================== APPOINTMENT RELATIONSHIPS ====================
        
        // Configure Appointment primary key
        modelBuilder.Entity<Appointment>()
            .HasKey(x => x.Id)
            .HasName("PK_Appointments");

        // Appointment property constraints
        modelBuilder.Entity<Appointment>()
            .Property(x => x.StatusId)
            .IsRequired();

        modelBuilder.Entity<Appointment>()
            .Property(x => x.AppointmentDate)
            .IsRequired();

        // Appointment -> AppointmentStatusLookup (Many-to-One)
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Status)
            .WithMany(s => s.Appointments)
            .HasForeignKey(a => a.StatusId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Appointment_AppointmentStatus");

        // Appointment -> Dealership (Many-to-One)
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Dealership)
            .WithMany(d => d.Appointments)
            .HasForeignKey(a => a.DealershipId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Appointment_Dealership");

        // Appointment -> Customer (Many-to-One)
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Customer)
            .WithMany(c => c.Appointments)
            .HasForeignKey(a => a.CustomerId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Appointment_Customer");

        // Appointment -> Vehicle (Many-to-One)
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Vehicle)
            .WithMany(v => v.Appointments)
            .HasForeignKey(a => a.VehicleId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Appointment_Vehicle");

        // Appointment -> Service (One-to-Many)
        modelBuilder.Entity<Service>()
            .HasOne(s => s.Appointment)
            .WithMany(a => a.Services)
            .HasForeignKey(s => s.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Service_Appointment");

        // Note: Service-level timing validation handled through Service entity
        // (EstimatedStartTime, EstimatedEndTime, ActualStartTime, ActualEndTime)

        // ==================== IDEMPOTENCY REQUEST CONFIGURATION ====================

        modelBuilder.Entity<IdempotencyRequestStatusLookup>()
            .HasKey(x => x.Id)
            .HasName("PK_IdempotencyRequestStatusLookups");

        modelBuilder.Entity<IdempotencyRequestStatusLookup>()
            .Property(x => x.Status)
            .IsRequired();

        modelBuilder.Entity<IdempotencyRequestStatusLookup>()
            .Property(x => x.Name)
            .HasMaxLength(50)
            .IsRequired();

        modelBuilder.Entity<IdempotencyRequestStatusLookup>()
            .Property(x => x.Description)
            .HasMaxLength(200)
            .IsRequired();

        modelBuilder.Entity<IdempotencyRequestStatusLookup>()
            .HasIndex(x => x.Status)
            .IsUnique()
            .HasDatabaseName("IX_IdempotencyRequestStatusLookup_Status_Unique");

        var idempotencyStatusSeedTimestamp = new DateTime(2026, 7, 18, 10, 59, 32, 895, DateTimeKind.Utc).AddTicks(3260);

        modelBuilder.Entity<IdempotencyRequestStatusLookup>().HasData(
            new IdempotencyRequestStatusLookup
            {
                Id = new Guid("00000000-0000-0000-0002-000000000001"),
                Status = IdempotencyRequestStatus.InProgress,
                Name = "In Progress",
                Description = "Request processing started and not yet completed",
                IsActive = true,
                CreatedAt = idempotencyStatusSeedTimestamp,
                UpdatedAt = idempotencyStatusSeedTimestamp
            },
            new IdempotencyRequestStatusLookup
            {
                Id = new Guid("00000000-0000-0000-0002-000000000002"),
                Status = IdempotencyRequestStatus.Completed,
                Name = "Completed",
                Description = "Request completed and response persisted for replay",
                IsActive = true,
                CreatedAt = idempotencyStatusSeedTimestamp,
                UpdatedAt = idempotencyStatusSeedTimestamp
            }
        );

        modelBuilder.Entity<IdempotencyRequest>()
            .HasKey(x => x.Id)
            .HasName("PK_IdempotencyRequests");

        modelBuilder.Entity<IdempotencyRequest>()
            .Property(x => x.IdempotencyKey)
            .HasMaxLength(100)
            .IsRequired();

        modelBuilder.Entity<IdempotencyRequest>()
            .Property(x => x.RequestPath)
            .HasMaxLength(200)
            .IsRequired();

        modelBuilder.Entity<IdempotencyRequest>()
            .Property(x => x.RequestHash)
            .HasMaxLength(128)
            .IsRequired();

        modelBuilder.Entity<IdempotencyRequest>()
            .Property(x => x.StatusId)
            .IsRequired();

        modelBuilder.Entity<IdempotencyRequest>()
            .HasOne(x => x.Status)
            .WithMany(x => x.IdempotencyRequests)
            .HasForeignKey(x => x.StatusId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_IdempotencyRequest_StatusLookup");

        modelBuilder.Entity<IdempotencyRequest>()
            .Property(x => x.ResponseBody)
            .HasColumnType("text");

        modelBuilder.Entity<IdempotencyRequest>()
            .Property(x => x.ExpiresAt)
            .IsRequired();

        modelBuilder.Entity<IdempotencyRequest>()
            .HasIndex(x => new { x.IdempotencyKey, x.RequestPath })
            .IsUnique()
            .HasDatabaseName("IX_IdempotencyRequest_Key_Path_Unique");

        modelBuilder.Entity<IdempotencyRequest>()
            .HasIndex(x => x.ExpiresAt)
            .HasDatabaseName("IX_IdempotencyRequest_ExpiresAt");

        // ==================== SERVICE STATUS LOOKUP CONFIGURATION ====================
        
        // Configure ServiceStatusLookup primary key
        modelBuilder.Entity<ServiceStatusLookup>()
            .HasKey(x => x.Id)
            .HasName("PK_ServiceStatusLookups");

        // ServiceStatusLookup property constraints
        modelBuilder.Entity<ServiceStatusLookup>()
            .Property(x => x.Status)
            .IsRequired();
        modelBuilder.Entity<ServiceStatusLookup>()
            .Property(x => x.Name)
            .HasMaxLength(50)
            .IsRequired();
        modelBuilder.Entity<ServiceStatusLookup>()
            .Property(x => x.Description)
            .HasMaxLength(200);

        // Unique constraint on Status enum value
        modelBuilder.Entity<ServiceStatusLookup>()
            .HasIndex(x => x.Status)
            .IsUnique()
            .HasDatabaseName("IX_ServiceStatusLookup_Status_Unique");

        // Seed ServiceStatusLookup data
        var serviceStatusPendingId = Guid.Parse("00000000-0000-0000-0001-000000000001");
        var serviceStatusInProgressId = Guid.Parse("00000000-0000-0000-0001-000000000002");
        var serviceStatusCompletedId = Guid.Parse("00000000-0000-0000-0001-000000000003");
        var serviceStatusSkippedId = Guid.Parse("00000000-0000-0000-0001-000000000004");
        var serviceStatusRescheduledId = Guid.Parse("00000000-0000-0000-0001-000000000005");
        var serviceStatusSeedTimestamp = new DateTime(2026, 7, 18, 10, 59, 32, 895, DateTimeKind.Utc).AddTicks(5190);

        modelBuilder.Entity<ServiceStatusLookup>().HasData(
            new ServiceStatusLookup { Id = serviceStatusPendingId, Status = ServiceStatus.Pending, Name = "Pending", Description = "Service scheduled but not started", IsActive = true, CreatedAt = serviceStatusSeedTimestamp, UpdatedAt = serviceStatusSeedTimestamp },
            new ServiceStatusLookup { Id = serviceStatusInProgressId, Status = ServiceStatus.InProgress, Name = "In Progress", Description = "Service is currently being performed", IsActive = true, CreatedAt = serviceStatusSeedTimestamp, UpdatedAt = serviceStatusSeedTimestamp },
            new ServiceStatusLookup { Id = serviceStatusCompletedId, Status = ServiceStatus.Completed, Name = "Completed", Description = "Service has been completed successfully", IsActive = true, CreatedAt = serviceStatusSeedTimestamp, UpdatedAt = serviceStatusSeedTimestamp },
            new ServiceStatusLookup { Id = serviceStatusSkippedId, Status = ServiceStatus.Skipped, Name = "Skipped", Description = "Service was cancelled or declined", IsActive = true, CreatedAt = serviceStatusSeedTimestamp, UpdatedAt = serviceStatusSeedTimestamp },
            new ServiceStatusLookup { Id = serviceStatusRescheduledId, Status = ServiceStatus.Rescheduled, Name = "Rescheduled", Description = "Service moved to a different appointment", IsActive = true, CreatedAt = serviceStatusSeedTimestamp, UpdatedAt = serviceStatusSeedTimestamp }
        );

        // ==================== SERVICE CONFIGURATION ====================
        
        // Configure Service primary key
        modelBuilder.Entity<Service>()
            .HasKey(x => x.Id)
            .HasName("PK_Services");

        // Service property constraints
        modelBuilder.Entity<Service>()
            .Property(x => x.AppointmentId)
            .IsRequired();
        modelBuilder.Entity<Service>()
            .Property(x => x.ServiceTypeId)
            .IsRequired();
        modelBuilder.Entity<Service>()
            .Property(x => x.DealershipId)
            .IsRequired();
        modelBuilder.Entity<Service>()
            .Property(x => x.ServiceStatusId)
            .IsRequired();
        modelBuilder.Entity<Service>()
            .Property(x => x.SequenceOrder)
            .IsRequired();
        modelBuilder.Entity<Service>()
            .Property(x => x.CreatedAt)
            .IsRequired();
        modelBuilder.Entity<Service>()
            .Property(x => x.UpdatedAt)
            .IsRequired();
        modelBuilder.Entity<Service>()
            .Property(x => x.Notes)
            .HasMaxLength(500);
        modelBuilder.Entity<Service>()
            .Property(x => x.EstimatedStartTimeSlotId)
            .IsRequired(false);
        modelBuilder.Entity<Service>()
            .Property(x => x.EstimatedEndTimeSlotId)
            .IsRequired(false);
        modelBuilder.Entity<Service>()
            .Property(x => x.BookingDate)
            .IsRequired();
        modelBuilder.Entity<Service>()
            .Property(x => x.EstimatedStartSlotSequence)
            .IsRequired();
        modelBuilder.Entity<Service>()
            .Property(x => x.EstimatedEndSlotSequenceExclusive)
            .IsRequired();

        // Service -> ServiceType (Many-to-One)
        modelBuilder.Entity<Service>()
            .HasOne(s => s.ServiceType)
            .WithMany(st => st.Services)
            .HasForeignKey(s => s.ServiceTypeId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Service_ServiceType");

        // Service -> Technician (Many-to-One, optional)
        modelBuilder.Entity<Service>()
            .HasOne(s => s.Technician)
            .WithMany(t => t.Services)
            .HasForeignKey(s => s.TechnicianId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("FK_Service_Technician");

        // Service -> ServiceBay (Many-to-One, optional)
        modelBuilder.Entity<Service>()
            .HasOne(s => s.ServiceBay)
            .WithMany(sb => sb.Services)
            .HasForeignKey(s => s.ServiceBayId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("FK_Service_ServiceBay");

        // Service -> Dealership (Many-to-One)
        modelBuilder.Entity<Service>()
            .HasOne(s => s.Dealership)
            .WithMany(d => d.Services)
            .HasForeignKey(s => s.DealershipId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Service_Dealership");

        // Service -> ServiceStatusLookup (Many-to-One)
        modelBuilder.Entity<Service>()
            .HasOne(s => s.ServiceStatus)
            .WithMany(ssl => ssl.Services)
            .HasForeignKey(s => s.ServiceStatusId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Service_ServiceStatus");

        // Unique constraint on (AppointmentId, ServiceTypeId, SequenceOrder)
        modelBuilder.Entity<Service>()
            .HasIndex(x => new { x.AppointmentId, x.ServiceTypeId, x.SequenceOrder })
            .IsUnique()
            .HasDatabaseName("IX_Service_Unique_AppointmentServiceTypeSequence");

        modelBuilder.Entity<Service>()
            .HasIndex(x => new { x.DealershipId, x.BookingDate })
            .HasDatabaseName("IX_Service_Dealership_BookingDate");

        modelBuilder.Entity<Service>()
            .HasIndex(x => new { x.TechnicianId, x.BookingDate })
            .HasDatabaseName("IX_Service_Technician_BookingDate");

        modelBuilder.Entity<Service>()
            .HasIndex(x => new { x.ServiceBayId, x.BookingDate })
            .HasDatabaseName("IX_Service_ServiceBay_BookingDate");

        // ==================== CUSTOMER CONFIGURATION ====================
        
        // Configure Customer primary key
        modelBuilder.Entity<Customer>()
            .HasKey(x => x.Id)
            .HasName("PK_Customers");

        // Customer property constraints
        modelBuilder.Entity<Customer>()
            .Property(x => x.AuthUserId)
            .IsRequired();
        modelBuilder.Entity<Customer>()
            .Property(x => x.FirstName)
            .HasMaxLength(100)
            .IsRequired();
        modelBuilder.Entity<Customer>()
            .Property(x => x.LastName)
            .HasMaxLength(100)
            .IsRequired();
        modelBuilder.Entity<Customer>()
            .Property(x => x.Email)
            .HasMaxLength(254)
            .IsRequired();
        modelBuilder.Entity<Customer>()
            .Property(x => x.PhoneNumber)
            .HasMaxLength(20)
            .IsRequired();

        modelBuilder.Entity<Customer>()
            .HasIndex(x => x.AuthUserId)
            .IsUnique()
            .HasDatabaseName("UX_Customers_AuthUserId");

        // Customer -> Vehicle (One-to-Many)
        modelBuilder.Entity<Vehicle>()
            .HasOne(v => v.Customer)
            .WithMany(c => c.Vehicles)
            .HasForeignKey(v => v.CustomerId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Vehicle_Customer");

        // Vehicle property constraints
        modelBuilder.Entity<Vehicle>()
            .Property(x => x.CustomerId)
            .IsRequired();

        // ==================== DEALERSHIP CONFIGURATION ====================
        
        // Configure Dealership primary key
        modelBuilder.Entity<Dealership>()
            .HasKey(x => x.Id)
            .HasName("PK_Dealerships");

        // Dealership property constraints
        modelBuilder.Entity<Dealership>()
            .Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();
        modelBuilder.Entity<Dealership>()
            .Property(x => x.Address)
            .HasMaxLength(500)
            .IsRequired();
        modelBuilder.Entity<Dealership>()
            .Property(x => x.Address)
            .HasMaxLength(500)
            .IsRequired();

        // Dealership -> BusinessHours (One-to-Many)
        modelBuilder.Entity<BusinessHours>()
            .HasOne(bh => bh.Dealership)
            .WithMany(d => d.BusinessHours)
            .HasForeignKey(bh => bh.DealershipId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_BusinessHours_Dealership");

        // BusinessHours property constraints
        modelBuilder.Entity<BusinessHours>()
            .Property(x => x.DealershipId)
            .IsRequired();

        // Dealership -> Technician (One-to-Many)
        modelBuilder.Entity<Technician>()
            .HasOne(t => t.Dealership)
            .WithMany(d => d.Technicians)
            .HasForeignKey(t => t.DealershipId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Technician_Dealership");

        // Technician property constraints
        modelBuilder.Entity<Technician>()
            .Property(x => x.DealershipId)
            .IsRequired();

        // Dealership -> ServiceBay (One-to-Many)
        modelBuilder.Entity<ServiceBay>()
            .HasOne(sb => sb.Dealership)
            .WithMany(d => d.ServiceBays)
            .HasForeignKey(sb => sb.DealershipId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_ServiceBay_Dealership");

        // ServiceBay property constraints
        modelBuilder.Entity<ServiceBay>()
            .Property(x => x.DealershipId)
            .IsRequired();

        // ==================== SERVICE TYPE CONFIGURATION ====================
        
        // Configure ServiceType primary key
        modelBuilder.Entity<ServiceType>()
            .HasKey(x => x.Id)
            .HasName("PK_ServiceTypes");

        // ServiceType property constraints
        modelBuilder.Entity<ServiceType>()
            .Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();
        modelBuilder.Entity<ServiceType>()
            .Property(x => x.DurationMinutes)
            .IsRequired();
        // DurationMinutes must be between 30 and 480 minutes (per original entity constraint)
        modelBuilder.Entity<ServiceType>()
            .ToTable(t => t.HasCheckConstraint("CK_ServiceType_DurationMinutes_Range",
                "\"DurationMinutes\" >= 30 AND \"DurationMinutes\" <= 480"));

        var oilChangeServiceTypeId = new Guid("11111111-1111-1111-1111-030000000001");
        var brakeInspectionServiceTypeId = new Guid("11111111-1111-1111-1111-030000000002");
        var majorServiceTypeId = new Guid("11111111-1111-1111-1111-030000000003");
        var serviceTypeSeedTimestamp = new DateTime(2026, 7, 18, 10, 52, 34, 110, DateTimeKind.Utc).AddTicks(7100);

        modelBuilder.Entity<ServiceType>().HasData(
            new ServiceType
            {
                Id = oilChangeServiceTypeId,
                Name = "Oil Change",
                DurationMinutes = 60,
                IsActive = true,
                CreatedAt = serviceTypeSeedTimestamp,
                UpdatedAt = serviceTypeSeedTimestamp
            },
            new ServiceType
            {
                Id = brakeInspectionServiceTypeId,
                Name = "Brake Inspection",
                DurationMinutes = 90,
                IsActive = true,
                CreatedAt = serviceTypeSeedTimestamp,
                UpdatedAt = serviceTypeSeedTimestamp
            },
            new ServiceType
            {
                Id = majorServiceTypeId,
                Name = "Major Service",
                DurationMinutes = 240,
                IsActive = true,
                CreatedAt = serviceTypeSeedTimestamp,
                UpdatedAt = serviceTypeSeedTimestamp
            }
        );

        // ==================== PAYMENT CURRENCY CONFIGURATION ====================

        modelBuilder.Entity<CurrencyLookup>()
            .HasKey(x => x.Id)
            .HasName("PK_CurrencyLookups");

        modelBuilder.Entity<CurrencyLookup>()
            .Property(x => x.Code)
            .HasMaxLength(3)
            .IsRequired();

        modelBuilder.Entity<CurrencyLookup>()
            .Property(x => x.Name)
            .HasMaxLength(50)
            .IsRequired();

        modelBuilder.Entity<CurrencyLookup>()
            .Property(x => x.Symbol)
            .HasMaxLength(8)
            .IsRequired();

        modelBuilder.Entity<CurrencyLookup>()
            .Property(x => x.DecimalPlaces)
            .IsRequired();

        modelBuilder.Entity<CurrencyLookup>()
            .HasIndex(x => x.Code)
            .IsUnique()
            .HasDatabaseName("IX_CurrencyLookup_Code_Unique");

        var currencySeedTimestamp = new DateTime(2026, 7, 18, 10, 52, 34, 110, DateTimeKind.Utc).AddTicks(7100);

        modelBuilder.Entity<CurrencyLookup>().HasData(
            new CurrencyLookup
            {
                Id = new Guid("00000000-0000-0000-0004-000000000001"),
                Code = "VND",
                Name = "Vietnamese Dong",
                Symbol = "VND",
                DecimalPlaces = 0,
                IsActive = true,
                CreatedAt = currencySeedTimestamp,
                UpdatedAt = currencySeedTimestamp
            },
            new CurrencyLookup
            {
                Id = new Guid("00000000-0000-0000-0004-000000000002"),
                Code = "USD",
                Name = "US Dollar",
                Symbol = "USD",
                DecimalPlaces = 2,
                IsActive = true,
                CreatedAt = currencySeedTimestamp,
                UpdatedAt = currencySeedTimestamp
            }
        );

        // ==================== SERVICE TYPE PRICE (MULTI-CURRENCY) CONFIGURATION ====================

        modelBuilder.Entity<ServiceTypePrice>()
            .HasKey(x => x.Id)
            .HasName("PK_ServiceTypePrices");

        modelBuilder.Entity<ServiceTypePrice>()
            .Property(x => x.ServiceTypeId)
            .IsRequired();

        modelBuilder.Entity<ServiceTypePrice>()
            .Property(x => x.CurrencyId)
            .IsRequired();

        modelBuilder.Entity<ServiceTypePrice>()
            .Property(x => x.Price)
            .HasColumnType("numeric(10,2)")
            .IsRequired();

        modelBuilder.Entity<ServiceTypePrice>()
            .ToTable(t => t.HasCheckConstraint("CK_ServiceTypePrice_Price_NonNegative",
                "\"Price\" >= 0"));

        modelBuilder.Entity<ServiceTypePrice>()
            .HasIndex(x => new { x.ServiceTypeId, x.CurrencyId })
            .IsUnique()
            .HasDatabaseName("IX_ServiceTypePrice_Unique_ServiceType_Currency");

        modelBuilder.Entity<ServiceTypePrice>()
            .HasOne(x => x.ServiceType)
            .WithMany(x => x.ServiceTypePrices)
            .HasForeignKey(x => x.ServiceTypeId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_ServiceTypePrice_ServiceType");

        modelBuilder.Entity<ServiceTypePrice>()
            .HasOne(x => x.Currency)
            .WithMany(x => x.ServiceTypePrices)
            .HasForeignKey(x => x.CurrencyId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_ServiceTypePrice_Currency");

        modelBuilder.Entity<ServiceTypePrice>().HasData(
            new ServiceTypePrice
            {
                Id = new Guid("11111111-1111-1111-1111-031000000001"),
                ServiceTypeId = oilChangeServiceTypeId,
                CurrencyId = new Guid("00000000-0000-0000-0004-000000000001"),
                Price = 500000m,
                IsActive = true,
                CreatedAt = serviceTypeSeedTimestamp,
                UpdatedAt = serviceTypeSeedTimestamp
            },
            new ServiceTypePrice
            {
                Id = new Guid("11111111-1111-1111-1111-031000000002"),
                ServiceTypeId = brakeInspectionServiceTypeId,
                CurrencyId = new Guid("00000000-0000-0000-0004-000000000001"),
                Price = 750000m,
                IsActive = true,
                CreatedAt = serviceTypeSeedTimestamp,
                UpdatedAt = serviceTypeSeedTimestamp
            },
            new ServiceTypePrice
            {
                Id = new Guid("11111111-1111-1111-1111-031000000003"),
                ServiceTypeId = majorServiceTypeId,
                CurrencyId = new Guid("00000000-0000-0000-0004-000000000001"),
                Price = 2200000m,
                IsActive = true,
                CreatedAt = serviceTypeSeedTimestamp,
                UpdatedAt = serviceTypeSeedTimestamp
            },
            new ServiceTypePrice
            {
                Id = new Guid("11111111-1111-1111-1111-031000000004"),
                ServiceTypeId = oilChangeServiceTypeId,
                CurrencyId = new Guid("00000000-0000-0000-0004-000000000002"),
                Price = 19.99m,
                IsActive = true,
                CreatedAt = serviceTypeSeedTimestamp,
                UpdatedAt = serviceTypeSeedTimestamp
            },
            new ServiceTypePrice
            {
                Id = new Guid("11111111-1111-1111-1111-031000000005"),
                ServiceTypeId = brakeInspectionServiceTypeId,
                CurrencyId = new Guid("00000000-0000-0000-0004-000000000002"),
                Price = 29.99m,
                IsActive = true,
                CreatedAt = serviceTypeSeedTimestamp,
                UpdatedAt = serviceTypeSeedTimestamp
            },
            new ServiceTypePrice
            {
                Id = new Guid("11111111-1111-1111-1111-031000000006"),
                ServiceTypeId = majorServiceTypeId,
                CurrencyId = new Guid("00000000-0000-0000-0004-000000000002"),
                Price = 89.99m,
                IsActive = true,
                CreatedAt = serviceTypeSeedTimestamp,
                UpdatedAt = serviceTypeSeedTimestamp
            }
        );

        // ==================== ORDER CONFIGURATION ====================

        modelBuilder.Entity<Order>()
            .HasKey(x => x.Id)
            .HasName("PK_Orders");

        modelBuilder.Entity<Order>()
            .Property(x => x.OrderCode)
            .HasMaxLength(50)
            .IsRequired();

        modelBuilder.Entity<Order>()
            .Property(x => x.CurrencyId)
            .IsRequired();

        modelBuilder.Entity<Order>()
            .Property(x => x.TotalAmount)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        modelBuilder.Entity<Order>()
            .ToTable(t => t.HasCheckConstraint("CK_Order_TotalAmount_NonNegative",
                "\"TotalAmount\" >= 0"));

        modelBuilder.Entity<Order>()
            .HasIndex(x => x.OrderCode)
            .IsUnique()
            .HasDatabaseName("IX_Order_OrderCode_Unique");

        modelBuilder.Entity<Order>()
            .HasOne(x => x.Currency)
            .WithMany(x => x.Orders)
            .HasForeignKey(x => x.CurrencyId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Order_Currency");

        // ==================== SERVICE TYPE ORDER JUNCTION CONFIGURATION ====================

        modelBuilder.Entity<ServiceTypeOrder>()
            .HasKey(x => x.Id)
            .HasName("PK_ServiceTypeOrders");

        modelBuilder.Entity<ServiceTypeOrder>()
            .Property(x => x.OrderId)
            .IsRequired();

        modelBuilder.Entity<ServiceTypeOrder>()
            .Property(x => x.ServiceTypeId)
            .IsRequired();

        modelBuilder.Entity<ServiceTypeOrder>()
            .Property(x => x.Quantity)
            .IsRequired();

        modelBuilder.Entity<ServiceTypeOrder>()
            .Property(x => x.UnitPrice)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        modelBuilder.Entity<ServiceTypeOrder>()
            .Property(x => x.LineTotal)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        modelBuilder.Entity<ServiceTypeOrder>()
            .ToTable(t => t.HasCheckConstraint("CK_ServiceTypeOrder_Quantity_Positive",
                "\"Quantity\" > 0"));

        modelBuilder.Entity<ServiceTypeOrder>()
            .ToTable(t => t.HasCheckConstraint("CK_ServiceTypeOrder_UnitPrice_NonNegative",
                "\"UnitPrice\" >= 0"));

        modelBuilder.Entity<ServiceTypeOrder>()
            .ToTable(t => t.HasCheckConstraint("CK_ServiceTypeOrder_LineTotal_NonNegative",
                "\"LineTotal\" >= 0"));

        modelBuilder.Entity<ServiceTypeOrder>()
            .HasIndex(x => new { x.OrderId, x.ServiceTypeId })
            .IsUnique()
            .HasDatabaseName("IX_ServiceTypeOrder_Unique_Order_ServiceType");

        modelBuilder.Entity<ServiceTypeOrder>()
            .HasOne(x => x.Order)
            .WithMany(x => x.ServiceTypeOrders)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_ServiceTypeOrder_Order");

        modelBuilder.Entity<ServiceTypeOrder>()
            .HasOne(x => x.ServiceType)
            .WithMany(x => x.ServiceTypeOrders)
            .HasForeignKey(x => x.ServiceTypeId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_ServiceTypeOrder_ServiceType");

        // ==================== ORDER CONFIGURATION ====================

        modelBuilder.Entity<Order>()
            .Property(x => x.PaymentStatusId)
            .IsRequired();

        modelBuilder.Entity<Order>()
            .Property(x => x.AmountPaid)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        modelBuilder.Entity<Order>()
            .Property(x => x.PaidAtUtc)
            .IsRequired(false);

        modelBuilder.Entity<Order>()
            .HasIndex(x => x.PaymentStatusId)
            .HasDatabaseName("IX_Order_PaymentStatusId");

        modelBuilder.Entity<Order>()
            .HasOne(x => x.PaymentStatus)
            .WithMany(x => x.Orders)
            .HasForeignKey(x => x.PaymentStatusId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Order_PaymentStatusLookup");

        modelBuilder.Entity<Order>()
            .ToTable(t => t.HasCheckConstraint("CK_Order_AmountPaid_NonNegative", "\"AmountPaid\" >= 0"));

        modelBuilder.Entity<Order>()
            .ToTable(t => t.HasCheckConstraint("CK_Order_AmountPaid_Lte_TotalAmount", "\"AmountPaid\" <= \"TotalAmount\""));

        // ==================== PAYMENT PROVIDER LOOKUP CONFIGURATION ====================

        modelBuilder.Entity<PaymentProviderLookup>()
            .HasKey(x => x.Id)
            .HasName("PK_PaymentProviderLookups");

        modelBuilder.Entity<PaymentProviderLookup>()
            .Property(x => x.Provider)
            .IsRequired();

        modelBuilder.Entity<PaymentProviderLookup>()
            .Property(x => x.Name)
            .HasMaxLength(50)
            .IsRequired();

        modelBuilder.Entity<PaymentProviderLookup>()
            .Property(x => x.Description)
            .HasMaxLength(200)
            .IsRequired();

        modelBuilder.Entity<PaymentProviderLookup>()
            .HasIndex(x => x.Provider)
            .IsUnique()
            .HasDatabaseName("IX_PaymentProviderLookup_Provider_Unique");

        // ==================== PAYMENT METHOD LOOKUP CONFIGURATION ====================

        modelBuilder.Entity<PaymentMethodLookup>()
            .HasKey(x => x.Id)
            .HasName("PK_PaymentMethodLookups");

        modelBuilder.Entity<PaymentMethodLookup>()
            .Property(x => x.Method)
            .IsRequired();

        modelBuilder.Entity<PaymentMethodLookup>()
            .Property(x => x.Name)
            .HasMaxLength(50)
            .IsRequired();

        modelBuilder.Entity<PaymentMethodLookup>()
            .Property(x => x.Description)
            .HasMaxLength(200)
            .IsRequired();

        modelBuilder.Entity<PaymentMethodLookup>()
            .HasIndex(x => x.Method)
            .IsUnique()
            .HasDatabaseName("IX_PaymentMethodLookup_Method_Unique");

        var paymentSeedTimestamp = new DateTime(2026, 7, 18, 10, 59, 32, 900, DateTimeKind.Utc).AddTicks(5000);

        // ==================== ORDER PAYMENT STATUS LOOKUP CONFIGURATION ====================

        modelBuilder.Entity<OrderPaymentStatusLookup>()
            .HasKey(x => x.Id)
            .HasName("PK_OrderPaymentStatusLookups");

        modelBuilder.Entity<OrderPaymentStatusLookup>()
            .Property(x => x.Status)
            .IsRequired();

        modelBuilder.Entity<OrderPaymentStatusLookup>()
            .Property(x => x.Name)
            .HasMaxLength(50)
            .IsRequired();

        modelBuilder.Entity<OrderPaymentStatusLookup>()
            .Property(x => x.Description)
            .HasMaxLength(200);

        modelBuilder.Entity<OrderPaymentStatusLookup>()
            .HasIndex(x => x.Status)
            .IsUnique()
            .HasDatabaseName("IX_OrderPaymentStatusLookup_Status_Unique");

        modelBuilder.Entity<OrderPaymentStatusLookup>().HasData(
            new OrderPaymentStatusLookup
            {
                Id = new Guid("00000000-0000-0000-0003-000000000101"),
                Status = OrderPaymentStatus.Pending,
                Name = "Pending",
                Description = "Order payment has not been completed yet",
                IsActive = true,
                CreatedAt = paymentSeedTimestamp,
                UpdatedAt = paymentSeedTimestamp
            },
            new OrderPaymentStatusLookup
            {
                Id = new Guid("00000000-0000-0000-0003-000000000102"),
                Status = OrderPaymentStatus.Paid,
                Name = "Paid",
                Description = "Order payment has been completed successfully",
                IsActive = true,
                CreatedAt = paymentSeedTimestamp,
                UpdatedAt = paymentSeedTimestamp
            },
            new OrderPaymentStatusLookup
            {
                Id = new Guid("00000000-0000-0000-0003-000000000103"),
                Status = OrderPaymentStatus.Failed,
                Name = "Failed",
                Description = "Order payment attempt failed",
                IsActive = true,
                CreatedAt = paymentSeedTimestamp,
                UpdatedAt = paymentSeedTimestamp
            },
            new OrderPaymentStatusLookup
            {
                Id = new Guid("00000000-0000-0000-0003-000000000104"),
                Status = OrderPaymentStatus.Expired,
                Name = "Expired",
                Description = "Order payment expired before completion",
                IsActive = true,
                CreatedAt = paymentSeedTimestamp,
                UpdatedAt = paymentSeedTimestamp
            },
            new OrderPaymentStatusLookup
            {
                Id = new Guid("00000000-0000-0000-0003-000000000105"),
                Status = OrderPaymentStatus.Cancelled,
                Name = "Cancelled",
                Description = "Order payment was cancelled",
                IsActive = true,
                CreatedAt = paymentSeedTimestamp,
                UpdatedAt = paymentSeedTimestamp
            },
            new OrderPaymentStatusLookup
            {
                Id = new Guid("00000000-0000-0000-0003-000000000106"),
                Status = OrderPaymentStatus.Refunded,
                Name = "Refunded",
                Description = "Order payment was refunded",
                IsActive = true,
                CreatedAt = paymentSeedTimestamp,
                UpdatedAt = paymentSeedTimestamp
            });

        // ==================== PAYMENT INTENT STATUS LOOKUP CONFIGURATION ====================

        modelBuilder.Entity<PaymentIntentStatusLookup>()
            .HasKey(x => x.Id)
            .HasName("PK_PaymentIntentStatusLookups");

        modelBuilder.Entity<PaymentIntentStatusLookup>()
            .Property(x => x.Status)
            .IsRequired();

        modelBuilder.Entity<PaymentIntentStatusLookup>()
            .Property(x => x.Name)
            .HasMaxLength(50)
            .IsRequired();

        modelBuilder.Entity<PaymentIntentStatusLookup>()
            .Property(x => x.Description)
            .HasMaxLength(200);

        modelBuilder.Entity<PaymentIntentStatusLookup>()
            .HasIndex(x => x.Status)
            .IsUnique()
            .HasDatabaseName("IX_PaymentIntentStatusLookup_Status_Unique");

        modelBuilder.Entity<PaymentIntentStatusLookup>().HasData(
            new PaymentIntentStatusLookup
            {
                Id = new Guid("00000000-0000-0000-0006-000000000101"),
                Status = PaymentIntentStatus.Initiated,
                Name = "Initiated",
                Description = "Payment intent has been created",
                IsActive = true,
                CreatedAt = paymentSeedTimestamp,
                UpdatedAt = paymentSeedTimestamp
            },
            new PaymentIntentStatusLookup
            {
                Id = new Guid("00000000-0000-0000-0006-000000000102"),
                Status = PaymentIntentStatus.Redirected,
                Name = "Redirected",
                Description = "User has been redirected to payment provider",
                IsActive = true,
                CreatedAt = paymentSeedTimestamp,
                UpdatedAt = paymentSeedTimestamp
            },
            new PaymentIntentStatusLookup
            {
                Id = new Guid("00000000-0000-0000-0006-000000000103"),
                Status = PaymentIntentStatus.Paid,
                Name = "Paid",
                Description = "Payment intent completed successfully",
                IsActive = true,
                CreatedAt = paymentSeedTimestamp,
                UpdatedAt = paymentSeedTimestamp
            },
            new PaymentIntentStatusLookup
            {
                Id = new Guid("00000000-0000-0000-0006-000000000104"),
                Status = PaymentIntentStatus.Failed,
                Name = "Failed",
                Description = "Payment intent failed",
                IsActive = true,
                CreatedAt = paymentSeedTimestamp,
                UpdatedAt = paymentSeedTimestamp
            },
            new PaymentIntentStatusLookup
            {
                Id = new Guid("00000000-0000-0000-0006-000000000105"),
                Status = PaymentIntentStatus.Expired,
                Name = "Expired",
                Description = "Payment intent expired",
                IsActive = true,
                CreatedAt = paymentSeedTimestamp,
                UpdatedAt = paymentSeedTimestamp
            },
            new PaymentIntentStatusLookup
            {
                Id = new Guid("00000000-0000-0000-0006-000000000106"),
                Status = PaymentIntentStatus.Cancelled,
                Name = "Cancelled",
                Description = "Payment intent was cancelled",
                IsActive = true,
                CreatedAt = paymentSeedTimestamp,
                UpdatedAt = paymentSeedTimestamp
            });

        // ==================== PAYMENT WEBHOOK PROCESS STATUS LOOKUP CONFIGURATION ====================

        modelBuilder.Entity<PaymentWebhookProcessStatusLookup>()
            .HasKey(x => x.Id)
            .HasName("PK_PaymentWebhookProcessStatusLookups");

        modelBuilder.Entity<PaymentWebhookProcessStatusLookup>()
            .Property(x => x.Status)
            .IsRequired();

        modelBuilder.Entity<PaymentWebhookProcessStatusLookup>()
            .Property(x => x.Name)
            .HasMaxLength(50)
            .IsRequired();

        modelBuilder.Entity<PaymentWebhookProcessStatusLookup>()
            .Property(x => x.Description)
            .HasMaxLength(200);

        modelBuilder.Entity<PaymentWebhookProcessStatusLookup>()
            .HasIndex(x => x.Status)
            .IsUnique()
            .HasDatabaseName("IX_PaymentWebhookProcessStatusLookup_Status_Unique");

        modelBuilder.Entity<PaymentWebhookProcessStatusLookup>().HasData(
            new PaymentWebhookProcessStatusLookup
            {
                Id = new Guid("00000000-0000-0000-0007-000000000101"),
                Status = PaymentWebhookProcessStatus.Received,
                Name = "Received",
                Description = "Webhook event has been received",
                IsActive = true,
                CreatedAt = paymentSeedTimestamp,
                UpdatedAt = paymentSeedTimestamp
            },
            new PaymentWebhookProcessStatusLookup
            {
                Id = new Guid("00000000-0000-0000-0007-000000000102"),
                Status = PaymentWebhookProcessStatus.Processed,
                Name = "Processed",
                Description = "Webhook event has been processed successfully",
                IsActive = true,
                CreatedAt = paymentSeedTimestamp,
                UpdatedAt = paymentSeedTimestamp
            },
            new PaymentWebhookProcessStatusLookup
            {
                Id = new Guid("00000000-0000-0000-0007-000000000103"),
                Status = PaymentWebhookProcessStatus.Ignored,
                Name = "Ignored",
                Description = "Webhook event was ignored as duplicate or irrelevant",
                IsActive = true,
                CreatedAt = paymentSeedTimestamp,
                UpdatedAt = paymentSeedTimestamp
            },
            new PaymentWebhookProcessStatusLookup
            {
                Id = new Guid("00000000-0000-0000-0007-000000000104"),
                Status = PaymentWebhookProcessStatus.Failed,
                Name = "Failed",
                Description = "Webhook event processing failed",
                IsActive = true,
                CreatedAt = paymentSeedTimestamp,
                UpdatedAt = paymentSeedTimestamp
            });

        modelBuilder.Entity<PaymentProviderLookup>().HasData(
            new PaymentProviderLookup
            {
                Id = new Guid("00000000-0000-0000-0003-000000000001"),
                Provider = PaymentProviderType.ZaloPay,
                Name = "ZaloPay",
                Description = "ZaloPay external payment gateway",
                IsActive = true,
                CreatedAt = paymentSeedTimestamp,
                UpdatedAt = paymentSeedTimestamp
            },
            new PaymentProviderLookup
            {
                Id = new Guid("00000000-0000-0000-0003-000000000002"),
                Provider = PaymentProviderType.Momo,
                Name = "MoMo",
                Description = "MoMo external payment gateway",
                IsActive = true,
                CreatedAt = paymentSeedTimestamp,
                UpdatedAt = paymentSeedTimestamp
            },
            new PaymentProviderLookup
            {
                Id = new Guid("00000000-0000-0000-0003-000000000003"),
                Provider = PaymentProviderType.VnPay,
                Name = "VNPay",
                Description = "VNPay external payment gateway",
                IsActive = true,
                CreatedAt = paymentSeedTimestamp,
                UpdatedAt = paymentSeedTimestamp
            },
            new PaymentProviderLookup
            {
                Id = new Guid("00000000-0000-0000-0003-000000000004"),
                Provider = PaymentProviderType.ShopeePay,
                Name = "ShopeePay",
                Description = "ShopeePay external payment gateway",
                IsActive = true,
                CreatedAt = paymentSeedTimestamp,
                UpdatedAt = paymentSeedTimestamp
            },
            new PaymentProviderLookup
            {
                Id = new Guid("00000000-0000-0000-0003-000000000005"),
                Provider = PaymentProviderType.OnePay,
                Name = "OnePay",
                Description = "OnePay external payment gateway",
                IsActive = true,
                CreatedAt = paymentSeedTimestamp,
                UpdatedAt = paymentSeedTimestamp
            }
        );

        modelBuilder.Entity<PaymentMethodLookup>().HasData(
            new PaymentMethodLookup
            {
                Id = new Guid("00000000-0000-0000-0008-000000000001"),
                Method = PaymentMethodType.AtmCardDomestic,
                Name = "ATM Card (Domestic)",
                Description = "Domestic ATM card payments",
                IsActive = true,
                CreatedAt = paymentSeedTimestamp,
                UpdatedAt = paymentSeedTimestamp
            },
            new PaymentMethodLookup
            {
                Id = new Guid("00000000-0000-0000-0008-000000000002"),
                Method = PaymentMethodType.CreditCard,
                Name = "Credit Card",
                Description = "Credit/debit card payments",
                IsActive = true,
                CreatedAt = paymentSeedTimestamp,
                UpdatedAt = paymentSeedTimestamp
            },
            new PaymentMethodLookup
            {
                Id = new Guid("00000000-0000-0000-0008-000000000003"),
                Method = PaymentMethodType.EWallet,
                Name = "E-Wallet",
                Description = "Wallet app payments",
                IsActive = true,
                CreatedAt = paymentSeedTimestamp,
                UpdatedAt = paymentSeedTimestamp
            },
            new PaymentMethodLookup
            {
                Id = new Guid("00000000-0000-0000-0008-000000000004"),
                Method = PaymentMethodType.ApplePay,
                Name = "Apple Pay",
                Description = "Apple Pay tokenized payments",
                IsActive = true,
                CreatedAt = paymentSeedTimestamp,
                UpdatedAt = paymentSeedTimestamp
            },
            new PaymentMethodLookup
            {
                Id = new Guid("00000000-0000-0000-0008-000000000005"),
                Method = PaymentMethodType.InternalWallet,
                Name = "Internal Wallet",
                Description = "Internal balance wallet",
                IsActive = true,
                CreatedAt = paymentSeedTimestamp,
                UpdatedAt = paymentSeedTimestamp
            }
        );

        // ==================== PAYMENT TRANSACTION STATUS LOOKUP CONFIGURATION ====================

        modelBuilder.Entity<PaymentTransactionStatusLookup>()
            .HasKey(x => x.Id)
            .HasName("PK_PaymentTransactionStatusLookups");

        modelBuilder.Entity<PaymentTransactionStatusLookup>()
            .Property(x => x.Status)
            .IsRequired();

        modelBuilder.Entity<PaymentTransactionStatusLookup>()
            .Property(x => x.Name)
            .HasMaxLength(50)
            .IsRequired();

        modelBuilder.Entity<PaymentTransactionStatusLookup>()
            .Property(x => x.Description)
            .HasMaxLength(200)
            .IsRequired();

        modelBuilder.Entity<PaymentTransactionStatusLookup>()
            .HasIndex(x => x.Status)
            .IsUnique()
            .HasDatabaseName("IX_PaymentTransactionStatusLookup_Status_Unique");

        modelBuilder.Entity<PaymentTransactionStatusLookup>().HasData(
            new PaymentTransactionStatusLookup
            {
                Id = new Guid("00000000-0000-0000-0005-000000000001"),
                Status = PaymentTransactionStatus.Pending,
                Name = "Pending",
                Description = "Transaction is created and waiting for processing",
                IsActive = true,
                CreatedAt = paymentSeedTimestamp,
                UpdatedAt = paymentSeedTimestamp
            },
            new PaymentTransactionStatusLookup
            {
                Id = new Guid("00000000-0000-0000-0005-000000000002"),
                Status = PaymentTransactionStatus.InProgress,
                Name = "In Progress",
                Description = "Transaction is being processed",
                IsActive = true,
                CreatedAt = paymentSeedTimestamp,
                UpdatedAt = paymentSeedTimestamp
            },
            new PaymentTransactionStatusLookup
            {
                Id = new Guid("00000000-0000-0000-0005-000000000003"),
                Status = PaymentTransactionStatus.Completed,
                Name = "Completed",
                Description = "Transaction has been completed successfully",
                IsActive = true,
                CreatedAt = paymentSeedTimestamp,
                UpdatedAt = paymentSeedTimestamp
            },
            new PaymentTransactionStatusLookup
            {
                Id = new Guid("00000000-0000-0000-0005-000000000004"),
                Status = PaymentTransactionStatus.Failed,
                Name = "Failed",
                Description = "Transaction failed during processing",
                IsActive = true,
                CreatedAt = paymentSeedTimestamp,
                UpdatedAt = paymentSeedTimestamp
            },
            new PaymentTransactionStatusLookup
            {
                Id = new Guid("00000000-0000-0000-0005-000000000005"),
                Status = PaymentTransactionStatus.Expired,
                Name = "Expired",
                Description = "Transaction expired before completion",
                IsActive = true,
                CreatedAt = paymentSeedTimestamp,
                UpdatedAt = paymentSeedTimestamp
            },
            new PaymentTransactionStatusLookup
            {
                Id = new Guid("00000000-0000-0000-0005-000000000006"),
                Status = PaymentTransactionStatus.Cancelled,
                Name = "Cancelled",
                Description = "Transaction was cancelled",
                IsActive = true,
                CreatedAt = paymentSeedTimestamp,
                UpdatedAt = paymentSeedTimestamp
            },
            new PaymentTransactionStatusLookup
            {
                Id = new Guid("00000000-0000-0000-0005-000000000007"),
                Status = PaymentTransactionStatus.Refunded,
                Name = "Refunded",
                Description = "Transaction amount has been refunded",
                IsActive = true,
                CreatedAt = paymentSeedTimestamp,
                UpdatedAt = paymentSeedTimestamp
            }
        );

        // ==================== PAYMENT TRANSACTION CONFIGURATION ====================

        modelBuilder.Entity<PaymentTransaction>()
            .HasKey(PaymentTransaction => PaymentTransaction.Id)
            .HasName("PK_PaymentTransactions");

        modelBuilder.Entity<PaymentTransaction>()
            .Property(PaymentTransaction => PaymentTransaction.TransactionCode)
            .HasMaxLength(50)
            .IsRequired();

        modelBuilder.Entity<PaymentTransaction>()
            .Property(PaymentTransaction => PaymentTransaction.PaymentProviderId)
            .IsRequired();

        modelBuilder.Entity<PaymentTransaction>()
            .Property(PaymentTransaction => PaymentTransaction.PaymentMethodId)
            .IsRequired();

        modelBuilder.Entity<PaymentTransaction>()
            .Property(PaymentTransaction => PaymentTransaction.FromAccount)
            .HasMaxLength(120)
            .IsRequired();

        modelBuilder.Entity<PaymentTransaction>()
            .Property(PaymentTransaction => PaymentTransaction.ToAccount)
            .HasMaxLength(120)
            .IsRequired();

        modelBuilder.Entity<PaymentTransaction>()
            .Property(PaymentTransaction => PaymentTransaction.Direction)
            .IsRequired();

        modelBuilder.Entity<PaymentTransaction>()
            .Property(PaymentTransaction => PaymentTransaction.Amount)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        modelBuilder.Entity<PaymentTransaction>()
            .Property(PaymentTransaction => PaymentTransaction.CurrencyId)
            .IsRequired();

        modelBuilder.Entity<PaymentTransaction>()
            .Property(PaymentTransaction => PaymentTransaction.StatusId)
            .IsRequired();

        modelBuilder.Entity<PaymentTransaction>()
            .Property(PaymentTransaction => PaymentTransaction.ProviderTransactionId)
            .HasMaxLength(120);

        modelBuilder.Entity<PaymentTransaction>()
            .Property(PaymentTransaction => PaymentTransaction.ProviderEventId)
            .HasMaxLength(120);

        modelBuilder.Entity<PaymentTransaction>()
            .Property(PaymentTransaction => PaymentTransaction.RawProviderPayload)
            .HasColumnType("text");

        modelBuilder.Entity<PaymentTransaction>()
            .Property(PaymentTransaction => PaymentTransaction.CompletedAtUtc)
            .IsRequired(false);

        modelBuilder.Entity<PaymentTransaction>()
            .Property(PaymentTransaction => PaymentTransaction.FailedAtUtc)
            .IsRequired(false);

        modelBuilder.Entity<PaymentTransaction>()
            .Property(PaymentTransaction => PaymentTransaction.FailureCode)
            .HasMaxLength(60);

        modelBuilder.Entity<PaymentTransaction>()
            .Property(PaymentTransaction => PaymentTransaction.FailureMessage)
            .HasMaxLength(500);

        modelBuilder.Entity<PaymentTransaction>()
            .Property(PaymentTransaction => PaymentTransaction.TransactionAtUtc)
            .IsRequired();

        modelBuilder.Entity<PaymentTransaction>()
            .ToTable(t => t.HasCheckConstraint("CK_PaymentTransaction_Amount_NonNegative",
                "\"Amount\" >= 0"));

        modelBuilder.Entity<PaymentTransaction>()
            .HasIndex(PaymentTransaction => PaymentTransaction.TransactionCode)
            .IsUnique()
            .HasDatabaseName("IX_PaymentTransaction_TransactionCode_Unique");

        modelBuilder.Entity<PaymentTransaction>()
            .HasIndex(PaymentTransaction => PaymentTransaction.PaymentProviderId)
            .HasDatabaseName("IX_PaymentTransaction_PaymentProviderId");

        modelBuilder.Entity<PaymentTransaction>()
            .HasIndex(PaymentTransaction => PaymentTransaction.PaymentMethodId)
            .HasDatabaseName("IX_PaymentTransaction_PaymentMethodId");

        modelBuilder.Entity<PaymentTransaction>()
            .HasIndex(PaymentTransaction => new { PaymentTransaction.PaymentProviderId, PaymentTransaction.ProviderEventId })
            .IsUnique()
            .HasDatabaseName("IX_PaymentTransaction_Provider_Event_Unique");

        modelBuilder.Entity<PaymentTransaction>()
            .HasOne(PaymentTransaction => PaymentTransaction.PaymentProvider)
            .WithMany()
            .HasForeignKey(PaymentTransaction => PaymentTransaction.PaymentProviderId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_PaymentTransaction_PaymentProvider");

        modelBuilder.Entity<PaymentTransaction>()
            .HasOne(PaymentTransaction => PaymentTransaction.PaymentMethod)
            .WithMany()
            .HasForeignKey(PaymentTransaction => PaymentTransaction.PaymentMethodId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_PaymentTransaction_PaymentMethod");

        modelBuilder.Entity<PaymentTransaction>()
            .HasOne(PaymentTransaction => PaymentTransaction.Currency)
            .WithMany(Currency => Currency.PaymentTransactions)
            .HasForeignKey(PaymentTransaction => PaymentTransaction.CurrencyId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_PaymentTransaction_Currency");

        modelBuilder.Entity<PaymentTransaction>()
            .HasOne(PaymentTransaction => PaymentTransaction.Status)
            .WithMany(PaymentTransactionStatusLookup => PaymentTransactionStatusLookup.PaymentTransactions)
            .HasForeignKey(PaymentTransaction => PaymentTransaction.StatusId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_PaymentTransaction_StatusLookup");

        // ==================== PAYMENT ORDER CONFIGURATION ====================

        modelBuilder.Entity<PaymentOrder>()
            .HasKey(x => x.Id)
            .HasName("PK_PaymentOrders");

        modelBuilder.Entity<PaymentOrder>()
            .Property(x => x.OrderId)
            .IsRequired();

        modelBuilder.Entity<PaymentOrder>()
            .Property(x => x.PaymentProviderId)
            .IsRequired();

        modelBuilder.Entity<PaymentOrder>()
            .Property(x => x.PaymentMethodId)
            .IsRequired();

        modelBuilder.Entity<PaymentOrder>()
            .Property(x => x.StatusId)
            .IsRequired();

        modelBuilder.Entity<PaymentOrder>()
            .Property(x => x.IntentCode)
            .HasMaxLength(80)
            .IsRequired();

        modelBuilder.Entity<PaymentOrder>()
            .Property(x => x.CheckoutUrl)
            .HasColumnType("text");

        modelBuilder.Entity<PaymentOrder>()
            .Property(x => x.ExpiresAtUtc)
            .IsRequired(false);

        modelBuilder.Entity<PaymentOrder>()
            .Property(x => x.LastProviderEventId)
            .HasMaxLength(120);

        modelBuilder.Entity<PaymentOrder>()
            .Property(x => x.LastProviderEventAtUtc)
            .IsRequired(false);

        modelBuilder.Entity<PaymentOrder>()
            .Property(x => x.PaymentTransactionId)
            .IsRequired(false);

        modelBuilder.Entity<PaymentOrder>()
            .Property(x => x.OrderedAtUtc)
            .IsRequired();

        modelBuilder.Entity<PaymentOrder>()
            .HasIndex(x => x.PaymentProviderId)
            .HasDatabaseName("IX_PaymentOrder_PaymentProviderId");

        modelBuilder.Entity<PaymentOrder>()
            .HasIndex(x => x.PaymentMethodId)
            .HasDatabaseName("IX_PaymentOrder_PaymentMethodId");

        modelBuilder.Entity<PaymentOrder>()
            .HasIndex(x => x.StatusId)
            .HasDatabaseName("IX_PaymentOrder_StatusId");

        modelBuilder.Entity<PaymentOrder>()
            .HasOne(x => x.Status)
            .WithMany(x => x.PaymentOrders)
            .HasForeignKey(x => x.StatusId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_PaymentOrder_StatusLookup");

        modelBuilder.Entity<PaymentOrder>()
            .HasIndex(x => x.IntentCode)
            .IsUnique()
            .HasDatabaseName("IX_PaymentOrder_IntentCode_Unique");

        modelBuilder.Entity<PaymentOrder>()
            .HasIndex(x => x.PaymentTransactionId)
            .IsUnique()
            .HasDatabaseName("IX_PaymentOrder_TransactionId_Unique");

        modelBuilder.Entity<PaymentOrder>()
            .HasIndex(x => x.OrderId)
            .IsUnique()
            .HasDatabaseName("IX_PaymentOrder_OrderId");

        modelBuilder.Entity<PaymentOrder>()
            .HasOne(x => x.PaymentProvider)
            .WithMany()
            .HasForeignKey(x => x.PaymentProviderId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_PaymentOrder_PaymentProvider");

        modelBuilder.Entity<PaymentOrder>()
            .HasOne(x => x.PaymentMethod)
            .WithMany()
            .HasForeignKey(x => x.PaymentMethodId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_PaymentOrder_PaymentMethod");

        modelBuilder.Entity<PaymentOrder>()
            .HasOne(PaymentOrder => PaymentOrder.PaymentTransaction)
            .WithOne(PaymentTransaction => PaymentTransaction.PaymentOrder)
            .HasForeignKey<PaymentOrder>(PaymentOrder => PaymentOrder.PaymentTransactionId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_PaymentOrder_PaymentTransaction");

        modelBuilder.Entity<PaymentOrder>()
            .HasOne(x => x.Order)
            .WithOne(x => x.PaymentOrder)
            .HasForeignKey<PaymentOrder>(x => x.OrderId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_PaymentOrder_Order");

        // ==================== PAYMENT WEBHOOK INBOX CONFIGURATION ====================

        modelBuilder.Entity<PaymentWebhookInbox>()
            .HasKey(x => x.Id)
            .HasName("PK_PaymentWebhookInboxes");

        modelBuilder.Entity<PaymentWebhookInbox>()
            .Property(x => x.PaymentProviderId)
            .IsRequired();

        modelBuilder.Entity<PaymentWebhookInbox>()
            .Property(x => x.EventId)
            .HasMaxLength(120)
            .IsRequired();

        modelBuilder.Entity<PaymentWebhookInbox>()
            .Property(x => x.SignatureHash)
            .HasMaxLength(128)
            .IsRequired();

        modelBuilder.Entity<PaymentWebhookInbox>()
            .Property(x => x.Payload)
            .HasColumnType("text")
            .IsRequired();

        modelBuilder.Entity<PaymentWebhookInbox>()
            .Property(x => x.ProcessStatusId)
            .IsRequired();

        modelBuilder.Entity<PaymentWebhookInbox>()
            .Property(x => x.ReceivedAtUtc)
            .IsRequired();

        modelBuilder.Entity<PaymentWebhookInbox>()
            .Property(x => x.ProcessedAtUtc)
            .IsRequired(false);

        modelBuilder.Entity<PaymentWebhookInbox>()
            .Property(x => x.ErrorMessage)
            .HasMaxLength(500);

        modelBuilder.Entity<PaymentWebhookInbox>()
            .HasIndex(x => x.ProcessStatusId)
            .HasDatabaseName("IX_PaymentWebhookInbox_ProcessStatusId");

        modelBuilder.Entity<PaymentWebhookInbox>()
            .HasIndex(x => new { x.PaymentProviderId, x.EventId })
            .IsUnique()
            .HasDatabaseName("IX_PaymentWebhookInbox_Provider_Event_Unique");

        modelBuilder.Entity<PaymentWebhookInbox>()
            .HasOne(x => x.PaymentProvider)
            .WithMany()
            .HasForeignKey(x => x.PaymentProviderId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_PaymentWebhookInbox_PaymentProvider");

        modelBuilder.Entity<PaymentWebhookInbox>()
            .HasOne(x => x.ProcessStatus)
            .WithMany(x => x.PaymentWebhookInboxes)
            .HasForeignKey(x => x.ProcessStatusId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_PaymentWebhookInbox_ProcessStatusLookup");

        // ==================== ORDER APPOINTMENT JUNCTION CONFIGURATION ====================

        modelBuilder.Entity<OrderAppointment>()
            .HasKey(x => x.Id)
            .HasName("PK_OrderAppointments");

        modelBuilder.Entity<OrderAppointment>()
            .Property(x => x.OrderId)
            .IsRequired();

        modelBuilder.Entity<OrderAppointment>()
            .Property(x => x.AppointmentId)
            .IsRequired();

        modelBuilder.Entity<OrderAppointment>()
            .HasIndex(x => new { x.OrderId, x.AppointmentId })
            .IsUnique()
            .HasDatabaseName("IX_OrderAppointment_Unique_Order_Appointment");

        modelBuilder.Entity<OrderAppointment>()
            .HasIndex(x => x.AppointmentId)
            .IsUnique()
            .HasDatabaseName("IX_OrderAppointment_Appointment_Unique");

        modelBuilder.Entity<OrderAppointment>()
            .HasOne(x => x.Order)
            .WithMany(x => x.OrderAppointments)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_OrderAppointment_Order");

        modelBuilder.Entity<OrderAppointment>()
            .HasOne(x => x.Appointment)
            .WithOne(x => x.OrderAppointment)
            .HasForeignKey<OrderAppointment>(x => x.AppointmentId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_OrderAppointment_Appointment");

        // ==================== TECHNICIAN CONFIGURATION ====================
        
        // Configure Technician primary key
        modelBuilder.Entity<Technician>()
            .HasKey(x => x.Id)
            .HasName("PK_Technicians");

        // Technician property constraints
        modelBuilder.Entity<Technician>()
            .Property(x => x.FirstName)
            .HasMaxLength(100)
            .IsRequired();
        modelBuilder.Entity<Technician>()
            .Property(x => x.LastName)
            .HasMaxLength(100)
            .IsRequired();

        // Technician -> TechnicianSchedule (One-to-Many)
        modelBuilder.Entity<TechnicianSchedule>()
            .HasOne(ts => ts.Technician)
            .WithMany(t => t.Schedules)
            .HasForeignKey(ts => ts.TechnicianId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_TechnicianSchedule_Technician");

        // TechnicianSchedule property constraints
        modelBuilder.Entity<TechnicianSchedule>()
            .Property(x => x.TechnicianId)
            .IsRequired();

        // Technician -> TechnicianSkill (One-to-Many)
        modelBuilder.Entity<TechnicianSkill>()
            .HasOne(ts => ts.Technician)
            .WithMany(t => t.Skills)
            .HasForeignKey(ts => ts.TechnicianId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_TechnicianSkill_Technician");

        // ServiceType -> TechnicianSkill (One-to-Many)
        modelBuilder.Entity<TechnicianSkill>()
            .HasOne(ts => ts.ServiceType)
            .WithMany(st => st.TechnicianSkills)
            .HasForeignKey(ts => ts.ServiceTypeId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_TechnicianSkill_ServiceType");

        // TechnicianSkill property constraints
        modelBuilder.Entity<TechnicianSkill>()
            .Property(x => x.TechnicianId)
            .IsRequired();
        modelBuilder.Entity<TechnicianSkill>()
            .Property(x => x.ServiceTypeId)
            .IsRequired();

        // ==================== SERVICE BAY CONFIGURATION ====================
        
        // Configure ServiceBay primary key
        modelBuilder.Entity<ServiceBay>()
            .HasKey(x => x.Id)
            .HasName("PK_ServiceBays");

        // ServiceBay property constraints
        modelBuilder.Entity<ServiceBay>()
            .Property(x => x.Name)
            .HasMaxLength(50)
            .IsRequired();
        // ==================== VEHICLE CONFIGURATION ====================
        
        // Configure Vehicle primary key
        modelBuilder.Entity<Vehicle>()
            .HasKey(x => x.Id)
            .HasName("PK_Vehicles");

        // Vehicle property constraints
        modelBuilder.Entity<Vehicle>()
            .Property(x => x.Make)
            .HasMaxLength(20)
            .IsRequired();
        modelBuilder.Entity<Vehicle>()
            .Property(x => x.Model)
            .HasMaxLength(50)
            .IsRequired();
        modelBuilder.Entity<Vehicle>()
            .Property(x => x.Vin)
            .HasMaxLength(17)
            .IsRequired();
        // VIN must be exactly 17 characters (per original entity constraint)
        modelBuilder.Entity<Vehicle>()
            .ToTable(t => t.HasCheckConstraint("CK_Vehicle_VIN_Length", 
                "LENGTH(\"Vin\") = 17"));
        modelBuilder.Entity<Vehicle>()
            .Property(x => x.Year)
            .IsRequired(false); // Optional field
        // Year must be between 1900 and 2100 (per original entity constraint)
        modelBuilder.Entity<Vehicle>()
            .ToTable(t => t.HasCheckConstraint("CK_Vehicle_Year_Range", 
                "\"Year\" IS NULL OR (\"Year\" >= 1900 AND \"Year\" <= 2100)"));

        // ==================== TECHNICIAN SKILL CONFIGURATION ====================
        
        // Configure TechnicianSkill primary key
        modelBuilder.Entity<TechnicianSkill>()
            .HasKey(x => x.Id)
            .HasName("PK_TechnicianSkills");

        // Unique constraint on (TechnicianId, ServiceTypeId)
        modelBuilder.Entity<TechnicianSkill>()
            .HasIndex(x => new { x.TechnicianId, x.ServiceTypeId })
            .IsUnique()
            .HasDatabaseName("IX_TechnicianSkill_Unique_TechnicianServiceType");

        // ==================== BUSINESS HOURS CONFIGURATION ====================
        
        // Configure BusinessHours primary key
        modelBuilder.Entity<BusinessHours>()
            .HasKey(x => x.Id)
            .HasName("PK_BusinessHours");

        // BusinessHours property constraints
        modelBuilder.Entity<BusinessHours>()
            .Property(x => x.DayOfWeek)
            .IsRequired();
        modelBuilder.Entity<BusinessHours>()
            .Property(x => x.OpenTime)
            .IsRequired();
        modelBuilder.Entity<BusinessHours>()
            .Property(x => x.CloseTime)
            .IsRequired();

        // ==================== TECHNICIAN SCHEDULE CONFIGURATION ====================
        
        // Configure TechnicianSchedule primary key
        modelBuilder.Entity<TechnicianSchedule>()
            .HasKey(x => x.Id)
            .HasName("PK_TechnicianSchedules");

        // TechnicianSchedule property constraints
        modelBuilder.Entity<TechnicianSchedule>()
            .Property(x => x.DayOfWeek)
            .IsRequired();
        modelBuilder.Entity<TechnicianSchedule>()
            .Property(x => x.StartTime)
            .IsRequired();
        modelBuilder.Entity<TechnicianSchedule>()
            .Property(x => x.EndTime)
            .IsRequired();

        // ==================== TIMESLOT CONFIGURATION ====================
        
        // Configure TimeSlot primary key
        modelBuilder.Entity<TimeSlot>()
            .HasKey(x => x.Id)
            .HasName("PK_TimeSlots");

        // Unique constraint on SequenceOrder
        modelBuilder.Entity<TimeSlot>()
            .HasIndex(x => x.SequenceOrder)
            .IsUnique()
            .HasDatabaseName("UK_TimeSlot_SequenceOrder");

        // TimeSlot property constraints
        modelBuilder.Entity<TimeSlot>()
            .Property(x => x.SequenceOrder)
            .IsRequired()
            .ValueGeneratedNever();

        modelBuilder.Entity<TimeSlot>()
            .Property(x => x.SlotStartTime)
            .IsRequired()
            .HasColumnType("time");

        modelBuilder.Entity<TimeSlot>()
            .Property(x => x.SlotEndTime)
            .IsRequired()
            .HasColumnType("time");

        // TimeSlot relationships to Service (for estimated timing)
        modelBuilder.Entity<TimeSlot>()
            .HasMany(x => x.EstimatedStartServices)
            .WithOne(s => s.EstimatedStartTimeSlot)
            .HasForeignKey("EstimatedStartTimeSlotId")
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Service_TimeSlot_Start");

        modelBuilder.Entity<TimeSlot>()
            .HasMany(x => x.EstimatedEndServices)
            .WithOne(s => s.EstimatedEndTimeSlot)
            .HasForeignKey("EstimatedEndTimeSlotId")
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Service_TimeSlot_End");

        // ==================== VIEW CONFIGURATIONS (READ-ONLY) ====================
        
        // TechnicianAvailableSlots View Configuration
        var technicianAvailableSlotsEntity = modelBuilder.Entity<TechnicianAvailableSlotsView>();
        technicianAvailableSlotsEntity
            .HasKey(x => new { x.TimeSlotId, x.TechnicianId })
            .HasName("PK_TechnicianAvailableSlots");
        technicianAvailableSlotsEntity
            .Property(x => x.IsActive)
            .HasColumnType("boolean");
        technicianAvailableSlotsEntity
            .HasQueryFilter(x => x.IsActive);
        technicianAvailableSlotsEntity
            .ToView("TechnicianAvailableSlots");

        // ServiceBayAvailableSlots View Configuration
        var serviceBayAvailableSlotsEntity = modelBuilder.Entity<ServiceBayAvailableSlotsView>();
        serviceBayAvailableSlotsEntity
            .HasKey(x => new { x.TimeSlotId, x.ServiceBayId })
            .HasName("PK_ServiceBayAvailableSlots");
        serviceBayAvailableSlotsEntity
            .Property(x => x.IsActive)
            .HasColumnType("boolean");
        serviceBayAvailableSlotsEntity
            .HasQueryFilter(x => x.IsActive);
        serviceBayAvailableSlotsEntity
            .ToView("ServiceBayAvailableSlots");

        // ServiceTypeAvailability View Configuration
        var serviceTypeAvailabilityEntity = modelBuilder.Entity<ServiceTypeAvailabilityView>();
        serviceTypeAvailabilityEntity
            .HasKey(x => new { x.ServiceTypeId, x.TimeSlotId, x.TechnicianId, x.ServiceBayId })
            .HasName("PK_ServiceTypeAvailability");
        serviceTypeAvailabilityEntity
            .Property(x => x.IsActive)
            .HasColumnType("boolean");
        serviceTypeAvailabilityEntity
            .HasQueryFilter(x => x.IsActive);
        serviceTypeAvailabilityEntity
            .ToView("ServiceTypeAvailability");

        // ==================== TIMESLOT SEEDING ====================
        
        // Seed 18 TimeSlots: 08:00-17:00 in 30-minute increments
        var timeSlots = new List<TimeSlot>();
        var businessStart = new TimeOnly(8, 0);    // 08:00
        var businessEnd = new TimeOnly(17, 0);     // 17:00
        var currentSlotTime = businessStart;
        int sequenceOrder = 1;
        
        while (currentSlotTime < businessEnd)
        {
            var slotEnd = currentSlotTime.AddMinutes(30);
            
            timeSlots.Add(new TimeSlot
            {
                Id = Guid.Parse($"00000000-0000-0000-0000-{sequenceOrder:000000000000}"),
                SequenceOrder = sequenceOrder,
                SlotStartTime = currentSlotTime,
                SlotEndTime = slotEnd,
                IsActive = true,
                CreatedAt = paymentSeedTimestamp,
                UpdatedAt = paymentSeedTimestamp
            });

            currentSlotTime = slotEnd;
            sequenceOrder++;
        }

        modelBuilder.Entity<TimeSlot>().HasData(timeSlots);
    }

    private static LambdaExpression CreateIsActiveFilter(Type clrType)
    {
        var parameter = Expression.Parameter(clrType, "entity");
        var propertyAccess = Expression.Call(
            typeof(EF),
            nameof(EF.Property),
            new[] { typeof(bool) },
            parameter,
            Expression.Constant("IsActive"));
        var body = Expression.Equal(propertyAccess, Expression.Constant(true));
        return Expression.Lambda(body, parameter);
    }
}