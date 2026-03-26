using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SW_Project.DTOs.Appointment;
using SW_Project.Models;
using SW_Project.Repositories.IRepository;
using SW_Project.Services.IServices;
using SW_Project.Services.Services;
using System.Security.Claims;

namespace SW_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService appointmentService;
        private readonly IPatientRepository  patientRepo;

        public AppointmentController(IAppointmentService appointmentService, IPatientRepository patientRepo)
        {
            this.appointmentService = appointmentService;
            this.patientRepo = patientRepo;
        }

        [HttpPost("Book")]
        [Authorize(Roles ="Patient")]
        public IActionResult Book([FromBody] BookAppointmentDto dto)
        {
            var userId = int.Parse(User.FindFirst("Id").Value);

            var patient = patientRepo.GetByUserId(userId);
            if(patient == null) 
                return NotFound(new {Message = "Patient profile not found."});

            try
            {
                appointmentService.BookAppointment(patient.Id,dto);
                return Ok(new { Message = "Appointment request send successfully . Status: Pending." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPatch("{id}/Status")]
        [Authorize(Roles = "Secretary,Doctor")] 
        public IActionResult UpdateStatus(int id, [FromBody] Status newStatus)
        {
            try
            {
                appointmentService.UpdateAppointmentStatus(id, newStatus);
                return Ok(new { Message = $"Appointment status updated to {newStatus}." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("my-appointments")]
        public async Task<IActionResult> GetMyAppointments()
        {
            var roleClaim = User.Claims.FirstOrDefault(c => c.Type.Contains("role", StringComparison.OrdinalIgnoreCase));

            if (roleClaim == null)
                return Unauthorized("Role claim not found in token. Available claims: " + string.Join(", ", User.Claims.Select(c => c.Type)));

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type.Equals("Id", StringComparison.OrdinalIgnoreCase) || c.Type.Contains("nameidentifier"));

            if (userIdClaim == null)
                return Unauthorized("User ID claim not found");

            int userId = int.Parse(userIdClaim.Value);
            var role = Enum.Parse<Role>(roleClaim.Value);

            var result = await appointmentService.GetUserAppointmentsAsync(userId, role);
            return Ok(result);
        }
    }
}
