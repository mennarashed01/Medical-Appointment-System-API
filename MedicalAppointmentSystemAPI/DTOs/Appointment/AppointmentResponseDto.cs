namespace SW_Project.DTOs.Appointment
{
    public class AppointmentResponseDto
    {
        public int Id { get; set; }
        public string DoctorName { get; set; }
        public string PatientName { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public decimal Price { get; set; }

    }
}
