using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using WebApplication6.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApplication6.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly Context _context;

        public PaymentRepository(Context context)
        {
            _context = context;
        }

        public void InsertPayment(Payment payment)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(payment, serviceProvider: null, items: null);

            if (!Validator.TryValidateObject(payment, validationContext, validationResults, validateAllProperties: true))
            {
                foreach (var validationResult in validationResults)
                {
                    Console.WriteLine(validationResult.ErrorMessage);
                }
                return;
            }

            var tenantDetails = _context.Registrationss.SingleOrDefault(r => r.ID == payment.Tenant_Id);
            if (tenantDetails == null)
            {
                Console.WriteLine("Tenant not found. Please try again.");
                return;
            }

            _context.Add(payment);
            _context.SaveChanges();
            //var noti = _context.Registrationss.SingleOrDefault();
            //string notipd = $"you have made a payment id:{payment.PaymentID} of amount :{payment.Amount} ";
            //_context.Database.ExecuteSqlRaw("EXEC InsertIntoNotificcation1 @p0, @p1, @p2", property.Owner_Id, property.Owner_Id, notipd);

        }

        public List<Payment> GetAllPayments()
        {
            return _context.Payments.ToList();
        }

       

        public Registration GetTenantDetailsFromRegistration(string tenantId)
        {
            return _context.Registrationss.SingleOrDefault(r => r.ID == tenantId);
        }

        public void ShowAllPayments(string userId)
        {
            var userPayments = _context.Payments.Where(payment => payment.Tenant_Id == userId).ToList();

            if (userPayments.Any())
            {
                foreach (var payment in userPayments)
                {
                    Console.WriteLine($"PaymentID: {payment.PaymentID}, TenantID: {payment.Tenant_Id}, " +
                        $"PropertyID: {payment.PropertyId}, Amount: {payment.Amount}, PaymentDate: {payment.PaymentDate}," +
                        $" Status: {payment.Status}, Ownerstatus: {payment.Ownerstatus}");
                }
            }
            else
            {
                Console.WriteLine($"No payments found for user with TenantID: {userId}");
            }
        }

        public void ShowAllPaymentsOwner(string userId)
        {
            var userPayments = (from payment in _context.Payments
                                join prop in _context.Properties on payment.PropertyId equals prop.Property_Id
                                where prop.Owner_Id == userId
                                select payment).ToList();

            if (userPayments.Any())
            {
                foreach (var payment in userPayments)
                {
                    Console.WriteLine($"PaymentID: {payment.PaymentID}, TenantID: {payment.Tenant_Id}," +
                        $" PropertyID: {payment.PropertyId}, Amount: {payment.Amount}, PaymentDate: {payment.PaymentDate}," +
                        $" Status: {payment.Status}, Ownerstatus: {payment.Ownerstatus}");
                }
            }
            else
            {
                Console.WriteLine($"No payments found for user with TenantID: {userId}");
            }
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByTenantIdAsync(string tenantId)
        {
            var commandText = "EXEC GetPaymentsByTenantId @Tenant_Id";
            var tenantParam = new SqlParameter("@Tenant_Id", tenantId);

            return await _context.Payments
                                 .FromSqlRaw(commandText, tenantParam)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByOwnerid(string oid)
        {
            var commandText = "EXEC GetPaymentsByOwnerId @Owner_Id";
            var tenantParam = new SqlParameter("@Owner_Id", oid);

            return await _context.Payments
                                 .FromSqlRaw(commandText, tenantParam)
                                 .ToListAsync();
        }

        public string GetOwnerIdIfPropertyExists(int propertyId)
        {
            var result = _context.Properties
                .FromSqlRaw("EXEC CheckPropertyAndFetchOwner @PropertyId = {0}", propertyId)
                .AsEnumerable()
                .Select(p => p.Owner_Id)
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(result))
            {
                Console.WriteLine($"Owner ID: {result}");
                return result;
            }
            else
            {
                Console.WriteLine("Property ID not available.");
                return null;
            }
        }
    }
}