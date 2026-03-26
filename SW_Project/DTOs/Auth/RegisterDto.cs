using SW_Project.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SW_Project.DTOs.Auth
{
    public class RegisterDto
    {

        //User

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        public string? Phone { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }
        [Required]
        [EnumDataType(typeof(Role))]
        public Role Role { get; set; }
        //Doctor
        [Required(ErrorMessage = "Specialization is required")]
        public int SpecializationId { get; set; }

        public string? ClinicLocation { get; set; }
        public string? ContactInfo { get; set; }
        public List<int> SymptomIds { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal AppointmentPrice { get; set; }

        // Patient
        public DateTime? DateOfBirth { get; set; }
        [Required]
        [EnumDataType(typeof(Gender))]
        public Gender Gender { get; set; }
        public string? BloodType { get; set; }     
        public string? ChronicDiseases { get; set; }

        //Secretary
        public int? AssignedDoctorId { get; set; }
    }
}
