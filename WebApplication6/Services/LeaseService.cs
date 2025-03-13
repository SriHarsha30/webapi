//using WebApplication6.Models;
//using WebApplication6.Repository;
//using Microsoft.EntityFrameworkCore;
//using WebApplication6.Services;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace testing_Web_Api.Services
//{
//    public class LeaseService : ILeaseService
//    {
//        private readonly ILeaseRepository _leaseRepository;
//        private readonly IRegistrationRepository _registrationRepository;
//        private readonly IProperty _propRepository;
//        private readonly INotificationService _notificationService;
//        private readonly Context _context;

//        public LeaseService(ILeaseRepository leaseRepository,
//                            IRegistrationRepository registrationRepository,
//                            IProperty propRepository,
//                            INotificationService notificationService,
//                            Context context)
//        {
//            _leaseRepository = leaseRepository;
//            _registrationRepository = registrationRepository;
//            _propRepository = propRepository;
//            _notificationService = notificationService;
//            _context = context;
//        }

//        public async Task<(string leaseId, string ownerId)?> CreateLeaseAsync(string tenantId, int propertyId, DateTime startDate, DateTime endDate, string tenantSignature)
//        {
//            var existingLeases = await _leaseRepository.GetAllLeasesAsync();
//            var existingLease = existingLeases.FirstOrDefault(l => l.Property_Id == propertyId && l.Lease_status == true);
//            if (existingLease != null)
//            {
//                return null;
//            }

//            //var tenant = await _registrationRepository.readData();
//            var tenant = _registrationRepository.readData().FirstOrDefault(i=> i.ID == tenantId);
//            if (tenant?.Signature != tenantSignature)
//            {
//                return null;
//            }

//            var property = _propRepository.ViewData().FirstOrDefault(i => i.Property_Id == propertyId);
//            if (property == null)
//            {
//                throw new KeyNotFoundException("Property not found.");
//            }

//            var lease = new Lease
//            {
//                ID = tenantId,
//                Property_Id = propertyId,
//                StartDate = startDate,
//                EndDate = endDate,
//                Tenant_Signature = true,
//                Owner_Signature = false,
//                Lease_status = false
//            };

//            await _leaseRepository.AddLeaseAsync(lease);
//            await _context.Database.ExecuteSqlRawAsync("EXEC InsertIntoNotification1 @p0, @p1, @p2",
//                                             tenantId, property.Owner_Id, "tenant signed successfully");

//            return (lease.LeaseId.ToString(), property.Owner_Id);
//        }

//        //public async Task<bool> FinalizeLeaseAsync(int leaseId, string ownerId, string ownerSignature)
//        //{
//        //    var lease = await _leaseRepository.GetLeaseByIdAsync(leaseId);
//        //    if (lease == null)
//        //    {
//        //        throw new KeyNotFoundException("Lease not found.");
//        //    }

//        //    var owner = _registrationRepository.readData().FirstOrDefault(i => i.ID == ownerId);
//        //    if (owner?.Signature != ownerSignature)
//        //    {
//        //        return false;
//        //    }

//        //    lease.Owner_Signature = true;
//        //    lease.Lease_status = true;

//        //    await _leaseRepository.UpdateLeaseAsync(lease);
//        //    await _context.Database.ExecuteSqlRawAsync("EXEC InsertIntoNotification1 @p0, @p1, @p2",
//        //                                      ownerId, lease.ID, "owner signed successfully");
//        //    return true;
//        //}

//        public bool FinalizeLease(int leaseId, string ownerId, string ownerSignature)
//        {
//            var lease = _leaseRepository.GetLeaseById(leaseId);
//            if (lease == null)
//            {
//                throw new KeyNotFoundException("Lease not found.");
//            }


//            var owner = _registrationRepository.GetById(ownerId);
//            if (owner?.Signature != ownerSignature)
//            {
//                return false;
//            }


//            lease.Owner_Signature = true;
//            lease.Lease_status = true;

//            _leaseRepository.UpdateLease(lease);
//            _context.Database.ExecuteSqlRaw("EXEC InsertIntoNotificcation1 @p0, @p1, @p2",
//                                              ownerId, lease.ID, "owner signed successfully");
//            return true;
//        }


//        public async Task<IEnumerable<Lease>> GetLeasesByOwnerAsync(string ownerId)
//        {
//            var properties = await _propRepository.GetPropertiesByOwnerAsync(ownerId);
//            if (!properties.Any())
//            {
//                return Enumerable.Empty<Lease>();
//            }

//            var propertyIds = properties.Select(p => p.Property_Id).ToList();
//            var leases = await _leaseRepository.GetAllLeasesAsync();
//            return leases.Where(l => propertyIds.Contains((int)l.Property_Id)).ToList();
//        }

//        public async Task<Lease> GetLeaseByIdAsync(int leaseId)
//        {
//            var lease = await _leaseRepository.GetLeaseByIdAsync(leaseId);
//            if (lease == null)
//            {
//                throw new KeyNotFoundException($"Lease with ID {leaseId} not found.");
//            }
//            return lease;
//        }

//        public async Task<IEnumerable<Lease>> GetAllLeasesAsync()
//        {
//            return await _leaseRepository.GetAllLeasesAsync();
//        }
//    }
//}