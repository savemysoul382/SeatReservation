// SeatReservation.Infrastructure.Postgres

using SeatReservation.Domain.Events;
using SeatReservation.Domain.Venues;

namespace SeatReservation.Application.Seats;

public interface ISeatsRepository
{
    Task<IReadOnlyList<Seat>> GetByIds(IEnumerable<SeatId> seatIds, CancellationToken ct);

    Task<IReadOnlyList<Seat>> GetAvailableSeats(VenueId venueId, EventId eventId, int? rowNumber, CancellationToken ct);
}