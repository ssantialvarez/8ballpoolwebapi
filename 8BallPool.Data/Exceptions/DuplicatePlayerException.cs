public class DuplicatePlayerException : Exception
{
    public DuplicatePlayerException(string message, Exception innerException)
        : base(message, innerException) { }
}