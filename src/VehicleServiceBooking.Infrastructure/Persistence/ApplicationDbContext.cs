using Microsoft.EntityFrameworkCore;
using VehicleServiceBooking.Application.Interfaces.Persistence;
using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbContext DbContext => this;

    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<ServiceType> ServiceTypes => Set<ServiceType>();
    public DbSet<Technician> Technicians => Set<Technician>();
    public DbSet<TechnicianSchedule> TechnicianSchedules => Set<TechnicianSchedule>();
    public DbSet<TechnicianSkill> TechnicianSkills => Set<TechnicianSkill>();
    public DbSet<ServiceBay> ServiceBays => Set<ServiceBay>();
    public DbSet<BusinessHours> BusinessHours => Set<BusinessHours>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Dealership> Dealerships => Set<Dealership>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Appointment>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<ServiceType>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<Technician>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<ServiceBay>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<TechnicianSkill>()
            .HasKey(x => x.Id);

        // Add unique constraint on TechnicianId + ServiceTypeId to prevent duplicates
        modelBuilder.Entity<TechnicianSkill>()
            .HasIndex(x => new { x.TechnicianId, x.ServiceTypeId })
            .IsUnique();

        modelBuilder.Entity<Customer>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<Dealership>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<Vehicle>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<BusinessHours>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<TechnicianSchedule>()
            .HasKey(x => x.Id);
    }
}