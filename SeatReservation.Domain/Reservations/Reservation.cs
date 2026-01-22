// SeatReservation.Domain

namespace SeatReservation.Domain.Reservations;

public record ReservationId(Guid Value);

public class Reservation
{
    private readonly List<ReservationSeat> _reservedSeats;

    public Reservation(ReservationId id, Guid eventId, Guid userId, IEnumerable<Guid> seatIds)
    {
        Id = id;
        EventId = eventId;
        UserId = userId;
        Status = ReservationStatus.PENDING;
        CreatedAt = DateTime.UtcNow;

        List<ReservationSeat> reservationSeats = seatIds.Select(seatId => new ReservationSeat(new ReservationSeatId(Guid.NewGuid()), this, seatId, DateTime.UtcNow)).ToList();
        _reservedSeats = reservationSeats;
    }

    // EF Core ctor
    private Reservation()
    {
    }

    public ReservationId Id { get; private set; }

    public Guid EventId { get; private set; }

    public Guid UserId { get; private set; }

    public ReservationStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public IReadOnlyList<ReservationSeat> ReservedSeats => this._reservedSeats;
}