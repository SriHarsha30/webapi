namespace WebApplication6.Exceptions
{
    public class InvalidDateRangeException : Exception
    {
        public InvalidDateRangeException()
        : base("End date must be greater than start date.") { }

        public InvalidDateRangeException(string message)
            : base(message) { }

        public InvalidDateRangeException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
