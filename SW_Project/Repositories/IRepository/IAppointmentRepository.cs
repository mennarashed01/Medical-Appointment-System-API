using SW_Project.Models;

namespace SW_Project.Repositories.IRepository
{
    public interface IAppointmentRepository
    {
        void Add(Appointment appointment);
        Appointment GetById(int id);
        Task<List<Appointment>> GetAllAsync();
        void Save();
    }
}
