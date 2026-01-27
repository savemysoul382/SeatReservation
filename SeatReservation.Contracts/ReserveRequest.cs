// SeatReservation.Contracts

namespace SeatReservation.Contracts;

public record ReserveRequest(Guid EventId, Guid UserId, IEnumerable<Guid> SeatsIds);
