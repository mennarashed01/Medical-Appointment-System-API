using SW_Project.Data;
using SW_Project.Models;
using SW_Project.Repositories.IRepository;
using SW_Project.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace SW_Project.Services.Services
{
    public class DoctorRecommendationService : IDoctorRecommendationService
    {
        private readonly IServiceProvider _serviceProvider;
    
        public DoctorRecommendationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public List<Doctor> GetRecommendedDoctors(List<string> symptomNames)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                return context.Doctors
                    .Include(d => d.User)
                    .Include(d => d.Specialization)
                    .Where(d => d.DoctorSymptoms.Any(ds => symptomNames.Contains(ds.Symptom.Name)))
                    .ToList();
            }
        }
    }
}
