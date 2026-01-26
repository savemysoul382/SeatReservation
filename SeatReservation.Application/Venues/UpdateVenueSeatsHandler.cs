// SeatReservation.Application

using CSharpFunctionalExtensions;
using SeatReservation.Application.DataBase;
using SeatReservation.Contracts;
using SeatReservation.Domain.Venues;
using Shared;

namespace SeatReservation.Application.Venues;

public class UpdateVenueSeatsHandler
{
    private readonly IVenuesRepository _repository;

    public UpdateVenueSeatsHandler(IVenuesRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Guid, Error>> Handle(UpdateVenueSeatsRequest request, CancellationToken ct)
    {
        var venueId = new VenueId(Value: request.VenueId);

        var venue = await _repository.GetById(venueId, ct);
        if (venue.IsFailure)
        {
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
                return seat.Error;
            }

            seats.Add(item: seat.Value);
        }

        UnitResult<Error> updateSeats = venue.Value.UpdateSeats(seats);
        if (updateSeats.IsFailure)
        {
            return updateSeats.Error;
        }

        await _repository.DeleteSeatsByVenueId(venueId, ct);

        await _repository.Save();

        return venueId.Value;
    }
}