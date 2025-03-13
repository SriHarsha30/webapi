using WebApplication6.Models;
using WebApplication6.Repository;

namespace WebApplication6.Services
{
    public class RegistrationServices
    {
        private readonly RegistrationRepository _repository;

        public RegistrationServices(RegistrationRepository repository)
        {
            _repository = repository;
        }

        public int LoginChk(string userId, string password, out bool isPasswordReset, string answer = null, string newPassword = null)
        {
            try
            {
                return _repository.LoginChk(userId, password, out isPasswordReset, answer, newPassword);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in LoginChk: {ex.Message}");
                isPasswordReset = false;
                return -1; // Indicate an error occurred
            }
        }

        public bool Insertion(Registration registration)
        {
            try
            {
                return _repository.Insertion(registration);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in Insertion: {ex.Message}");
                return false;
            }
        }

        //public bool Inser(Models.Property property)
        //{
        //    try
        //    {
        //        return _repository.Insertion(property);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception
        //        Console.WriteLine($"Error in Inser: {ex.Message}");
        //        return false;
        //    }
        //}

        //public bool DeleteProp(Models.Property property)
        //{
        //    try
        //    {
        //        return _repository.DeleteProp(property);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception
        //        Console.WriteLine($"Error in DeleteProp: {ex.Message}");
        //        return false;
        //    }
        //}

        public List<Registration> ReadData()
        {
            try
            {
                return _repository.readData();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in ReadData: {ex.Message}");
                return new List<Registration>();
            }
        }

        public List<Models.Property> ViewData()
        {
            try
            {
                return _repository.viewData();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in ViewData: {ex.Message}");
                return new List<Models.Property>();
            }
        }
    }
}
