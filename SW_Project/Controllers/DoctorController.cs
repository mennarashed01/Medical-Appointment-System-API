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
        private readonly IDoctorService _doctorService;
        private readonly IDiagnosisService diagnosis;
        private readonly ISecretaryRepository secretaryRepo;
        private readonly IAuthService authService;
        private readonly IDoctorRepository doctorRepo;

        public DoctorController(IDoctorService doctorService, IDiagnosisService diagnosis,ISecretaryRepository secretaryRepo,IAuthService authService,IDoctorRepository doctorRepo)
        {
            _doctorService = doctorService;
            this.diagnosis = diagnosis;
            this.secretaryRepo = secretaryRepo;
            this.authService = authService;
            this.doctorRepo = doctorRepo;
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

        [HttpPut("my-profile")]
        [Authorize(Roles = "Doctor")]
        public IActionResult Update([FromBody] UpdateDoctorDto doctor)
        {
            var userId = int.Parse(User.FindFirst("Id").Value);

            _doctorService.Update(userId, doctor);

            return Ok(new { Message = "Doctor updated successfully." });

        }

        [HttpDelete("delete-my-account")]
        [Authorize(Roles = "Doctor")]
        public IActionResult Delete()
        {
            var userId = int.Parse(User.FindFirst("Id").Value);

            _doctorService.Delete(userId);
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

        [HttpPost("add-secretary")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> AddSecretary([FromBody] RegisterSecretaryDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var doctorUserId = int.Parse(User.FindFirst("Id").Value);

                var doctor = doctorRepo.GetByUserId(doctorUserId);
                if (doctor == null) return NotFound("Doctor profile not found.");

                var secretaryUserId = await authService.RegisterSecretaryUser(dto);

                var newSecretary = new Secretary
                {
                    UserId = secretaryUserId,
                    DoctorId = doctor.Id
                };

                secretaryRepo.Add(newSecretary);
                secretaryRepo.Save(); 

                return Ok(new { Message = $"Secretary {dto.Name} added and linked successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpDelete("remove-secretary/{id}")]
        [Authorize(Roles = "Doctor")]
        public IActionResult DeleteSecretary(int id)
        {
            var doctorUserId = int.Parse(User.FindFirst("Id").Value);
            var doctor = doctorRepo.GetByUserId(doctorUserId);

            var secretary = secretaryRepo.GetById(id);

            if (secretary == null)
                return NotFound("Secretary not found.");

            if (secretary.DoctorId != doctor.Id)
                return Forbid("You are not authorized to remove this secretary.");

            secretaryRepo.Delete(id);

            return Ok(new { Message = "Secretary removed from your clinic successfully." });
        }
    }
}
