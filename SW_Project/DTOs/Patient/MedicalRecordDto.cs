namespace SW_Project.DTOs.Patient
{
    public class MedicalRecordDto
    {
        public string  PatientName { get; set; }
        public string? BloodType { get; set; }
        public string? ChronicDiseases { get; set; }

        public List<PatientDiagnosisHistoryDto> Diagnoses { get; set; } = new List<PatientDiagnosisHistoryDto>();

    }
}
