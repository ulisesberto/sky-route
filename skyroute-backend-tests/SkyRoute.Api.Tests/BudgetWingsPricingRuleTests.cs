using SkyRoute.Api.BusinessLogic;

namespace SkyRoute.Api.Tests;

public sealed class BudgetWingsPricingRuleTests
{
    // ── CA-1: ProviderName ────────────────────────────────────────────────────

    [Fact]
    public void ProviderName_IsBudgetWings()
    {
        var rule = new BudgetWingsPricingRule();

        Assert.Equal("BudgetWings", rule.ProviderName);
    }

    // ── CA-2: Calculate — valor estándar ──────────────────────────────────────

    [Fact]
    public void Calculate_StandardFare_AppliesDiscount()
    {
        var rule = new BudgetWingsPricingRule();

        // 100 * 0.90 = 90.00 → Math.Max(90.00, 29.99) = 90.00
        var result = rule.Calculate(100m);

        Assert.Equal(90.00m, result);
    }

    // ── CA-3: base fare = 0 — retorna mínimo ─────────────────────────────────

    [Fact]
    public void Calculate_ZeroBaseFare_ReturnsMinimumPrice()
    {
        var rule = new BudgetWingsPricingRule();

        // 0 * 0.90 = 0.00 → Math.Max(0.00, 29.99) = 29.99
        var result = rule.Calculate(0m);

        Assert.Equal(29.99m, result);
    }

    // ── CA-3: tarifa baja — retorna mínimo ───────────────────────────────────

    [Fact]
    public void Calculate_LowFare_ReturnsMinimumPrice()
    {
        var rule = new BudgetWingsPricingRule();

        // 10 * 0.90 = 9.00 → Math.Max(9.00, 29.99) = 29.99
        var result = rule.Calculate(10m);

        Assert.Equal(29.99m, result);
    }

    // ── CA-3 (caso borde crítico): tarifa exactamente en el límite ────────────

    [Fact]
    public void Calculate_FareAtExactBoundary_ReturnsMinimumPrice()
    {
        var rule = new BudgetWingsPricingRule();

        // 33.32 * 0.90 = 29.988 → Math.Round(29.988, 2, AwayFromZero) = 29.99
        // → Math.Max(29.99, 29.99) = 29.99
        var result = rule.Calculate(33.32m);

        Assert.Equal(29.99m, result);
    }

    // ── CA-3: tarifa justo encima del límite — aplica descuento real ──────────

    [Fact]
    public void Calculate_FareJustAboveBoundary_ExceedsMinimum()
    {
        var rule = new BudgetWingsPricingRule();

        // 33.34 * 0.90 = 30.006 → Math.Round(30.006, 2, AwayFromZero) = 30.01
        // → Math.Max(30.01, 29.99) = 30.01
        var result = rule.Calculate(33.34m);

        Assert.Equal(30.01m, result);
    }

    // ── CA-2: tarifa grande — precisión correcta ──────────────────────────────

    [Fact]
    public void Calculate_LargeFare_CorrectPrecision()
    {
        var rule = new BudgetWingsPricingRule();

        // 999.99 * 0.90 = 899.991 → Math.Round(899.991, 2, AwayFromZero) = 899.99
        // → Math.Max(899.99, 29.99) = 899.99
        Assert.Equal(899.99m, rule.Calculate(999.99m));
    }

    // ── CA-3: resultado nunca cae por debajo del mínimo ───────────────────────

    [Fact]
    public void Calculate_ResultNeverBelowMinimum()
    {
        var rule = new BudgetWingsPricingRule();
        decimal[] lowFares = [0m, 1m, 5m, 10m, 20m, 29m, 33m];

        foreach (var fare in lowFares)
        {
            var result = rule.Calculate(fare);
            Assert.True(result >= 29.99m, $"Expected result >= 29.99 for baseFare={fare}, but got {result}");
        }
    }
}
