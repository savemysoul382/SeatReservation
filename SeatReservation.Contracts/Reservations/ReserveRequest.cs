// SeatReservation.Contracts

namespace SeatReservation.Contracts.Reservations;

public record ReserveRequest(Guid EventId, Guid UserId, IEnumerable<Guid> SeatsIds);
