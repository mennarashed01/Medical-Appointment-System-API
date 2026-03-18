using System.ComponentModel.DataAnnotations;

namespace SW_Project.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="Name is required")]
        [StringLength(50,MinimumLength =2,ErrorMessage ="Name must be between 2 and 50 characters")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        public string? Phone { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }
        [Required]
        [EnumDataType(typeof(Role))]
        public Role Role { get; set; }

    }

    public enum Role
    {
        Doctor,
        Patient,
        Secretary
    }
}
