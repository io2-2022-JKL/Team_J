using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using VaccinationSystem.Models;

namespace VaccinationSystem.Config
{
    public class VaccinationSystemDbContext : DbContext
    { 

        public DbSet<Admin> Admin { get; set; }
        public DbSet<Appointment> Appointment { get; set; }
        public DbSet<Certificate> Certificate { get; set; }
        public DbSet<Doctor> Doctor { get; set; }
        public DbSet<OpeningHours> OpeningHours { get; set; }
        public DbSet<Patient> Patient { get; set; }
        public DbSet<TimeSlot> TimeSlot { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<VaccinationCenter> VaccinationCenter { get; set; }
        public DbSet<Vaccine> Vaccine { get; set; }

        public VaccinationSystemDbContext()
        {

        }
        public VaccinationSystemDbContext(DbContextOptions<VaccinationSystemDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var connectionString = configuration.GetConnectionString("AppDb");
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(connectionString);
        }

    }
}
