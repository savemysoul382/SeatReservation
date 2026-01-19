// SeatReservation.Domain

using CSharpFunctionalExtensions;

namespace SeatReservation.Domain.Venue;

public class Venue
{
    private List<Seat> _seats = new List<Seat>();

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
        if (this._seats.Count >= SeatsLimit)
        {
            return UnitResult.Failure<Error>(Error("Max seats count exceeded"));
        }

        this._seats.Add(seat);
        return UnitResult.Success<Error>();
    }

    public void ExpandSeatsLimit(int newSeatsLimit)
    {
        SeatsLimit = newSeatsLimit;
    }
}