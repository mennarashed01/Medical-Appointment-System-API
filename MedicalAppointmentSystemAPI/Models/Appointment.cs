using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SW_Project.Models
{
    public class Appointment
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
        [EnumDataType(typeof(Status))]
        public Status Status { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public Decimal Price { get; set; }
    }
    public enum Status
    {
        Pending,
        Confirmed,
        Cancelled
    }
}
