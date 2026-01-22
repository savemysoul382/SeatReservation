// SeatReservation.Domain

namespace SeatReservation.Domain.Events;

public class EventDetails
{
    public EventDetails(int capacity, string description)
    {
        Capacity = capacity;
        Description = description;
    }


    // EF Core ctor
    private EventDetails()
    {
    }

    // EventId будет и primary key и foreign key к Event. id заполниться автоматически в EF core из-за связи
    public EventId EventId { get; }

    public int Capacity { get; private set; }

    public string Description { get; private set; }
}