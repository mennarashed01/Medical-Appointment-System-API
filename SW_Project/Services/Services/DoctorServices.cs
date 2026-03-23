using SW_Project.DTOs;
using SW_Project.Repositories.IRepository;
using SW_Project.Services.IServices;
using SW_Project.Models;
using BCrypt.Net;

namespace SW_Project.Services.Services
{
    public class DoctorServices : IDoctorService
    {
        private readonly IDoctorRepository _repo;

        public DoctorServices(IDoctorRepository repo)
        {
            _repo = repo;
        }
        public void Create(CreateDoctorDto dto)
        {
            var allDoctors = _repo.GetAll();
            if (allDoctors.Any(d => d.User.Email == dto.Email))
            {
                throw new Exception("This email is already registered.");
            }

            try
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);
                var user = new User
                {
                    Name = dto.Name,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    Password = hashedPassword,
                    Role = Role.Doctor
                };
                var doctor = new Doctor
                {
                    User = user,
                    AppointmentPrice = dto.AppointmentPrice,
                    ClinicLocation = dto.ClinicLocation,
                    SpecializationId = dto.SpecializationId,
                    ContactInfo = dto.ContactInfo,

                };
                _repo.Add(doctor);
                _repo.Save();
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new Exception($"Database Error: {innerMessage}");
            }
        }

        public void Delete(int id)
        {
            var doctor = _repo.GetById(id);
            var user = doctor.User;
            if (doctor != null)
            {
                
                _repo.Delete(doctor);
                _repo.Save();
            }
        }

        public DoctorResponseDto Get(int id)
        {
           var doctor =  _repo.GetById(id);
            if (doctor == null) return null;
            return new DoctorResponseDto
            {
                Id= doctor.Id,
                Name = doctor.User.Name,
                Email = doctor.User.Email,
                Phone = doctor.User.Phone,
                SpecializationName = doctor.Specialization.Name,
                ClinicLocation = doctor.ClinicLocation,
                AppointmentPrice= doctor.AppointmentPrice,
                ContactInfo= doctor.ContactInfo
            };
        }

        public List<DoctorResponseDto> GetAll()
        {
            var doctors = _repo.GetAll();
            if (doctors == null) return null;
            return doctors.Select(d => new DoctorResponseDto
            {
                Id = d.Id,
                Name = d.User.Name,
                Email = d.User.Email,
                Phone = d.User.Phone,
                SpecializationName= d.Specialization.Name,
                ClinicLocation = d.ClinicLocation,
                AppointmentPrice= d.AppointmentPrice,
                ContactInfo= d.ContactInfo
            }).ToList();
        }

        public void Update(int id, UpdateDoctorDto dto)
        {
            var doctor = _repo.GetById(id);
            if (doctor == null) return;

            doctor.User.Name = dto.Name;
            doctor.User.Email = dto.Email;
            doctor.User.Phone = dto.Phone;

            if(!string.IsNullOrEmpty(dto.Password)) 
                doctor.User.Password = dto.Password;

            doctor.SpecializationId = dto.SpecializationId;
            doctor.ClinicLocation = dto.ClinicLocation;
            doctor.ContactInfo = dto.ContactInfo;
            doctor.AppointmentPrice = dto.AppointmentPrice;

            _repo.Update(doctor);
            _repo.Save();
        }
    }
}
