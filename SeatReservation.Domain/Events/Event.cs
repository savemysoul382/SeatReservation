// SeatReservation.Domain

namespace SeatReservation.Domain.Events;

public class Event
{
    public Event(Guid id, Guid venueId, EventDetails details, string name, DateTime eventDate)
    {
        Id = id;
        VenueId = venueId;
        Details = details;
        Name = name;
        EventDate = eventDate;
    }

    public Guid Id { get; private set; }

    // навигационное свойство
    public EventDetails Details { get; set; }

    public Guid VenueId { get; private set; }

    public string Name { get; private set; }

    public DateTime EventDate { get; private set; }
}