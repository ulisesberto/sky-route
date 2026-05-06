namespace SkyRoute.Api.DTOs;

public sealed class FlightSearchResponseDto
{
    public IReadOnlyList<FlightResultDto> Results { get; set; } = [];
}
