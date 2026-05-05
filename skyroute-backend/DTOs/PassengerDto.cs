using System.ComponentModel.DataAnnotations;

namespace SkyRoute.Api.DTOs;

public sealed class PassengerDto
{
    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = "";

    [Required]
    [EmailAddress]
    [MaxLength(320)]
    public string Email { get; set; } = "";

    [Required]
    [MaxLength(64)]
    public string DocumentNumber { get; set; } = "";
}
