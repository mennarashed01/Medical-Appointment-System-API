using SW_Project.Models;
using System.ComponentModel.DataAnnotations;

namespace SW_Project.DTOs.Patient
{
    public class PatientResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public Gender Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? Age => DateOfBirth.HasValue ? DateTime.Today.Year - DateOfBirth.Value.Year : null;
    }
}
