namespace DataVisualization.Exceptions;

public class MissingOrEmptyConfigurationException : Exception
{
    public MissingOrEmptyConfigurationException() { }

    public MissingOrEmptyConfigurationException(string message)
        : base(message) { }

    public MissingOrEmptyConfigurationException(string message, Exception innerException)
        : base(message, innerException) { }
}

