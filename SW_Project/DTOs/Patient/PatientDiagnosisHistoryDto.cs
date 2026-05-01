namespace SW_Project.DTOs.Patient
{
    public class PatientDiagnosisHistoryDto
    {
        public int DiagnosisId { get; set; }
        public string DoctorName { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; } 
        public string Prescription { get; set; } 
        public string Notes { get; set; } 
        public string BloodTestResults { get; set; } 
    }
}
