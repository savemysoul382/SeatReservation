// SeatReservation.Domain

namespace SeatReservation.Domain.Reservations;

public class Reservation
{
    private readonly List<ReservationSeat> _reservedSeats;

    public Reservation(Guid id, Guid eventId, Guid userId, IEnumerable<Guid> seatIds)
    {
        Id = id;
        EventId = eventId;
        UserId = userId;
        Status = ReservationStatus.PENDING;
        CreatedAt = DateTime.UtcNow;

        List<ReservationSeat> reservationSeats = seatIds.Select(seatId => new ReservationSeat(Guid.NewGuid(), this, seatId, DateTime.UtcNow)).ToList();
        _reservedSeats = reservationSeats;
    }

    public Guid Id { get; private set; }

    public Guid EventId { get; private set; }

    public Guid UserId { get; private set; }

    public ReservationStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public IReadOnlyList<ReservationSeat> ReservedSeats => this._reservedSeats;
}