using SW_Project.DTOs.Patient;

namespace SW_Project.Services.IServices
{
    public interface IPatientServices
    {
        void Create(CreatePatientDto dto);
        void Update(int id, UpdatePatientDto dto);
        void Delete(int id);

        PatientResponseDto GetById(int id);
        List<PatientResponseDto> GetByName(string name);
        List<PatientResponseDto> GetAll();

    }
}
