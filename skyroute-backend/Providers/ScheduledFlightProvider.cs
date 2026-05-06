using SkyRoute.Api.DTOs;
using SkyRoute.Api.Interfaces.IBusinessLogic;
using SkyRoute.Api.Providers.Models;

namespace SkyRoute.Api.Providers;

/// <summary>
/// Base class for fixed-schedule flight providers. Subclasses supply the provider name
/// and schedule table; this class handles the common loop that builds FlightOfferDto instances.
/// </summary>
public abstract class ScheduledFlightProvider : IFlightProvider
{
    protected abstract string ProviderName { get; }
    protected abstract (string Number, int Hour, int Minute, int DurationMinutes, decimal BaseFare)[] Schedule { get; }

    public Task<IReadOnlyList<FlightOfferDto>> GetFlightsAsync(
        FlightSearchRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Origin, nameof(request.Origin));
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Destination, nameof(request.Destination));
        cancellationToken.ThrowIfCancellationRequested();

        var offers = new List<FlightOfferDto>(Schedule.Length);
        foreach (var (number, hour, minute, durationMinutes, baseFare) in Schedule)
        {
            var departure = new DateTimeOffset(
                request.DepartureDate.Year, request.DepartureDate.Month, request.DepartureDate.Day,
                hour, minute, 0, TimeSpan.Zero);
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
