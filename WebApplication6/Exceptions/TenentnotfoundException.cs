namespace WebApplication6.Exceptions
{
    public class TenentnotfoundException:Exception
    {
        public TenentnotfoundException()
      : base("Tenent not found") { }

        public TenentnotfoundException(string message)
            : base(message) { }

        public TenentnotfoundException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
