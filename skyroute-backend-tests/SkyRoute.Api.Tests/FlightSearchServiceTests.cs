using SkyRoute.Api.DTOs;
using SkyRoute.Api.Interfaces.IBusinessLogic;
using SkyRoute.Api.Models;
using SkyRoute.Api.Providers.Models;
using SkyRoute.Api.Services;

namespace SkyRoute.Api.Tests;

public sealed class FlightSearchServiceTests
{
    // ── helpers ──────────────────────────────────────────────────────────────

    private static FlightSearchRequestDto BuildRequest(
        string origin      = "EZE",
        string destination = "MIA",
        int    passengers  = 2,
        CabinClass cabin   = CabinClass.Economy) =>
        new()
        {
            Origin        = origin,
            Destination   = destination,
            DepartureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            Passengers    = passengers,
            CabinClass    = cabin,
        };

    private static IFlightProvider MakeProvider(string providerName, params FlightOfferDto[] offers) =>
        new MockProvider(offers);

    private static IPricingRule MakePricingRule(string providerName, Func<decimal, decimal> calculate) =>
        new MockPricingRule(providerName, calculate);

    // ── tests ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task SearchAsync_QueriesAllProviders()
    {
        var offer1 = new FlightOfferDto { Provider = "A", FlightNumber = "A1", Origin = "EZE", Destination = "MIA", BaseFare = 100m };
        var offer2 = new FlightOfferDto { Provider = "B", FlightNumber = "B1", Origin = "EZE", Destination = "MIA", BaseFare = 200m };

        var svc = new FlightSearchService(
            [MakeProvider("A", offer1), MakeProvider("B", offer2)],
            [MakePricingRule("A", f => f), MakePricingRule("B", f => f)]);

        var result = await svc.SearchAsync(BuildRequest());

        Assert.Equal(2, result.Results.Count);
        Assert.Contains(result.Results, r => r.FlightNumber == "A1");
        Assert.Contains(result.Results, r => r.FlightNumber == "B1");
    }

    [Fact]
    public async Task SearchAsync_AppliesPricingRulePerProvider()
    {
        var offer = new FlightOfferDto { Provider = "GlobalAir", BaseFare = 100m, FlightNumber = "GA1" };

        var svc = new FlightSearchService(
            [MakeProvider("GlobalAir", offer)],
            [MakePricingRule("GlobalAir", f => Math.Round(f * 1.15m, 2))]);

        var result = await svc.SearchAsync(BuildRequest(passengers: 1));

        Assert.Single(result.Results);
        Assert.Equal(115m, result.Results[0].PerPassengerPrice);
    }

    [Fact]
    public async Task SearchAsync_TotalPrice_IsPerPassengerPriceTimesPassengers()
    {
        var offer = new FlightOfferDto { Provider = "P", BaseFare = 100m, FlightNumber = "P1" };

        var svc = new FlightSearchService(
            [MakeProvider("P", offer)],
            [MakePricingRule("P", f => f)]);

        var result = await svc.SearchAsync(BuildRequest(passengers: 3));

        Assert.Equal(300m, result.Results[0].TotalPrice);
    }

    [Fact]
    public async Task SearchAsync_NoPricingRuleForProvider_UsesBaseFare()
    {
        var offer = new FlightOfferDto { Provider = "Unknown", BaseFare = 150m, FlightNumber = "U1" };

        var svc = new FlightSearchService(
            [MakeProvider("Unknown", offer)],
            []); // no rules

        var result = await svc.SearchAsync(BuildRequest(passengers: 1));

        Assert.Equal(150m, result.Results[0].PerPassengerPrice);
    }

    [Fact]
    public async Task SearchAsync_IsInternational_True_WhenDifferentCountries()
    {
        // EZE = AR, MIA = US → international
        var offer = new FlightOfferDto { Provider = "P", BaseFare = 100m, Origin = "EZE", Destination = "MIA" };

        var svc = new FlightSearchService([MakeProvider("P", offer)], []);

        var result = await svc.SearchAsync(BuildRequest(origin: "EZE", destination: "MIA"));

        Assert.True(result.Results[0].IsInternational);
    }

    [Fact]
    public async Task SearchAsync_IsInternational_False_WhenSameCountry()
    {
        // EZE = AR, AEP = AR → domestic
        var offer = new FlightOfferDto { Provider = "P", BaseFare = 100m, Origin = "EZE", Destination = "AEP" };

        var svc = new FlightSearchService([MakeProvider("P", offer)], []);

        var result = await svc.SearchAsync(BuildRequest(origin: "EZE", destination: "AEP"));

        Assert.False(result.Results[0].IsInternational);
    }

    [Fact]
    public async Task SearchAsync_IsInternational_True_WhenAirportUnknown()
    {
        // Unknown airport codes → treated as international by convention
        var offer = new FlightOfferDto { Provider = "P", BaseFare = 100m, Origin = "XXX", Destination = "YYY" };

        var svc = new FlightSearchService([MakeProvider("P", offer)], []);

        var result = await svc.SearchAsync(BuildRequest(origin: "XXX", destination: "YYY"));

        Assert.True(result.Results[0].IsInternational);
    }

    [Fact]
    public async Task SearchAsync_CabinClass_PropagatedFromRequest()
    {
        var offer = new FlightOfferDto { Provider = "P", BaseFare = 100m };

        var svc = new FlightSearchService([MakeProvider("P", offer)], []);

        var result = await svc.SearchAsync(BuildRequest(cabin: CabinClass.Business));

        Assert.Equal(CabinClass.Business, result.Results[0].CabinClass);
    }

    [Fact]
    public async Task SearchAsync_EmptyProviders_ReturnsEmptyList()
    {
        var svc = new FlightSearchService([], []);

        var result = await svc.SearchAsync(BuildRequest());

        Assert.Empty(result.Results);
    }

    [Fact]
    public async Task SearchAsync_MultipleProviders_ResultsConcatenated()
    {
        var offersA = Enumerable.Range(1, 3)
            .Select(i => new FlightOfferDto { Provider = "A", FlightNumber = $"A{i}", BaseFare = 100m })
            .ToArray();
        var offersB = Enumerable.Range(1, 2)
            .Select(i => new FlightOfferDto { Provider = "B", FlightNumber = $"B{i}", BaseFare = 200m })
            .ToArray();

        var svc = new FlightSearchService(
            [MakeProvider("A", offersA), MakeProvider("B", offersB)],
            []);

        var result = await svc.SearchAsync(BuildRequest());

        Assert.Equal(5, result.Results.Count);
    }

    // ── fakes ─────────────────────────────────────────────────────────────────

    private sealed class MockProvider(FlightOfferDto[] offers) : IFlightProvider
    {
        public Task<IReadOnlyList<FlightOfferDto>> GetFlightsAsync(
            FlightSearchRequestDto request,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<FlightOfferDto>>(offers);
    }

    private sealed class MockPricingRule(string providerName, Func<decimal, decimal> calculate) : IPricingRule
    {
        public string ProviderName => providerName;
        public decimal Calculate(decimal baseFare) => calculate(baseFare);
    }
}
