namespace SkyRoute.Api.Interfaces.IBusinessLogic;

/// <summary>Central rules for passport vs national ID (BE-5 will expand this).</summary>
public interface IDocumentValidationBusinessLogic
{
    void EnsureDocumentValid(bool isInternational, string documentNumber);
}
