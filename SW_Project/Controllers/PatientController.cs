using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SW_Project.Data;
using SW_Project.DTOs.Patient;
using SW_Project.Models;
using SW_Project.Services.IServices;
using SW_Project.Services.Services;
using System.Security.Claims;

namespace SW_Project.Controllers
{
    [Route("api/patients")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientServices _patientServices;
        private readonly IDoctorRecommendationService _recommendationService;
        private readonly IDoctorRatingService ratingService;

        //private readonly ApplicationDbContext context; //Before using context for rating doctor


        public PatientController(
            IPatientServices services, 
            IDoctorRecommendationService recommendationService,
            IDoctorRatingService ratingService)//ApplicationDbContext _context)
        {
            _patientServices = services;
            _recommendationService = recommendationService;
            this.ratingService = ratingService;
            //context = _context;
        }

        [HttpPost("recommend-doctors")]
        //[Authorize(Roles ="Patient")]
        public IActionResult RecommendDoctors([FromBody] SymptomSearchDto dto)
        {
            var doctors = _recommendationService.GetRecommendedDoctors(dto.Symptoms);

            var result = doctors.Select(d => new {
                id = d.Id,
                DoctorName = d.User.Name,
                Specialization = d.Specialization.Name,
                Location = d.ClinicLocation,
                Price = d.AppointmentPrice,
                Ratings = d.Rating
            });

            return Ok(result);
        }

        [HttpGet]
        public ActionResult<List<PatientResponseDto>> GetAll()
        {
            var PatientList = _patientServices.GetAll();
            //if (PatientList == null) //Before
            if (PatientList == null || !PatientList.Any()) //After
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
            var patients = _patientServices.GetByName(name);
            //if (patients == null)  //Before
            if (patients == null || !patients.Any()) //After
                return NotFound(new { Message = $"Patient with name: {name} not found." });

            return Ok(patients);
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

        [HttpPut("my-profile")]
        [Authorize(Roles = "Patient")]
        public IActionResult Update([FromBody] UpdatePatientDto dto)
        {
            //var userId = int.Parse(User.FindFirst("Id").Value);
            var userId = User.GetUserId();


            _patientServices.Update(userId,dto);
            return Ok(new { Message = "Patient Updated Successfully!" });
        }

        [HttpDelete("delete-my-profile")]
        [Authorize(Roles = "Patient")]
        public IActionResult Delete()
        {
            //var userId = int.Parse(User.FindFirst("Id").Value);
            var userId = User.GetUserId();

            _patientServices.Delete(userId);
            return Ok(new { Message = "Patient Deleted Successfully!" });
        }

        [HttpGet("my-medical-record")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetMyMedicalRecord()
        {
            //Before
            //var claim = User.FindFirst("Id");

            //if (claim == null) return Unauthorized("User ID  not found in token.");

            //int userId = int.Parse(claim.Value);
            //After
            var userId = User.GetUserId() ;
            var record = await _patientServices.GetFullMedicalRecordAsync(userId);

            return Ok(record);
        }

        [HttpPost("rate-doctor")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> RateDoctor(RateDoctorDto dto)
        {
            
            var userIdClaim = User.FindFirst("Id");
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            //Before >> //Move Rating Logic to Service "DoctorRatingService"
            //var patient = await context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            //if (patient == null) return NotFound("Patient profile not found.");

            //var rating = new DoctorRating
            //{
            //    PatientId = patient.Id,
            //    DoctorId = dto.DoctorId,
            //    Score = dto.Score,
            //    Comment = dto.Comment
            //};

            //context.DoctorRatings.Add(rating);
            //await context.SaveChangesAsync();

            //var doctor = await context.Doctors.FindAsync(dto.DoctorId);
            //if (doctor != null)
            //{
            //    var average = await context.DoctorRatings
            //        .Where(r => r.DoctorId == dto.DoctorId)
            //        .AverageAsync(r => (decimal)r.Score); 

            //    doctor.Rating = Math.Round(average, 1);

            //    await context.SaveChangesAsync();
            //}

            await ratingService.RateDoctorAsync(userId, dto);
            return Ok(new { Message = "Rating submitted successfully!" });
        }
        
    }

    // Extension Method (Extract Helper for UserId)
    public static class ClaimsExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirst("Id")?.Value);
        }
    }
}
