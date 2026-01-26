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
        var venueId = new VenueId(Value: request.Id);

        (_, bool isFailure, Venue? venue, Error? error) = await _repository.GetById(id: venueId, ct: ct);

        if (isFailure)
        {
            return error;
        }

        venue.UpdateName(name: request.Name);

        await _repository.Save();

        return venueId.Value;
    }
}