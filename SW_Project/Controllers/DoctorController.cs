using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SW_Project.DTOs;
using SW_Project.Repositories.IRepository;
using SW_Project.Services.IServices;
using System.Diagnostics.CodeAnalysis;

namespace SW_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet("{id:int}")]
        public ActionResult<DoctorResponseDto> GetById(int id)
        {
            var doctor = _doctorService.GetById(id);
            if (doctor == null) 
                return NotFound(new { Message = $"Doctor with ID {id} not found." });
            
            return Ok(doctor);
        }
        [HttpGet("{Name:alpha}")]
        public ActionResult<List<DoctorResponseDto>> GetByName(string Name)
        {
            var doctor = _doctorService.GetByName(Name);
            if (doctor == null) 
                return NotFound(new { Message = $"Doctor with Name:  {Name} not found." });
            
            return Ok(doctor);
        }
        [HttpGet("search-by-symptom/{symptomName}")]
        public ActionResult<List<DoctorResponseDto>> GeyBySymptom(string symptomName)
        {
            var doctors = _doctorService.GetBySymptoms(symptomName);
            if (doctors == null) 
                return NotFound(new { Message = $"Doctors with Symptom:  {symptomName} not found." });
            
            return Ok(doctors);
        }
        [HttpGet]
        public ActionResult<List<DoctorResponseDto>> GetAll()
        {
            var doctors = _doctorService.GetAll();
            return Ok(doctors);
        }


        [HttpPost]
        public ActionResult<CreateDoctorDto> Add(CreateDoctorDto doctor)
        {
            try
            {
                _doctorService.Create(doctor);
                return Ok(new { Message = "Doctor created successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("{id}")]
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
    }
}
