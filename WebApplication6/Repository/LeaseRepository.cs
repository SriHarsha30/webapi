using WebApplication6.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication6.Repository
{
    public class LeaseRepository : ILeaseRepository
    {
        private readonly Context _context;

        public LeaseRepository(Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Lease>> GetAllLeasesAsync()
        {
            return await _context.Leases1.ToListAsync();
        }

        public async Task<Lease> GetLeaseByIdAsync(int leaseId)
        {
            return await _context.Leases1.FirstOrDefaultAsync(l => l.LeaseId == leaseId);
        }

        public async Task AddLeaseAsync(Lease lease)
        {
            await _context.Leases1.AddAsync(lease);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateLeaseAsync(Lease lease)
        {
            _context.Leases1.Update(lease);
            await _context.SaveChangesAsync();
        }
    }
}