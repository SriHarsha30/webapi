using WebApplication6.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication6.Services
{
    public interface ILeaseService
    {
        Task<(string leaseId, string ownerId)?> CreateLeaseAsync(string tenantId, int propertyId, DateTime startDate, DateTime endDate, string tenantSignature);
        Task<bool> FinalizeLeaseAsync(int leaseId, string ownerId, string ownerSignature);
        Task<Lease> GetLeaseByIdAsync(int leaseId);
        Task<IEnumerable<Lease>> GetLeasesByOwnerAsync(string ownerId);
        Task<IEnumerable<Lease>> GetAllLeasesAsync();
        Task<IEnumerable<Lease>> GetLeasesByTenantAsync(string tenantId);
    }
}