// SeatReservation.Application

using CSharpFunctionalExtensions;
using SeatReservation.Application.DataBase;
using SeatReservation.Domain.Venues;
using Shared;

namespace SeatReservation.Application;

public class CreateVenueHandler
{
    private readonly IReservationServiceDbContext _dbContext;

    public CreateVenueHandler(IReservationServiceDbContext dbContext)
    {
        _dbContext = dbContext;
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

        var entries1 = _dbContext.ChangeTracker.Entries();

        // сохранение доменных моделей в БД
        await _dbContext.Venues.AddAsync(entity: venue.Value, cancellationToken: ct);

        var entries2 = _dbContext.ChangeTracker.Entries();

        venue.Value.AddSeats(seats);

        var entries3 = _dbContext.ChangeTracker.Entries();

        await _dbContext.SaveChangesAsync(cancellationToken: ct);

        var entries4 = _dbContext.ChangeTracker.Entries();

        venue.Value.Name = new VenueName("Рок площадка", "РОК");

        var entries5 = _dbContext.ChangeTracker.Entries();

        await _dbContext.SaveChangesAsync(cancellationToken: ct);

        var entries6 = _dbContext.ChangeTracker.Entries();

        return venue.Value.Id.Value;
    }
}