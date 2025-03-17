using WebApplication6.Models;

namespace WebApplication6.Repository
{
    public interface IMaintainanceRepository

    {
        bool LeaseSigned(int proportyId);

        int CheckUserId(string userId, out bool isPresent);

        void InsertMaintainance(int propertyId, string tenantId, string description, String status, string imagePath);

        List<Maintainance> ViewTenantRequests(string userId);

        List<Maintainance> ViewOwnerRequests(string userId);

        bool UpdateStatus(int RequestId, string newStatus);

        bool PropertyExists(int propertyId);

        bool TenantExists(string tenantId);

    }

}
