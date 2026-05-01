using Xunit;
using Moq;
using SW_Project.Services.Services;
using SW_Project.Repositories.IRepository;
using SW_Project.Models;
using SW_Project.DTOs.Appointment;
using SW_Project.DTOs.Patient;
using SW_Project.Data;
using Microsoft.EntityFrameworkCore;

namespace SW_Project.Tests
{
    public class AppointmentServiceTests
    {
        private ApplicationDbContext CreateInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public void BookAppointment_ValidData_CreatesAppointmentWithPendingStatus()
        {
            var appointmentRepo = new Mock<IAppointmentRepository>();
            var doctorRepo      = new Mock<IDoctorRepository>();
            var patientRepo     = new Mock<IPatientRepository>();
            var secretaryRepo   = new Mock<ISecretaryRepository>();

            patientRepo.Setup(r => r.GetByUserId(1))
                       .Returns(new Patient { Id = 10, UserId = 1, Gender = Gender.Male });

            doctorRepo.Setup(r => r.GetById(5))
                      .Returns(new Doctor
                      {
                          Id = 5,
                          UserId = 99,
                          SpecializationId = 1,
                          AppointmentPrice = 200
                      });

            Appointment? savedAppointment = null;
            appointmentRepo.Setup(r => r.Add(It.IsAny<Appointment>()))
                           .Callback<Appointment>(a => savedAppointment = a);
            appointmentRepo.Setup(r => r.Save());

            var service = new AppointmentService(
                appointmentRepo.Object,
                doctorRepo.Object,
                secretaryRepo.Object,
                patientRepo.Object,
                CreateInMemoryDb());

            var dto = new BookAppointmentDto
            {
                DoctorId = 5,
                Date = DateTime.Now.AddDays(2)
            };

            service.BookAppointment(userId: 1, dto: dto);

            Assert.NotNull(savedAppointment);
            Assert.Equal(Status.Pending, savedAppointment!.Status);
            Assert.Equal(10, savedAppointment.PatientId);
            Assert.Equal(5, savedAppointment.DoctorId);
            Assert.Equal(200, savedAppointment.Price);
        }

        [Fact]
        public void BookAppointment_PatientNotFound_ThrowsException()
        {
            var appointmentRepo = new Mock<IAppointmentRepository>();
            var doctorRepo      = new Mock<IDoctorRepository>();
            var patientRepo     = new Mock<IPatientRepository>();
            var secretaryRepo   = new Mock<ISecretaryRepository>();

            patientRepo.Setup(r => r.GetByUserId(999)).Returns((Patient)null);

            var service = new AppointmentService(
                appointmentRepo.Object,
                doctorRepo.Object,
                secretaryRepo.Object,
                patientRepo.Object,
                CreateInMemoryDb());

            var dto = new BookAppointmentDto { DoctorId = 1, Date = DateTime.Now.AddDays(1) };

            var ex = Assert.Throws<Exception>(() => service.BookAppointment(userId: 999, dto: dto));
            Assert.Equal("Patient profile not found.", ex.Message);
        }

        [Fact]
        public void BookAppointment_DoctorNotFound_ThrowsException()
        {
            var appointmentRepo = new Mock<IAppointmentRepository>();
            var doctorRepo      = new Mock<IDoctorRepository>();
            var patientRepo     = new Mock<IPatientRepository>();
            var secretaryRepo   = new Mock<ISecretaryRepository>();

            patientRepo.Setup(r => r.GetByUserId(1))
                       .Returns(new Patient { Id = 10, UserId = 1, Gender = Gender.Male });

            doctorRepo.Setup(r => r.GetById(999)).Returns((Doctor)null);

            var service = new AppointmentService(
                appointmentRepo.Object,
                doctorRepo.Object,
                secretaryRepo.Object,
                patientRepo.Object,
                CreateInMemoryDb());

            var dto = new BookAppointmentDto { DoctorId = 999, Date = DateTime.Now.AddDays(1) };

            var ex = Assert.Throws<Exception>(() => service.BookAppointment(userId: 1, dto: dto));
            Assert.Equal("Doctor not found.", ex.Message);
        }

        [Fact]
        public void UpdateAppointmentStatus_ValidId_UpdatesStatus()
        {
            var appointmentRepo = new Mock<IAppointmentRepository>();

            var appointment = new Appointment
            {
                Id = 1,
                PatientId = 10,
                DoctorId = 5,
                Date = DateTime.Now.AddDays(1),
                Status = Status.Pending,
                Price = 200
            };

            appointmentRepo.Setup(r => r.GetById(1)).Returns(appointment);
            appointmentRepo.Setup(r => r.Save());

            var service = new AppointmentService(
                appointmentRepo.Object,
                new Mock<IDoctorRepository>().Object,
                new Mock<ISecretaryRepository>().Object,
                new Mock<IPatientRepository>().Object,
                CreateInMemoryDb());

            service.UpdateAppointmentStatus(1, Status.Confirmed);

            Assert.Equal(Status.Confirmed, appointment.Status);
            appointmentRepo.Verify(r => r.Save(), Times.Once);
        }

        [Fact]
        public void UpdateAppointmentStatus_InvalidId_ThrowsException()
        {
            var appointmentRepo = new Mock<IAppointmentRepository>();
            appointmentRepo.Setup(r => r.GetById(999)).Returns((Appointment)null);

            var service = new AppointmentService(
                appointmentRepo.Object,
                new Mock<IDoctorRepository>().Object,
                new Mock<ISecretaryRepository>().Object,
                new Mock<IPatientRepository>().Object,
                CreateInMemoryDb());

            var ex = Assert.Throws<Exception>(() =>
                service.UpdateAppointmentStatus(999, Status.Confirmed));
            Assert.Equal("Appointment not found.", ex.Message);
        }
    }

    public class DoctorRatingServiceTests
    {
        private ApplicationDbContext CreateDbWithData()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var db = new ApplicationDbContext(options);

            var user = new User { Id = 1, Name = "Patient One", Email = "p@test.com", Password = "x", Role = Role.Patient };
            db.Users.Add(user);

            var patient = new Patient { Id = 1, UserId = 1, Gender = Gender.Male };
            db.Patients.Add(patient);

            var doctorUser = new User { Id = 2, Name = "Dr. Smith", Email = "doc@test.com", Password = "x", Role = Role.Doctor };
            db.Users.Add(doctorUser);

            var doctor = new Doctor { Id = 1, UserId = 2, SpecializationId = 1, AppointmentPrice = 300, Rating = 0 };
            db.Doctors.Add(doctor);

            db.SaveChanges();
            return db;
        }

        [Fact]
        public async Task RateDoctorAsync_ValidRating_UpdatesDoctorAverageRating()
        {
            var db = CreateDbWithData();
            var service = new DoctorRatingService(db);

            var dto = new RateDoctorDto { DoctorId = 1, Score = 4, Comment = "Great doctor" };

            await service.RateDoctorAsync(userId: 1, dto: dto);

            var doctor = db.Doctors.Find(1);
            Assert.Equal(4.0m, doctor!.Rating);
        }

        [Fact]
        public async Task RateDoctorAsync_MultipleRatings_CalculatesCorrectAverage()
        {
            var db = CreateDbWithData();
            var service = new DoctorRatingService(db);

            await service.RateDoctorAsync(userId: 1, dto: new RateDoctorDto { DoctorId = 1, Score = 4 });

            db.Users.Add(new User { Id = 3, Name = "Patient Two", Email = "p2@test.com", Password = "x", Role = Role.Patient });
            db.Patients.Add(new Patient { Id = 2, UserId = 3, Gender = Gender.Female });
            db.SaveChanges();

            await service.RateDoctorAsync(userId: 3, dto: new RateDoctorDto { DoctorId = 1, Score = 2 });

            var doctor = db.Doctors.Find(1);
            Assert.Equal(3.0m, doctor!.Rating);
        }

        [Fact]
        public async Task RateDoctorAsync_PatientNotFound_ThrowsException()
        {
            var db = CreateDbWithData();
            var service = new DoctorRatingService(db);

            var dto = new RateDoctorDto { DoctorId = 1, Score = 5 };

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                service.RateDoctorAsync(userId: 999, dto: dto));
            Assert.Equal("Patient not found", ex.Message);
        }
    }
}