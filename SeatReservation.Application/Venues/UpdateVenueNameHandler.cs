// SeatReservation.Application

using CSharpFunctionalExtensions;
using SeatReservation.Application.DataBase;
using SeatReservation.Contracts.VenuesFolder;
using SeatReservation.Domain.Venues;
using Shared;

namespace SeatReservation.Application.Venues;

public class UpdateVenueNameHandler
{
    private readonly IVenuesRepository _repository;
    private readonly ITransactionManager _transactionManager;

    public UpdateVenueNameHandler(IVenuesRepository repository, ITransactionManager transactionManager)
    {
        _repository = repository;
        _transactionManager = transactionManager;
    }

    public async Task<Result<Guid, Error>> Handle(UpdateVenueNameRequest request, CancellationToken ct)
    {
        var venueId = new VenueId(Value: request.Id);

        Result<Venue, Error> venueResult = await _repository.GetById(id: venueId, ct: ct);

        if (venueResult.IsFailure)
        {
            return venueResult.Error;
        }

        venueResult.Value.UpdateName(name: request.Name);

        await _transactionManager.SaveChangesAsync(ct);

        return venueId.Value;
    }
}