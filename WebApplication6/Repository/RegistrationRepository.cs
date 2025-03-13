using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebApplication6.Models;

namespace WebApplication6.Repository
{
    public class RegistrationRepository : IRegistrationRepository
    {
        private readonly Context _context;

        public RegistrationRepository(Context context)
        {
            _context = context;
        }

        public int LoginChk(string userId, string password, out bool isPasswordReset, string answer = null, string newPassword = null)
        {
            isPasswordReset = false;
            if (password != null)
            {
                var u = new SqlParameter("@UserName", userId);
                var p = new SqlParameter("@Password", password);
                var resParam = new SqlParameter("@res", System.Data.SqlDbType.Int)
                {
                    Direction = System.Data.ParameterDirection.Output
                };

                _context.Database.ExecuteSqlRaw("EXEC spGetLoginDetails @UserName, @Password, @res OUTPUT", u, p, resParam);

                return (int)resParam.Value;
            }
            else if (answer != null && newPassword != null)
            {
                var paak = new SqlParameter("@USERID", userId);
                var kaak = new SqlParameter("@pass", newPassword);
                var aak = new SqlParameter("@ans", answer);
                _context.Database.ExecuteSqlRaw("EXEC ChangePas @USERID, @pass, @ans", paak, kaak, aak);
                isPasswordReset = true;
                return 1;
            }
            return 0;
        }

        public bool Insertion(Registration a)
        {
            if (a == null)
            {
                return false;
            }

            _context.Add(a);
            _context.SaveChanges();
            _context.Database.ExecuteSqlRaw("EXEC InsertIntoNotificcation1 @p0, @p1, @p2", a.ID, a.ID, "registered as a new user");

            return true;
        }

        public List<Registration> readData()
        {
            return _context.Registrationss.ToList();
        }

        //public List<Models.Property> viewData()
        //{
        //    return _context.Properties.ToList();
        //}

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        internal List<Property> viewData()
        {
            throw new NotImplementedException();
        }

        List<Property> IRegistrationRepository.viewData()
        {
            throw new NotImplementedException();
        }
    }
}
