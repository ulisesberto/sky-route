using SkyRoute.Api.Models;

namespace SkyRoute.Api.Interfaces.IRepositories;

public interface IBookingRepository
{
    Task AddAsync(Booking booking, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
