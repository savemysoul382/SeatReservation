// SeatReservation.Infrastructure.Postgres

using CSharpFunctionalExtensions;
using SeatReservation.Domain.Reservations;
using SeatReservation.Domain.Venues;
using Shared;

namespace SeatReservation.Application.Reservations;

public interface IReservationsRepository
{
    Task<Result<Guid, Error>> Add(Reservation reservation);

    Task<bool> AnySeatsAlreadyReserved(Guid eventId, IEnumerable<SeatId> seatIds, CancellationToken ct);
}