using SW_Project.DTOs.Auth;
using SW_Project.DTOs.Doctor;
using SW_Project.DTOs.Patient;
using SW_Project.Models;
using SW_Project.Repositories.IRepository;
using SW_Project.Services.IServices;

namespace SW_Project.Services.Services
{
    public class DoctorFacade : IDoctorFacade
    {
        private readonly IDoctorService _doctorService;
        private readonly IDiagnosisService _diagnosis;
        private readonly ISecretaryRepository _secretaryRepo;
        private readonly IAuthService _authService;
        private readonly IDoctorRepository _doctorRepo;

        public DoctorFacade(IDoctorService doctorService, IDiagnosisService diagnosis,
                            ISecretaryRepository secretaryRepo, IAuthService authService,
                            IDoctorRepository doctorRepo)
        {
            _doctorService = doctorService;
            _diagnosis = diagnosis;
            _secretaryRepo = secretaryRepo;
            _authService = authService;
            _doctorRepo = doctorRepo;
        }

        // ---------- Doctor Methods ----------
        public DoctorResponseDto GetById(int id) => _doctorService.GetById(id);
        public List<DoctorResponseDto> GetByName(string name) => _doctorService.GetByName(name);
        public List<DoctorResponseDto> GetBySymptoms(string symptomName) => _doctorService.GetBySymptoms(symptomName);
        public List<DoctorResponseDto> GetBySpecialization(string specName) => _doctorService.GetBySpecialization(specName);
        public List<DoctorResponseDto> GetAll() => _doctorService.GetAll();
        public void UpdateDoctorProfile(int userId, UpdateDoctorDto dto) => _doctorService.Update(userId, dto);
        public void DeleteDoctor(int userId) => _doctorService.Delete(userId);

        // ---------- Patients ----------
        public List<DoctorPatientDto> GetMyPatients(int doctorUserId)
        {            
            return  _diagnosis.GetPatientsForDoctor(doctorUserId).ToList();

        }

        // ---------- Secretary ----------
        public async Task<string> AddSecretaryAsync(int doctorUserId, RegisterSecretaryDto dto)
        {
            var doctor = _doctorRepo.GetByUserId(doctorUserId);
            if (doctor == null) throw new KeyNotFoundException("Doctor profile not found.");

            var secretaryUserId = await _authService.RegisterSecretaryUser(dto);

            var newSecretary = new Secretary
            {
                UserId = secretaryUserId,
                DoctorId = doctor.Id
            };

            _secretaryRepo.Add(newSecretary);
            _secretaryRepo.Save();

            return $"Secretary {dto.Name} added and linked successfully!";
        }

        public void RemoveSecretary(int doctorUserId, int secretaryId)
        {
            var doctor = _doctorRepo.GetByUserId(doctorUserId);
            var secretary = _secretaryRepo.GetById(secretaryId);

            if (secretary == null) throw new KeyNotFoundException("Secretary not found.");
            if (secretary.DoctorId != doctor.Id) throw new UnauthorizedAccessException("Not authorized.");

            _secretaryRepo.Delete(secretaryId);
            _secretaryRepo.Save();
        }
    }
}
