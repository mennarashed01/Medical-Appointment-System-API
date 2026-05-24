using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SW_Project.Models
{
    public class Doctor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required(ErrorMessage = "Specialization is required")]
        public int SpecializationId { get; set; }
        public Specialization Specialization { get; set; }

        public string? ClinicLocation { get; set; }
        public string? ContactInfo { get; set; }

        [Range(0, 5)]
        [Column(TypeName = "decimal(3,2)")]
        public decimal Rating { get; set; } = 0;

        [Required]
        [Column(TypeName ="decimal(18,2)")]
        public decimal AppointmentPrice { get; set; }

        public virtual ICollection<DoctorSymptom> DoctorSymptoms { get; set; } = new List<DoctorSymptom>();
    }
}
