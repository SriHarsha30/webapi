using WebApplication6.Models;

namespace WebApplication6.Services
{
    public interface IPaymentService
    {
        Task<IEnumerable<Payment>> GetPaymentsByTenantIdAsync(string tenantId);
        Task<IEnumerable<Payment>> GetPaymentsByOwnerid(string oid);
        Task<bool> CheckTenantAndPropertyAsync(string tenantId, int propertyId);
    }
}
