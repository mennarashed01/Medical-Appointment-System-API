using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using SW_Project.DTOs;
using SW_Project.Models;
using SW_Project.Repositories.IRepository;
using SW_Project.Services.IServices;

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
                    DoctorSymptoms = new List<DoctorSymptom>()
                };

                //add symptoms
                if(dto.SymptomIds != null && dto.SymptomIds.Any())
                {
                    foreach(var sId in dto.SymptomIds)
                    {
                        doctor.DoctorSymptoms.Add(new DoctorSymptom
                        {
                            SymptomId = sId
                        });
                    }
                }

                _repo.Add(doctor);
                
                _repo.Save();
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new Exception($"Database Error: {innerMessage}");
            }
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

            if (doctor.DoctorSymptoms != null)
            {
                doctor.DoctorSymptoms.Clear();
            }

           
            if (dto.SymptomIds != null && dto.SymptomIds.Any())
            {
                foreach (var sId in dto.SymptomIds)
                {
                    doctor.DoctorSymptoms.Add(new DoctorSymptom
                    {
                        DoctorId = id,
                        SymptomId = sId
                    });
                }
            }

            _repo.Update(doctor);
            _repo.Save();
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
                ContactInfo= d.ContactInfo,
                Symptoms = d.DoctorSymptoms.Select(ds => ds.Symptom.Name).ToList()
            }).ToList();
        }

        public DoctorResponseDto GetById(int id)
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
                ContactInfo= doctor.ContactInfo,
                Symptoms = doctor.DoctorSymptoms.Select(ds => ds.Symptom.Name).ToList()
            };
        }
        public List<DoctorResponseDto> GetByName(string name)
        {
            var doctorLIst = _repo.GetAll()
                 .Where(d => d.User.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                 .ToList();

            if (doctorLIst == null || !doctorLIst.Any()) return new List<DoctorResponseDto>();
            return doctorLIst.Select(doctor => new DoctorResponseDto
            {
                Id = doctor.Id,
                Name = doctor.User.Name,
                Email = doctor.User.Email,
                Phone = doctor.User.Phone,
                SpecializationName = doctor.Specialization.Name,
                ClinicLocation = doctor.ClinicLocation,
                AppointmentPrice = doctor.AppointmentPrice,
                ContactInfo = doctor.ContactInfo,
                Symptoms = doctor.DoctorSymptoms.Select(ds => ds.Symptom.Name).ToList()
            }).ToList();
        }

        public List<DoctorResponseDto> GetBySymptoms(string symptomName)
        {
            var doctors = _repo.GetAll()
                .Where( d=> d.DoctorSymptoms
                .Any(ds => ds.Symptom.Name.Contains(symptomName, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            return doctors.Select(d => new DoctorResponseDto
            {
                Id = d.Id,
                Name = d.User.Name,
                Email = d.User.Email,
                Phone = d.User.Phone,
                SpecializationName = d.Specialization.Name,
                ClinicLocation = d.ClinicLocation,
                AppointmentPrice = d.AppointmentPrice,
                ContactInfo = d.ContactInfo,
                Symptoms = d.DoctorSymptoms.Select(ds => ds.Symptom.Name).ToList()
            }).ToList();
        }

    }
}
