using SW_Project.Models;
using SW_Project.Repositories.IRepository;
using SW_Project.Data;
using Microsoft.EntityFrameworkCore;

namespace SW_Project.Repositories.Repository
{
    public class PatientRepository : IPatientRepository
    {
        private readonly ApplicationDbContext _context;

        public PatientRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Add(Patient patient)
        {
            _context.Patients.Add(patient);
        }

        public void Delete(Patient patient)
        {
            _context.Patients.Remove(patient);
            _context.Users.Remove(patient.User);    
        }

        public List<Patient> GetAll()
        {
            var patients = _context.Patients
                .Include(p => p.User)
                .ToList();
            return patients;
        }

        public Patient GetById(int id)
        {
            var patient = _context.Patients
                .Include(p => p.User)
                .FirstOrDefault(p => p.Id == id);
            return patient;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(Patient patient)
        {
            _context.Patients.Update(patient);
        }

        public Patient GetByUserId(int userId)
        {
            return _context.Patients.FirstOrDefault(p => p.UserId == userId);
        }
    }
}
