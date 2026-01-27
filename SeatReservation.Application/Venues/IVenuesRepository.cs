// SeatReservation.Application

using CSharpFunctionalExtensions;
using SeatReservation.Domain.Venues;
using Shared;

namespace SeatReservation.Application.Venues;

public interface IVenuesRepository
{
    Task<Result<Venue, Error>> GetById(VenueId id, CancellationToken ct);

    Task<Result<Venue, Error>> GetByIdWithSeats(VenueId id, CancellationToken ct);

    Task<IReadOnlyList<Venue>> GetByPrefix(string prefix, CancellationToken ct);

    Task<Result<Guid, Error>> Add(Venue venue, CancellationToken ct = default);

    Task Update(Venue venue);

    Task<Result<Guid, Error>> UpdateVenueName(VenueId venueId, VenueName venueName, CancellationToken ct);

    Task<UnitResult<Error>> UpdateVenueNameByPrefix(string prefix, VenueName venueName, CancellationToken ct);

    Task<UnitResult<Error>> DeleteSeatsByVenueId(VenueId venueId, CancellationToken ct);
}