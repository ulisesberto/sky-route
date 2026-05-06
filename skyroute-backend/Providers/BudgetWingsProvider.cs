using SkyRoute.Api.DTOs;
using SkyRoute.Api.Interfaces.IBusinessLogic;
using SkyRoute.Api.Providers.Models;

namespace SkyRoute.Api.Providers;

public sealed class BudgetWingsProvider : IFlightProvider
{
    private const string ProviderName = ProviderNames.BudgetWings;

    // Fixed schedule table: (flightNumber, departureHour, departureMinute, durationMinutes, baseFare)
    // Slots intentionally differ from GlobalAir to simulate a competing low-cost carrier.
    // Durations are varied to ensure realistic sort/filter scenarios in the aggregated results.
    private static readonly (string Number, int Hour, int Minute, int DurationMinutes, decimal BaseFare)[] Schedule =
    [
        ("BW501",  7, 30,  85,  80.00m),
        ("BW612", 12,  0, 100,  95.00m),
        ("BW748", 17, 45,  90,  75.00m),
        ("BW893", 22, 15, 115, 110.00m),
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
