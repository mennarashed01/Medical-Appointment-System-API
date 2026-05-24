using SW_Project.Models;

namespace SW_Project.Repositories.IRepository
{
    public interface IDiagnosisRepository
    {
        void Add(Diagnosis diagnosis);
        Diagnosis GetById(int id);
        IEnumerable<Diagnosis> GetByPatientId(int patientId);
        IEnumerable<Diagnosis> GetByDoctorId(int doctorId);
        void Save();
    }
}
