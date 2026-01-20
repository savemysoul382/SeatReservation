// SeatReservation.Domain

namespace SeatReservation.Domain.Reservations;

public enum ReservationStatus
{
    /// <summary>
    /// Ожидает подтверждения
    /// </summary>
    PENDING,

    /// <summary>
    /// Подтверждено
    /// </summary>
    CONFIRMED,

    /// <summary>
    /// Отменено
    /// </summary>
    CANCELED,
}