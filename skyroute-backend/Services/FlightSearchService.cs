using SkyRoute.Api.BusinessLogic;
using SkyRoute.Api.Configuration;
using SkyRoute.Api.DTOs;
using SkyRoute.Api.Interfaces.IBusinessLogic;
using SkyRoute.Api.Interfaces.IServices;
using SkyRoute.Api.Providers.Models;

namespace SkyRoute.Api.Services;

/// <summary>
/// Aggregates flight offers from all registered <see cref="IFlightProvider"/> implementations,
/// applies per-provider pricing rules, and returns a unified result list.
/// New providers and pricing rules are picked up automatically via DI.
/// </summary>
public sealed class FlightSearchService : IFlightSearchService
{
    private readonly IEnumerable<IFlightProvider> _providers;
    private readonly IReadOnlyDictionary<string, IPricingRule> _pricingRules;

    public FlightSearchService(
        IEnumerable<IFlightProvider> providers,
        IEnumerable<IPricingRule> pricingRules)
    {
        _providers    = providers;
        _pricingRules = pricingRules.ToDictionary(r => r.ProviderName, StringComparer.OrdinalIgnoreCase);
    }

    public async Task<FlightSearchResponseDto> SearchAsync(
        FlightSearchRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // T1: query all providers in parallel
        var tasks   = _providers.Select(p => p.GetFlightsAsync(request, cancellationToken));
        var batches = await Task.WhenAll(tasks).ConfigureAwait(false);

        // T4: compute once per request — unknown airports default to international
        bool isInternational = AirportRegistry.IsInternational(request.Origin, request.Destination);

        // T2 & T3: normalize each offer and concatenate into a single list
        var results = batches
            .SelectMany(offers => offers ?? [])
            .Select(offer => MapToResult(offer, request, isInternational))
            .ToList();

        return new FlightSearchResponseDto { Results = results };
    }

    private FlightResultDto MapToResult(
        FlightOfferDto offer,
        FlightSearchRequestDto request,
        bool isInternational)
    {
        var rulePrice = _pricingRules.TryGetValue(offer.Provider, out var rule)
            ? rule.Calculate(offer.BaseFare)
            : offer.BaseFare;

        var cabinMultiplier   = CabinPriceMultiplier.GetMultiplier(request.CabinClass);
        var perPassengerPrice = Math.Round(rulePrice * cabinMultiplier, 2, MidpointRounding.AwayFromZero);

        return new FlightResultDto
        {
            Provider          = offer.Provider,
            FlightNumber      = offer.FlightNumber,
            Origin            = offer.Origin,
            Destination       = offer.Destination,
            DepartureTime     = offer.DepartureTime,
            ArrivalTime       = offer.ArrivalTime,
            DurationMinutes   = offer.DurationMinutes,
            CabinClass        = request.CabinClass,
            PerPassengerPrice = perPassengerPrice,
            TotalPrice        = perPassengerPrice * request.Passengers,
            IsInternational   = isInternational,
        };
    }
}
