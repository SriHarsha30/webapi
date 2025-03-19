namespace WebApplication6.Exceptions
{
    public class Propertynotfoundexception:Exception
    {
        
            public Propertynotfoundexception()
            : base("Requestued Property Not Found") { }

            public Propertynotfoundexception(string message)
                : base(message) { }

            public Propertynotfoundexception(string message, Exception innerException)
                : base(message, innerException) { }
 
    }
}
