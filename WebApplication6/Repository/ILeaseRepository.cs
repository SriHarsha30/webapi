using WebApplication6.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication6.Repository
{
    public interface ILeaseRepository
    {
        IEnumerable<Lease> GetAllLeases();
        Lease GetLeaseById(int leaseId);
        void AddLease(Lease lease);
        void UpdateLease(Lease lease);
    }
}
