using Microsoft.EntityFrameworkCore;
using SW_Project.DTOs.Appointment;
using SW_Project.Models;
using SW_Project.Repositories.IRepository;
using SW_Project.Services.IServices;
using System.Security.Cryptography.Xml;

namespace SW_Project.Services.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository appointmentRepo;
        private readonly IDoctorRepository doctorRepo;

        public AppointmentService(IAppointmentRepository appointmentRepo, IDoctorRepository doctorRepo)
        {
            this.appointmentRepo = appointmentRepo;
            this.doctorRepo = doctorRepo;
        }
        public void BookAppointment(int patientId, BookAppointmentDto dto)
        {
            var doctor = doctorRepo.GetById(dto.DoctorId);
            if (doctor == null)
                throw new Exception("Doctor not found.");

            var appointment = new Appointment
            {
                PatientId = patientId,
                DoctorId = dto.DoctorId,
                Date = dto.Date,
                Status = Status.Pending,
                Price = doctor.AppointmentPrice
            };
            appointmentRepo.Add(appointment);
            appointmentRepo.Save();
        }

        public async Task<IEnumerable<AppointmentResponseDto>> GetUserAppointmentsAsync(int userId, Role role)
        {
            var allAppointments = await appointmentRepo.GetAllAsync();

            var query = allAppointments.AsQueryable();

            if (role == Role.Patient)
            {
                query = query.Where(a => a.Patient.UserId == userId);
            }
            else if (role == Role.Doctor)
            {
                query = query.Where(a => a.Doctor.UserId == userId);
            }

            return query.Select(a => new AppointmentResponseDto
            {
                Id = a.Id,
                PatientName = a.Patient.User.Name ,
                DoctorName = a.Doctor.User.Name,
                Date = a.Date,
                Status = a.Status.ToString(),
                Price = a.Doctor.AppointmentPrice
            }).ToList();
        }

        public void UpdateAppointmentStatus(int appointmentId, Status newStatus)
        {
            var appointment = appointmentRepo.GetById(appointmentId);
            if (appointment == null)
                throw new Exception("Appointment not found.");

            appointment.Status = newStatus;
            appointmentRepo.Save();
        }
    }
}
