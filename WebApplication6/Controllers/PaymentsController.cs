using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Payment>>> GetPayments()
        {
            return await _context.Payments.ToListAsync();
        }

        // GET: api/Payments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Payment>> GetPayment(int id)
        {
            var payment = await _context.Payments.FindAsync(id);

            if (payment == null)
            {
                return NotFound();
            }

            return payment;
        }

        [HttpGet("GetPaymentsByTenant/{tenantId}")]
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
        public async Task<IActionResult> PutPayment(int id, [FromBody] string ownerStatus)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            payment.Ownerstatus = ownerStatus;

            // If Ownerstatus is true, set Status to true
            if (ownerStatus.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                payment.Status = "true";
            }

            _context.Entry(payment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentExists(id))
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

        // POST: api/Payments
        [HttpPost]
        public async Task<ActionResult<Payment>> PostPayment(Payment payment)
        {
            if (payment == null)
            {
                return BadRequest(new { message = "Invalid payment data." });
            }

            // Check if Tenant_Id and PropertyId exist
            try
            {
                var checkResult = await CheckTenantAndPropertyAsync(payment.Tenant_Id, payment.PropertyId);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }

            // Create a new Payment object to ensure the constructor is called
            var newPayment = new Payment
            {
                Tenant_Id = payment.Tenant_Id,
                PropertyId = payment.PropertyId,
                Amount = payment.Amount,
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
             
            //string notipd = $"a payment was made with id:{payment.PaymentID} of amount :{payment.Amount} ";
            //_context.Database.ExecuteSqlRaw("EXEC InsertIntoNotificcation1 @p0, @p1, @p2", newPayment.Tenant_Id,ownerid, notipd);
            return CreatedAtAction("GetPayment", new { id = newPayment.PaymentID }, newPayment);

        }


        private async Task<bool> CheckTenantAndPropertyAsync(string tenantId, int propertyId)
        {
            var commandText = "EXEC CheckTenantAndProperty @Tenant_Id, @Property_Id";
            var tenantParam = new SqlParameter("@Tenant_Id", tenantId);
            var propertyParam = new SqlParameter("@Property_Id", propertyId);

            try
            {
                var result = await _context.Database.ExecuteSqlRawAsync(commandText, tenantParam, propertyParam);
                return result == 0; // If no error is raised, the procedure returns 0
            }
            catch (SqlException ex)
            {
                // Handle specific error messages if needed
                if (ex.Message.Contains("Tenant does not exist"))
                {
                    throw new Exception("Tenant does not exist.");
                }
                else if (ex.Message.Contains("Property does not exist"))
                {
                    throw new Exception("Property does not exist.");
                }
                throw;
            }
        }

        // DELETE: api/Payments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PaymentExists(int id)
        {
            return _context.Payments.Any(e => e.PaymentID == id);
        }


    }
}
