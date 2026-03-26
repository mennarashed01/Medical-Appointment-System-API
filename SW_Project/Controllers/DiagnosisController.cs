using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SW_Project.DTOs.Diagnosis;
using SW_Project.DTOs.Doctor;
using SW_Project.Models;
using SW_Project.Services.IServices;
using SW_Project.Services.Services;

namespace SW_Project.Controllers
{
    [Authorize(Roles = "Doctor")]
    [Route("api/[controller]")]
    [ApiController]
    public class DiagnosisController : ControllerBase
    {
        private readonly IDiagnosisService service;

        public DiagnosisController(IDiagnosisService service)
        {
            this.service = service;
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] CreateDiagnosisDto dto)
        {
            var doctorId = int.Parse(User.FindFirst("Id").Value);
            service.AddInitialDiagnosis(doctorId, dto);
            return Ok(new { Message = "Diagnosis created ." });
        }

        [HttpPost("re-examine")]
        public IActionResult ReExamination([FromBody] ReExaminationDto dto)
        {
            var doctorId = int.Parse(User.FindFirst("Id").Value);
            service.AddReExamination(doctorId, dto);
            return Ok(new { Message = "Re-examination saved as a new version." });
        }

        [HttpGet("history/{patientId}")]
        public IActionResult GetHistory(int patientId)
        {
            var history = service.GetPatientHistory(patientId);
            return Ok(history);
        }
    }
}
