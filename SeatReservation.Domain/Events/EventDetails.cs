// SeatReservation.Domain

namespace SeatReservation.Domain.Events;

public class EventDetails
{
    public EventDetails(Guid eventId, int capacity, string description)
    {
        Capacity = capacity;
        Description = description;
    }

    // EventId будет и primary key и foreign key к Event. id заполниться автоматически в EF core из-за связи
    public Guid EventId { get; } = Guid.Empty;

    public int Capacity { get; private set; }

    public string Description { get; private set; }
}