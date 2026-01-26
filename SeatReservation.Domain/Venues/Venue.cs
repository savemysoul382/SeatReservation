// SeatReservation.Domain

using CSharpFunctionalExtensions;
using Shared;

namespace SeatReservation.Domain.Venues;

public record VenueId(Guid Value);

public class Venue
{
    private List<Seat> _seats = [];

    public static Result<Venue, Error> Create(
        string prefix,
        string name,
        int seatsLimit,
        VenueId? venueId = null)
    {
        if (seatsLimit <= 0)
        {
            return Error.Validation("venue.seatsLimit", "Seats limit must be greater than zero");
        }

        var venueNameResult = VenueName.Create(name, prefix);
        if (venueNameResult.IsFailure)
        {
            return venueNameResult.Error;
        }

        // var venueSeats = seats.ToList();
        // if (venueSeats.Count < 1)
        // {
        //    return Error.Validation("venue.seats", "Number of seats can not be zero");
        // }
        // if (venueSeats.Count > seatsLimit)
        // {
        //    return Error.Validation("venue.seats", "Number of seats exceeds the venue's seat limit");
        // }
        return new Venue(venueId ?? new VenueId(Guid.NewGuid()), venueNameResult.Value, seatsLimit);
    }

    public Venue(VenueId id, VenueName name, int seatsLimit)
    {
        Id = id;
        Name = name;
        SeatsLimit = seatsLimit;
    }

    // EF Core ctor
    private Venue()
    {
    }

    public VenueId Id { get; private set; } = null!;

    public VenueName Name { get; set; } = null!;

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

    public void AddSeats(IEnumerable<Seat> seats)
    {
        _seats.AddRange(seats);
    }

    public UnitResult<Error> UpdateSeats(IEnumerable<Seat> seats)
    {
        List<Seat> seatsList = seats.ToList();
        if (seatsList.Count > this.SeatsLimit)
        {
            Error.Failure("venue.sits.limit", "Too many seats");
        }

        _seats = seatsList;
        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> UpdateName(string name)
    {
        var newVenueName = VenueName.Create(name, this.Name.Prefix);
        if (newVenueName.IsFailure)
        {
            return newVenueName.Error;
        }

        Name = newVenueName.Value;
        return UnitResult.Success<Error>();
    }

    public void ExpandSeatsLimit(int newSeatsLimit)
    {
        SeatsLimit = newSeatsLimit;
    }
}