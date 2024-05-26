using MedicalBackend.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MedicalBackend.Database;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Price> Prices { get; set; } 
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<TestimonialPerson> TestimonialPersons { get; set; }
    public DbSet<WorkHoursDay> WorkHoursDays { get; set; }
    public DbSet<FreeDay> FreeDays { get; set; }
    public DbSet<RoomOrDevice> RoomsOrDevices { get; set; }
    public DbSet<MedicalService> MedicalServices { get; set; }
    public DbSet<Disease> Diseases { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<SendSmsQueue> SendSmsQueue { get; set; }
    public DbSet<Settings> Settings { get; set; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
}