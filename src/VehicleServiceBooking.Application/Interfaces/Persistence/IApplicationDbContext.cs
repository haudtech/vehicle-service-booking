using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VehicleServiceBooking.Application.Models.ViewModels;
using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Application.Interfaces.Persistence;

public interface IApplicationDbContext
{
    DbContext DbContext { get; }

    DbSet<Appointment> Appointments { get; }

    DbSet<Service> Services { get; }

    DbSet<ServiceStatusLookup> ServiceStatusLookups { get; }

    DbSet<ServiceType> ServiceTypes { get; }

    DbSet<Technician> Technicians { get; }

    DbSet<TechnicianSchedule> TechnicianSchedules { get; }

    DbSet<TechnicianSkill> TechnicianSkills { get; }

    DbSet<ServiceBay> ServiceBays { get; }

    DbSet<BusinessHours> BusinessHours { get; }

    DbSet<Customer> Customers { get; }

    DbSet<Dealership> Dealerships { get; }

    DbSet<Vehicle> Vehicles { get; }

    DbSet<IdempotencyRequest> IdempotencyRequests { get; }

    DbSet<IdempotencyRequestStatusLookup> IdempotencyRequestStatusLookups { get; }

    DbSet<AppointmentStatusLookup> AppointmentStatusLookups { get; }

    DbSet<TimeSlot> TimeSlots { get; }

    // ==================== VIEW DbSETS (READ-ONLY) ====================

    /// <summary>
    /// Query-only DbSet for TechnicianAvailableSlots view.
    /// Returns pre-computed available time slots for each technician on the query date.
    /// </summary>
    DbSet<TechnicianAvailableSlotsView> TechnicianAvailableSlotsView { get; }

    /// <summary>
    /// Query-only DbSet for ServiceBayAvailableSlots view.
    /// Returns pre-computed available time slots for each service bay on the query date.
    /// </summary>
    DbSet<ServiceBayAvailableSlotsView> ServiceBayAvailableSlotsView { get; }

    /// <summary>
    /// Query-only DbSet for ServiceTypeAvailability master view.
    /// Returns pre-computed availability combining service requirements with technician and bay availability.
    /// </summary>
    DbSet<ServiceTypeAvailabilityView> ServiceTypeAvailabilityView { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}