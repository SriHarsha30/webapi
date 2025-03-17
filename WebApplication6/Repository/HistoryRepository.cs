using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication6.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApplication6.Repository
{
    public class HistoryRepository : IHistoryRepository
    {
        private readonly Context _context;

        public HistoryRepository(Context context)
        {
            _context = context;
        }
      

        public IEnumerable<History> GetTenantHistory(string tenantId)
        {
            return _context.Histories
                .FromSqlRaw("EXEC GetTenantHistory @Tenant_id = {0}", tenantId)
                .ToList();
        }

        public IEnumerable<dynamic> GetTenantHistoryForOwner(string tenantId)
        {
            return _context.Histories
                .FromSqlRaw("EXEC GetTenantHistoryForOwner @Tenant_id = {0}", tenantId)
                .Select(h => new
                {
                    h.Tenant_name,
                    h.Tenant_Phonenumber,
                    h.leased_property_id,
                    h.startTime,
                    h.endTime
                })
                .ToList();
        }
    }
}

