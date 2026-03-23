using SW_Project.Models;
using System.ComponentModel.DataAnnotations;

namespace SW_Project.DTOs.Patient
{
    public class CreatePatientDto
    {

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }

        public string? Phone { get; set; }
        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }

        public DateTime? DateOfBirth { get; set; }
        [Required]
        [EnumDataType(typeof(Gender), ErrorMessage = "Please select a valid gender")]
        public Gender Gender { get; set; }
    }
}
