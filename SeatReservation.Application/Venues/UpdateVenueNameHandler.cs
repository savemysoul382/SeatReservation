// SeatReservation.Application

using CSharpFunctionalExtensions;
using SeatReservation.Application.DataBase;
using SeatReservation.Contracts;
using SeatReservation.Domain.Venues;
using Shared;

namespace SeatReservation.Application.Venues;

public class UpdateVenueNameHandler
{
    private readonly IVenuesRepository _repository;

    public UpdateVenueNameHandler(IVenuesRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Guid, Error>> Handle(UpdateVenueNameRequest request, CancellationToken ct)
    {
        var venueId = new VenueId(request.Id);
        var venueName = VenueName.CreateWithoutPrefix(request.Name);
        if (venueName.IsFailure)
        {
            return venueName.Error;
        }

        Result<Guid, Error> result = await _repository.UpdateVenueName(venueId, venueName.Value, ct);
        if (result.IsFailure)
        {
            return result.Error;
        }

        return result.Value;
    }
}