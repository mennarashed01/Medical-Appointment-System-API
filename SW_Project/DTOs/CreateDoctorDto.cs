using System.ComponentModel.DataAnnotations;

namespace SW_Project.DTOs
{
    public class CreateDoctorDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")] 
        public string Email { get; set; }
        public string? Phone { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Specialization is required")]
        public int SpecializationId { get; set; }
        public string? ClinicLocation { get; set; }
        public string? ContactInfo { get; set; }
        [Required]
        [Range(0, 100000)]
        public decimal AppointmentPrice { get; set; }
    }
}
