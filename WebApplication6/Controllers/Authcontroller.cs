using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using WebApplication6.Models;

namespace WebApplication6.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [AllowAnonymous]
    [EnableCors("MyCorsPolicy")]
    public class AuthController : ControllerBase
    {
        private readonly IAuth _authService;

        public AuthController(IAuth authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var token = _authService.Authentication(request.user_id, request.password);
            if (token == null)
            {
                return Unauthorized("Invalid username or password");
            }

            return Ok(new { token });
        }
    }
}
