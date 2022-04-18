using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using VaccinationSystem.Models;

namespace VaccinationSystem.Config
{
    public class VaccinationSystemDbContext : DbContext
    { 

        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<Appointment> Appointments { get; set; }
        public virtual DbSet<Certificate> Certificates { get; set; }
        public virtual DbSet<Doctor> Doctors { get; set; }
        public virtual DbSet<OpeningHours> OpeningHours { get; set; }
        public virtual DbSet<Patient> Patients { get; set; }
        public virtual DbSet<TimeSlot> TimeSlots { get; set; }
        public virtual DbSet<VaccinationCenter> VaccinationCenters { get; set; }
        public virtual DbSet<Vaccine> Vaccines { get; set; }
        public virtual DbSet<VaccinesInVaccinationCenter> VaccinesInVaccinationCenter { get; set; }

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>().ToTable("Admin");
            modelBuilder.Entity<Appointment>().ToTable("Appointment");
            modelBuilder.Entity<Certificate>().ToTable("Certificate");
            modelBuilder.Entity<Doctor>().ToTable("Doctor");
            modelBuilder.Entity<OpeningHours>().ToTable("OpeningHours");
            modelBuilder.Entity<Patient>().ToTable("Patient");
            modelBuilder.Entity<TimeSlot>().ToTable("TimeSlot");
            modelBuilder.Entity<VaccinationCenter>().ToTable("VaccinationCenter");
            modelBuilder.Entity<Vaccine>().ToTable("Vaccine");
            modelBuilder.Entity<VaccinesInVaccinationCenter>().ToTable("VaccineeInVaccinationCenter");
        }

    }
}
