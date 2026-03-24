using Microsoft.IdentityModel.Tokens;
using SW_Project.DTOs.Auth;
using SW_Project.Models;
using SW_Project.Repositories.IRepository;
using SW_Project.Services.IServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SW_Project.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IPatientRepository _patientRepo;
        private readonly IDoctorRepository _doctorRepo;

        public AuthService(IUserRepository userRepo, IPatientRepository patientRepo, IDoctorRepository doctorRepo)
        {
            _userRepo = userRepo;
            _patientRepo = patientRepo;
            _doctorRepo = doctorRepo;
        }

        public string ChangePassword(int userId, string OldPassword, string newPassword)
        {
            var user = _userRepo.GetById(userId);
            if (user == null) throw new Exception("User not found.");

            if (!BCrypt.Net.BCrypt.Verify(OldPassword, user.Password))
                throw new Exception("Old password is incorrect.");

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);

            _userRepo.Update(user); 
            _userRepo.Save();

            return "Password updated successfully!";
        }

        public string Login(LoginDto dto)
        {
            var user = _userRepo.GetByEmail(dto.Email);
            if (user == null) throw new Exception("User not Found");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                throw new Exception("Invalid Passwoid");

            return CreateTokent(user);
        }

        public string Register(RegisterDto dto)
        {
            if (_userRepo.GetByEmail(dto.Email) != null)
                throw new Exception("This email is already registered.");


            try
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

                var user = new User
                {
                    Name = dto.Name,
                    Email = dto.Email,
                    Password = hashedPassword,
                    Phone = dto.Phone,
                    Role = dto.Role
                };
                _userRepo.Add(user);
                _userRepo.Save();

                //Roles
                if (dto.Role == Role.Doctor)
                {
                    var doctor = new Doctor
                    {
                        UserId = user.Id,
                        AppointmentPrice = dto.AppointmentPrice,
                        ClinicLocation = dto.ClinicLocation,
                        SpecializationId = dto.SpecializationId,
                        ContactInfo = dto.ContactInfo,
                        DoctorSymptoms = new List<DoctorSymptom>()
                    };

                    if (dto.SymptomIds != null && dto.SymptomIds.Any())
                    {
                        foreach (var sId in dto.SymptomIds)
                        {
                            doctor.DoctorSymptoms.Add(new DoctorSymptom { SymptomId = sId });
                        }
                    }
                    _doctorRepo.Add(doctor);

                }
                else if (dto.Role == Role.Patient)
                {
                    if (dto.DateOfBirth.HasValue && dto.DateOfBirth.Value > DateTime.Now)
                        throw new Exception("Date of birth cannot be in the future.");
                    
                    if (dto.DateOfBirth.HasValue && dto.DateOfBirth.Value < DateTime.Now.AddYears(-100))
                    {
                        throw new Exception("Please enter a valid date of birth.");
                    }

                    if (!Enum.IsDefined(typeof(Gender), dto.Gender))
                    {
                        throw new Exception("Selected Gender is not valid.");
                    }
                    var patient = new Patient
                    {
                        User = user,
                        Gender = dto.Gender,
                        DateOfBirth = dto.DateOfBirth
                    };
                    _patientRepo.Add(patient);
                }
                _userRepo.Save();

                return "Registration Successful!";
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new Exception($"Database Error: {innerMessage}");
            }
        }

        private string CreateTokent(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("Id", user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MySecretSuperLongKey1234567890123456MySecretSuperLongKey1234567890123456"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds,
                Issuer = "SW_Project",
                Audience = "SW_Project_Users"
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

        }
    }
}
