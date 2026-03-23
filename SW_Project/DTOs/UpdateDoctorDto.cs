using System.ComponentModel.DataAnnotations;

namespace SW_Project.DTOs
{
    public class UpdateDoctorDto
    {
        [Required]
        public string Name { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? Password { get; set; }

        public int SpecializationId { get; set; }
        public string? ClinicLocation { get; set; }
        public string? ContactInfo { get; set; }
        public decimal AppointmentPrice { get; set; }
    }
}
