// SeatReservation.Infrastructure.Postgres

using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SeatReservation.Application.Reservations;
using SeatReservation.Domain.Reservations;
using SeatReservation.Domain.Venues;
using Shared;

namespace SeatReservation.Infrastructure.Postgres.Repositories;

public class ReservationsRepository : IReservationsRepository
{
    private readonly ReservationServiceDbContext _dbContext;
    private readonly ILogger<ReservationsRepository> _logger;

    public ReservationsRepository(ReservationServiceDbContext dbContext, ILogger<ReservationsRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> Add(Reservation reservation)
    {
        try
        {
            await _dbContext.Reservations.AddAsync(reservation);
            await _dbContext.SaveChangesAsync();

            return reservation.Id.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding reservation");
            return Error.Failure("venue.insert", "Fail to insert venue");
        }
    }

    public async Task<bool> AnySeatsAlreadyReserved(Guid eventId, IEnumerable<SeatId> seatIds, CancellationToken ct)
    {
        var hasReservedSeats = await _dbContext.Reservations
            .Where(r => r.EventId == eventId)
            .Where(r => r.ReservedSeats.Any(rs => seatIds.Contains(rs.SeatId)))
            .AnyAsync(ct);

        return hasReservedSeats;
    }
}