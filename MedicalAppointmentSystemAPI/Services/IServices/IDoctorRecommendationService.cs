using SW_Project.Models;

namespace SW_Project.Services.IServices
{
    public interface IDoctorRecommendationService
    {
        List<Doctor> GetRecommendedDoctors(List<string> symptomNames);
    }
}
