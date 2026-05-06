using SkyRoute.Api.DTOs;
using SkyRoute.Api.Providers.Models;

namespace SkyRoute.Api.Interfaces.IBusinessLogic;

public interface IFlightProvider
{
    Task<IReadOnlyList<FlightOfferDto>> GetFlightsAsync(
        FlightSearchRequestDto request,
        CancellationToken cancellationToken = default);
}
