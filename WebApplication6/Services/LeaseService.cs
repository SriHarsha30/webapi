using WebApplication6.Models;
using WebApplication6.Repository;
using Microsoft.EntityFrameworkCore;
using WebApplication6.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace testing_Web_Api.Services
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


        public (string leaseId, string ownerId)? CreateLease(string tenantId, int propertyId, DateTime startDate, DateTime endDate, string tenantSignature)
        {
            if (endDate <= startDate)
            {
                throw new ArgumentException("End date must be greater than start date.");
            }

            var existingLease = _leaseRepository.GetAllLeases()
                .FirstOrDefault(l => l.Property_Id == propertyId && l.Lease_status == true);
            if (existingLease != null)
            {
                return null;
            }


            var tenant = _registrationRepository.readData().FirstOrDefault(i => i.ID == tenantId);
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
            
                _leaseRepository.AddLease(lease);
            _context.Database.ExecuteSqlRaw("EXEC InsertIntoNotificcation1 @p0, @p1, @p2", tenantId, property.Owner_Id, "tenant signed successfully");

            return (lease.LeaseId.ToString(), property.Owner_Id);
        }

        public bool FinalizeLease(int leaseId, string ownerId, string ownerSignature)
        {

            var lease = _leaseRepository.GetLeaseById(leaseId);
            if (lease == null)
            {
                throw new KeyNotFoundException("Lease not found.");
            }


            var owner = _registrationRepository.readData().FirstOrDefault(i => i.ID == ownerId);
            if (owner?.Signature != ownerSignature)
            {
                return false;
            }


            lease.Owner_Signature = true;
    //        string ownerStatus = _context.Database.ExecuteSqlRaw(
    //    "EXEC GetOwnerStatusByLeaseId @p0", leaseId
    //).ToString();
            
    //        if (ownerStatus != "true")
    //        {
    //            return false; 
    //        }

            lease.Lease_status = false;

            _leaseRepository.UpdateLease(lease);
            _context.Database.ExecuteSqlRaw("EXEC InsertIntoNotificcation1 @p0, @p1, @p2", ownerId, lease.ID, "owner signed successfully and the lease gets updated when the payment gets finallised");

            var property = _propRepository.ViewData().FirstOrDefault(i => i.Owner_Id == ownerId);
            if (property != null)
            {
                property.AvailableStatus = false;
                _propRepository.Update(property);
            }
            return true;

        }


        public IEnumerable<Lease> GetLeasesByOwner(string ownerId)
        {
            var properties = _context.Properties
                .FromSqlRaw("EXEC GetPropertiesByOwner @OwnerId", new SqlParameter("@OwnerId", ownerId))
                .ToList();

            if (properties == null || !properties.Any())
            {
                return Enumerable.Empty<Lease>();
            }
            var propertyIds = properties.Select(p => p.Property_Id).ToList();

            return _leaseRepository.GetAllLeases().Where(l => propertyIds.Contains((int)l.Property_Id)).ToList();
        }


        public Lease GetLeaseById(int leaseId)
        {
            var lease = _leaseRepository.GetLeaseById(leaseId);
            if (lease == null)
            {
                throw new KeyNotFoundException($"Lease with ID {leaseId} not found.");
            }
            return lease;
        }

        public IEnumerable<Lease> GetAllLeases()
        {
            return _leaseRepository.GetAllLeases();
        }
    }
}