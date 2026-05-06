using SkyRoute.Api.BusinessLogic;

namespace SkyRoute.Api.Tests;

public sealed class GlobalAirPricingRuleTests
{
    // ── CA-1: ProviderName ────────────────────────────────────────────────────

    [Fact]
    public void ProviderName_IsGlobalAir()
    {
        var rule = new GlobalAirPricingRule();

        Assert.Equal("GlobalAir", rule.ProviderName);
    }

    // ── CA-2: Calculate — valor estándar ──────────────────────────────────────

    [Fact]
    public void Calculate_StandardFare_AppliesSurcharge()
    {
        var rule = new GlobalAirPricingRule();

        var result = rule.Calculate(100m);

        Assert.Equal(115.00m, result);
    }

    // ── CA-2 / CA-3: redondeo con MidpointRounding.AwayFromZero ──────────────

    [Fact]
    public void Calculate_FractionalResult_RoundsToTwoDecimals()
    {
        var rule = new GlobalAirPricingRule();

        // 10.01 * 1.15 = 11.5115 → rounds to 11.51 (2 decimal places)
        var result = rule.Calculate(10.01m);

        Assert.Equal(11.51m, result);
        // AwayFromZero is intentional — not banker's rounding (ToEven)
        // While 1.15 doesn't produce exact midpoints for finite decimals,
        // AwayFromZero ensures consistent behavior for partial-unit fares.
    }

    // ── CA-3: base fare = 0 ───────────────────────────────────────────────────

    [Fact]
    public void Calculate_ZeroBaseFare_ReturnsZero()
    {
        var rule = new GlobalAirPricingRule();

        var result = rule.Calculate(0m);

        Assert.Equal(0.00m, result);
    }

    // ── CA-3: tarifa grande — precisión correcta ──────────────────────────────

    [Fact]
    public void Calculate_LargeFare_CorrectPrecision()
    {
        var rule = new GlobalAirPricingRule();

        // 999.99 * 1.15 = 1149.9885 → rounds to 1149.99
        Assert.Equal(1149.99m, rule.Calculate(999.99m));
    }

    // ── CA-3: resultado nunca supera 2 decimales ──────────────────────────────

    [Fact]
    public void Calculate_ResultNeverExceedsTwoDecimalPlaces()
    {
        var rule = new GlobalAirPricingRule();
        decimal[] testFares = [0m, 1m, 10.01m, 100m, 100.005m, 999.99m, 1234.56m];
        foreach (var fare in testFares)
        {
            var result = rule.Calculate(fare);
            // Verify at most 2 decimal places: multiply by 100, check no fractional part
            Assert.Equal(0m, result * 100m % 1m);
        }
    }
}
