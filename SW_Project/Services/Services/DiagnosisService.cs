using SW_Project.DTOs.Diagnosis;
using SW_Project.DTOs.Doctor;
using SW_Project.Models;
using SW_Project.Repositories.IRepository;
using SW_Project.Services.IServices;

namespace SW_Project.Services.Services
{

    public class DiagnosisService : IDiagnosisService
    {
        private readonly IDiagnosisRepository diagnosisRepo;
        private readonly IDoctorRepository doctorRepo;

        public DiagnosisService(IDiagnosisRepository diagnosisRepo, IDoctorRepository doctorRepo)
        {
            this.diagnosisRepo = diagnosisRepo;
            this.doctorRepo = doctorRepo;
        }

        public void AddInitialDiagnosis(int userId, CreateDiagnosisDto dto)
        {
            var doctor = doctorRepo.GetByUserId(userId);

            if (doctor == null)
            {
                throw new Exception("This user is not registered as a doctor!");
            }
            var diagnosis = new Diagnosis
            {
                DoctorId = doctor.Id,
                PatientId = dto.PatientId,
                Description = dto.Description,
                Prescription = dto.Prescription,
                BloodTestResults = dto.BloodTestResults,
                DoctorNotes = dto.DoctorNotes,
                Date = DateTime.Now,
                Version = 1
            };

            diagnosisRepo.Add(diagnosis);
            diagnosisRepo.Save();

            NotifyPatient(dto.PatientId, "Your doctor added a new diagnosis.");
        }

        public void AddReExamination(int userId, ReExaminationDto dto)
        {
            var oldDiag = diagnosisRepo.GetById(dto.PreviousDiagnosisId);
            if (oldDiag == null)
                throw new Exception("Diagnosis not found.");

            var doctor = doctorRepo.GetByUserId(userId);

            var nextExam = new Diagnosis
            {
                DoctorId = doctor.Id,
                PatientId = oldDiag.PatientId,
                Description = oldDiag.Description,
                Prescription = dto.NewPrescription,
                DoctorNotes = dto.NewNotes,
                Date = DateTime.Now,
                Version = oldDiag.Version + 1
            };

            diagnosisRepo.Add(nextExam);
            diagnosisRepo.Save();
        }

        public IEnumerable<DiagnosisHistoryDto> GetPatientHistory(int patientId)
        {
            var diagnosis =  diagnosisRepo.GetByPatientId(patientId);
            return diagnosis.Select(d => new DiagnosisHistoryDto
            {
                Id= d.Id,
                Date= d.Date,
                DoctorName = d.Doctor.User.Name,
                Description = d.Description,
                Prescription = d.Prescription,
                BloodTestResults = d.BloodTestResults,
                DoctorNotes= d.DoctorNotes,
                Version = d.Version,
            }).ToList();
        }

        public IEnumerable<DoctorPatientDto> GetPatientsForDoctor(int userId)
        {
            var doctor = doctorRepo.GetByUserId(userId);

            if (doctor == null) return new List<DoctorPatientDto>();

            var diagnoses = diagnosisRepo.GetByDoctorId(doctor.Id);

            return diagnoses
                .Where(d => d.Patient != null && d.Patient.User != null)
                .Select(d => d.Patient)
                .DistinctBy(p => p.Id)
                .Select(p => new DoctorPatientDto
                {
                    PatientId = p.Id,
                    PatientName = p.User.Name,
                    PatientPhone = p.User?.Phone
                }).ToList();

        }

        private void NotifyPatient(int patientId, string message) => Console.WriteLine(message);
    }
}

