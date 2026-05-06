using SkyRoute.Api.Interfaces.IBusinessLogic;
using SkyRoute.Api.Providers;

namespace SkyRoute.Api.BusinessLogic;

/// <summary>
/// BudgetWings pricing: base fare − 10%, rounded to 2 decimal places, with a floor of 29.99.
/// </summary>
public sealed class BudgetWingsPricingRule : IPricingRule
{
    private const decimal MinimumPrice = 29.99m;

    public string ProviderName => ProviderNames.BudgetWings;

    public decimal Calculate(decimal baseFare) =>
        Math.Max(Math.Round(baseFare * 0.90m, 2, MidpointRounding.AwayFromZero), MinimumPrice);
}
