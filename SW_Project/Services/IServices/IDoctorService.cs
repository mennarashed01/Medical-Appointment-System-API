using SW_Project.DTOs;

namespace SW_Project.Services.IServices
{
    public interface IDoctorService
    {
        void Create(CreateDoctorDto dto);
        void Update(int id, UpdateDoctorDto dto);
        void Delete(int id);

        DoctorResponseDto GetById(int id);
        List<DoctorResponseDto> GetAll();
        List<DoctorResponseDto> GetByName(string name);
        List<DoctorResponseDto> GetBySymptoms(string symptomName);

    }

}
