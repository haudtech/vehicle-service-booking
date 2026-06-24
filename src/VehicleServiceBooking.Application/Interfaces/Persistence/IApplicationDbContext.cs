using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Application.Interfaces.Persistence;

public interface IApplicationDbContext
{
    DbSet<Appointment> Appointments { get; }

    DbSet<ServiceType> ServiceTypes { get; }

    DbSet<Technician> Technicians { get; }

    DbSet<TechnicianSchedule> TechnicianSchedules { get; }

    DbSet<TechnicianSkill> TechnicianSkills { get; }

    DbSet<ServiceBay> ServiceBays { get; }

    DbSet<BusinessHours> BusinessHours { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}