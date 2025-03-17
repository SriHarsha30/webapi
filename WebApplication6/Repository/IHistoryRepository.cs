using WebApplication6.Models;

namespace WebApplication6.Repository
{
    public interface IHistoryRepository
    {

        public IEnumerable<History> GetTenantHistory(string tenantId);

        public IEnumerable<dynamic> GetTenantHistoryForOwner(string tenantId);
    }
}
