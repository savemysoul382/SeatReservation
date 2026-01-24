// SeatReservation.Application

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SeatReservation.Domain.Venues;

namespace SeatReservation.Application.DataBase;

public interface IReservationServiceDbContext
{
    DbSet<Venue> Venues { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    ChangeTracker ChangeTracker { get; }
}