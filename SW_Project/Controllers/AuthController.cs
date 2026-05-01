using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SW_Project.DTOs.Auth;
using SW_Project.Services.IServices;
using SW_Project.Services.Services;

namespace SW_Project.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }
        [HttpPost("login")]
        public ActionResult Login( [FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var token = authService.Login(dto);

                return Ok(
                    new
                    {
                        Message = "Login Successful",
                        Token = token
                    });

            }
            catch(Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = authService.Register(dto);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            { 
                return BadRequest(new { Error = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("change-password")]
        public IActionResult ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var userIdClaim = User.FindFirst("Id")?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized(new { Message = "Invalid Token Claims" });

                int userId = int.Parse(userIdClaim);
                var result = authService.ChangePassword(userId,dto.OldPassword,dto.NewPassword);

                return Ok(new { Message = result });

            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

    }
}
