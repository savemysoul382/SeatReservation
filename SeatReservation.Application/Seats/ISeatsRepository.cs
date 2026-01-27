// SeatReservation.Infrastructure.Postgres

using SeatReservation.Domain.Venues;

namespace SeatReservation.Application.Seats;

public interface ISeatsRepository
{
    Task<IReadOnlyList<Seat>> GetByIds(IEnumerable<SeatId> seatIds, CancellationToken ct);
}