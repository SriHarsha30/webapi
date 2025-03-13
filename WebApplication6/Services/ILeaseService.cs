using WebApplication6.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication6.Services
{
    public interface ILeaseService
    {
        (string leaseId, string ownerId)? CreateLease(string tenantId, int propertyId, DateTime startDate, DateTime endDate, string tenantSignature);
        bool FinalizeLease(int leaseId, string ownerId, string ownerSignature);
        IEnumerable<Lease> GetLeasesByOwner(string ownerId);
        Lease GetLeaseById(int leaseId);
        IEnumerable<Lease> GetAllLeases();
    }
}