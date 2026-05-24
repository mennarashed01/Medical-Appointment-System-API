using SW_Project.DTOs.Appointment;
using SW_Project.Models;

namespace SW_Project.Services.IServices
{
    public interface IAppointmentService
    {
        void BookAppointment(int patientId, BookAppointmentDto dto);
        void UpdateAppointmentStatus(int appointmentId, Status newStatus);
        Task<IEnumerable<AppointmentResponseDto>> GetUserAppointmentsAsync(int userId, Role role);
    }
}
