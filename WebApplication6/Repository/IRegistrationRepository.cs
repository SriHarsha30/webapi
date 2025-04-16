using WebApplication6.Models;

namespace WebApplication6.Repository
{
    public interface IRegistrationRepository
    {
        public int LoginChk(string userId, string password, out bool isPasswordReset, string answer = null, string newPassword = null);
        public bool Insertion(Registration a);
        public List<Registration> readData();
        //public List<Models.Property> viewData();

        Task SaveChangesAsync();
        //Task<string> GetByIdAsync(string ownerId);
    }
}
