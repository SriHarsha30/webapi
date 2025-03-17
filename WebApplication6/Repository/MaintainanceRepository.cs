using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebApplication6.Models;

namespace WebApplication6.Repository
{
    public class MaintainanceRepository : IMaintainanceRepository
    {
        private readonly Context _context;
        private bool isPresent;

        public MaintainanceRepository(Context context)
        {
            _context = context;
        }

        int IMaintainanceRepository.CheckUserId(string userId, out bool isPresent)
        {
            var isPresentParam = new SqlParameter
            {
                ParameterName = "@IsPresent",
                SqlDbType = System.Data.SqlDbType.Bit,
                Direction = System.Data.ParameterDirection.Output
            };

            var userIdParam = new SqlParameter("@UserId", userId);

            // Execute the stored procedure
            _context.Database.ExecuteSqlRaw("EXEC sp_CheckUserId @UserId, @IsPresent OUTPUT", userIdParam, isPresentParam);

            // Retrieve the output parameter value
            isPresent = (bool)isPresentParam.Value;

            // Fetch the user data using LINQ
            var user = _context.Registrationss.FirstOrDefault(r => r.ID == userId);
            return user != null ? int.Parse(user.ID) : 0;
        }

        void IMaintainanceRepository.InsertMaintainance(int propertyId, string tenantId, string description, String status, string imagePath)
        {
            try
            {
                var maintainance = new Maintainance
                {
                    PropertyId = propertyId,
                    TenantId = tenantId,
                    Description = description,
                    Status = "Pending",
                    ImagePath = imagePath
                };
                _context.Maintainances.Add(maintainance);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Log the exception details
                // Example: _logger.LogError(ex, "Error inserting maintenance into database");
                throw new Exception("An (repo) error occurred while inserting the maintenance request.", ex);
            }
        }

        bool IMaintainanceRepository.UpdateStatus(int requestId, string newStatus)
        {
            var maintainance = _context.Maintainances.FirstOrDefault(m => m.RequestId == requestId);
            if (maintainance != null)
            {
                maintainance.Status = (newStatus); // Convert string to enum
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public List<Maintainance> ViewOwnerRequests(string userId)
        {
            try
            {
                // Define the output parameter for the stored procedure
                var ownerIdParam = new SqlParameter("@OwnerId", userId);

                // Execute the stored procedure and retrieve the results
                var maintainances = _context.Maintainances
                    .FromSqlRaw("EXEC sp_GetMaintainancesByOwnerId @OwnerId", ownerIdParam)
                    .ToList();

                return maintainances;
            }
            catch (Exception ex)
            {
                // Log the exception details
                //_logger.LogError(ex, "Error retrieving owner requests from database");
                throw new Exception("An (repo) error occurred while viewing owner requests.", ex);
            }
        }

        public List<Maintainance> ViewTenantRequests(string userId)
        {
            try
            {
                // Assuming TenantId is stored as a string in the database
                var tenantRequests = _context.Maintainances
                    .Where(m => m.TenantId == userId)
                    .ToList();

                return tenantRequests;
            }
            catch (Exception ex)
            {
                // Log the exception details
                // Example: _logger.LogError(ex, "Error retrieving tenant requests from database");
                throw new Exception("An (repo) error occurred while viewing tenant requests.", ex);
            }
        }

        public bool TenantExists(string tenantId)
        {
            // Implement logic to check if the tenant exists in the database
            // For example:
            return _context.Registrationss.Any(t => t.ID == tenantId);
        }

        public bool PropertyExists(int propertyId)
        {
            // Implement logic to check if the property exists in the database
            // For example:
            return _context.Properties.Any(p => p.Property_Id == propertyId);
        }

        public bool LeaseSigned(int propertyId)
        {
            // Implement logic to check if the lease status is true for the given property ID
            return _context.Leases1.Any(l => l.Property_Id == propertyId && l.Lease_status == true);
        }

    }
}
