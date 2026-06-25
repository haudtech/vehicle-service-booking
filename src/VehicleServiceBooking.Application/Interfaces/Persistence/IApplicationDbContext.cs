using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

    DbSet<AppointmentStatusLookup> AppointmentStatusLookups { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}