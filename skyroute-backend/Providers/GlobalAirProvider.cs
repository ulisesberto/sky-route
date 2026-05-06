using SkyRoute.Api.DTOs;
using SkyRoute.Api.Interfaces.IBusinessLogic;
using SkyRoute.Api.Providers.Models;

namespace SkyRoute.Api.Providers;

public sealed class GlobalAirProvider : IFlightProvider
{
    private const string ProviderName = "GlobalAir";

    // Fixed schedule table: (flightNumber, departureHour, departureMinute, durationMinutes, baseFare)
    // Four daily departures covering morning / midday / afternoon / evening slots.
    // Durations are intentionally varied to simulate realistic flight-time differences.
    private static readonly (string Number, int Hour, int Minute, int DurationMinutes, decimal BaseFare)[] Schedule =
    [
        ("GA101",  6,  0,  95, 180.00m),
        ("GA207", 10, 30, 110, 220.00m),
        ("GA318", 15, 15, 125, 195.00m),
        ("GA455", 20, 45, 140, 310.00m),
    ];

    public Task<IReadOnlyList<FlightOfferDto>> GetFlightsAsync(
        FlightSearchRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        var offers = new List<FlightOfferDto>(Schedule.Length);

        foreach (var (number, hour, minute, durationMinutes, baseFare) in Schedule)
        {
            var departure = new DateTimeOffset(
                request.DepartureDate.Year,
                request.DepartureDate.Month,
                request.DepartureDate.Day,
                hour, minute, 0,
                TimeSpan.Zero);

            var arrival = departure.AddMinutes(durationMinutes);

            offers.Add(new FlightOfferDto
            {
                Provider        = ProviderName,
                FlightNumber    = number,
                Origin          = request.Origin,
                Destination     = request.Destination,
                DepartureTime   = departure,
                ArrivalTime     = arrival,
                DurationMinutes = durationMinutes,
                BaseFare        = baseFare,
            });
        }

        return Task.FromResult<IReadOnlyList<FlightOfferDto>>(offers);
    }
}
