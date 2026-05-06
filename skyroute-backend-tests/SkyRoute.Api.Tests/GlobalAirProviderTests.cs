using SkyRoute.Api.DTOs;
using SkyRoute.Api.Models;
using SkyRoute.Api.Providers;

namespace SkyRoute.Api.Tests;

public sealed class GlobalAirProviderTests
{
    private static FlightSearchRequestDto BuildRequest(DateOnly? date = null) => new()
    {
        Origin        = "EZE",
        Destination   = "MIA",
        DepartureDate = date ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
        Passengers    = 1,
        CabinClass    = CabinClass.Economy,
    };

    // ── AC: returns exactly 4 flights ────────────────────────────────────────

    [Fact]
    public async Task GetFlightsAsync_ReturnsAtLeastThreeFlights()
    {
        var provider = new GlobalAirProvider();

        var result = await provider.GetFlightsAsync(BuildRequest());

        Assert.Equal(4, result.Count);
    }

    // ── AC: FlightNumber is not empty ─────────────────────────────────────────

    [Fact]
    public async Task GetFlightsAsync_AllFlightsHaveNonEmptyFlightNumber()
    {
        var provider = new GlobalAirProvider();

        var result = await provider.GetFlightsAsync(BuildRequest());

        Assert.All(result, f => Assert.False(string.IsNullOrWhiteSpace(f.FlightNumber),
            "FlightNumber must not be empty."));
    }

    // ── AC: DepartureTime is on the requested date ────────────────────────────

    [Fact]
    public async Task GetFlightsAsync_DepartureTimeMatchesRequestDate()
    {
        var provider    = new GlobalAirProvider();
        var requestDate = new DateOnly(2027, 8, 15);

        var result = await provider.GetFlightsAsync(BuildRequest(requestDate));

        Assert.All(result, f =>
        {
            Assert.Equal(requestDate.Year,  f.DepartureTime.Year);
            Assert.Equal(requestDate.Month, f.DepartureTime.Month);
            Assert.Equal(requestDate.Day,   f.DepartureTime.Day);
        });
    }

    // ── AC: ArrivalTime = DepartureTime + DurationMinutes ────────────────────

    [Fact]
    public async Task GetFlightsAsync_ArrivalTimeEqualsDeparturePlusDuration()
    {
        var provider = new GlobalAirProvider();

        var result = await provider.GetFlightsAsync(BuildRequest());

        Assert.All(result, f =>
        {
            var expected = f.DepartureTime.AddMinutes(f.DurationMinutes);
            Assert.Equal(expected, f.ArrivalTime);
        });
    }

    // ── AC: BaseFare > 0 ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetFlightsAsync_AllFlightsHavePositiveBaseFare()
    {
        var provider = new GlobalAirProvider();

        var result = await provider.GetFlightsAsync(BuildRequest());

        Assert.All(result, f => Assert.True(f.BaseFare > 0m,
            $"BaseFare must be positive for flight {f.FlightNumber}."));
    }

    // ── AC: flights are distinct (unique FlightNumbers) ───────────────────────

    [Fact]
    public async Task GetFlightsAsync_FlightNumbersAreUnique()
    {
        var provider = new GlobalAirProvider();

        var result = await provider.GetFlightsAsync(BuildRequest());

        var unique = result.Select(f => f.FlightNumber).Distinct().Count();
        Assert.Equal(result.Count, unique);
    }

    // ── AC: cancelled token throws OperationCanceledException ─────────────────

    [Fact]
    public async Task GetFlightsAsync_CancelledToken_ThrowsOperationCanceledException()
    {
        var provider = new GlobalAirProvider();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => provider.GetFlightsAsync(BuildRequest(), cts.Token));
    }

    // ── BUG-BE2-01: Provider field equals "GlobalAir" ────────────────────────

    [Fact]
    public async Task GetFlightsAsync_AllFlightsHaveGlobalAirProvider()
    {
        var provider = new GlobalAirProvider();

        var result = await provider.GetFlightsAsync(BuildRequest());

        Assert.All(result, r => Assert.Equal(ProviderNames.GlobalAir, r.Provider));
    }

    // ── BUG-BE2-02: BaseFare values are varied across flights ─────────────────

    [Fact]
    public async Task GetFlightsAsync_BasefaresAreVariedAcrossFlights()
    {
        var provider = new GlobalAirProvider();

        var result = await provider.GetFlightsAsync(BuildRequest());

        var distinctCount = result.Select(f => f.BaseFare).Distinct().Count();
        Assert.True(distinctCount >= 2,
            $"Expected at least 2 distinct BaseFare values but found {distinctCount}.");
    }

    // ── BUG-BE2-03: Null request throws ArgumentNullException ─────────────────

    [Fact]
    public async Task GetFlightsAsync_NullRequest_ThrowsArgumentNullException()
    {
        var provider = new GlobalAirProvider();

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => provider.GetFlightsAsync(null!));
    }
}
