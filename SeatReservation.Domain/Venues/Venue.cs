// SeatReservation.Domain

using CSharpFunctionalExtensions;
using Shared;

namespace SeatReservation.Domain.Venues;

public class Venue
{
    private readonly List<Seat> _seats = new List<Seat>();

    public Venue(Guid id, string name, int seatsLimit, IEnumerable<Seat> seats)
    {
        Id = id;
        Name = name;
        SeatsLimit = seatsLimit;
        this._seats = seats.ToList();
    }

    public Guid Id { get; set; }

    public string Name { get; private set; }

    public int SeatsLimit { get; private set; }

    public int SeatsCount => this._seats.Count;

    public IReadOnlyList<Seat> Seats => this._seats;

    public UnitResult<Error> AddSeat(Seat seat)
    {
        if (SeatsCount >= SeatsLimit)
        {
            return Error.Conflict("venue.seats.limit", "The number of places is limited");
        }

        _seats.Add(item: seat);

        return UnitResult.Success<Error>();
    }

    public void ExpandSeatsLimit(int newSeatsLimit)
    {
        SeatsLimit = newSeatsLimit;
    }
}