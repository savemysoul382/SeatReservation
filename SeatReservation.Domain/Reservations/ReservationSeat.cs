// SeatReservation.Domain

using SeatReservation.Domain.Venues;

namespace SeatReservation.Domain.Reservations;

public record ReservationSeatId(Guid Value);

public class ReservationSeat
{
    public ReservationSeat(ReservationSeatId id, Reservation reservation, SeatId seatId)
    {
        Id = id;
        Reservation = reservation;
        SeatId = seatId;
        ReservedAt = DateTime.UtcNow;
    }

    // EF Core ctor
    private ReservationSeat()
    {
    }

    public ReservationSeatId Id { get; set; }

    public Reservation Reservation { get; private set; }

    public SeatId SeatId { get; private set; }

    public DateTime ReservedAt { get; set; }
}