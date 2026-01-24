// SeatReservation.Application

namespace SeatReservation.Contracts;

public record CreateVenueRequest(string Name, int SeatsLimit, IEnumerable<CreateSeatRequest> Seats);