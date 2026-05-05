using SkyRoute.Api.Interfaces.IBusinessLogic;

namespace SkyRoute.Api.BusinessLogic;

public sealed class BookingReferenceBusinessLogic : IBookingReferenceBusinessLogic
{
    public string Generate() =>
        $"SR-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";
}
