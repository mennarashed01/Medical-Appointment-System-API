using SW_Project.DTOs.Patient;

namespace SW_Project.Services.IServices
{
    public interface IDoctorRatingService
    {
        Task RateDoctorAsync(int userId, RateDoctorDto dto);
    }
}
