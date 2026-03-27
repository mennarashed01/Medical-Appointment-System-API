using SW_Project.DTOs.Patient;

namespace SW_Project.Services.IServices
{
    public interface IPatientServices
    {
        //void Create(CreatePatientDto dto);
        void Update(int userId, UpdatePatientDto dto);
        void Delete(int userId);

        PatientResponseDto GetById(int id);
        List<PatientResponseDto> GetByName(string name);
        List<PatientResponseDto> GetAll();

        Task<MedicalRecordDto> GetFullMedicalRecordAsync(int patientUserId);

    }
}
