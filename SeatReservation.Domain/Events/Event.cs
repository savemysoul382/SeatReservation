// SeatReservation.Domain

using SeatReservation.Domain.Venues;

namespace SeatReservation.Domain.Events;

public record EventId(Guid Value);

public class Event
{
    // EF Core ctor
    private Event()
    {
    }

    public Event(EventId id, VenueId venueId, EventDetails details, string name, DateTime eventDate)
    {
        Id = id;
        VenueId = venueId;
        Details = details;
        Name = name;
        EventDate = eventDate;
    }

    public EventId Id { get; private set; }

    // навигационное свойство
    public EventDetails Details { get; set; } = null!;

    public VenueId VenueId { get; private set; }

    public string Name { get; private set; }

    public DateTime EventDate { get; private set; }
}