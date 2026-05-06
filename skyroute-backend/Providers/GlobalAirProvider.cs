using SkyRoute.Api.Providers.Models;

namespace SkyRoute.Api.Providers;

public sealed class GlobalAirProvider : ScheduledFlightProvider
{
    protected override string ProviderName => ProviderNames.GlobalAir;

    // Fixed schedule table: (flightNumber, departureHour, departureMinute, durationMinutes, baseFare)
    // Four daily departures covering morning / midday / afternoon / evening slots.
    // Durations are intentionally varied to simulate realistic flight-time differences.
    protected override (string Number, int Hour, int Minute, int DurationMinutes, decimal BaseFare)[] Schedule =>
    [
        ("GA101",  6,  0,  95, 180.00m),
        ("GA207", 10, 30, 110, 220.00m),
        ("GA318", 15, 15, 125, 195.00m),
        ("GA455", 20, 45, 140, 310.00m),
    ];
}
