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
    public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();

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

        modelBuilder.Entity<AppointmentStatusLookup>().HasData(
            new AppointmentStatusLookup { Id = bookedId, Status = AppointmentStatus.Booked, Name = "Booked", Description = "Appointment is scheduled" },
            new AppointmentStatusLookup { Id = inProgressId, Status = AppointmentStatus.InProgress, Name = "In Progress", Description = "Service is currently being performed" },
            new AppointmentStatusLookup { Id = completedId, Status = AppointmentStatus.Completed, Name = "Completed", Description = "Service has been completed" },
            new AppointmentStatusLookup { Id = cancelledId, Status = AppointmentStatus.Cancelled, Name = "Cancelled", Description = "Appointment has been cancelled" },
            new AppointmentStatusLookup { Id = partiallyCompletedId, Status = AppointmentStatus.PartiallyCompleted, Name = "Partially Completed", Description = "Some services completed, others rescheduled" }
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

        modelBuilder.Entity<ServiceStatusLookup>().HasData(
            new ServiceStatusLookup { Id = serviceStatusPendingId, Status = ServiceStatus.Pending, Name = "Pending", Description = "Service scheduled but not started" },
            new ServiceStatusLookup { Id = serviceStatusInProgressId, Status = ServiceStatus.InProgress, Name = "In Progress", Description = "Service is currently being performed" },
            new ServiceStatusLookup { Id = serviceStatusCompletedId, Status = ServiceStatus.Completed, Name = "Completed", Description = "Service has been completed successfully" },
            new ServiceStatusLookup { Id = serviceStatusSkippedId, Status = ServiceStatus.Skipped, Name = "Skipped", Description = "Service was cancelled or declined" },
            new ServiceStatusLookup { Id = serviceStatusRescheduledId, Status = ServiceStatus.Rescheduled, Name = "Rescheduled", Description = "Service moved to a different appointment" }
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

        // ==================== CUSTOMER CONFIGURATION ====================
        
        // Configure Customer primary key
        modelBuilder.Entity<Customer>()
            .HasKey(x => x.Id)
            .HasName("PK_Customers");

        // Customer property constraints
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
        modelBuilder.Entity<ServiceType>()
            .Property(x => x.Price)
            .HasColumnType("numeric(10,2)")
            .HasDefaultValue(0m)
            .IsRequired();
        // DurationMinutes must be between 30 and 480 minutes (per original entity constraint)
        modelBuilder.Entity<ServiceType>()
            .ToTable(t => t.HasCheckConstraint("CK_ServiceType_DurationMinutes_Range",
                "\"DurationMinutes\" >= 30 AND \"DurationMinutes\" <= 480"));
        modelBuilder.Entity<ServiceType>()
            .ToTable(t => t.HasCheckConstraint("CK_ServiceType_Price_NonNegative",
                "\"Price\" >= 0"));

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
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
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