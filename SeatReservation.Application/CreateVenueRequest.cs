// SeatReservation.Application

namespace SeatReservation.Application;

public record CreateVenueRequest(string Name, string prefix, int SeatsLimit, IEnumerable<CreateSeatRequest> Seats);