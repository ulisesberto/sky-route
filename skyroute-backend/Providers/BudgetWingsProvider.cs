using SkyRoute.Api.Providers.Models;

namespace SkyRoute.Api.Providers;

public sealed class BudgetWingsProvider : ScheduledFlightProvider
{
    protected override string ProviderName => ProviderNames.BudgetWings;

    // Fixed schedule table: (flightNumber, departureHour, departureMinute, durationMinutes, baseFare)
    // Slots intentionally differ from GlobalAir to simulate a competing low-cost carrier.
    // Durations are varied to ensure realistic sort/filter scenarios in the aggregated results.
    protected override (string Number, int Hour, int Minute, int DurationMinutes, decimal BaseFare)[] Schedule =>
    [
        ("BW501",  7, 30,  85,  80.00m),
        ("BW612", 12,  0, 100,  95.00m),
        ("BW748", 17, 45,  90,  75.00m),
        ("BW893", 22, 15, 115, 110.00m),
    ];
}
