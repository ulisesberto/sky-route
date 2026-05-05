using SkyRoute.Api.Interfaces.IModels;

namespace SkyRoute.Api.Models;

public sealed class Booking : IEntity
{
    public Guid Id { get; set; }
    public string BookingReference { get; set; } = "";
    public string Provider { get; set; } = "";
    public string FlightNumber { get; set; } = "";
    public string Origin { get; set; } = "";
    public string Destination { get; set; } = "";
    public DateTime DepartureTimeUtc { get; set; }
    public DateTime ArrivalTimeUtc { get; set; }
    public CabinClass CabinClass { get; set; }
    public int Passengers { get; set; }
    public decimal PerPassengerPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string Currency { get; set; } = "USD";
    public bool IsInternational { get; set; }
    public string PassengerFullName { get; set; } = "";
    public string PassengerEmail { get; set; } = "";
    public string PassengerDocumentNumber { get; set; } = "";
    public DateTime CreatedAtUtc { get; set; }
}
