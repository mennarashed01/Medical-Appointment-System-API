using System.ComponentModel.DataAnnotations;

namespace SW_Project.Models
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        public User User { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        [Required]
        [EnumDataType(typeof(Gender))]
        public Gender Gender { get; set; }

        public string? BloodType { get; set; }
        public string? ChronicDiseases { get; set; }

    }
    public enum Gender
    {
        Male,
        Female
    }
}
