using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VehicleServiceBooking.Application.Configuration;
using VehicleServiceBooking.Application.Interfaces.Persistence;
using VehicleServiceBooking.Application.Services;
using VehicleServiceBooking.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

//
// Controllers
//
builder.Services.AddControllers();

//
// Swagger
//
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//
// Scheduling Options (from appsettings.json)
//
builder.Services.Configure<SchedulingOptions>(
    builder.Configuration.GetSection(SchedulingOptions.SectionName));

builder.Services.AddSingleton<ISchedulingConfiguration>(sp =>
    sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<SchedulingOptions>>().Value);

//
// DbContext (InMemory for now)
//
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseInMemoryDatabase("VehicleServiceBookingDb");
});

//
// Application DbContext abstraction
//
builder.Services.AddScoped<IApplicationDbContext>(sp =>
    sp.GetRequiredService<ApplicationDbContext>());

//
// Application Services
//
builder.Services.AddScoped<IAvailabilityService, AvailabilityService>();

var app = builder.Build();

//
// HTTP pipeline
//
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();