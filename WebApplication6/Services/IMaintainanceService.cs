using WebApplication6.Models;

namespace WebApplication6.Services
{
    public interface IMaintainanceService
    {
        void InsertMaintainance(int requestId, int propertyId, string tenantId, string description, string status, string imagePath);

        List<Maintainance> ViewTenantRequests(string userId);

        List<Maintainance> ViewOwnerRequests(string userId);

        bool UpdateStatus(int RequestId, string newStatus);
    }
}
