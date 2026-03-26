using SW_Project.Models;

namespace SW_Project.Repositories.IRepository
{
    public interface IPatientRepository
    {
        List<Patient> GetAll();
        Patient GetById(int id);
        Patient GetByUserId(int userId);
        void Add(Patient patient);
        void Update(Patient patient);
        void Delete(Patient patient);
        void Save();
        
    }
}
