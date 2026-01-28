// SeatReservation.Domain

using SeatReservation.Domain.Venues;

namespace SeatReservation.Domain.Reservations;

public record ReservationSeatId(Guid Value);

public class ReservationSeat
{
    public ReservationSeat(ReservationSeatId id, Reservation reservation, SeatId seatId, Guid eventId)
    {
        Id = id;
        Reservation = reservation;
        SeatId = seatId;
        EventId = eventId;
        ReservedAt = DateTime.UtcNow;
    }

    // EF Core ctor
    private ReservationSeat()
    {
    }

    public ReservationSeatId Id { get; set; }

    public Reservation Reservation { get; private set; }

    public SeatId SeatId { get; private set; }

    // денормализация, но она тут нужна
    public Guid EventId { get; private set; }

    public DateTime ReservedAt { get; set; }
}