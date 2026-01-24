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

    public static Result<Venue, Error> Create(
        string prefix,
        string name,
        int seatsLimit,
        IEnumerable<Seat> seats)
    {
        if (seatsLimit <= 0)
        {
            return Error.Validation("venue.seatsLimit", "Seats limit must be greater than zero");
        }

        var venueNameResult = VenueName.Create(prefix, name);
        if (venueNameResult.IsFailure)
        {
            return venueNameResult.Error;
        }

        var venueSeats = seats.ToList();

        if (venueSeats.Count < 1)
        {
            return Error.Validation("venue.seats", "Number of seats can not be zero");
        }

        if (venueSeats.Count > seatsLimit)
        {
            return Error.Validation("venue.seats", "Number of seats exceeds the venue's seat limit");
        }

        return new Venue(new VenueId(Guid.NewGuid()), venueNameResult.Value, seatsLimit, venueSeats);
    }
}