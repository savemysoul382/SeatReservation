// SeatReservation.Application

using CSharpFunctionalExtensions;
using SeatReservation.Application.DataBase;
using SeatReservation.Contracts.VenuesFolder;
using SeatReservation.Domain.Venues;
using Shared;

namespace SeatReservation.Application.Venues.Commands;

public class UpdateVenueSeatsHandler
{
    private readonly IVenuesRepository _repository;
    private readonly ITransactionManager _transactionManager;

    public UpdateVenueSeatsHandler(IVenuesRepository repository, ITransactionManager transactionManager)
    {
        _repository = repository;
        _transactionManager = transactionManager;
    }

    public async Task<Result<Guid, Error>> Handle(UpdateVenueSeatsRequest request, CancellationToken ct)
    {
        var venueId = new VenueId(Value: request.VenueId);

        Result<ITransactionScope, Error> transactionScopeResult = await _transactionManager.BeginTransactionAsync(ct);
        if (transactionScopeResult.IsFailure)
        {
            return transactionScopeResult.Error;
        }

        using var transactionScope = transactionScopeResult.Value;

        var venue = await _repository.GetById(venueId, ct);
        if (venue.IsFailure)
        {
            transactionScope.Rollback();
            return venue.Error;
        }

        List<Seat> seats = new List<Seat>();
        foreach (var st in request.Seats)
        {
            var seat = Seat.Create(
                venueId: venueId,
                rowNumber: st.RowNumber,
                seatNumber: st.SeatNumber);
            if (seat.IsFailure)
            {
                transactionScope.Rollback();
                return seat.Error;
            }

            seats.Add(item: seat.Value);
        }

        UnitResult<Error> updateSeats = venue.Value.UpdateSeats(seats);
        if (updateSeats.IsFailure)
        {
            transactionScope.Rollback();
            return updateSeats.Error;
        }

        await _repository.DeleteSeatsByVenueId(venueId, ct);

        await _transactionManager.SaveChangesAsync(ct);

        UnitResult<Error> commitedResult = transactionScope.Commit();
        if (commitedResult.IsFailure)
        {
            return commitedResult.Error;
        }

        return venueId.Value;
    }
}