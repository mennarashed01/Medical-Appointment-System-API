using SW_Project.Models;

namespace SW_Project.Repositories.IRepository
{
    public interface IPatientRepository
    {
        List<Patient> GetAll();
        Patient GetById(int id);
        void Add(Patient patient);
        void Update(Patient patient);
        void Delete(Patient patient);
        void Save();

    }
}
