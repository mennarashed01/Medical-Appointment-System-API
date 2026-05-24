using SW_Project.DTOs.Diagnosis;
using SW_Project.DTOs.Doctor;
using SW_Project.Models;

namespace SW_Project.Services.IServices
{
    public interface IDiagnosisService
    {
        void AddInitialDiagnosis(int doctorId, CreateDiagnosisDto dto);
        void AddReExamination(int doctorId, ReExaminationDto dto);
        IEnumerable<DiagnosisHistoryDto> GetPatientHistory(int patientId);
        IEnumerable<DoctorPatientDto> GetPatientsForDoctor(int doctorId);
    }
}
