using System.ComponentModel.DataAnnotations;

namespace SW_Project.Models
{
    public class Symptom
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Name must be at most 100 characters")]
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
