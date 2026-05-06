namespace SkyRoute.Api.Interfaces.IBusinessLogic;

/// <summary>
/// Pricing rule for a specific flight provider.
/// Implementations are resolved as IEnumerable&lt;IPricingRule&gt; and matched by ProviderName.
/// </summary>
public interface IPricingRule
{
    /// <summary>Provider name this rule applies to — must match FlightOfferDto.Provider exactly.</summary>
    string ProviderName { get; }

    /// <summary>
    /// Calculates the per-passenger price from a raw base fare.
    /// </summary>
    decimal Calculate(decimal baseFare);
}
