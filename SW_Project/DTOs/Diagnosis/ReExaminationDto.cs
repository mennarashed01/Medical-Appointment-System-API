using System.ComponentModel.DataAnnotations;

namespace SW_Project.DTOs.Diagnosis
{
    public class ReExaminationDto
    {
        [Required]
        public int PreviousDiagnosisId { get; set; }
        public string NewPrescription { get; set; }
        public string? NewNotes { get; set; }
    }
}
