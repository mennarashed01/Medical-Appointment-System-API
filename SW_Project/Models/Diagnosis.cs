using System.ComponentModel.DataAnnotations;

namespace SW_Project.Models
{
    public class Diagnosis
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        [Required]
        public int PatientId { get; set; }
        public Patient Patient { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Prescription { get; set; }
        public string? BloodTestResults { get; set; }
        public string? DoctorNotes { get; set; }

        public int Version { get; set; } = 1;

    }
}
