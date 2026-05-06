using Microsoft.EntityFrameworkCore;
using SkyRoute.Api.Interfaces.IRepositories;
using SkyRoute.Api.Models;

namespace SkyRoute.Api.Persistence;

public sealed class BookingRepository(SkyRouteDbContext db) : IBookingRepository
{
    public async Task AddAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        await db.Bookings.AddAsync(booking, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
