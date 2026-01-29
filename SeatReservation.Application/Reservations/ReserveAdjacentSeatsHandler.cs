// SeatReservation.Application

using CSharpFunctionalExtensions;
using SeatReservation.Application.DataBase;
using SeatReservation.Application.EventsFolder;
using SeatReservation.Application.Seats;
using SeatReservation.Contracts.Reservations;
using SeatReservation.Domain.Events;
using SeatReservation.Domain.Reservations;
using SeatReservation.Domain.Venues;
using Shared;

namespace SeatReservation.Application.Reservations;

public class ReserveAdjacentSeatsHandler
{
    private readonly ISeatsRepository _seatsRepository;
    private readonly IReservationsRepository _reservationsRepository;
    private readonly IEventsRepository _eventsRepository;
    private readonly ITransactionManager _transactionManager;

    public ReserveAdjacentSeatsHandler(
        ISeatsRepository seatsRepository,
        IReservationsRepository reservationsRepository,
        IEventsRepository eventsRepository,
        ITransactionManager transactionManager)
    {
        _seatsRepository = seatsRepository;
        _reservationsRepository = reservationsRepository;
        _eventsRepository = eventsRepository;
        _transactionManager = transactionManager;
    }

    public async Task<Result<bool, Error>> Handle(ReserveAdjacentSeatsRequest request, CancellationToken ct)
    {
        if (request.RequiredSeatsCount <= 0)
        {
            return Error.Validation("reserveAdjacent.seatsCount", "Required seats count must be greater than zero");
        }

        if (request.RequiredSeatsCount > 10)
        {
            return Error.Validation("reserveAdjacent.seatsCount", "Cannot reserve more than 10 adjacent seats at once");
        }

        Result<ITransactionScope, Error> transactionResult = await _transactionManager.BeginTransactionAsync(ct);
        if (transactionResult.IsFailure)
        {
            return transactionResult.Error;
        }

        using ITransactionScope transactionScope = transactionResult.Value;

        (bool _, bool isFailure, Event? @event, Error? error) = await _eventsRepository.GetByIdWithLock(new EventId(request.EventId), ct);
        if (isFailure)
        {
            return error;
        }

        var availableSeats = await _seatsRepository.GetAvailableSeats(
            new VenueId(request.VenueId),
            new EventId(request.EventId),
            request.PreferredRowNumber,
            ct);

        if (availableSeats.Count == 0)
        {
            return Error.Validation("reserveAdjacent.seats", "No available seats found");
        }

        var selectedSeats = request.PreferredRowNumber.HasValue ? AdjacentSeatsFinder.FindAdjacentSeats(availableSeats, request.RequiredSeatsCount, request.PreferredRowNumber.Value) : AdjacentSeatsFinder.FindBestAdjacentSeats(availableSeats, request.RequiredSeatsCount);

        if (selectedSeats.Count == 0)
        {
            return Error.Validation("reserveAdjacent.seats", "No adjacent seats found");
        }

        if (selectedSeats.Count < request.RequiredSeatsCount)
        {
            return Error.Validation("reserveAdjacent.seats", "Not enough adjacent seats found");
        }

        var seatsId = selectedSeats.Select(s => s.Id).ToList();

        var reservationResult = Reservation.Create(
            request.EventId,
            request.UserId,
            seatsId.Select(s => s.Value));

        if (reservationResult.IsFailure)
        {
            return reservationResult.Error;
        }

        Reservation reservation = reservationResult.Value;
        Result<Guid, Error> addResult = await _reservationsRepository.Add(reservation, ct);
        if (addResult.IsFailure)
        {
            return addResult.Error;
        }

        var commitResult = transactionScope.Commit();
        if (commitResult.IsFailure)
        {
            return commitResult.Error;
        }

        return Result.Success<bool, Error>(true);
    }
}