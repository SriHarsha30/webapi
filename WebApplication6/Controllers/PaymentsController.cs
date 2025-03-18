using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebApplication6.Models;
using WebApplication6.Repository;

namespace WebApplication6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]


    public class PaymentsController : ControllerBase
    {
        private readonly Context _context;
        private readonly IPaymentRepository _paymentRepository;

        public PaymentsController(Context context, IPaymentRepository paymentRepository)
        {
            _context = context;
            _paymentRepository = paymentRepository;
        }

        // GET: api/Payments
        //[HttpGet]
        //[Authorize(Roles = "o,t")]
        //public async Task<ActionResult<IEnumerable<Payment>>> GetPayments()
        //{
        //    return await _context.Payments.ToListAsync();
        //}

        // GET: api/Payments/5
        //[HttpGet("{id}")]
        //[Authorize(Roles = "o,t")]
        //public async Task<ActionResult<Payment>> GetPayment(int id)
        //{
        //    var payment = await _context.Payments.FindAsync(id);

        //    if (payment == null)
        //    {
        //        return NotFound();
        //    }

        //    return payment;
        //}

        [HttpGet("GetPaymentsByTenant/{tenantId}")]
        [Authorize(Roles = "t")]
        public async Task<IActionResult> GetPaymentsByTenant(string tenantId)
        {
            var payments = await _paymentRepository.GetPaymentsByTenantIdAsync(tenantId);

            if (payments == null || !payments.Any())
            {
                return NotFound(new { message = "No payments found for the given tenant." });
            }

            return Ok(payments);
        }

        [HttpGet("GetPaymentsByOwnerid/{oid}")]
        [Authorize(Roles = "o")]
        public async Task<IActionResult> GetPaymentsByOwnerid(string oid)
        {

            var payments = await _paymentRepository.GetPaymentsByOwnerid(oid);

            if (payments == null || !payments.Any())
            {
                return NotFound(new { message = "No payments found for the given tenant." });
            }

            return Ok(payments);
        }

        // PUT: api/Payments/5
        [HttpPut("{id}")]
        [Authorize(Roles = "o")]
        public async Task<IActionResult> PutPayment(int id, [FromBody] string ownerStatus)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            // Use a stored procedure to update the payment status
            var commandText = "EXEC UpdatePaymentStatus @PaymentID, @OwnerStatus";
            var parameters = new[]
            {
            new SqlParameter("@PaymentID", id),
            new SqlParameter("@OwnerStatus", ownerStatus)
             };

            try
            {
                await _context.Database.ExecuteSqlRawAsync(commandText, parameters);
            }
            catch (SqlException ex)
            {
                return NotFound(new { message = ex.Message });
            }

            // If Ownerstatus is true, update the lease status
            if (ownerStatus.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                var lease = await _context.Leases1.FirstOrDefaultAsync(l => l.Property_Id == payment.PropertyId);
                if (lease != null)
                {
                    lease.Lease_status = true;
                    _context.Entry(lease).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
            }

            return NoContent();
        }

        //POST: api/Payments
        [HttpPost]
       [Authorize(Roles = "t")]
        public async Task<ActionResult<Payment>> PostPayment(Payment payment)
        {
            if (payment == null)
            {
                return BadRequest(new { message = "Invalid payment data." });
            }

            // Check if Tenant_Id and PropertyId exist
            bool tenantAndPropertyExist = await CheckTenantAndPropertyAsync(payment.Tenant_Id, payment.PropertyId);
            if (!tenantAndPropertyExist)
            {
                return BadRequest(new { message = "Tenant or Property does not exist." });
            }
            // Check if Tenant_Id and PropertyId exist and if the lease is confirmed
            bool isLeaseConfirmed = false;
            try
            {
                var isLeaseConfirmedParam = new SqlParameter("@IsLeaseConfirmed", SqlDbType.Bit) { Direction = ParameterDirection.Output };
                await _context.Database.ExecuteSqlRawAsync("EXEC CheckLeaseStatus @Tenant_Id, @PropertyId, @IsLeaseConfirmed OUTPUT",
                    new SqlParameter("@Tenant_Id", payment.Tenant_Id),
                    new SqlParameter("@PropertyId", payment.PropertyId),
                    isLeaseConfirmedParam);

                isLeaseConfirmed = (bool)isLeaseConfirmedParam.Value;

            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }


            if (!isLeaseConfirmed)
            {
                return BadRequest(new { message = "Lease is not yet confirmed." });
            }

            // Fetch the amount from the property
            var property = await _context.Properties.FindAsync(payment.PropertyId);
            if (property == null)
            {
                return NotFound(new { message = "Property not found." });
            }

            // Create a new Payment object to ensure the constructor is called
            var newPayment = new Payment
            {
                Tenant_Id = payment.Tenant_Id,
                PropertyId = payment.PropertyId,
                Amount = property.PriceOfTheProperty, // Fetch the amount from the property
                Status = payment.Status,
                Ownerstatus = "Active"
            };

            _context.Payments.Add(newPayment);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PaymentExists(newPayment.PaymentID))
                {
                    return Conflict(new { message = "Payment already exists." });
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPayment", new { id = newPayment.PaymentID }, newPayment);

        }



        private async Task<bool> CheckTenantAndPropertyAsync(string tenantId, int propertyId)
        {
            var commandText = "EXEC CheckTenantAndProperty @Tenant_Id, @Property_Id";
            var tenantParam = new SqlParameter("@Tenant_Id", tenantId);
            var propertyParam = new SqlParameter("@Property_Id", propertyId);

            try
            {
                await _context.Database.ExecuteSqlRawAsync(commandText, tenantParam, propertyParam);
                return true; // If no error is raised, both tenant and property exist
            }
            catch (SqlException ex)
            {
                // Handle specific error messages if needed
                if (ex.Message.Contains("Tenant does not exist"))
                {
                    return false;
                }
                else if (ex.Message.Contains("Property does not exist"))
                {
                    return false;
                }
                throw;
            }
        }

        // DELETE: api/Payments/5
        //[HttpDelete("{id}")]
        //[Authorize(Roles = "a")]
        //public async Task<IActionResult> DeletePayment(int id)
        //{
        //    var payment = await _context.Payments.FindAsync(id);
        //    if (payment == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Payments.Remove(payment);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}


        private bool PaymentExists(int id)
        {
            return _context.Payments.Any(e => e.PaymentID == id);
        }


    }
}
