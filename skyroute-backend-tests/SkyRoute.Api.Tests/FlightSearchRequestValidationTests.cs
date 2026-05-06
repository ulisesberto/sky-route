using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using SkyRoute.Api.DTOs;
using SkyRoute.Api.Models;

namespace SkyRoute.Api.Tests;

public sealed class FlightSearchRequestValidationTests
{
    // ── helpers ──────────────────────────────────────────────────────────────

    private static IList<ValidationResult> Validate(FlightSearchRequestDto dto, TimeProvider? clock = null)
    {
        var services = new ServiceCollection();
        if (clock is not null)
            services.AddSingleton(clock);

        var ctx     = new ValidationContext(dto, services.BuildServiceProvider(), null);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(dto, ctx, results, validateAllProperties: true);
        return results;
    }

    private static FlightSearchRequestDto BuildDto(
        DateOnly? departureDate = null,
        string origin      = "EZE",
        string destination = "MIA",
        int    passengers  = 1,
        CabinClass cabin   = CabinClass.Economy) =>
        new()
        {
            Origin        = origin,
            Destination   = destination,
            DepartureDate = departureDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            Passengers    = passengers,
            CabinClass    = cabin,
        };

    // ── clock stub ───────────────────────────────────────────────────────────

    /// <summary>Freezes the clock at a specific UTC instant for deterministic date tests.</summary>
    private sealed class FrozenClock(DateTimeOffset frozenUtc) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => frozenUtc;
    }

    // ── valid request ────────────────────────────────────────────────────────

    [Fact]
    public void Valid_Request_PassesValidation()
    {
        var clock  = new FrozenClock(new DateTimeOffset(2026, 5, 6, 12, 0, 0, TimeSpan.Zero));
        var errors = Validate(BuildDto(departureDate: new DateOnly(2026, 5, 7)), clock);

        Assert.Empty(errors);
    }

    // ── past date ────────────────────────────────────────────────────────────

    [Fact]
    public void PastDepartureDate_FailsValidation()
    {
        var clock  = new FrozenClock(new DateTimeOffset(2026, 5, 6, 12, 0, 0, TimeSpan.Zero));
        var errors = Validate(BuildDto(departureDate: new DateOnly(2026, 5, 5)), clock);

        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(FlightSearchRequestDto.DepartureDate)));
    }

    [Fact]
    public void TodayDepartureDate_PassesValidation()
    {
        // "Today" in local server time — FrozenClock LocalTimeZone defaults to Local,
        // so GetLocalNow().Date == 2026-05-06.
        var clock  = new FrozenClock(new DateTimeOffset(2026, 5, 6, 12, 0, 0, TimeSpan.Zero));
        var errors = Validate(BuildDto(departureDate: new DateOnly(2026, 5, 6)), clock);

        Assert.DoesNotContain(errors, e => e.MemberNames.Contains(nameof(FlightSearchRequestDto.DepartureDate)));
    }

    /// <summary>
    /// UTC-5 user at 23:30 local (= 04:30 UTC next day). Their "today" is May 5.
    /// Without the fix, UTC check would see May 6 and block the booking. With TimeProvider
    /// using local time, the server's local today drives the check — this test documents
    /// that the implementation is timezone-agnostic at the code level.
    /// </summary>
    [Fact]
    public void UtcMinus_User_BookingToday_PassesWhenClockIsSetToTheirLocalDate()
    {
        // Server clock frozen to May 5 at 23:30 local (local = UTC-5 simulated via UTC 04:30 May 6,
        // but here we freeze GetLocalNow to represent the user's actual local date as May 5).
        // We set the frozen UTC to a time where the local date is May 5.
        var utcNow = new DateTimeOffset(2026, 5, 6, 4, 30, 0, TimeSpan.Zero); // UTC 04:30 May 6
        var clock  = new FrozenClock(utcNow);

        // The local date from GetLocalNow depends on the test machine's timezone.
        // Instead of asserting the exact date (machine-dependent), assert the rule:
        // DepartureDate == the local date derived from the clock must NOT fail.
        var localToday = DateOnly.FromDateTime(clock.GetLocalNow().DateTime);
        var errors     = Validate(BuildDto(departureDate: localToday), clock);

        Assert.DoesNotContain(errors, e => e.MemberNames.Contains(nameof(FlightSearchRequestDto.DepartureDate)));
    }

    // ── other validations (not date-related) ─────────────────────────────────

    [Fact]
    public void SameOriginAndDestination_FailsValidation()
    {
        var errors = Validate(BuildDto(origin: "EZE", destination: "EZE"));

        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(FlightSearchRequestDto.Destination)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    public void OutOfRangePassengers_FailsValidation(int passengers)
    {
        var errors = Validate(BuildDto(passengers: passengers));

        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(FlightSearchRequestDto.Passengers)));
    }
}
