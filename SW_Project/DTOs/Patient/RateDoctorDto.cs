using System.ComponentModel.DataAnnotations;

namespace SW_Project.DTOs.Patient
{
    public class RateDoctorDto
    {
        public int DoctorId { get; set; }
        [Range(1, 5)]
        public int Score { get; set; }
        public string? Comment { get; set; }
    }
}
