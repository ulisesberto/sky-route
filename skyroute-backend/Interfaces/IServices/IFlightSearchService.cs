using SkyRoute.Api.DTOs;

namespace SkyRoute.Api.Interfaces.IServices;

public interface IFlightSearchService
{
    Task<FlightSearchResponseDto> SearchAsync(FlightSearchRequestDto request, CancellationToken cancellationToken = default);
}
