using WebApplication6.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication6.Repository
{
    public interface ILeaseRepository
    {
        Task<IEnumerable<Lease>> GetAllLeasesAsync();
        Task<Lease> GetLeaseByIdAsync(int leaseId);
        Task AddLeaseAsync(Lease lease);
        Task UpdateLeaseAsync(Lease lease);
    }
}
