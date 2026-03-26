using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SW_Project.DTOs.Doctor;
using SW_Project.Repositories.IRepository;
using SW_Project.Services.IServices;
using SW_Project.Services.Services;
using System.Diagnostics.CodeAnalysis;

namespace SW_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly IDiagnosisService diagnosis;

        public DoctorController(IDoctorService doctorService, IDiagnosisService diagnosis)
        {
            _doctorService = doctorService;
            this.diagnosis = diagnosis;
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public ActionResult<DoctorResponseDto> GetById(int id)
        {
            var doctor = _doctorService.GetById(id);
            if (doctor == null) 
                return NotFound(new { Message = $"Doctor with ID {id} not found." });
            
            return Ok(doctor);
        }
        
        [HttpGet("search-by-name/{name}")]
        [Authorize]
        public ActionResult<List<DoctorResponseDto>> GetByName(string name)
        {
            var doctor = _doctorService.GetByName(name);
            if (doctor == null) 
                return NotFound(new { Message = $"Doctor with Name:  {name} not found." });
            
            return Ok(doctor);
        }
        
        [HttpGet("search-by-symptom/{symptomName}")]
        [Authorize]
        public ActionResult<List<DoctorResponseDto>> GeyBySymptom(string symptomName)
        {
            var doctors = _doctorService.GetBySymptoms(symptomName);
            if (doctors == null) 
                return NotFound(new { Message = $"Doctors with Symptom:  {symptomName} not found." });
            
            return Ok(doctors);
        }
        
        [HttpGet("specialization/{specName}")]
        [Authorize]
        public ActionResult<List<DoctorResponseDto>> GetBySpecialization(string specName)
        {
            var doctors = _doctorService.GetBySpecialization(specName);
            if (doctors == null) 
                return NotFound(new { Message = $"Doctors with Specialization:  {specName} not found." });
            
            return Ok(doctors);
        }
        
        [HttpGet]
        [Authorize]
        public ActionResult<List<DoctorResponseDto>> GetAll()
        {
            var doctors = _doctorService.GetAll();
            return Ok(doctors);
        }


        //[HttpPost]
        //public ActionResult<CreateDoctorDto> Add(CreateDoctorDto doctor)
        //{
        //    try
        //    {
        //        _doctorService.Create(doctor);
        //        return Ok(new { Message = "Doctor created successfully." });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { Message = ex.Message });
        //    }
        //}

        [HttpPut("{id}")]
        [Authorize(Roles = "Doctor")]
        public IActionResult Update(int id, [FromBody] UpdateDoctorDto doctor)
        {
            var existingDoctor = _doctorService.GetById(id);
            if (existingDoctor == null)
            {
                return NotFound(new { Message = $"Cannot update. Doctor with ID {id} not found." });
            }

            _doctorService.Update(id, doctor);
            return Ok(new { Message = "Doctor updated successfully." });

        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Doctor")]
        public IActionResult Delete(int id)
        {
            var existingDoctor = _doctorService.GetById(id);
            if (existingDoctor == null)
            {
                return NotFound(new { Message = $"Cannot delete. Doctor with ID {id} not found." });
            }
            _doctorService.Delete(id);
            return Ok(new { Message = "Doctor deleted successfully." });

        }

        [HttpGet("my-patients")]
        [Authorize(Roles = "Doctor")]
        public IActionResult GetMyPatients()
        {
            var userId = int.Parse(User.FindFirst("Id").Value);

            var patients = diagnosis.GetPatientsForDoctor(userId);

            return Ok(patients);
        }
    }
}
