using System.ComponentModel.DataAnnotations;

namespace SW_Project.DTOs.Diagnosis
{
    public class CreateDiagnosisDto
    {
        [Required]
        public int PatientId { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Prescription { get; set; }
        public string? BloodTestResults { get; set; }
        public string? DoctorNotes { get; set; }
    }
}
