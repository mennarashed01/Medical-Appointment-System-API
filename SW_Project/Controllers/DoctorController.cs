using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SW_Project.Data;
using SW_Project.DTOs.Auth;
using SW_Project.DTOs.Doctor;
using SW_Project.Models;
using SW_Project.Repositories.IRepository;
using SW_Project.Services.IServices;
using SW_Project.Services.Services;
using System.Diagnostics.CodeAnalysis;

namespace SW_Project.Controllers
{
    [Route("api/doctors")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorFacade _facade;

        //public DoctorController(IDoctorService doctorService, IDiagnosisService diagnosis,ISecretaryRepository secretaryRepo,IAuthService authService,IDoctorRepository doctorRepo)
        //{
        //    _doctorService = doctorService;
        //    this.diagnosis = diagnosis;
        //    this.secretaryRepo = secretaryRepo;
        //    this.authService = authService;
        //    this.doctorRepo = doctorRepo;
        //}

        public DoctorController(IDoctorFacade _facade)
        {
            this._facade = _facade;
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public ActionResult<DoctorResponseDto> GetById(int id)
        {
            var doctor = _facade.GetById(id);
            if (doctor == null) 
                return NotFound(new { Message = $"Doctor with ID {id} not found." });
            
            return Ok(doctor);
        }
        
        [HttpGet("search-by-name/{name}")]
        [Authorize]
        public ActionResult<List<DoctorResponseDto>> GetByName(string name)
        {
            var doctor = _facade.GetByName(name);
            if (doctor == null) 
                return NotFound(new { Message = $"Doctor with Name:  {name} not found." });
            
            return Ok(doctor);
        }
        
        [HttpGet("search-by-symptom/{symptomName}")]
        [Authorize]
        public ActionResult<List<DoctorResponseDto>> GeyBySymptom(string symptomName)
        {
            var doctors = _facade.GetBySymptoms(symptomName);
            if (doctors == null) 
                return NotFound(new { Message = $"Doctors with Symptom:  {symptomName} not found." });
            
            return Ok(doctors);
        }
        
        [HttpGet("specialization/{specName}")]
        [Authorize]
        public ActionResult<List<DoctorResponseDto>> GetBySpecialization(string specName)
        {
            var doctors = _facade.GetBySpecialization(specName);
            if (doctors == null) 
                return NotFound(new { Message = $"Doctors with Specialization:  {specName} not found." });
            
            return Ok(doctors);
        }
        
        [HttpGet]
        [Authorize]
        public ActionResult<List<DoctorResponseDto>> GetAll()
        {
            var doctors = _facade.GetAll();
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

        [HttpPut("my-profile")]
        [Authorize(Roles = "Doctor")]
        public IActionResult Update([FromBody] UpdateDoctorDto doctor)              
        {
            var userId = int.Parse(User.FindFirst("Id").Value);

            _facade.UpdateDoctorProfile(userId, doctor);

            return Ok(new { Message = "Doctor updated successfully." });

        }

        [HttpDelete("delete-my-account")]
        [Authorize(Roles = "Doctor")]
        public IActionResult Delete()
        {
            var userId = int.Parse(User.FindFirst("Id").Value);

            _facade.DeleteDoctor(userId);
            return Ok(new { Message = "Doctor deleted successfully." });

        }

        [HttpGet("my-patients")]
        [Authorize(Roles = "Doctor")]
        public IActionResult GetMyPatients()
        {
            var userId = int.Parse(User.FindFirst("Id").Value);

            var patients = _facade.GetMyPatients(userId);

            return Ok(patients);
        }

        [HttpPost("add-secretary")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> AddSecretary([FromBody] RegisterSecretaryDto dto)
        {

            var userId = int.Parse(User.FindFirst("Id").Value);

            var message = await _facade.AddSecretaryAsync(userId, dto);

            return Ok(new { Message = $"Secretary {dto.Name} added and linked successfully!" });
            
        }

        [HttpDelete("remove-secretary/{id}")]
        [Authorize(Roles = "Doctor")]
        public IActionResult DeleteSecretary(int id)
        {
            var userId = int.Parse(User.FindFirst("Id").Value);

            _facade.RemoveSecretary(userId, id);

            return Ok(new { Message = "Secretary removed successfully." }); 
        }
    }
}
