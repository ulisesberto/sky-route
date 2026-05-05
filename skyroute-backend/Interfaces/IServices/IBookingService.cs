using SkyRoute.Api.DTOs;

namespace SkyRoute.Api.Interfaces.IServices;

public interface IBookingService
{
    Task<CreateBookingResponseDto> CreateAsync(CreateBookingRequestDto request, CancellationToken cancellationToken = default);
}
