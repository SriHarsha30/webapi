using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebApplication6.Repository;
using WebApplication6.Models;
using Microsoft.AspNetCore.Authorization;
using WebApplication6.Exceptions;
using Microsoft.AspNetCore.Cors;

namespace WebApplication5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("MyCorsPolicy")]
    public class PropertiesController : ControllerBase
    {
        private readonly IProperty _repository;
        private readonly Context _context;
        private readonly IRegistrationRepository _registrationRepository;

        public PropertiesController(IProperty repository, Context context, IRegistrationRepository registrationRepository)
        {
            _repository = repository;
            _context = context;
            _registrationRepository = registrationRepository;
        }

        // GET: api/Properties
        [HttpGet]
        //[Authorize(Roles = "o,t")]
        public async Task<ActionResult<IEnumerable<Property>>> GetProperties()
        {
            var properties = await Task.FromResult(_repository.ViewData());

            foreach (var property in properties)
            {
                // Call the stored procedure to get owner details
                var ownerIdParam = new SqlParameter("@Owner_Id", property.Owner_Id);
                var ownerNameParam = new SqlParameter("@Owner_Name", System.Data.SqlDbType.NVarChar, 100) { Direction = System.Data.ParameterDirection.Output };
                var ownerPhoneNumberParam = new SqlParameter("@Owner_PhoneNumber", System.Data.SqlDbType.NVarChar, 15) { Direction = System.Data.ParameterDirection.Output };

                await _context.Database.ExecuteSqlRawAsync("EXEC GetOwnerDetailsByOwnerId @Owner_Id, @Owner_Name OUTPUT, @Owner_PhoneNumber OUTPUT",
                    ownerIdParam, ownerNameParam, ownerPhoneNumberParam);

                property.Owner_Name = ownerNameParam.Value != DBNull.Value ? (string)ownerNameParam.Value : null;
                property.Owner_PhoneNumber = ownerPhoneNumberParam.Value != DBNull.Value ? Convert.ToInt64(ownerPhoneNumberParam.Value) : (long?)null;
            }

            return properties;
        }

        // GET: api/Properties/5
        [HttpGet("{id}")]
        //[Authorize(Roles = "o")]
        public async Task<ActionResult<Property>> GetProperty(int id)
        {
            var property = await _repository.FindAsync(id);
            try
            {
                if (property == null)
                {
                    throw new Propertynotfoundexception();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"error: {ex.Message}");
            }

            // Call the stored procedure to get owner details
            var ownerIdParam = new SqlParameter("@Owner_Id", property.Owner_Id);
            var ownerNameParam = new SqlParameter("@Owner_Name", System.Data.SqlDbType.NVarChar, 100) { Direction = System.Data.ParameterDirection.Output };
            var ownerPhoneNumberParam = new SqlParameter("@Owner_PhoneNumber", System.Data.SqlDbType.NVarChar, 15) { Direction = System.Data.ParameterDirection.Output };

            await _context.Database.ExecuteSqlRawAsync("EXEC GetOwnerDetailsByOwnerId @Owner_Id, @Owner_Name OUTPUT, @Owner_PhoneNumber OUTPUT",
                ownerIdParam, ownerNameParam, ownerPhoneNumberParam);

            property.Owner_Name = ownerNameParam.Value != DBNull.Value ? (string)ownerNameParam.Value : null;
            property.Owner_PhoneNumber = ownerPhoneNumberParam.Value != DBNull.Value ? Convert.ToInt64(ownerPhoneNumberParam.Value) : (long?)null;

            return property;
        }

        // GET: api/Properties/owner/{ownerId}
        [HttpGet("owner/{ownerId}")]
        //[Authorize(Roles = "o,t")]
        public async Task<ActionResult<IEnumerable<Property>>> GetPropertiesByOwnerId(string ownerId)
        {
            var ownerIdParam = new SqlParameter("@OwnerId", ownerId);
            var properties = await _context.Properties
                .FromSqlRaw("EXEC GetPropertiesByOwner @OwnerId", ownerIdParam)
                .ToListAsync();

            if (properties == null || !properties.Any())
            {
                return NotFound($"No properties found for owner ID: {ownerId}");
            }

            foreach (var property in properties)
            {
                // Call the stored procedure to get owner details
                var ownerIdParamDetail = new SqlParameter("@Owner_Id", property.Owner_Id);
                var ownerNameParam = new SqlParameter("@Owner_Name", System.Data.SqlDbType.NVarChar, 100) { Direction = System.Data.ParameterDirection.Output };
                var ownerPhoneNumberParam = new SqlParameter("@Owner_PhoneNumber", System.Data.SqlDbType.NVarChar, 15) { Direction = System.Data.ParameterDirection.Output };

                await _context.Database.ExecuteSqlRawAsync("EXEC GetOwnerDetailsByOwnerId @Owner_Id, @Owner_Name OUTPUT, @Owner_PhoneNumber OUTPUT",
                    ownerIdParamDetail, ownerNameParam, ownerPhoneNumberParam);

                property.Owner_Name = ownerNameParam.Value != DBNull.Value ? (string)ownerNameParam.Value : null;
                property.Owner_PhoneNumber = ownerPhoneNumberParam.Value != DBNull.Value ? Convert.ToInt64(ownerPhoneNumberParam.Value) : (long?)null;
            }

            return properties;
        }

        // PUT: api/Properties/5
        [HttpPut("{id}")]
        //[Authorize(Roles = "o")]
        public async Task<IActionResult> PutProperty(int id, Property property)
        {
            if (id != property.Property_Id)
            {
                return BadRequest();
            }

            _repository.Update(property);

            try
            {
                await _repository.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_repository.Exists(id))
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

        [HttpPost]
        //[Authorize(Roles = "o")]
        public async Task<ActionResult<Property>> PostProperty([FromForm] string address, [FromForm] string description, [FromForm] string owner_Id, [FromForm] bool availableStatus, [FromForm] string owner_Signature, [FromForm] decimal Price)
        {
            var property = new Property
            {
                Address = address,
                Description = description,
                Owner_Id = owner_Id,
                AvailableStatus = availableStatus,
                Owner_Signature = owner_Signature,
                PriceOfTheProperty = Price
            };

            var tenant = _registrationRepository.readData().FirstOrDefault(i => i.ID == owner_Id);

            if (tenant == null)
            {
                return BadRequest("Owner not found.");
            }

            if (tenant.Signature != owner_Signature)
            {
                return BadRequest("Invalid owner signature.");
            }

            _repository.Insert(property);
            await _repository.SaveChangesAsync();

            return CreatedAtAction("GetProperty", new { id = property.Property_Id }, property);
        }

        // DELETE: api/Properties/5
        [HttpDelete("{id}")]
        //[Authorize(Roles = "o")]
        public async Task<IActionResult> DeleteProperty(int id)
        {
            var property = await _repository.FindAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            _repository.DeleteProp(property);
            await _repository.SaveChangesAsync();

            return NoContent();
        }
    }
}