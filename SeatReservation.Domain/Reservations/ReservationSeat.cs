// SeatReservation.Domain

namespace SeatReservation.Domain.Reservations;

public record ReservationSeatId(Guid Value);

public class ReservationSeat
{
    public ReservationSeat(ReservationSeatId id, Reservation reservation, Guid seatId, DateTime reservedAt)
    {
        Id = id;
        Reservation = reservation;
        SeatId = seatId;
        ReservedAt = reservedAt;
    }

    // EF Core ctor
    private ReservationSeat()
    {
    }

    public ReservationSeatId Id { get; set; }

    public Reservation Reservation { get; private set; }

    public Guid SeatId { get; private set; }

    public DateTime ReservedAt { get; set; }
}