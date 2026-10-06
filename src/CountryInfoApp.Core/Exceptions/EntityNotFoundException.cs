namespace CountryInfoApp.Core.Exceptions;

/// <summary>
/// Raised when the service answers correctly but has no data for the given key.
/// </summary>
public class EntityNotFoundException : CountryInfoServiceException
{
    public EntityNotFoundException()
    {
    }

    public EntityNotFoundException(string message)
        : base(message)
    {
    }

    public EntityNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
