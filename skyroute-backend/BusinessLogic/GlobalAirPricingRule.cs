using SkyRoute.Api.Interfaces.IBusinessLogic;
using SkyRoute.Api.Providers;

namespace SkyRoute.Api.BusinessLogic;

/// <summary>
/// GlobalAir pricing: base fare + 15% fuel surcharge, rounded to 2 decimal places.
/// </summary>
public sealed class GlobalAirPricingRule : IPricingRule
{
    public string ProviderName => ProviderNames.GlobalAir;

    public decimal Calculate(decimal baseFare) =>
        Math.Round(baseFare * 1.15m, 2, MidpointRounding.AwayFromZero);
}
