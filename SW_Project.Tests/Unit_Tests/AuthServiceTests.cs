using Xunit;
using Moq;
using SW_Project.Services.Services;
using SW_Project.Repositories.IRepository;
using SW_Project.Models;
using SW_Project.DTOs.Auth;
using SW_Project.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace SW_Project.Tests
{
    public class AuthServiceTests
    {
        private ApplicationDbContext CreateInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            return new ApplicationDbContext(options);
        }

        private AuthService BuildService(
            Mock<IUserRepository> userRepo,
            Mock<IPatientRepository> patientRepo,
            Mock<IDoctorRepository> doctorRepo,
            Mock<ISecretaryRepository> secretaryRepo,
            ApplicationDbContext db)
        {
            return new AuthService(
                userRepo.Object,
                patientRepo.Object,
                doctorRepo.Object,
                secretaryRepo.Object,
                db);
        }

        [Fact]
        public void Login_ValidCredentials_ReturnsToken()
        {
            var userRepo = new Mock<IUserRepository>();
            var plainPassword = "password123";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);

            userRepo.Setup(r => r.GetByEmail("test@test.com"))
                    .Returns(new User
                    {
                        Id = 1,
                        Name = "Test User",
                        Email = "test@test.com",
                        Password = hashedPassword,
                        Role = Role.Patient
                    });

            var service = BuildService(
                userRepo,
                new Mock<IPatientRepository>(),
                new Mock<IDoctorRepository>(),
                new Mock<ISecretaryRepository>(),
                CreateInMemoryDb());

            var dto = new LoginDto { Email = "test@test.com", Password = plainPassword };

            var token = service.Login(dto);

            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }

        [Fact]
        public void Login_UserNotFound_ThrowsException()
        {
            var userRepo = new Mock<IUserRepository>();
            userRepo.Setup(r => r.GetByEmail(It.IsAny<string>())).Returns((User)null!);

            var service = BuildService(
                userRepo,
                new Mock<IPatientRepository>(),
                new Mock<IDoctorRepository>(),
                new Mock<ISecretaryRepository>(),
                CreateInMemoryDb());

            var dto = new LoginDto { Email = "nobody@test.com", Password = "anything" };

            var ex = Assert.Throws<Exception>(() => service.Login(dto));
            Assert.Equal("User not Found", ex.Message);
        }

        [Fact]
        public void Login_WrongPassword_ThrowsException()
        {
            var userRepo = new Mock<IUserRepository>();
            userRepo.Setup(r => r.GetByEmail("test@test.com"))
                    .Returns(new User
                    {
                        Id = 1,
                        Name = "Test",
                        Email = "test@test.com",
                        Password = BCrypt.Net.BCrypt.HashPassword("correctpassword"),
                        Role = Role.Patient
                    });

            var service = BuildService(
                userRepo,
                new Mock<IPatientRepository>(),
                new Mock<IDoctorRepository>(),
                new Mock<ISecretaryRepository>(),
                CreateInMemoryDb());

            var dto = new LoginDto { Email = "test@test.com", Password = "wrongpassword" };

            var ex = Assert.Throws<Exception>(() => service.Login(dto));
            Assert.Equal("Invalid Passwoid", ex.Message);
        }

        [Fact]
        public void Register_DuplicateEmail_ThrowsException()
        {
            var userRepo = new Mock<IUserRepository>();
            userRepo.Setup(r => r.GetByEmail("duplicate@test.com"))
                    .Returns(new User { Id = 99, Email = "duplicate@test.com", Name="X", Password="X", Role=Role.Patient });

            var service = BuildService(
                userRepo,
                new Mock<IPatientRepository>(),
                new Mock<IDoctorRepository>(),
                new Mock<ISecretaryRepository>(),
                CreateInMemoryDb());

            var dto = new RegisterDto
            {
                Email = "duplicate@test.com",
                Name = "Someone",
                Password = "pass123",
                Role = Role.Patient,
                Gender = Gender.Male
            };

            var ex = Assert.Throws<Exception>(() => service.Register(dto));
            Assert.Equal("This email is already registered.", ex.Message);
        }

        [Fact]
        public void Register_FutureDateOfBirth_ThrowsException()
        {
            var userRepo = new Mock<IUserRepository>();
            userRepo.Setup(r => r.GetByEmail(It.IsAny<string>())).Returns((User)null!);
            userRepo.Setup(r => r.Add(It.IsAny<User>()));
            userRepo.Setup(r => r.Save());

            var service = BuildService(
                userRepo,
                new Mock<IPatientRepository>(),
                new Mock<IDoctorRepository>(),
                new Mock<ISecretaryRepository>(),
                CreateInMemoryDb());

            var dto = new RegisterDto
            {
                Name = "Future Baby",
                Email = "baby@test.com",
                Password = "pass123",
                Role = Role.Patient,
                Gender = Gender.Female,
                DateOfBirth = DateTime.Now.AddYears(1)
            };

            var ex = Assert.Throws<Exception>(() => service.Register(dto));
            Assert.Contains("Date of birth cannot be in the future", ex.Message);
        }

        [Fact]
        public void ChangePassword_CorrectOldPassword_ReturnsSuccessMessage()
        {
            var userRepo = new Mock<IUserRepository>();
            var oldPassword = "oldpass123";

            userRepo.Setup(r => r.GetById(1))
                    .Returns(new User
                    {
                        Id = 1,
                        Name = "User",
                        Email = "u@test.com",
                        Password = BCrypt.Net.BCrypt.HashPassword(oldPassword),
                        Role = Role.Patient
                    });
            userRepo.Setup(r => r.Update(It.IsAny<User>()));
            userRepo.Setup(r => r.Save());

            var service = BuildService(
                userRepo,
                new Mock<IPatientRepository>(),
                new Mock<IDoctorRepository>(),
                new Mock<ISecretaryRepository>(),
                CreateInMemoryDb());

            var result = service.ChangePassword(1, oldPassword, "newpass456");

            Assert.Equal("Password updated successfully!", result);
        }

        [Fact]
        public void ChangePassword_WrongOldPassword_ThrowsException()
        {
            var userRepo = new Mock<IUserRepository>();
            userRepo.Setup(r => r.GetById(1))
                    .Returns(new User
                    {
                        Id = 1,
                        Name = "User",
                        Email = "u@test.com",
                        Password = BCrypt.Net.BCrypt.HashPassword("correctpassword"),
                        Role = Role.Patient
                    });

            var service = BuildService(
                userRepo,
                new Mock<IPatientRepository>(),
                new Mock<IDoctorRepository>(),
                new Mock<ISecretaryRepository>(),
                CreateInMemoryDb());

            var ex = Assert.Throws<Exception>(() =>
                service.ChangePassword(1, "wrongpassword", "newpass456"));
            Assert.Equal("Old password is incorrect.", ex.Message);
        }
    }
}