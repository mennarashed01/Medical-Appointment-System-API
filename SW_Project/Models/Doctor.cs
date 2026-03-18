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

        [Required]
        [Column(TypeName ="decimal(18,2)")]
        public decimal AppointmentPrice { get; set; }
    }
}
