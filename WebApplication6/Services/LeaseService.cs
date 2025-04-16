using WebApplication6.Models;
using WebApplication6.Repository;
using Microsoft.EntityFrameworkCore;
using WebApplication6.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using WebApplication6.Exceptions;

namespace WebApplication6.Services
{
    public class LeaseService : ILeaseService
    {
        private readonly ILeaseRepository _leaseRepository;
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IProperty _propRepository;
        private readonly INotificationService _notificationService;
        private readonly IPaymentRepository _paymentRepository;
        private readonly Context _context;

        public LeaseService(ILeaseRepository leaseRepository,
                            IRegistrationRepository registrationRepository,
                            IProperty propRepository,
                            INotificationService notificationService,
                            IPaymentRepository paymentRepository,
                            Context context)
        {
            _leaseRepository = leaseRepository;
            _registrationRepository = registrationRepository;
            _propRepository = propRepository;
            _notificationService = notificationService;
            _paymentRepository = paymentRepository;
            _context = context;
        }

        
        public async Task<(string leaseId, string ownerId)?> CreateLeaseAsync(string tenantId, int propertyId, DateTime startDate, DateTime endDate, string tenantSignature)
        {
            if (endDate <= startDate)
            {
                throw new InvalidDateRangeException();
            }

            var existingLease = (await _leaseRepository.GetAllLeasesAsync())
                .FirstOrDefault(l => l.Property_Id == propertyId && l.Lease_status == true);
            if (existingLease != null)
            {
                return null;
            }

            var tenant = (_registrationRepository.readData()).FirstOrDefault(i => i.ID == tenantId);
            if (tenant?.Signature != tenantSignature)
            {
                return null;
            }

            var property = _propRepository.ViewData().FirstOrDefault(i => i.Property_Id == propertyId);
            if (property == null)
            {
                throw new KeyNotFoundException("Property not found.");
            }

            var lease = new Lease
            {
                ID = tenantId,
                Property_Id = propertyId,
                StartDate = startDate,
                EndDate = endDate,
                Tenant_Signature = true,
                Owner_Signature = false,
                Lease_status = false
            };

            await _leaseRepository.AddLeaseAsync(lease);
            await _context.Database.ExecuteSqlRawAsync("EXEC InsertIntoNotificcation1 @p0, @p1, @p2",
                tenantId, property.Owner_Id, "tenant signed successfully");

            return (lease.LeaseId.ToString(), property.Owner_Id);
        }

       
        public async Task<bool> FinalizeLeaseAsync(int leaseId, string ownerId, string ownerSignature)
        {
            var lease = await _leaseRepository.GetLeaseByIdAsync(leaseId);
            if (lease == null)
            {
                throw new KeyNotFoundException("Lease not found.");
            }

            var owner = (_registrationRepository.readData()).FirstOrDefault(i => i.ID == ownerId);
            if (owner?.Signature != ownerSignature)
            {
                return false;
            }

            lease.Owner_Signature = true;
            lease.Lease_status = false;

            await _leaseRepository.UpdateLeaseAsync(lease);
            await _context.Database.ExecuteSqlRawAsync("EXEC InsertIntoNotificcation1 @p0, @p1, @p2",
                ownerId, lease.ID, "owner signed successfully and the lease gets updated when the payment gets finalized");

            var property = _propRepository.ViewData().FirstOrDefault(i => i.Owner_Id == ownerId);
            if (property != null)
            {
                property.AvailableStatus = false;
                _propRepository.Update(property);
            }

            return true;
        }

        
        public async Task<IEnumerable<Lease>> GetLeasesByOwnerAsync(string ownerId)
        {
            var properties = await _context.Properties
                .FromSqlRaw("EXEC GetPropertiesByOwner @OwnerId", new SqlParameter("@OwnerId", ownerId))
                .ToListAsync();

            if (!properties.Any())
            {
                return Enumerable.Empty<Lease>();
            }

            var propertyIds = properties.Select(p => p.Property_Id).ToList();
            var leases = await _leaseRepository.GetAllLeasesAsync();
            return leases.Where(l => propertyIds.Contains((int)l.Property_Id)).ToList();
        }

        
        public async Task<Lease> GetLeaseByIdAsync(int leaseId)
        {
            var lease = await _leaseRepository.GetLeaseByIdAsync(leaseId);
            if (lease == null)
            {
                throw new KeyNotFoundException($"Lease with ID {leaseId} not found.");
            }
            return lease;
        }

       
        public async Task<IEnumerable<Lease>> GetAllLeasesAsync()
        {
            return await _leaseRepository.GetAllLeasesAsync();
        }

        public async Task<IEnumerable<Lease>> GetLeasesByTenantAsync(string tenantId)
        {
            var leases = await _leaseRepository.GetLeasesByTenantIdAsync(tenantId);

            if (!leases.Any())
            {
                throw new KeyNotFoundException($"No leases found for tenant ID {tenantId}.");
            }

            return leases;
        }

    }
}
