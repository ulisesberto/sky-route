namespace SkyRoute.Api.Configuration;

/// <summary>
/// Static lookup table mapping IATA airport codes to ISO 3166-1 alpha-2 country codes.
/// Used by the flight aggregator to determine whether a route is domestic or international.
/// </summary>
public static class AirportRegistry
{
    private static readonly Dictionary<string, string> CountryCodes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            { "EZE", "AR" },
            { "AEP", "AR" },
            { "SCL", "CL" },
            { "LIM", "PE" },
            { "MIA", "US" },
            { "JFK", "US" },
        };

    /// <summary>
    /// Returns the ISO country code for the given airport code, or <c>null</c> if unknown.
    /// </summary>
    public static string? GetCountryCode(string airportCode) =>
        CountryCodes.TryGetValue(airportCode, out var country) ? country : null;

    /// <summary>
    /// Returns <c>true</c> when origin and destination belong to different countries.
    /// Unknown airport codes are treated as distinct countries to err on the side of caution.
    /// </summary>
    public static bool IsInternational(string originCode, string destinationCode)
    {
        var originCountry      = GetCountryCode(originCode);
        var destinationCountry = GetCountryCode(destinationCode);

        if (originCountry is null || destinationCountry is null)
            return true;

        return !string.Equals(originCountry, destinationCountry, StringComparison.OrdinalIgnoreCase);
    }
}
