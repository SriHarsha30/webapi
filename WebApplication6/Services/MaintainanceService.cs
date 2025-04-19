using Microsoft.EntityFrameworkCore;
using WebApplication6.Models;
using WebApplication6.Repository;

namespace WebApplication6.Services
{
    public class MaintainanceService : IMaintainanceService

    {

        private readonly IMaintainanceRepository _repository;
        private readonly Context _context;

        public MaintainanceService(IMaintainanceRepository repository, Context context)

        {
            _context = context;
            _repository = repository;

        }



        public void InsertMaintainance(int propertyId, string tenantId, string description, String status, string imagePath)
        {
            try
            {
                if (!_repository.PropertyExists(propertyId))
                {
                    throw new Exception($"Property with ID {propertyId} does not exist.");
                }

                if (!_repository.TenantExists(tenantId))
                {
                    throw new Exception($"Tenant with ID {tenantId} does not exist.");
                }

                if (!_repository.LeaseSigned(propertyId))
                {
                    throw new Exception($"Property with ID {tenantId} has not been taken to lease.");
                }
                _repository.InsertMaintainance(propertyId, tenantId, description, status, imagePath);
            }
            catch (Exception ex)
            {
                // Log the exception details
                // Example: _logger.LogError(ex, "Error in service layer while inserting maintenance");
                throw new Exception("An (ser) error occurred while inserting the maintenance request.", ex);
            }
        }

        public List<Maintainance> ViewTenantRequests(string tenantId)
        {
            try
            {
                if (!_repository.TenantExists(tenantId))
                {
                    throw new Exception($"Tenant with ID {tenantId} does not exist.");
                }
                return _repository.ViewTenantRequests(tenantId);
            }
            catch (Exception ex)
            {
                // Log the exception details
                // Example: _logger.LogError(ex, "Error in service layer while viewing tenant requests");
                throw new Exception("An (ser) error occurred while viewing tenant requests.", ex);
            }
        }

        public List<Maintainance> ViewOwnerRequests(string userId)
        {
            try
            {
                if (!_repository.TenantExists(userId))
                {
                    throw new Exception($"Owner with ID {userId} does not exist.");
                }
                return _repository.ViewOwnerRequests(userId);
            }
            catch (Exception ex)
            {
                // Log the exception details
                //_logger.LogError(ex, "Error in service layer while viewing owner requests");
                throw new Exception("An (ser) error occurred while viewing owner requests.", ex);
            }
        }

        public bool UpdateStatus(int requestId, string newStatus)
        {

            try

            {

                return _repository.UpdateStatus(requestId, newStatus);
                //_context.Database.ExecuteSqlRaw("EXEC InsertIntoNotification1 @p0, @p1, @p2", tenantId, ownerId, "a Maintainence request was made");


            }

            catch (Exception ex)

            {

                // Handle exception (log it, rethrow it, etc.)

                throw new Exception("An error occurred while updating the status.", ex);

            }

        }

        public List<Maintainance> ViewallOwnerRequests(string userId)
        {
            try
            {
                if (!_repository.TenantExists(userId))
                {
                    throw new Exception($"Owner with ID {userId} does not exist.");
                }
                return _repository.ViewallOwnerRequests(userId);
            }
            catch (Exception ex)
            {
                throw new Exception("An (ser) error occurred while viewing owner requests.", ex);
            }
        }

    }

}
