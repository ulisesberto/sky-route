using System.ComponentModel.DataAnnotations;

namespace SkyRoute.Api.DTOs;

public sealed class CreateBookingRequestDto
{
    [Required]
    public FlightSnapshotDto Flight { get; set; } = null!;

    [Range(1, 9)]
    public int Passengers { get; set; }

    [Required]
    public PassengerDto Passenger { get; set; } = null!;
}
