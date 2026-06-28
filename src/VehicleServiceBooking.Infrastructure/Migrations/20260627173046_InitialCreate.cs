using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VehicleServiceBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppointmentStatusLookups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentStatusLookups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dealerships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dealerships", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceStatusLookups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceStatusLookups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTypes", x => x.Id);
                    table.CheckConstraint("CK_ServiceType_DurationMinutes_Range", "\"DurationMinutes\" >= 30 AND \"DurationMinutes\" <= 480");
                });

            migrationBuilder.CreateTable(
                name: "TimeSlots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SequenceOrder = table.Column<int>(type: "integer", nullable: false),
                    SlotStartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    SlotEndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSlots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Vin = table.Column<string>(type: "character varying(17)", maxLength: 17, nullable: false),
                    Make = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Model = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                    table.CheckConstraint("CK_Vehicle_VIN_Length", "LENGTH(\"Vin\") = 17");
                    table.CheckConstraint("CK_Vehicle_Year_Range", "\"Year\" IS NULL OR (\"Year\" >= 1900 AND \"Year\" <= 2100)");
                    table.ForeignKey(
                        name: "FK_Vehicle_Customer",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusinessHours",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DealershipId = table.Column<Guid>(type: "uuid", nullable: false),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    OpenTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    CloseTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessHours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessHours_Dealership",
                        column: x => x.DealershipId,
                        principalTable: "Dealerships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceBays",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DealershipId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceBays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceBay_Dealership",
                        column: x => x.DealershipId,
                        principalTable: "Dealerships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Technicians",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DealershipId = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Technicians", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Technician_Dealership",
                        column: x => x.DealershipId,
                        principalTable: "Dealerships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DealershipId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: false),
                    AppointmentDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StatusId = table.Column<Guid>(type: "uuid", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointment_AppointmentStatus",
                        column: x => x.StatusId,
                        principalTable: "AppointmentStatusLookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointment_Customer",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointment_Dealership",
                        column: x => x.DealershipId,
                        principalTable: "Dealerships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointment_Vehicle",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TechnicianSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TechnicianId = table.Column<Guid>(type: "uuid", nullable: false),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TechnicianSchedule_Technician",
                        column: x => x.TechnicianId,
                        principalTable: "Technicians",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TechnicianSkills",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TechnicianId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TechnicianSkill_ServiceType",
                        column: x => x.ServiceTypeId,
                        principalTable: "ServiceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TechnicianSkill_Technician",
                        column: x => x.TechnicianId,
                        principalTable: "Technicians",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AppointmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    TechnicianId = table.Column<Guid>(type: "uuid", nullable: true),
                    ServiceBayId = table.Column<Guid>(type: "uuid", nullable: true),
                    DealershipId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceStatusId = table.Column<Guid>(type: "uuid", nullable: false),
                    EstimatedStartTimeSlotId = table.Column<Guid>(type: "uuid", nullable: true),
                    EstimatedEndTimeSlotId = table.Column<Guid>(type: "uuid", nullable: true),
                    SequenceOrder = table.Column<int>(type: "integer", nullable: false),
                    ActualStartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActualEndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Service_Appointment",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Service_Dealership",
                        column: x => x.DealershipId,
                        principalTable: "Dealerships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Service_ServiceBay",
                        column: x => x.ServiceBayId,
                        principalTable: "ServiceBays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Service_ServiceStatus",
                        column: x => x.ServiceStatusId,
                        principalTable: "ServiceStatusLookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Service_ServiceType",
                        column: x => x.ServiceTypeId,
                        principalTable: "ServiceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Service_Technician",
                        column: x => x.TechnicianId,
                        principalTable: "Technicians",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Service_TimeSlot_End",
                        column: x => x.EstimatedEndTimeSlotId,
                        principalTable: "TimeSlots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Service_TimeSlot_Start",
                        column: x => x.EstimatedStartTimeSlotId,
                        principalTable: "TimeSlots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AppointmentStatusLookups",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2026, 6, 27, 17, 30, 46, 128, DateTimeKind.Utc).AddTicks(4850), "Appointment is scheduled", "Booked", 1, new DateTime(2026, 6, 27, 17, 30, 46, 128, DateTimeKind.Utc).AddTicks(4850) },
                    { new Guid("00000000-0000-0000-0000-000000000002"), new DateTime(2026, 6, 27, 17, 30, 46, 128, DateTimeKind.Utc).AddTicks(4860), "Service is currently being performed", "In Progress", 2, new DateTime(2026, 6, 27, 17, 30, 46, 128, DateTimeKind.Utc).AddTicks(4860) },
                    { new Guid("00000000-0000-0000-0000-000000000003"), new DateTime(2026, 6, 27, 17, 30, 46, 128, DateTimeKind.Utc).AddTicks(4860), "Service has been completed", "Completed", 3, new DateTime(2026, 6, 27, 17, 30, 46, 128, DateTimeKind.Utc).AddTicks(4860) },
                    { new Guid("00000000-0000-0000-0000-000000000004"), new DateTime(2026, 6, 27, 17, 30, 46, 128, DateTimeKind.Utc).AddTicks(4870), "Appointment has been cancelled", "Cancelled", 4, new DateTime(2026, 6, 27, 17, 30, 46, 128, DateTimeKind.Utc).AddTicks(4870) },
                    { new Guid("00000000-0000-0000-0000-000000000005"), new DateTime(2026, 6, 27, 17, 30, 46, 128, DateTimeKind.Utc).AddTicks(4870), "Some services completed, others rescheduled", "Partially Completed", 5, new DateTime(2026, 6, 27, 17, 30, 46, 128, DateTimeKind.Utc).AddTicks(4870) }
                });

            migrationBuilder.InsertData(
                table: "ServiceStatusLookups",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0001-000000000001"), new DateTime(2026, 6, 27, 17, 30, 46, 129, DateTimeKind.Utc).AddTicks(670), "Service scheduled but not started", "Pending", 0, new DateTime(2026, 6, 27, 17, 30, 46, 129, DateTimeKind.Utc).AddTicks(670) },
                    { new Guid("00000000-0000-0000-0001-000000000002"), new DateTime(2026, 6, 27, 17, 30, 46, 129, DateTimeKind.Utc).AddTicks(680), "Service is currently being performed", "In Progress", 1, new DateTime(2026, 6, 27, 17, 30, 46, 129, DateTimeKind.Utc).AddTicks(680) },
                    { new Guid("00000000-0000-0000-0001-000000000003"), new DateTime(2026, 6, 27, 17, 30, 46, 129, DateTimeKind.Utc).AddTicks(680), "Service has been completed successfully", "Completed", 2, new DateTime(2026, 6, 27, 17, 30, 46, 129, DateTimeKind.Utc).AddTicks(680) },
                    { new Guid("00000000-0000-0000-0001-000000000004"), new DateTime(2026, 6, 27, 17, 30, 46, 129, DateTimeKind.Utc).AddTicks(680), "Service was cancelled or declined", "Skipped", 3, new DateTime(2026, 6, 27, 17, 30, 46, 129, DateTimeKind.Utc).AddTicks(680) },
                    { new Guid("00000000-0000-0000-0001-000000000005"), new DateTime(2026, 6, 27, 17, 30, 46, 129, DateTimeKind.Utc).AddTicks(690), "Service moved to a different appointment", "Rescheduled", 4, new DateTime(2026, 6, 27, 17, 30, 46, 129, DateTimeKind.Utc).AddTicks(690) }
                });

            migrationBuilder.InsertData(
                table: "TimeSlots",
                columns: new[] { "Id", "CreatedAt", "IsActive", "SequenceOrder", "SlotEndTime", "SlotStartTime", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2026, 6, 27, 17, 30, 46, 132, DateTimeKind.Utc).AddTicks(5280), true, 1, new TimeOnly(8, 30, 0), new TimeOnly(8, 0, 0), new DateTime(2026, 6, 27, 17, 30, 46, 132, DateTimeKind.Utc).AddTicks(5280) },
                    { new Guid("00000000-0000-0000-0000-000000000002"), new DateTime(2026, 6, 27, 17, 30, 46, 132, DateTimeKind.Utc).AddTicks(5290), true, 2, new TimeOnly(9, 0, 0), new TimeOnly(8, 30, 0), new DateTime(2026, 6, 27, 17, 30, 46, 132, DateTimeKind.Utc).AddTicks(5290) },
                    { new Guid("00000000-0000-0000-0000-000000000003"), new DateTime(2026, 6, 27, 17, 30, 46, 132, DateTimeKind.Utc).AddTicks(5300), true, 3, new TimeOnly(9, 30, 0), new TimeOnly(9, 0, 0), new DateTime(2026, 6, 27, 17, 30, 46, 132, DateTimeKind.Utc).AddTicks(5300) },
                    { new Guid("00000000-0000-0000-0000-000000000004"), new DateTime(2026, 6, 27, 17, 30, 46, 132, DateTimeKind.Utc).AddTicks(5320), true, 4, new TimeOnly(10, 0, 0), new TimeOnly(9, 30, 0), new DateTime(2026, 6, 27, 17, 30, 46, 132, DateTimeKind.Utc).AddTicks(5320) },
                    { new Guid("00000000-0000-0000-0000-000000000005"), new DateTime(2026, 6, 27, 17, 30, 46, 132, DateTimeKind.Utc).AddTicks(5330), true, 5, new TimeOnly(10, 30, 0), new TimeOnly(10, 0, 0), new DateTime(2026, 6, 27, 17, 30, 46, 132, DateTimeKind.Utc).AddTicks(5330) },
                    { new Guid("00000000-0000-0000-0000-000000000006"), new DateTime(2026, 6, 27, 17, 30, 46, 132, DateTimeKind.Utc).AddTicks(5330), true, 6, new TimeOnly(11, 0, 0), new TimeOnly(10, 30, 0), new DateTime(2026, 6, 27, 17, 30, 46, 132, DateTimeKind.Utc).AddTicks(5330) },
                    { new Guid("00000000-0000-0000-0000-000000000007"), new DateTime(2026, 6, 27, 17, 30, 46, 132, DateTimeKind.Utc).AddTicks(5340), true, 7, new TimeOnly(11, 30, 0), new TimeOnly(11, 0, 0), new DateTime(2026, 6, 27, 17, 30, 46, 132, DateTimeKind.Utc).AddTicks(5340) },
                    { new Guid("00000000-0000-0000-0000-000000000008"), new DateTime(2026, 6, 27, 17, 30, 46, 132, DateTimeKind.Utc).AddTicks(5350), true, 8, new TimeOnly(12, 0, 0), new TimeOnly(11, 30, 0), new DateTime(2026, 6, 27, 17, 30, 46, 132, DateTimeKind.Utc).AddTicks(5350) },
                    { new Guid("00000000-0000-0000-0000-000000000009"), new DateTime(2026, 6, 27, 17, 30, 46, 132, DateTimeKind.Utc).AddTicks(5350), true, 9, new TimeOnly(12, 30, 0), new TimeOnly(12, 0, 0), new DateTime(2026, 6, 27, 17, 30, 46, 132, DateTimeKind.Utc).AddTicks(5350) },
                    { new Guid("00000000-0000-0000-0000-000000000010"), new DateTime(2026, 6, 27, 17, 30, 46, 132, DateTimeKind.Utc).AddTicks(5360), true, 10, new TimeOnly(13, 0, 0), new TimeOnly(12, 30, 0), new DateTime(2026, 6, 27, 17, 30, 46, 132, DateTimeKind.Utc).AddTicks(5360) },
                    { new Guid("00000000-0000-0000-0000-000000000011"), new DateTime(2026, 6, 27, 17, 30, 46, 132, DateTimeKind.Utc).AddTicks(5360), true, 11, new TimeOnly(13, 30, 0), new TimeOnly(13, 0, 0), new DateTime(2026, 6, 27, 17, 30, 46, 132, DateTimeKind.Utc).AddTicks(5360) },
                    { new Guid("00000000-0000-0000-0000-000000000012"), new DateTime(2026, 6, 27, 17, 30, 46, 132, DateTimeKind.Utc).AddTicks(5370), true, 12, new TimeOnly(14, 0, 0), new TimeOnly(13, 30, 0), new DateTime(2026, 6, 27, 17, 30, 46, 132, DateTimeKind.Utc).AddTicks(5370) },
                    { new Guid("00000000-0000-0000-0000-000000000013"), new DateTime(2026, 6, 27, 17, 30, 46, 132, DateTimeKind.Utc).AddTicks(5380), true, 13, new TimeOnly(14, 30, 0), new TimeOnly(14, 0, 0), new DateTime(2026, 6, 27, 17, 30, 46, 132, DateTimeKind.Utc).AddTicks(5380) },
                    { new Guid("00000000-0000-0000-0000-000000000014"), new DateTime(2026, 6, 27, 17, 30, 46, 132, DateTimeKind.Utc).AddTicks(5380), true, 14, new TimeOnly(15, 0, 0), new TimeOnly(14, 30, 0), new DateTime(2026, 6, 27, 17, 30, 46, 132, DateTimeKind.Utc).AddTicks(5380) },
                    { new Guid("00000000-0000-0000-0000-000000000015"), new DateTime(2026, 6, 27, 17, 30, 46, 132, DateTimeKind.Utc).AddTicks(5390), true, 15, new TimeOnly(15, 30, 0), new TimeOnly(15, 0, 0), new DateTime(2026, 6, 27, 17, 30, 46, 132, DateTimeKind.Utc).AddTicks(5390) },
                    { new Guid("00000000-0000-0000-0000-000000000016"), new DateTime(2026, 6, 27, 17, 30, 46, 132, DateTimeKind.Utc).AddTicks(5400), true, 16, new TimeOnly(16, 0, 0), new TimeOnly(15, 30, 0), new DateTime(2026, 6, 27, 17, 30, 46, 132, DateTimeKind.Utc).AddTicks(5400) },
                    { new Guid("00000000-0000-0000-0000-000000000017"), new DateTime(2026, 6, 27, 17, 30, 46, 132, DateTimeKind.Utc).AddTicks(5400), true, 17, new TimeOnly(16, 30, 0), new TimeOnly(16, 0, 0), new DateTime(2026, 6, 27, 17, 30, 46, 132, DateTimeKind.Utc).AddTicks(5400) },
                    { new Guid("00000000-0000-0000-0000-000000000018"), new DateTime(2026, 6, 27, 17, 30, 46, 132, DateTimeKind.Utc).AddTicks(5410), true, 18, new TimeOnly(17, 0, 0), new TimeOnly(16, 30, 0), new DateTime(2026, 6, 27, 17, 30, 46, 132, DateTimeKind.Utc).AddTicks(5410) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_CustomerId",
                table: "Appointments",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DealershipId",
                table: "Appointments",
                column: "DealershipId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_StatusId",
                table: "Appointments",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_VehicleId",
                table: "Appointments",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentStatusLookup_Status_Unique",
                table: "AppointmentStatusLookups",
                column: "Status",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessHours_DealershipId",
                table: "BusinessHours",
                column: "DealershipId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceBays_DealershipId",
                table: "ServiceBays",
                column: "DealershipId");

            migrationBuilder.CreateIndex(
                name: "IX_Service_Unique_AppointmentServiceTypeSequence",
                table: "Services",
                columns: new[] { "AppointmentId", "ServiceTypeId", "SequenceOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Services_DealershipId",
                table: "Services",
                column: "DealershipId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_EstimatedEndTimeSlotId",
                table: "Services",
                column: "EstimatedEndTimeSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_EstimatedStartTimeSlotId",
                table: "Services",
                column: "EstimatedStartTimeSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_ServiceBayId",
                table: "Services",
                column: "ServiceBayId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_ServiceStatusId",
                table: "Services",
                column: "ServiceStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_ServiceTypeId",
                table: "Services",
                column: "ServiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_TechnicianId",
                table: "Services",
                column: "TechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceStatusLookup_Status_Unique",
                table: "ServiceStatusLookups",
                column: "Status",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Technicians_DealershipId",
                table: "Technicians",
                column: "DealershipId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianSchedules_TechnicianId",
                table: "TechnicianSchedules",
                column: "TechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianSkill_Unique_TechnicianServiceType",
                table: "TechnicianSkills",
                columns: new[] { "TechnicianId", "ServiceTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianSkills_ServiceTypeId",
                table: "TechnicianSkills",
                column: "ServiceTypeId");

            migrationBuilder.CreateIndex(
                name: "UK_TimeSlot_SequenceOrder",
                table: "TimeSlots",
                column: "SequenceOrder",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_CustomerId",
                table: "Vehicles",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessHours");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "TechnicianSchedules");

            migrationBuilder.DropTable(
                name: "TechnicianSkills");

            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "ServiceBays");

            migrationBuilder.DropTable(
                name: "ServiceStatusLookups");

            migrationBuilder.DropTable(
                name: "TimeSlots");

            migrationBuilder.DropTable(
                name: "ServiceTypes");

            migrationBuilder.DropTable(
                name: "Technicians");

            migrationBuilder.DropTable(
                name: "AppointmentStatusLookups");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "Dealerships");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
