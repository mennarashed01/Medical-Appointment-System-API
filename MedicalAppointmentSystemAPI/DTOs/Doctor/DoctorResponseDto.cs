namespace SW_Project.DTOs.Doctor
{
    public class DoctorResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string SpecializationName { get; set; }
        public string? ClinicLocation { get; set; }
        public string? ContactInfo { get; set; }
        public decimal AppointmentPrice { get; set; }
        public decimal Ratings { get; set; }

        public List<string>? Symptoms { get; set; } = new List<string>();
    }
}
