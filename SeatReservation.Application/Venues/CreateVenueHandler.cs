// SeatReservation.Application

using CSharpFunctionalExtensions;
using SeatReservation.Application.DataBase;
using SeatReservation.Contracts;
using SeatReservation.Domain.Venues;
using Shared;

namespace SeatReservation.Application.Venues;

public class CreateVenueHandler
{
    private readonly IVenuesRepository _venuesRepository;

    public CreateVenueHandler(IVenuesRepository venuesRepository)
    {
        _venuesRepository = venuesRepository;
    }

    /// <summary>
    /// Создает площадку со всеми местами.
    /// </summary>
    /// <param name="request">запрос.</param>
    /// <param name="ct">токен отмены.</param>
    /// <returns>Guid сущности или ошибка.</returns>
    public async Task<Result<Guid, Error>> Handle(CreateVenueRequest request, CancellationToken ct)
    {
        // валидация входных данных

        // бизнес валидация

        // создание доменных моделей
        var venue = Venue.Create(
            prefix: request.prefix,
            name: request.Name,
            seatsLimit: request.SeatsLimit);

        if (venue.IsFailure)
        {
            return venue.Error;
        }

        List<Seat> seats = new List<Seat>();
        foreach (CreateSeatRequest createSeatRequest in request.Seats)
        {
            var seat = Seat.Create(
                venue: venue.Value,
                rowNumber: createSeatRequest.RowNumber,
                seatNumber: createSeatRequest.SeatNumber);
            if (seat.IsFailure)
            {
                return seat.Error;
            }

            seats.Add(item: seat.Value);
        }

        venue.Value.AddSeats(seats);

        // сохранение доменных моделей в БД
        Result<Guid, Error> result = await _venuesRepository.Add(venue: venue.Value, ct: ct);
        if (result.IsFailure)
        {
            return result.Error;
        }

        return result.Value;
    }
}