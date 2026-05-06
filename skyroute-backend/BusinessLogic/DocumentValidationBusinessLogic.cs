using SkyRoute.Api.Exceptions;
using SkyRoute.Api.Interfaces.IBusinessLogic;

namespace SkyRoute.Api.BusinessLogic;

public sealed class DocumentValidationBusinessLogic : IDocumentValidationBusinessLogic
{
    public void EnsureDocumentValid(bool isInternational, string documentNumber)
    {
        _ = isInternational;
        if (string.IsNullOrWhiteSpace(documentNumber))
            throw new DocumentRuleViolationException("Document number is required.");
    }
}
