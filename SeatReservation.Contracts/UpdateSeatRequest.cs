// SeatReservation.Application

namespace SeatReservation.Contracts;

public record UpdateSeatRequest(int RowNumber, int SeatNumber);