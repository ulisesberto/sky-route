namespace SkyRoute.Api.Providers;

/// <summary>
/// Canonical provider name constants shared by IFlightProvider implementations and IPricingRule implementations.
/// Using these constants instead of literals prevents silent matching failures in the aggregator.
/// </summary>
public static class ProviderNames
{
    public const string GlobalAir    = "GlobalAir";
    public const string BudgetWings  = "BudgetWings";
}
