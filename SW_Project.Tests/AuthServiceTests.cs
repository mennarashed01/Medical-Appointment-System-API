using Xunit;
using Moq;
using SW_Project.Services.Services;
using SW_Project.Repositories.IRepository;
using SW_Project.Models;
using SW_Project.DTOs.Auth;
using SW_Project.Data;
using Microsoft.EntityFrameworkCore;

namespace SW_Project.Tests
{
    public class AuthServiceTests
    {
        // ─── Helpers ────────────────────────────────────────────────────────────

        // Creates a real in-memory database so AuthService can run transactions
        private ApplicationDbContext CreateInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        // Builds AuthService with mocked repositories + real in-memory DB
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

        // ════════════════════════════════════════════════════════════════════════
        // FUNCTION 1 — Login()
        // ════════════════════════════════════════════════════════════════════════

        [Fact]
        public void Login_ValidCredentials_ReturnsToken()
        {
            // ARRANGE
            var userRepo = new Mock<IUserRepository>();
            var plainPassword = "password123";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);

            // Simulate a user that already exists in the DB
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

            // ACT
            var token = service.Login(dto);

            // ASSERT — token should be a non-empty JWT string
            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }

        [Fact]
        public void Login_UserNotFound_ThrowsException()
        {
            // ARRANGE
            var userRepo = new Mock<IUserRepository>();
            // Simulate no user found for this email
            userRepo.Setup(r => r.GetByEmail(It.IsAny<string>())).Returns((User)null);

            var service = BuildService(
                userRepo,
                new Mock<IPatientRepository>(),
                new Mock<IDoctorRepository>(),
                new Mock<ISecretaryRepository>(),
                CreateInMemoryDb());

            var dto = new LoginDto { Email = "nobody@test.com", Password = "anything" };

            // ACT + ASSERT — must throw with this message
            var ex = Assert.Throws<Exception>(() => service.Login(dto));
            Assert.Equal("User not Found", ex.Message);
        }

        [Fact]
        public void Login_WrongPassword_ThrowsException()
        {
            // ARRANGE
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

            // ACT + ASSERT
            var ex = Assert.Throws<Exception>(() => service.Login(dto));
            Assert.Equal("Invalid Passwoid", ex.Message); // matches the typo in source code
        }

        // ════════════════════════════════════════════════════════════════════════
        // FUNCTION 2 — Register()
        // ════════════════════════════════════════════════════════════════════════

        [Fact]
        public void Register_DuplicateEmail_ThrowsException()
        {
            // ARRANGE
            var userRepo = new Mock<IUserRepository>();
            // Simulate that this email already exists
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

            // ACT + ASSERT
            var ex = Assert.Throws<Exception>(() => service.Register(dto));
            Assert.Equal("This email is already registered.", ex.Message);
        }

        [Fact]
        public void Register_FutureDateOfBirth_ThrowsException()
        {
            // ARRANGE
            var userRepo = new Mock<IUserRepository>();
            userRepo.Setup(r => r.GetByEmail(It.IsAny<string>())).Returns((User)null);
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
                DateOfBirth = DateTime.Now.AddYears(1) // future date — invalid
            };

            // ACT + ASSERT
            var ex = Assert.Throws<Exception>(() => service.Register(dto));
            Assert.Contains("Date of birth cannot be in the future", ex.Message);
        }

        // ════════════════════════════════════════════════════════════════════════
        // FUNCTION 3 — ChangePassword()
        // ════════════════════════════════════════════════════════════════════════

        [Fact]
        public void ChangePassword_CorrectOldPassword_ReturnsSuccessMessage()
        {
            // ARRANGE
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

            // ACT
            var result = service.ChangePassword(1, oldPassword, "newpass456");

            // ASSERT
            Assert.Equal("Password updated successfully!", result);
        }

        [Fact]
        public void ChangePassword_WrongOldPassword_ThrowsException()
        {
            // ARRANGE
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

            // ACT + ASSERT
            var ex = Assert.Throws<Exception>(() =>
                service.ChangePassword(1, "wrongpassword", "newpass456"));
            Assert.Equal("Old password is incorrect.", ex.Message);
        }
    }
}
