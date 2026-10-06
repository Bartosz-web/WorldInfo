namespace CountryInfoApp.Core.Exceptions;

/// <summary>
/// Raised when the remote service cannot be reached or returns an invalid response.
/// </summary>
public class CountryInfoServiceException : Exception
{
    public CountryInfoServiceException()
    {
    }

    public CountryInfoServiceException(string message)
        : base(message)
    {
    }

    public CountryInfoServiceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
