namespace SkyRoute.Api.Providers.Models;

/// <summary>
/// Internal model returned by flight providers (pre-pricing).
/// Not exposed via API — gets mapped to FlightResultDto after pricing rules are applied.
/// </summary>
public sealed class FlightOfferDto
{
    public string Provider { get; set; } = "";
    public string FlightNumber { get; set; } = "";
    public string Origin { get; set; } = "";
    public string Destination { get; set; } = "";
    public DateTimeOffset DepartureTime { get; set; }
    public DateTimeOffset ArrivalTime { get; set; }
    public int DurationMinutes { get; set; }
    public decimal BaseFare { get; set; }
}
