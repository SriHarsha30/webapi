using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebApplication6.Models;
using WebApplication6.Repository;

namespace WebApplication6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationsController : ControllerBase
    {
        private readonly IRegistrationRepository _registrationRepository;
        private readonly Context _context;
        private readonly IAuth _jwtAuth;


        public RegistrationsController(IRegistrationRepository registrationRepository, Context context,IAuth jwtAuth)
        {
            _registrationRepository = registrationRepository;
            _context = context;
            _jwtAuth = jwtAuth;
        }

        // GET: api/Registrations
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Registration>>> GetRegistrationss()
        {
            return await Task.FromResult(_registrationRepository.readData());
        }

        // GET: api/Registrations/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Registration>> GetRegistration(string id)
        {
            var registration = _registrationRepository.readData().FirstOrDefault(r => r.ID == id);

            if (registration == null)
            {
                return NotFound();
            }

            return await Task.FromResult(registration);
        }

        // PUT: api/Registrations/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutRegistration(string id, Registration registration)
        {
            if (id != registration.ID)
            {
                return BadRequest();
            }

            try
            {
                _registrationRepository.Insertion(registration);
                _context.Database.ExecuteSqlRaw("EXEC InsertIntoNotificcation1 @p0, @p1, @p2", registration.ID, registration.ID, "made a change to the registration");

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_registrationRepository.readData().Any(e => e.ID == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Registrations
        [HttpPost]
        //[Authorize]
        public async Task<ActionResult<Registration>> PostRegistration(Registration registration)
        {
            if (registration == null)
            {
                return BadRequest("Registration object is null.");
            }

            try
            {
                bool insertionResult = _registrationRepository.Insertion(registration);
                if (!insertionResult)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error inserting registration.");
                }

                // Log before calling the stored procedure
                Console.WriteLine($"Before stored procedure: ID = {registration.ID}, Role = {registration.RoleofUser}");

                // Call the stored procedure to update the ID based on the Role
                var roleParam = new SqlParameter("@RoleofUser", registration.RoleofUser);
                var idParam = new SqlParameter("@ID", registration.ID);
                await _context.Database.ExecuteSqlRawAsync("EXEC UpdateID @ID, @RoleofUser", idParam, roleParam);

                // Log after calling the stored procedure
                Console.WriteLine($"After stored procedure: ID = {registration.ID}, Role = {registration.RoleofUser}");

                // Retrieve the updated registration
                // Retrieve the updated registration
                var updatedRegistration = _registrationRepository.readData().FirstOrDefault(r => r.ID == registration.ID);
                if (updatedRegistration == null)
                {
                    Console.WriteLine("Updated registration not found.");
                    return Ok(registration); // Return status code 200 with the posted data
                }
                // Log the retrieved registration
                Console.WriteLine($"Retrieved registration: ID = {updatedRegistration.ID}, Role = {updatedRegistration.RoleofUser}");

                // Update the registration object with the updated values
                registration = updatedRegistration;
            }
            catch (DbUpdateException ex)
            {
                if (_registrationRepository.readData().Any(e => e.ID == registration.ID))
                {
                    return Conflict();
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, $"Database update exception: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An unexpected error occurred: {ex.Message}");
            }

            return CreatedAtAction("GetRegistration", new { id = registration.ID }, registration);
        }

        //// DELETE: api/Registrations/5
        //[HttpDelete("{id}")]
        //[Authorize]
        //public async Task<IActionResult> DeleteRegistration(string id)
        //{
        //    var registration = _registrationRepository.readData().FirstOrDefault(r => r.ID == id);
        //    if (registration == null)
        //    {
        //        return NotFound();
        //    }

        //    _registrationRepository.readData().Remove(registration);
        //    await _registrationRepository.SaveChangesAsync();

        //    return NoContent();
        //}


        [AllowAnonymous]
        // POST api/<UsersController>/authentication
        [HttpPost("authentication")]
        public IActionResult Authentication([FromBody] Registration user)
        {
            var token = _jwtAuth.Authentication(user.ID, user.Password);
            if (token == null)
                return Unauthorized();
            return Ok(new { Token = token });
        }
    }
}