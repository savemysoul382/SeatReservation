// SeatReservation.Application

using CSharpFunctionalExtensions;
using SeatReservation.Application.DataBase;
using SeatReservation.Application.EventsFolder;
using SeatReservation.Application.Seats;
using SeatReservation.Contracts;
using SeatReservation.Domain.Events;
using SeatReservation.Domain.Reservations;
using SeatReservation.Domain.Venues;
using Shared;

namespace SeatReservation.Application.Reservations;

public class ReserveHandler
{
    private readonly IReservationsRepository _reservationsRepository;
    private readonly IEventsRepository _eventsRepository;
    private readonly ISeatsRepository _seatsRepository;
    private readonly ITransactionManager _transactionManager;

    public ReserveHandler(
        IReservationsRepository reservationsRepository,
        IEventsRepository eventsRepository,
        ISeatsRepository seatsRepository,
        ITransactionManager transactionManager)
    {
        _reservationsRepository = reservationsRepository;
        _eventsRepository = eventsRepository;
        _seatsRepository = seatsRepository;
        _transactionManager = transactionManager;
    }

    public async Task<Result<Guid, Error>> Handle(ReserveRequest request, CancellationToken ct)
    {
        // Бронирование мест на мероприятии

        // 1. Валидация входных параметров. Пропускаем тут пока, делается с помощью FluentValidation
        Result<ITransactionScope, Error> beginTransaction = await _transactionManager.BeginTransactionAsync(ct);
        if (beginTransaction.IsFailure)
        {
            return beginTransaction.Error;
        }

        using var transactionScope = beginTransaction.Value;

        // 2. Доступно ли мероприятие для бронирования. Проверить даты, статус
        var eventId = new EventId(request.EventId);
        var eventResult = await _eventsRepository.GetByIdWithLock(eventId, ct);
        if (eventResult.IsFailure)
        {
            transactionScope.Rollback();
            return eventResult.Error;
        }

        var reservedSeatsCount = await _reservationsRepository.GetReservedSeatsCount(request.EventId, ct);

        if (eventResult.Value.IsAvailableForReservation(reservedSeatsCount + request.SeatsIds.Count()) == false)
        {
            transactionScope.Rollback();
            return Error.Failure("reservation.unavailable", "Event is not available for booking at this date");
        }

        // 3. Проверить, что места принадлежат той площадке и мероприятию

        // 4. Проверить, что места не забронированы и не проданы. Больше не нужно делать, тк проверка внесена в базу данных через индекс по 2 полям (SeatId, EventId)
        // var isSeatsReserved = await _reservationsRepository.AnySeatsAlreadyReserved(request.EventId, seatIds, ct);
        // if (isSeatsReserved)
        // {
        //    return Error.Conflict("seat.conflict", "One or more seats are already reserved");
        // }

        // Создать Reservation с ReservedSeats
        var reservationResult = Reservation.Create(request.EventId, request.UserId, request.SeatsIds);
        if (reservationResult.IsFailure)
        {
            transactionScope.Rollback();
            return reservationResult.Error;
        }

        var reservation = reservationResult.Value;

        // Сохранить бронирование в базу данных
        (bool _, bool isFailure, Guid value, Error? error) = await _reservationsRepository.Add(reservation);
        if (isFailure)
        {
            transactionScope.Rollback();
            return error;
        }

        eventResult.Value.Details.ReserveSeat();

        UnitResult<Error> saveResult = await _transactionManager.SaveChangesAsync(ct);
        if (saveResult.IsFailure)
        {
            transactionScope.Rollback();
            return saveResult.Error;
        }

        UnitResult<Error> commitResult = transactionScope.Commit();
        if (commitResult.IsFailure)
        {
            return commitResult.Error;
        }

        return reservation.Id.Value;
    }
}