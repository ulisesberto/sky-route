using SkyRoute.Api.Configuration;

namespace SkyRoute.Api.Tests;

public sealed class AirportRegistryTests
{
    [Theory]
    [InlineData("EZE", "AR")]
    [InlineData("AEP", "AR")]
    [InlineData("SCL", "CL")]
    [InlineData("LIM", "PE")]
    [InlineData("MIA", "US")]
    [InlineData("JFK", "US")]
    public void GetCountryCode_KnownAirport_ReturnsCorrectCode(string iata, string expected) =>
        Assert.Equal(expected, AirportRegistry.GetCountryCode(iata));

    [Fact]
    public void GetCountryCode_UnknownAirport_ReturnsNull() =>
        Assert.Null(AirportRegistry.GetCountryCode("ZZZ"));

    [Fact]
    public void GetCountryCode_IsCaseInsensitive() =>
        Assert.Equal("AR", AirportRegistry.GetCountryCode("eze"));

    [Theory]
    [InlineData("EZE", "MIA", true)]   // AR → US
    [InlineData("EZE", "JFK", true)]   // AR → US
    [InlineData("SCL", "LIM", true)]   // CL → PE
    [InlineData("MIA", "JFK", false)]  // US → US
    [InlineData("EZE", "AEP", false)]  // AR → AR
    public void IsInternational_ReturnsExpected(string origin, string destination, bool expected) =>
        Assert.Equal(expected, AirportRegistry.IsInternational(origin, destination));

    [Fact]
    public void IsInternational_UnknownOrigin_ReturnsTrue() =>
        Assert.True(AirportRegistry.IsInternational("XXX", "EZE"));

    [Fact]
    public void IsInternational_UnknownDestination_ReturnsTrue() =>
        Assert.True(AirportRegistry.IsInternational("EZE", "XXX"));
}
