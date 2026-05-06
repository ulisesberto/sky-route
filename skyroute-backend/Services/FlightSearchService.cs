using SkyRoute.Api.DTOs;
using SkyRoute.Api.Interfaces.IBusinessLogic;
using SkyRoute.Api.Interfaces.IServices;

namespace SkyRoute.Api.Services;

/// <summary>
/// Aggregates flight offers from all registered <see cref="IFlightProvider"/> implementations.
/// New providers are picked up automatically via DI — no changes required here.
/// Pricing and full result mapping will be added in BE-4.1.
/// </summary>
public sealed class FlightSearchService : IFlightSearchService
{
    private readonly IEnumerable<IFlightProvider> _providers;

    public FlightSearchService(IEnumerable<IFlightProvider> providers)
    {
        _providers = providers;
    }

    public Task<FlightSearchResponseDto> SearchAsync(FlightSearchRequestDto request, CancellationToken cancellationToken = default)
    {
        _ = request;
        _ = cancellationToken;

        // TODO: iterate _providers, apply pricing rules per provider, and aggregate results.
        // Providers are injected and available via _providers (IEnumerable<IFlightProvider>).
        return Task.FromResult(new FlightSearchResponseDto { Results = [] });
    }
}
