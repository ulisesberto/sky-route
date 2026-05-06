using SkyRoute.Api.DTOs;
using SkyRoute.Api.Models;
using SkyRoute.Api.Providers;

namespace SkyRoute.Api.Tests;

public sealed class BudgetWingsProviderTests
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
        var provider = new BudgetWingsProvider();

        var result = await provider.GetFlightsAsync(BuildRequest());

        Assert.Equal(4, result.Count);
    }

    // ── AC: FlightNumber is not empty ─────────────────────────────────────────

    [Fact]
    public async Task GetFlightsAsync_AllFlightsHaveNonEmptyFlightNumber()
    {
        var provider = new BudgetWingsProvider();

        var result = await provider.GetFlightsAsync(BuildRequest());

        Assert.All(result, f => Assert.False(string.IsNullOrWhiteSpace(f.FlightNumber),
            "FlightNumber must not be empty."));
    }

    // ── AC: DepartureTime is on the requested date ────────────────────────────

    [Fact]
    public async Task GetFlightsAsync_DepartureTimeMatchesRequestDate()
    {
        var provider    = new BudgetWingsProvider();
        var requestDate = new DateOnly(2027, 8, 15);

        var result = await provider.GetFlightsAsync(BuildRequest(requestDate));

        Assert.All(result, f =>
        {
            Assert.Equal(requestDate.Year,  f.DepartureTime.Year);
            Assert.Equal(requestDate.Month, f.DepartureTime.Month);
            Assert.Equal(requestDate.Day,   f.DepartureTime.Day);
            Assert.Equal(TimeSpan.Zero, f.DepartureTime.Offset);
        });
    }

    // ── AC: ArrivalTime = DepartureTime + DurationMinutes ────────────────────

    [Fact]
    public async Task GetFlightsAsync_ArrivalTimeEqualsDeparturePlusDuration()
    {
        var provider = new BudgetWingsProvider();

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
        var provider = new BudgetWingsProvider();

        var result = await provider.GetFlightsAsync(BuildRequest());

        Assert.All(result, f => Assert.True(f.BaseFare > 0m,
            $"BaseFare must be positive for flight {f.FlightNumber}."));
    }

    // ── AC: flights are distinct (unique FlightNumbers) ───────────────────────

    [Fact]
    public async Task GetFlightsAsync_FlightNumbersAreUnique()
    {
        var provider = new BudgetWingsProvider();

        var result = await provider.GetFlightsAsync(BuildRequest());

        var unique = result.Select(f => f.FlightNumber).Distinct().Count();
        Assert.Equal(result.Count, unique);
    }

    // ── AC: cancelled token throws OperationCanceledException ─────────────────

    [Fact]
    public async Task GetFlightsAsync_CancelledToken_ThrowsOperationCanceledException()
    {
        var provider = new BudgetWingsProvider();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => provider.GetFlightsAsync(BuildRequest(), cts.Token));
    }

    // ── AC: Provider field equals "BudgetWings" ───────────────────────────────

    [Fact]
    public async Task GetFlightsAsync_AllFlightsHaveBudgetWingsProvider()
    {
        var provider = new BudgetWingsProvider();

        var result = await provider.GetFlightsAsync(BuildRequest());

        Assert.All(result, r => Assert.Equal(ProviderNames.BudgetWings, r.Provider));
    }

    // ── AC: BaseFare values are varied across flights ─────────────────────────

    [Fact]
    public async Task GetFlightsAsync_BasefaresAreVariedAcrossFlights()
    {
        var provider = new BudgetWingsProvider();

        var result = await provider.GetFlightsAsync(BuildRequest());

        var distinctCount = result.Select(f => f.BaseFare).Distinct().Count();
        Assert.True(distinctCount >= 2,
            $"Expected at least 2 distinct BaseFare values but found {distinctCount}.");
    }

    // ── AC: Null request throws ArgumentNullException ─────────────────────────

    [Fact]
    public async Task GetFlightsAsync_NullRequest_ThrowsArgumentNullException()
    {
        var provider = new BudgetWingsProvider();

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => provider.GetFlightsAsync(null!));
    }

    // ── AC: Flight numbers are differentiated from GlobalAir (no GA* prefix) ──

    [Fact]
    public async Task GetFlightsAsync_FlightNumbersAreBudgetWingsSpecific()
    {
        var provider = new BudgetWingsProvider();

        var result = await provider.GetFlightsAsync(BuildRequest());

        Assert.All(result, f =>
        {
            Assert.StartsWith("BW", f.FlightNumber,
                StringComparison.OrdinalIgnoreCase);
            Assert.False(f.FlightNumber.StartsWith("GA", StringComparison.OrdinalIgnoreCase),
                $"FlightNumber '{f.FlightNumber}' must not use the GlobalAir 'GA' prefix.");
        });
    }

    // ── AC: Origin and Destination match the request ──────────────────────────

    [Fact]
    public async Task GetFlightsAsync_OriginAndDestinationMatchRequest()
    {
        var provider = new BudgetWingsProvider();
        var result = await provider.GetFlightsAsync(BuildRequest());
        Assert.All(result, f => { Assert.Equal("EZE", f.Origin); Assert.Equal("MIA", f.Destination); });
    }

    // ── AC: DurationMinutes are varied across flights ─────────────────────────

    [Fact]
    public async Task GetFlightsAsync_DurationMinutesAreVaried()
    {
        var provider = new BudgetWingsProvider();
        var result = await provider.GetFlightsAsync(BuildRequest());
        var distinct = result.Select(f => f.DurationMinutes).Distinct().Count();
        Assert.True(distinct >= 2, $"Expected >= 2 distinct durations but got {distinct}.");
    }
}
