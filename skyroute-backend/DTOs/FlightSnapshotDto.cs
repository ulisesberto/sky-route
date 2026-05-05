using System.ComponentModel.DataAnnotations;
using SkyRoute.Api.Models;

namespace SkyRoute.Api.DTOs;

public sealed class FlightSnapshotDto
{
    [Required]
    [MaxLength(64)]
    public string Provider { get; set; } = "";

    [Required]
    [MaxLength(32)]
    public string FlightNumber { get; set; } = "";

    [Required]
    [MaxLength(8)]
    public string Origin { get; set; } = "";

    [Required]
    [MaxLength(8)]
    public string Destination { get; set; } = "";

    [Required]
    public DateTimeOffset DepartureTime { get; set; }

    [Required]
    public DateTimeOffset ArrivalTime { get; set; }

    [Required]
    public CabinClass CabinClass { get; set; }

    [Range(typeof(decimal), "0", "999999.99")]
    public decimal PerPassengerPrice { get; set; }

    [Range(typeof(decimal), "0", "9999999.99")]
    public decimal TotalPrice { get; set; }

    [Required]
    [MaxLength(8)]
    public string Currency { get; set; } = "USD";

    public bool IsInternational { get; set; }
}
