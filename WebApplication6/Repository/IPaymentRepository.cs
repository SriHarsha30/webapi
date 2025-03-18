using WebApplication6.Models;

namespace WebApplication6.Repository
{
    public interface IPaymentRepository
    {
        void InsertPayment(Payment payment);
        List<Payment> GetAllPayments();
        Registration GetTenantDetailsFromRegistration(string tenantId);
        void ShowAllPayments(string userId);
        void ShowAllPaymentsOwner(string userId);
        string GetOwnerIdIfPropertyExists(int propertyId);
        Task<IEnumerable<Payment>> GetPaymentsByOwnerid(string oid);
        Task<IEnumerable<Payment>> GetPaymentsByTenantIdAsync(string tenantId);
    }
}
