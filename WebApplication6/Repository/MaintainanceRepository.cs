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

        void IMaintainanceRepository.InsertMaintainance(int requestId, int propertyId, string tenantId, string description, string status, string imagePath)
        {
            var maintainance = new Maintainance
            {
                RequestId = requestId,
                PropertyId = propertyId,
                TenantId = tenantId,
                Description = description,
                Status = status,
                ImagePath = imagePath
            };
            _context.Maintainances.Add(maintainance);
            _context.SaveChanges();
        }

        bool IMaintainanceRepository.UpdateStatus(int requestId, string newStatus)
        {
            var maintainance = _context.Maintainances.FirstOrDefault(m => m.RequestId == requestId);
            if (maintainance != null)
            {
                maintainance.Status = newStatus;
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        List<Maintainance> IMaintainanceRepository.ViewOwnerRequests(string userId)
        {
            // Define the output parameter for the stored procedure
            var ownerIdParam = new SqlParameter("@OwnerId", userId);

            // Execute the stored procedure and retrieve the results
            var maintainances = _context.Maintainances
                .FromSqlRaw("EXEC sp_GetMaintainancesByOwnerId @OwnerId", ownerIdParam)
                .ToList();

            return maintainances;
        }

        List<Maintainance> IMaintainanceRepository.ViewTenantRequests(string userId)
        {
            return _context.Maintainances.Where(m => m.TenantId == userId).ToList();
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

    }
}
