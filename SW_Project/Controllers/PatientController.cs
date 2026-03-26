using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SW_Project.DTOs.Patient;
using SW_Project.Models;
using SW_Project.Services.IServices;
using SW_Project.Services.Services;
using System.Security.Claims;

namespace SW_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientServices _patientServices;
        private readonly IDoctorRecommendationService _recommendationService;

        public PatientController(IPatientServices services, IDoctorRecommendationService recommendationService)
        {
            _patientServices = services;
            _recommendationService = recommendationService;
        }

        [HttpPost("recommend-doctors")]
        public IActionResult RecommendDoctors([FromBody] SymptomSearchDto dto)
        {
            var doctors = _recommendationService.GetRecommendedDoctors(dto.Symptoms);

            var result = doctors.Select(d => new {
                DoctorName = d.User.Name,
                Specialization = d.Specialization.Name,
                Location = d.ClinicLocation,
                Price = d.AppointmentPrice
            });

            return Ok(result);
        }

        [HttpGet]
        public ActionResult<List<PatientResponseDto>> GetAll()
        {
            var PatientList = _patientServices.GetAll();
            if (PatientList == null) 
                return NotFound();
            return Ok(PatientList);
        }
        [HttpGet("{id:int}")]
        public ActionResult<PatientResponseDto> GetById(int id)
        {
            var patient = _patientServices.GetById(id);
            if (patient == null) return NotFound(new { Message = $"Patient with ID: {id} not found." });

            return Ok(patient);
        }
        [HttpGet("search/{name}")]
        public ActionResult<List<PatientResponseDto>> GetByName(string name)
        {
            var patient = _patientServices.GetByName(name);
            if (patient == null) return NotFound(new { Message = $"Patient with name: {name} not found." });

            return Ok(patient);
        }

        //[HttpPost]
        //public IActionResult Add([FromBody]CreatePatientDto dto)
        //{
        //    if (!ModelState.IsValid) return BadRequest(ModelState);
        //    try
        //    {
        //        _patientServices.Create(dto);
        //        return Ok(new { Message = "Patient Created Successfully!" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { Error = ex.Message });
        //    }
        //}

        [HttpPut("{id}")]
        public IActionResult Update(int id ,[FromBody] UpdatePatientDto dto)
        {
            var existingPatient = _patientServices.GetById(id);
            if (existingPatient == null)
                return NotFound(new { Message = $"Cannot update. Patient with ID {id} not found." });

            _patientServices.Update(id,dto);
            return Ok(new { Message = "Patient Updated Successfully!" });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existingPatient = _patientServices.GetById(id);
            if (existingPatient == null)
                return NotFound(new { Message = $"Cannot Delete. Patient with ID {id} not found." });
            _patientServices.Delete(id);
            return Ok(new { Message = "Patient Deleted Successfully!" });
        }

        [HttpGet("my-medical-record")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetMyMedicalRecord()
        {
            var claim = User.FindFirst("Id");

            if (claim == null) return Unauthorized("User ID  not found in token.");

            int userId = int.Parse(claim.Value);
            var record = await _patientServices.GetFullMedicalRecordAsync(userId);

            return Ok(record);
        }
    }
}
