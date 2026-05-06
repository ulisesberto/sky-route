using System.Text.RegularExpressions;
using SkyRoute.Api.Exceptions;
using SkyRoute.Api.Interfaces.IBusinessLogic;

namespace SkyRoute.Api.BusinessLogic;

public sealed partial class DocumentValidationBusinessLogic : IDocumentValidationBusinessLogic
{
    [GeneratedRegex(@"^[A-Z0-9]{6,9}$")]
    private static partial Regex PassportRegex();

    [GeneratedRegex(@"^[0-9]{6,10}$")]
    private static partial Regex NationalIdRegex();

    public void EnsureDocumentValid(bool isInternational, string documentNumber)
    {
        if (string.IsNullOrWhiteSpace(documentNumber))
            throw new DocumentRuleViolationException("Document number is required.");

        if (isInternational)
        {
            if (!PassportRegex().IsMatch(documentNumber))
                throw new DocumentRuleViolationException(
                    "Passport number must be 6–9 uppercase alphanumeric characters (e.g. AB123456).");
        }
        else
        {
            if (!NationalIdRegex().IsMatch(documentNumber))
                throw new DocumentRuleViolationException(
                    "National ID must be 6–10 numeric digits.");
        }
    }
}
