// SeatReservation.Domain

namespace SeatReservation.Domain.Reservation;

public class ReservationSeat
{
    public ReservationSeat(Guid id, Reservation reservation, Guid seatId, DateTime reservedAt)
    {
        Id = id;
        Reservation = reservation;
        SeatId = seatId;
        ReservedAt = reservedAt;
    }

    public Guid Id { get; set; }
    public Reservation Reservation { get; private set; }
    public Guid SeatId { get; private set; }
    public DateTime ReservedAt { get; set; }
}