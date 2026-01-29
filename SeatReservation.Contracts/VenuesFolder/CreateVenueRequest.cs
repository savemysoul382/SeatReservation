// SeatReservation.Application


// SeatReservation.Application

using SeatReservation.Contracts.Seats;

namespace SeatReservation.Contracts.VenuesFolder;

public record CreateVenueRequest(string Name, string Prefix, int SeatsLimit, IEnumerable<CreateSeatRequest> Seats);