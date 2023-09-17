namespace DataVisualization.Exceptions;

public class NullOrEmptyTokenException : Exception
{
    public NullOrEmptyTokenException() { }

    public NullOrEmptyTokenException(string message)
        : base(message) { }

    public NullOrEmptyTokenException(string message, Exception innerException)
        : base(message, innerException) { }
}