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
    [ApiController()]
    
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
        [Authorize(Roles ="o")]
        public async Task<ActionResult<IEnumerable<Registration>>> GetRegistrationss()
        {
            return await Task.FromResult(_registrationRepository.readData());
        }

        // GET: api/Registrations/5
        [HttpGet("{id}")]
        [Authorize(Roles = "o")]
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
        [Authorize(Roles = "o")]
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
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<Registration>> PostRegistration(string id, string name, long phonenumber, string role, DateOnly dob, string password, string answer)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(name) || phonenumber == 0 || string.IsNullOrEmpty(role) || dob == default || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(answer))
            {
                return BadRequest("Invalid parameters.");
            }

            var registration = new Registration
            {
                ID = id,
                Name = name,
                PhoneNumber = phonenumber,
                RoleofUser = role,
                D_O_B = dob,
                Password = password,
                Answer = answer,
                Signature = "string"
            };
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
                var updatedRegistration = _registrationRepository.readData().FirstOrDefault(r => r.ID == registration.ID);
                if (updatedRegistration == null)
                {
                    Console.WriteLine("Updated registration not found.");
                    return Ok(registration); // Return status code 200 with the posted data
                }

                // Log the retrieved registration
                Console.WriteLine($"Retrieved registration: ID = {updatedRegistration.ID}, Role = {updatedRegistration.RoleofUser}, Signature = {updatedRegistration.Signature}");

                // Update the registration object with the updated values
                registration = updatedRegistration;
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException?.Message;
                Console.WriteLine($"Database update exception: {innerException}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Database update exception: {innerException}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
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
        public IActionResult Authentication(string user_id, string password)
        {
            var token = _jwtAuth.Authentication(user_id, password);
            if (token == null)
                return Unauthorized();
            return Ok(new { Token = token });
        }
    }
}