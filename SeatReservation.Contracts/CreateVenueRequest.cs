// SeatReservation.Application

using SeatReservation.Application.Venues;

namespace SeatReservation.Contracts;

public record CreateVenueRequest(string Name, string Prefix, int SeatsLimit, IEnumerable<CreateSeatRequest> Seats);