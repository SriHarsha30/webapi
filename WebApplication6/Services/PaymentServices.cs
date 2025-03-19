using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebApplication6.Exceptions;
using WebApplication6.Models;

namespace WebApplication6.Services
{
    public class PaymentServices : IPaymentService
    {
        private readonly Context _context;

        public PaymentServices(Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByTenantIdAsync(string tenantId)
        {
            var commandText = "EXEC GetPaymentsByTenantId @Tenant_Id";
            var tenantParam = new SqlParameter("@Tenant_Id", tenantId);

            return await _context.Payments
                                 .FromSqlRaw(commandText, tenantParam)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByOwnerid(string oid)
        {
            var commandText = "EXEC GetPaymentsByOwnerId @Owner_Id";
            var tenantParam = new SqlParameter("@Owner_Id", oid);

            return await _context.Payments
                                 .FromSqlRaw(commandText, tenantParam)
                                 .ToListAsync();
        }

        public async Task<bool> CheckTenantAndPropertyAsync(string tenantId, int propertyId)
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
                    throw new TenentnotfoundException("Tenant does not exist");
                }
                else if (ex.Message.Contains("Property does not exist"))
                {
                    throw new Propertynotfoundexception("PropertyNotFound");
                }
                throw;
            }
        }
    }
}