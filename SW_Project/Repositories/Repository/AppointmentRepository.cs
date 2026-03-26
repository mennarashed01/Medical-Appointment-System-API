using Microsoft.EntityFrameworkCore;
using SW_Project.Data;
using SW_Project.Models;
using SW_Project.Repositories.IRepository;

namespace SW_Project.Repositories.Repository
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ApplicationDbContext context;

        public AppointmentRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public void Add(Appointment appointment)
        {
            context.Appointments.Add(appointment);
        }

        public async Task<List<Appointment>> GetAllAsync()
        {
            return  await context.Appointments
                        .Include(a => a.Patient)
                            .ThenInclude(p => p.User)
                        .Include(a => a.Doctor)
                        .ThenInclude(d => d.User)
                        .ToListAsync();
        }

        public Appointment GetById(int id)
        {
            return context.Appointments.FirstOrDefault(a => a.Id == id);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
