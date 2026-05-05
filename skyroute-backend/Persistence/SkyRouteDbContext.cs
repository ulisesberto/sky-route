using Microsoft.EntityFrameworkCore;
using SkyRoute.Api.Models;

namespace SkyRoute.Api.Persistence;

public sealed class SkyRouteDbContext(DbContextOptions<SkyRouteDbContext> options) : DbContext(options)
{
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(e =>
        {
            e.ToTable("Bookings");
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.BookingReference).IsUnique();
            e.Property(x => x.BookingReference).HasMaxLength(48).IsRequired();
            e.Property(x => x.Provider).HasMaxLength(64).IsRequired();
            e.Property(x => x.FlightNumber).HasMaxLength(32).IsRequired();
            e.Property(x => x.Origin).HasMaxLength(8).IsRequired();
            e.Property(x => x.Destination).HasMaxLength(8).IsRequired();
            e.Property(x => x.Currency).HasMaxLength(8).IsRequired();
            e.Property(x => x.PassengerFullName).HasMaxLength(200).IsRequired();
            e.Property(x => x.PassengerEmail).HasMaxLength(320).IsRequired();
            e.Property(x => x.PassengerDocumentNumber).HasMaxLength(64).IsRequired();
            e.Property(x => x.PerPassengerPrice).HasPrecision(18, 2);
            e.Property(x => x.TotalPrice).HasPrecision(18, 2);
        });
    }
}
