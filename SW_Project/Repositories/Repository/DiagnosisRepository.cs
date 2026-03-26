using Microsoft.EntityFrameworkCore;
using SW_Project.Data;
using SW_Project.Models;
using SW_Project.Repositories.IRepository;

namespace SW_Project.Repositories.Repository
{
    public class DiagnosisRepository : IDiagnosisRepository
    {
        private readonly ApplicationDbContext context;

        public DiagnosisRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public void Add(Diagnosis diagnosis)
        {
            context.Diagnoses.Add(diagnosis);
        }

        public IEnumerable<Diagnosis> GetByDoctorId(int doctorId)
        {
            return context.Diagnoses
                .Where(d => d.DoctorId == doctorId)
                .Include(d=> d.Patient.User)
                .ToList();
        }

        public Diagnosis GetById(int id)
        {
            return context.Diagnoses.Find(id);
        }

        public IEnumerable<Diagnosis> GetByPatientId(int patientId)
        {
            return context.Diagnoses
                .Where(p => p.PatientId == patientId)
                .Include(d => d.Doctor)
                .ThenInclude(doc => doc.User)
                .OrderByDescending(d => d.Date)
                .ToList();
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
