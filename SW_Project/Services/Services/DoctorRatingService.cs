using Microsoft.EntityFrameworkCore;
using SW_Project.Data;
using SW_Project.DTOs.Patient;
using SW_Project.Models;
using SW_Project.Services.IServices;

namespace SW_Project.Services.Services
{
    public class DoctorRatingService : IDoctorRatingService
    {
        private readonly ApplicationDbContext _context;

        public DoctorRatingService(ApplicationDbContext context)
        {
            _context = context;
        }
        //Move Rating Logic to Service
        public async Task RateDoctorAsync(int userId, RateDoctorDto dto)
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
                throw new Exception("Patient not found");

            var doctor = await _context.Doctors.FindAsync(dto.DoctorId)
                         ?? throw new Exception("Doctor not found");

            var rating = new DoctorRating
            {
                PatientId = patient.Id,
                DoctorId = dto.DoctorId,
                Score = dto.Score,
                Comment = dto.Comment,
                Patient = patient,
                Doctor = doctor
            };

            _context.DoctorRatings.Add(rating);

            await _context.SaveChangesAsync();

            var average = await _context.DoctorRatings
                .Where(r => r.DoctorId == dto.DoctorId)
                .AverageAsync(r => (decimal)r.Score);

            doctor.Rating = Math.Round(average, 1);

            await _context.SaveChangesAsync();
        }
    }
}
