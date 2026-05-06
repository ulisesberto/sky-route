using SkyRoute.Api.BusinessLogic;
using SkyRoute.Api.Interfaces.IBusinessLogic;
using SkyRoute.Api.Interfaces.IRepositories;
using SkyRoute.Api.Interfaces.IServices;
using SkyRoute.Api.Persistence;
using SkyRoute.Api.Providers;
using SkyRoute.Api.Services;

namespace SkyRoute.Api.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddSkyRouteApplication(this IServiceCollection services)
    {
        // Flight providers — registered as IEnumerable<IFlightProvider> so the aggregator
        // can resolve all of them without hardcoding each type.
        services.AddTransient<IFlightProvider, GlobalAirProvider>();

        services.AddScoped<IFlightSearchService, FlightSearchService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddSingleton<IBookingReferenceBusinessLogic, BookingReferenceBusinessLogic>();
        services.AddSingleton<IDocumentValidationBusinessLogic, DocumentValidationBusinessLogic>();
        return services;
    }
}
