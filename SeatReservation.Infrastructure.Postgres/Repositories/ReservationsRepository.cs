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

    public async Task<Result<Guid, Error>> Add(Reservation reservation, CancellationToken ct)
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

        // можно сделать индекс в табл. reservation_seats (reservation_id, seat_id)
        return hasReservedSeats;
    }

    public async Task<int> GetReservedSeatsCount(Guid eventId, CancellationToken ct)
    {
        // блокировка (пессимистичная) значения для таблицы
        // FOR UPDATE - другая транзакция ждет
        // FOR UPDATE NOWAIT - другая транзакция получает ошибку сразу
        // await _dbContext.Database.ExecuteSqlRawAsync(
        // "SELECT capacity FROM events_details WHERE event_id = {0} FOR UPDATE", eventId, ct);
        return await _dbContext.Reservations
            .Where(r => r.EventId == eventId)
            .Where(r => r.Status == ReservationStatus.CONFIRMED || r.Status == ReservationStatus.PENDING)
            .SelectMany(r => r.ReservedSeats)
            .CountAsync(ct);
    }
}