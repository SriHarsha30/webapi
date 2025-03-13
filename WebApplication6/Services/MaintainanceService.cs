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



        public void InsertMaintainance(int requestId, int propertyId, string tenantId, string description, string status, string imagePath)

        {

            try

            {

                // Check if PropertyId exists

                if (!_repository.PropertyExists(propertyId))

                {

                    throw new Exception($"Property with ID {propertyId} does not exist.");

                }

                // Check if TenantId exists

                if (!_repository.TenantExists(tenantId))

                {

                    throw new Exception($"Tenant with ID {tenantId} does not exist.");

                }

                // Insert the maintenance request

                _repository.InsertMaintainance(requestId, propertyId, tenantId, description, status, imagePath);
                //_context.Database.ExecuteSqlRaw("EXEC InsertIntoNotificcation1 @p0, @p1, @p2", tenantId, ownerId, "a Maintainance request was made");


            }

            catch (Exception ex)

            {

                // Handle exception (log it, rethrow it, etc.)

                throw new Exception("An error occurred while inserting the maintenance request.", ex);

            }

        }

        public List<Maintainance> ViewTenantRequests(string userId)

        {

            try

            {

                if (!_repository.TenantExists(userId))

                {

                    throw new Exception($"Tenant with ID {userId} does not exist.");

                }

                return _repository.ViewTenantRequests(userId);

            }

            catch (Exception ex)

            {

                // Handle exception (log it, rethrow it, etc.)

                throw new Exception("An error occurred while viewing tenant requests.", ex);

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

                // Handle exception (log it, rethrow it, etc.)

                throw new Exception("An error occurred while viewing owner requests.", ex);

            }

        }

        public bool UpdateStatus(int requestId, string newStatus)

        {

            try

            {

                return _repository.UpdateStatus(requestId, newStatus);
                //_context.Database.ExecuteSqlRaw("EXEC InsertIntoNotificcation1 @p0, @p1, @p2", tenantId, ownerId, "a Maintainance request was made");


            }

            catch (Exception ex)

            {

                // Handle exception (log it, rethrow it, etc.)

                throw new Exception("An error occurred while updating the status.", ex);

            }

        }

    }

}
