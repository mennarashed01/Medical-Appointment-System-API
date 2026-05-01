using SW_Project.Models;

namespace SW_Project.Repositories.IRepository
{
    public interface IDoctorRepository
    {
        List<Doctor> GetAll();
        Doctor GetById(int id);
        Doctor GetByUserId(int userId);
        void Add (Doctor doctor);
        void Update (Doctor doctor);
        void Delete (Doctor doctor);
        void Save();
        

    }
}
