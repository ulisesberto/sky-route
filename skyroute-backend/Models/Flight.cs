namespace SkyRoute.Api.Models;

/// <summary>Domain representation of a flight offer (not an API contract).</summary>
public sealed class Flight
{
    public string Provider { get; set; } = "";
    public string FlightNumber { get; set; } = "";
    public string Origin { get; set; } = "";
    public string Destination { get; set; } = "";
    public DateTimeOffset DepartureTime { get; set; }
    public DateTimeOffset ArrivalTime { get; set; }
    public int DurationMinutes { get; set; }
    public CabinClass CabinClass { get; set; }
    public decimal PerPassengerPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string Currency { get; set; } = "USD";
    public bool IsInternational { get; set; }
}
