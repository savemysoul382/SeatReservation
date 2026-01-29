// SeatReservation.Application

using SeatReservation.Contracts.Seats;

namespace SeatReservation.Contracts.VenuesFolder;

public record UpdateVenueSeatsRequest(Guid VenueId, IEnumerable<UpdateSeatRequest> Seats);