using System.ComponentModel.DataAnnotations;

namespace SW_Project.Models
{
    public class DoctorRating
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        [Required]
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        [Range(1, 5)]
        public int Score { get; set; } 
        public string? Comment { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
