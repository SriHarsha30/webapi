using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebApplication6.Models;
using WebApplication6.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Cors;

namespace WebApplication6.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("MyCorsPolicy")]
    public class ForgetController : ControllerBase
    {
        private readonly IRegistrationRepository _registrationRepository;
        private readonly ILogger<ForgetController> _logger;

        public ForgetController(IRegistrationRepository registrationRepository, ILogger<ForgetController> logger)
        {
            _registrationRepository = registrationRepository;
            _logger = logger;
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] PasswordResetRequest request)
        {
            bool isPasswordReset;
            int result = _registrationRepository.LoginChk(request.UserId, null, out isPasswordReset, request.Answer, request.NewPassword);

            _logger.LogInformation("LoginChk result: {Result}, isPasswordReset: {IsPasswordReset}", result, isPasswordReset);

            if (result == 1 && isPasswordReset)
            {
                _logger.LogInformation("Password reset successful for UserId: {UserId}", request.UserId);
                return Ok(new { message = "Password reset successful!" });
            }
            else
            {
                _logger.LogWarning("Invalid answer or user ID for UserId: {UserId}", request.UserId);
                return BadRequest(new { message = "Invalid answer or user ID!" });
            }
        }
    }
}
