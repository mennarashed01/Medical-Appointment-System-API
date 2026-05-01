using System.ComponentModel.DataAnnotations;

namespace SW_Project.Models
{
    public class Specialization
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Name must be at most 50 characters")]
        public string Name { get; set; }
    }
}
