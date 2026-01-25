// SeatReservation.Application

namespace SeatReservation.Application.Venues;

public record CreateVenueRequest(string Name, string prefix, int SeatsLimit, IEnumerable<CreateSeatRequest> Seats);