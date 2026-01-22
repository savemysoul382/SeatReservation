// SeatReservation.Domain

using CSharpFunctionalExtensions;
using Shared;

namespace SeatReservation.Domain.Venues;

public record SeatId(Guid Value);

public class Seat
{
    private Seat(SeatId id, int rowNumber, int seatNumber)
    {
        Id = id;
        RowNumber = rowNumber;
        SeatNumber = seatNumber;
    }

    // EF Core ctor
    private Seat()
    {
    }

    public SeatId Id { get; }

    // можно не указывать связь, но она есть. Место не может существовать вне зала
    // public Venue Venue { get; private set; } = null!;
    public int RowNumber { get; private set; }

    public int SeatNumber { get; private set; }

    public static Result<Seat, Error> Create(int rowNumber, int seatNumber)
    {
        if (rowNumber <= 0 || seatNumber <= 0)
        {
            return Error.Validation("seat.rowNumber", "Row number and seat number must be greater than zero");
        }

        return new Seat(id: new SeatId(Guid.NewGuid()), rowNumber: rowNumber, seatNumber: seatNumber);
    }
}