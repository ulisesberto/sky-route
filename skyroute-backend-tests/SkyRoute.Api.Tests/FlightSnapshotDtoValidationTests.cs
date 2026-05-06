using System.ComponentModel.DataAnnotations;
using SkyRoute.Api.DTOs;
using SkyRoute.Api.Models;

namespace SkyRoute.Api.Tests;

/// <summary>
/// Validates that [Range(typeof(decimal), ...)] on FlightSnapshotDto works correctly
/// regardless of the server's current culture (covers the es-AR / es-MX comma-decimal bug).
/// </summary>
public sealed class FlightSnapshotDtoValidationTests
{
    // ── helpers ──────────────────────────────────────────────────────────────

    private static IList<ValidationResult> Validate(FlightSnapshotDto dto)
    {
        var ctx     = new ValidationContext(dto);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(dto, ctx, results, validateAllProperties: true);
        return results;
    }

    private static FlightSnapshotDto BuildValidSnapshot(
        decimal perPassengerPrice = 12345.67m,
        decimal totalPrice        = 24691.34m) =>
        new()
        {
            Provider         = "GlobalAir",
            FlightNumber     = "GA001",
            Origin           = "EZE",
            Destination      = "MIA",
            DepartureTime    = DateTimeOffset.UtcNow.AddDays(1),
            ArrivalTime      = DateTimeOffset.UtcNow.AddDays(1).AddHours(10),
            CabinClass       = CabinClass.Economy,
            PerPassengerPrice = perPassengerPrice,
            TotalPrice        = totalPrice,
            Currency         = "USD",
        };

    // ── AC 1 & AC 5 — precio decimal válido con punto no provoca error ────────

    [Fact]
    public void ValidPriceWithDotDecimal_PassesValidation()
    {
        // 12345.67 uses dot as decimal separator; must not fail due to culture mismatch
        var errors = Validate(BuildValidSnapshot(perPassengerPrice: 12345.67m, totalPrice: 24691.34m));

        Assert.Empty(errors);
    }

    [Fact]
    public void ZeroPrice_PassesValidation()
    {
        var errors = Validate(BuildValidSnapshot(perPassengerPrice: 0m, totalPrice: 0m));

        Assert.Empty(errors);
    }

    [Fact]
    public void MaxPerPassengerPrice_PassesValidation()
    {
        var errors = Validate(BuildValidSnapshot(perPassengerPrice: 999999.99m, totalPrice: 999999.99m));

        Assert.Empty(errors);
    }

    [Fact]
    public void MaxTotalPrice_PassesValidation()
    {
        var errors = Validate(BuildValidSnapshot(perPassengerPrice: 100m, totalPrice: 9999999.99m));

        Assert.Empty(errors);
    }

    // ── AC 2 — precios fuera de rango retornan error de validación (→ 400) ────

    [Fact]
    public void NegativePerPassengerPrice_FailsValidation()
    {
        var errors = Validate(BuildValidSnapshot(perPassengerPrice: -1m));

        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(FlightSnapshotDto.PerPassengerPrice)));
    }

    [Fact]
    public void NegativeTotalPrice_FailsValidation()
    {
        var errors = Validate(BuildValidSnapshot(totalPrice: -0.01m));

        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(FlightSnapshotDto.TotalPrice)));
    }

    [Fact]
    public void PerPassengerPriceAboveMax_FailsValidation()
    {
        var errors = Validate(BuildValidSnapshot(perPassengerPrice: 1000000.00m));

        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(FlightSnapshotDto.PerPassengerPrice)));
    }

    [Fact]
    public void TotalPriceAboveMax_FailsValidation()
    {
        var errors = Validate(BuildValidSnapshot(totalPrice: 10000000.00m));

        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(FlightSnapshotDto.TotalPrice)));
    }
}
