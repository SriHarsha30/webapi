namespace WebApplication6.Models
{
    public class PasswordResetRequest
    {
        public string UserId { get; set; }
        public string Answer { get; set; }
        public string NewPassword { get; set; }
    }

}
