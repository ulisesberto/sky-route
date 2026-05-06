namespace SkyRoute.Api.Exceptions;

public sealed class DocumentRuleViolationException : Exception
{
    public DocumentRuleViolationException(string message) : base(message)
    {
    }
}
