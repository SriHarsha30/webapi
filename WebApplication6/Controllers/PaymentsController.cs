using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WebApplication6.Models;
using WebApplication6.Services;

namespace WebApplication6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly Context _context;
        private readonly IPaymentService _paymentService;

        public PaymentsController(Context context, IPaymentService paymentService)
        {
            _context = context;
            _paymentService = paymentService;
        }

        [HttpGet("GetPaymentsByTenant/{tenantId}")]
        [Authorize(Roles = "t")]
        public async Task<IActionResult> GetPaymentsByTenant(string tenantId)
        {
            var payments = await _paymentService.GetPaymentsByTenantIdAsync(tenantId);

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
            var payments = await _paymentService.GetPaymentsByOwnerid(oid);

            if (payments == null || !payments.Any())
            {
                return NotFound(new { message = "No payments found for the given tenant." });
            }

            return Ok(payments);
        }

        [HttpGet("{id}", Name = "GetPaymentById")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            return Ok(payment);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "o")]
        public async Task<IActionResult> PutPayment(int id, [FromBody] string ownerStatus)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

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

            return Ok(new { message = "Updated successfully." });
        }

        [HttpPost]
        [Authorize(Roles = "t")]
        public async Task<ActionResult<Payment>> PostPayment( string Tenant_Id, int PropertyId, string Status)
        {
            Console.WriteLine("PostPayment method called");

            if ( Tenant_Id== null)
            {
                return BadRequest(new { message = "Invalid payment data." });
            }

            bool tenantAndPropertyExist = await _paymentService.CheckTenantAndPropertyAsync(Tenant_Id,PropertyId);
            if (!tenantAndPropertyExist)
            {
                return BadRequest(new { message = "Tenant or Property does not exist." });
            }

            bool isLeaseConfirmed = false;
            try
            {
                var isLeaseConfirmedParam = new SqlParameter("@IsLeaseConfirmed", SqlDbType.Bit) { Direction = ParameterDirection.Output };
                await _context.Database.ExecuteSqlRawAsync("EXEC CheckLeaseStatus @Tenant_Id, @PropertyId, @IsLeaseConfirmed OUTPUT",
                    new SqlParameter("@Tenant_Id", Tenant_Id),
                    new SqlParameter("@PropertyId", PropertyId),
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

            var property = await _context.Properties.FindAsync(PropertyId);
            if (property == null)
            {
                return NotFound(new { message = "Property not found." });
            }

            var newPayment = new Payment
            {
                Tenant_Id = Tenant_Id,
                PropertyId = PropertyId,
                Amount = property.PriceOfTheProperty,
                Status = Status,
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

            return CreatedAtRoute("GetPaymentById", new { id = newPayment.PaymentID }, newPayment);
        }



        private bool PaymentExists(int id)
        {
            return _context.Payments.Any(e => e.PaymentID == id);
        }
    }
}