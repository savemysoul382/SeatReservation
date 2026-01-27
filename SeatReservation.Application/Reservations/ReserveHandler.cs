// SeatReservation.Application

using CSharpFunctionalExtensions;
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

    public ReserveHandler(
        IReservationsRepository reservationsRepository,
        IEventsRepository eventsRepository,
        ISeatsRepository seatsRepository)
    {
        _reservationsRepository = reservationsRepository;
        _eventsRepository = eventsRepository;
        _seatsRepository = seatsRepository;
    }

    public async Task<Result<Guid, Error>> Handle(ReserveRequest request, CancellationToken ct)
    {
        // Бронирование мест на мероприятии

        // 1. Валидация входных параметров. Пропускаем тут пока, делается с помощью FluentValidation

        // 2. Доступно ли мероприятие для бронирования. Проверить даты, статус
        var eventId = new EventId(request.EventId);
        var eventResult = await _eventsRepository.GetById(eventId, ct);
        if (eventResult.IsFailure)
        {
            return eventResult.Error;
        }

        if (eventResult.Value.isAvailableForReservation() == false)
        {
            return Error.Failure("reservation.unavailable", "Event is not available for booking at this date");
        }

        // 3. Проверить, что места принадлежат той площадке и мероприятию
        var seatIds = request.SeatsIds.Select(id => new SeatId(id)).ToList();
        var seatsResult = await _seatsRepository.GetByIds(seatIds, ct);


        if (seatsResult.Any(seat => seat.VenueId != eventResult.Value.VenueId) && seatsResult.Count == 0)
        {
            return Error.Conflict("seat.conflict", "Seat does not belong to venue");
        }

        // 4. Проверить, что места не забронированы и не проданы
        var isSeatsReserved = await _reservationsRepository.AnySeatsAlreadyReserved(request.EventId, seatIds, ct);
        if (isSeatsReserved)
        {
            return Error.Conflict("seat.conflict", "One or more seats are already reserved");
        }

        // Создать Reservation с ReservedSeats
        var reservationResult = Reservation.Create(request.EventId, request.UserId, request.SeatsIds);
        if (reservationResult.IsFailure)
        {
            return reservationResult.Error;
        }

        var reservation = reservationResult.Value;

        // Сохранить бронирование в базу данных
        (bool _, bool isFailure, Guid value, Error? error) = await _reservationsRepository.Add(reservation);
        if (isFailure)
        {
            return error;
        }

        return reservation.Id.Value;
    }
}