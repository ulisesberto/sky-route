using System.ComponentModel.DataAnnotations;
using SkyRoute.Api.Models;

namespace SkyRoute.Api.DTOs;

public sealed class FlightSearchRequestDto : IValidatableObject
{
    [Required]
    [MaxLength(8)]
    public string Origin { get; set; } = "";

    [Required]
    [MaxLength(8)]
    public string Destination { get; set; } = "";

    [Required]
    public DateOnly DepartureDate { get; set; }

    [Range(1, 9)]
    public int Passengers { get; set; }

    [Required]
    public CabinClass CabinClass { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrWhiteSpace(Origin) &&
            !string.IsNullOrWhiteSpace(Destination) &&
            string.Equals(Origin.Trim(), Destination.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            yield return new ValidationResult(
                "Origin and destination must differ.",
                [nameof(Destination)]);
        }
    }
}
