namespace SW_Project.DTOs.Diagnosis
{
    public class DiagnosisHistoryDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string DoctorName { get; set; }
        public string Description { get; set; }
        public string Prescription { get; set; }
        public string? BloodTestResults { get; set; }
        public string? DoctorNotes { get; set; }
        public int Version { get; set; }
    }
}
