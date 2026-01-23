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

    public EventType Type { get; private set; }

    public DateTime EventDate { get; private set; }

    public IEventInfo Info { get; private set; }
}

public interface IEventInfo
{
}

public record ConcertInfo(string Performer) : IEventInfo;

public record ConferenceInfo(string Topic, string Speaker) : IEventInfo;

public record OnlineInfo(string Url) : IEventInfo;

public enum EventType
{
    /// <summary>
    /// Концерт
    /// </summary>
    CONCERT,

    /// <summary>
    /// Спортивное мероприятие
    /// </summary>
    CONFERENCE,

    /// <summary>
    /// Онлайн мероприятие
    /// </summary>
    ONLINE,
}