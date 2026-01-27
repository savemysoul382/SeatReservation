// SeatReservation.Application

using CSharpFunctionalExtensions;
using SeatReservation.Contracts;
using SeatReservation.Domain.Venues;
using Shared;

namespace SeatReservation.Application.Venues;

public class UpdateVenueNameByPrefixHandler
{
    private readonly IVenuesRepository _repository;

    public UpdateVenueNameByPrefixHandler(IVenuesRepository repository)
    {
        _repository = repository;
    }

    public async Task<UnitResult<Error>> Handle(UpdateVenueNameByPrefixRequest request, CancellationToken ct)
    {
        var venueName = VenueName.CreateWithoutPrefix(request.Name);
        if (venueName.IsFailure)
        {
            return venueName.Error;
        }

        UnitResult<Error> result = await _repository.UpdateVenueNameByPrefix(request.Prefix, venueName.Value, ct);
        if (result.IsFailure)
        {
            return result.Error;
        }

        return UnitResult.Success<Error>();
    }
}