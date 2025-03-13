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

        public IEnumerable<Lease> GetAllLeases()
        {
            return _context.Leases1.ToList();
        }

        public Lease GetLeaseById(int leaseId)
        {
            return _context.Leases1.FirstOrDefault(l => l.LeaseId == leaseId);
        }

        public void AddLease(Lease lease)
        {
            _context.Leases1.Add(lease);
            _context.SaveChanges();
        }

        public void UpdateLease(Lease lease)
        {
            _context.Leases1.Update(lease);
            _context.SaveChanges();
        }
    }
}