using SkyRoute.Api.DTOs;
using SkyRoute.Api.Interfaces.IServices;

namespace SkyRoute.Api.Services;

public sealed class FlightSearchService : IFlightSearchService
{
    public Task<FlightSearchResponseDto> SearchAsync(FlightSearchRequestDto request, CancellationToken cancellationToken = default)
    {
        _ = request;
        _ = cancellationToken;
        return Task.FromResult(new FlightSearchResponseDto { Results = [] });
    }
}
