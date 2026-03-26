using Microsoft.EntityFrameworkCore;
using SW_Project.Models;

namespace SW_Project.Data
{
    public class ApplicationDbContext :DbContext 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Diagnosis> Diagnoses { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<Symptom> Symptoms { get; set; }
        public DbSet<DoctorSymptom> DoctorSymptoms { get; set; }
        public DbSet<Secretary> Secretarys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite Key  for DoctorSymptom
            modelBuilder.Entity<DoctorSymptom>()
                .HasKey(ds => new { ds.DoctorId, ds.SymptomId });

            //  Doctor - Specialization
            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.Specialization)
                .WithMany()
                .HasForeignKey(d => d.SpecializationId)
                .OnDelete(DeleteBehavior.Restrict);

            //  Doctor - User
            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //  Patient - User
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //  Appointment
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany()
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany()
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            //  Diagnosis
            modelBuilder.Entity<Diagnosis>()
                .HasOne(d => d.Doctor)
                .WithMany()
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Diagnosis>()
                .HasOne(d => d.Patient)
                .WithMany()
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

        }

    }
}
