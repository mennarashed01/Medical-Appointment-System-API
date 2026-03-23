namespace SW_Project.DTOs
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
    }
}
