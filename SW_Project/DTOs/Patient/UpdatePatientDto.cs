using SW_Project.Models;
using System.ComponentModel.DataAnnotations;

namespace SW_Project.DTOs.Patient
{
    public class UpdatePatientDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }

        public string? Phone { get; set; }

        [Required]
        public Gender Gender { get; set; } 

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        public string? BloodType { get; set; }
        public string? ChronicDiseases { get; set; }

    }
}
