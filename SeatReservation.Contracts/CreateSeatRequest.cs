// SeatReservation.Application

namespace SeatReservation.Contracts;

public record CreateSeatRequest(int RowNumber, bool SeatNumber);