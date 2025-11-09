namespace _8BallPool.Data.Exceptions;

public class DoubleBookingException : Exception
{
    public DoubleBookingException(string message) 
        : base(message) { }
    
    public DoubleBookingException(string message, Exception innerException) 
        : base(message, innerException) { }
}
