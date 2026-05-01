using Microsoft.EntityFrameworkCore;
using SW_Project.Data;
using SW_Project.Models;
using SW_Project.Repositories.IRepository;

namespace SW_Project.Repositories.Repository
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly ApplicationDbContext context;

        public DoctorRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public void Add(Doctor doctor)
        {
            context.Doctors.Add(doctor);
        }

        public void Delete(Doctor doctor)
        {
            context.Doctors.Remove(doctor);
            context.Users.Remove(doctor.User);
        }
        public void Update(Doctor doctor)
        {
            context.Doctors.Update(doctor);
        }
        public List<Doctor> GetAll()
        {
            List<Doctor> doctors= context.Doctors
                        .Include(d=> d.User)
                        .Include(d=> d.Specialization)
                        .Include(d => d.DoctorSymptoms)
                        .ThenInclude(ds => ds.Symptom)
                        .ToList();
            return doctors;
        }

        public Doctor GetById(int id)
        {
            var doctor = context.Doctors
                        .Include(d => d.User)
                        .Include(d => d.Specialization)
                        .Include(d => d.DoctorSymptoms)
                        .ThenInclude(ds => ds.Symptom)
                        .FirstOrDefault(d => d.Id == id);
            return doctor;
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public Doctor GetByUserId(int userId)
        {
            return context.Doctors.FirstOrDefault(d=> d.UserId == userId);
        }
    }
}
