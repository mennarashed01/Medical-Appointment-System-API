using SW_Project.DTOs.Auth;
using SW_Project.DTOs.Doctor;
using SW_Project.DTOs.Patient;

namespace SW_Project.Services.IServices
{
    public interface IDoctorFacade
    {
        // Docotor Operations
        DoctorResponseDto GetById(int id);
        List<DoctorResponseDto> GetByName(string name);
        List<DoctorResponseDto> GetBySymptoms(string symptomName);
        List<DoctorResponseDto> GetBySpecialization(string specName);
        List<DoctorResponseDto> GetAll();
        void UpdateDoctorProfile(int userId, UpdateDoctorDto dto);
        void DeleteDoctor(int userId);

        // Patient Opreations
        List<DoctorPatientDto> GetMyPatients(int doctorUserId);


        // Secretary Opreations
        Task<string> AddSecretaryAsync(int doctorUserId, RegisterSecretaryDto dto);
        void RemoveSecretary(int doctorUserId, int secretaryId);

    }
}
