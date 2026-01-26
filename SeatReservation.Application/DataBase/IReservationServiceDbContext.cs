// SeatReservation.Application

using CSharpFunctionalExtensions;
using SeatReservation.Domain.Venues;
using Shared;

namespace SeatReservation.Application.DataBase;

public interface IVenuesRepository
{
    Task<Result<Guid, Error>> Add(Venue venue, CancellationToken ct = default);

    Task<Result<Guid, Error>> UpdateVenueName(VenueId venueId, VenueName venueName, CancellationToken ct);

    Task<UnitResult<Error>> UpdateVenueNameByPrefix(string prefix, VenueName venueName, CancellationToken ct);
}