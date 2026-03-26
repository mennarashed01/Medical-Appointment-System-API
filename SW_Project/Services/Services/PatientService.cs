using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SW_Project.Data;
using SW_Project.DTOs.Patient;
using SW_Project.Models;
using SW_Project.Repositories.IRepository;
using SW_Project.Services.IServices;
using System.Numerics;

namespace SW_Project.Services.Services
{
    public class PatientService : IPatientServices
    {
        private readonly IPatientRepository _repo;
        private readonly ApplicationDbContext context;

        public PatientService(IPatientRepository repo,ApplicationDbContext context)
        {
            _repo = repo;
            this.context = context;
        }

        #region CreatePatient
        /*
        public void Create(CreatePatientDto dto)
        {
            if (dto.DateOfBirth.HasValue && dto.DateOfBirth.Value > DateTime.Now)
            {
                throw new Exception("Date of birth cannot be in the future.");
            }


            if (dto.DateOfBirth.HasValue && dto.DateOfBirth.Value < DateTime.Now.AddYears(-100))
            {
                throw new Exception("Please enter a valid date of birth.");
            }

            if (!Enum.IsDefined(typeof(Gender), dto.Gender))
            {
                throw new Exception("Selected Gender is not valid.");
            }

            var allPatients = _repo.GetAll();
            if(allPatients.Any(p => p.User.Email == dto.Email))
            {
                throw new Exception("This Email is already registered");
            }

            
            

            try
            {
                string hashedpassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

                var user = new User
                {
                    Name = dto.Name,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    Password = hashedpassword,
                    Role = Role.Patient
                };
                var patient = new Patient
                {
                    User = user,
                    Gender = dto.Gender,
                    DateOfBirth = dto.DateOfBirth
                };
                _repo.Add(patient);
                _repo.Save();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }

        }
        */

        #endregion
        public void Delete(int id)
        {
            var patient = _repo.GetById(id);
            if(patient != null)
            {
                _repo.Delete(patient);
                _repo.Save();
            }
        }

        public List<PatientResponseDto> GetAll()
        {
            var patients = _repo.GetAll();
            if (patients == null) return null;
            return patients.Select(p => new PatientResponseDto
            {
                Id = p.Id,
                Name = p.User.Name,
                Email = p.User.Email,
                Phone= p.User.Phone,
                DateOfBirth = p.DateOfBirth,
                Gender = p.Gender,
                BloodType = p.BloodType,
                ChronicDiseases = p.ChronicDiseases,
            }).ToList();
        }

        public PatientResponseDto GetById(int id)
        {
            var patient = _repo.GetById(id);
            if (patient == null) return null;
            return new PatientResponseDto
            {
                Id = patient.Id,
                Name = patient.User.Name,
                Email = patient.User.Email,
                Phone = patient.User.Phone,
                DateOfBirth= patient.DateOfBirth,
                Gender = patient.Gender,
                ChronicDiseases= patient.ChronicDiseases,
                BloodType = patient.BloodType
            };
        }

        public List<PatientResponseDto> GetByName(string name)
        {
            var patientList = _repo.GetAll()
                .Where(p => p.User.Name.Trim().ToLower().Contains(name.Trim().ToLower()))
                .ToList();
            if (patientList == null) return new List<PatientResponseDto>();
            return patientList.Select(patient => new PatientResponseDto
            {
                Id = patient.Id,
                Name = patient.User.Name,
                Email = patient.User.Email,
                Phone = patient.User.Phone,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                BloodType= patient.BloodType,
                ChronicDiseases = patient.ChronicDiseases
            }).ToList();
        }

        public async Task<MedicalRecordDto> GetFullMedicalRecordAsync(int patientUserId)
        {
            var patient = await context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == patientUserId);

            if (patient == null) return null;

            var diagnoses = await context.Diagnoses
                .Include(d => d.Doctor).ThenInclude(doc => doc.User)
                .Where(d => d.PatientId == patient.Id)
                .OrderByDescending(d => d.Date)
                .Select(d => new PatientDiagnosisHistoryDto
                {
                    DiagnosisId = d.Id,
                    DoctorName = d.Doctor.User.Name,
                    Date = d.Date,
                    Description = d.Description,
                    Prescription = d.Prescription,
                    Notes = d.DoctorNotes,
                    BloodTestResults = d.BloodTestResults
                }).ToListAsync();

            return new MedicalRecordDto
            {
                PatientName = patient.User.Name,
                BloodType = patient.BloodType,
                ChronicDiseases = patient.ChronicDiseases,
                Diagnoses = diagnoses
            };
        }

        public void Update(int id, UpdatePatientDto dto)
        {
            var patient = _repo.GetById(id);
            if (patient == null) 
                throw new Exception("Patient not found");

            patient.User.Name = dto.Name;
            patient.User.Phone = dto.Phone;
            patient.Gender = dto.Gender;
            patient.DateOfBirth = dto.DateOfBirth;
            patient.BloodType = dto.BloodType;
            patient.ChronicDiseases = dto.ChronicDiseases;
            _repo.Update(patient);
            _repo.Save();

        }
    }
}
