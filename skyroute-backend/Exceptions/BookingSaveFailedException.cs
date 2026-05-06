namespace SkyRoute.Api.Exceptions;

public sealed class BookingSaveFailedException : Exception
{
    public BookingSaveFailedException(Exception innerException)
        : base("Could not persist booking.", innerException)
    {
    }
}
