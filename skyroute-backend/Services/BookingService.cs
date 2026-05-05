using Microsoft.EntityFrameworkCore;
using SkyRoute.Api.DTOs;
using SkyRoute.Api.Exceptions;
using SkyRoute.Api.Interfaces.IBusinessLogic;
using SkyRoute.Api.Interfaces.IRepositories;
using SkyRoute.Api.Interfaces.IServices;
using SkyRoute.Api.Models;

namespace SkyRoute.Api.Services;

public sealed class BookingService(
    IBookingRepository bookingRepository,
    IBookingReferenceBusinessLogic bookingReferenceBusinessLogic,
    IDocumentValidationBusinessLogic documentValidation) : IBookingService
{
    public async Task<CreateBookingResponseDto> CreateAsync(
        CreateBookingRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var f = request.Flight;
        var p = request.Passenger;
        documentValidation.EnsureDocumentValid(f.IsInternational, p.DocumentNumber);

        var reference = bookingReferenceBusinessLogic.Generate();

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            BookingReference = reference,
            Provider = f.Provider,
            FlightNumber = f.FlightNumber,
            Origin = f.Origin,
            Destination = f.Destination,
            DepartureTimeUtc = f.DepartureTime.UtcDateTime,
            ArrivalTimeUtc = f.ArrivalTime.UtcDateTime,
            CabinClass = f.CabinClass,
            Passengers = request.Passengers,
            PerPassengerPrice = f.PerPassengerPrice,
            TotalPrice = f.TotalPrice,
            Currency = f.Currency,
            IsInternational = f.IsInternational,
            PassengerFullName = p.FullName,
            PassengerEmail = p.Email,
            PassengerDocumentNumber = p.DocumentNumber,
            CreatedAtUtc = DateTime.UtcNow
        };

        await bookingRepository.AddAsync(booking, cancellationToken);
        try
        {
            await bookingRepository.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            throw new BookingSaveFailedException(ex);
        }

        return new CreateBookingResponseDto { BookingReference = reference };
    }
}
