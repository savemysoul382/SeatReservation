// SeatReservation.Domain

using CSharpFunctionalExtensions;
using Shared;

namespace SeatReservation.Domain.Venues;

public record VenueId(Guid Value);

public class Venue
{
    private readonly List<Seat> _seats = [];

    public Venue(VenueId id, VenueName name, int seatsLimit, IEnumerable<Seat> seats)
    {
        Id = id;
        Name = name;
        SeatsLimit = seatsLimit;
        this._seats = seats.ToList();
    }

    // EF Core ctor
    private Venue()
    {
    }

    public VenueId Id { get; private set; } = null!;

    public VenueName Name { get; private set; } = null!;

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