using SkyRoute.Api.Models;

namespace SkyRoute.Api.BusinessLogic;

/// <summary>
/// Per-cabin price multiplier applied after provider pricing rules.
/// Economy × 1.0, Business × 1.6, First × 2.5.
/// </summary>
public static class CabinPriceMultiplier
{
    public static decimal GetMultiplier(CabinClass cabin) => cabin switch
    {
        CabinClass.Economy  => 1.0m,
        CabinClass.Business => 1.6m,
        CabinClass.First    => 2.5m,
        _ => throw new ArgumentOutOfRangeException(nameof(cabin), cabin, null)
    };
}
