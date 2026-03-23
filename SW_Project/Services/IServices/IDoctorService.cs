using SW_Project.DTOs;

namespace SW_Project.Services.IServices
{
    public interface IDoctorService
    {
        void Create(CreateDoctorDto dto);
        void Update(int id, UpdateDoctorDto dto);
        void Delete(int id);

        DoctorResponseDto Get(int id);
        List<DoctorResponseDto> GetAll();
    }

}
