// SeatReservation.Application

using SeatReservation.Contracts.Events;
using SeatReservation.Domain.Events;

namespace SeatReservation.Application.EventsFolder;

public class GetByIdHandler
{
    private readonly IEventsRepository _eventsRepository;

    public GetByIdHandler(IEventsRepository seatsRepository)
    {
        _eventsRepository = seatsRepository;
    }

    public async Task<GetEventDto?> Handle(GetByIdRequest request, CancellationToken ct)
    {
        var @event = await _eventsRepository.GetById(new EventId(request.EventId), ct);
        if (@event is null)
        {
            return null;
        }

        return new GetEventDto()
        {
            Id = @event.Id.Value,
            Capacity = @event.Details.Capacity,
            Description = @event.Details.Description,
            LastReservationUtc = @event.Details.LastReservationUtc,
            VenueId = @event.VenueId.Value,
            Name = @event.Name,
            EventDate = @event.EventDate,
            StartDate = @event.StartDate,
            EndDate = @event.EndDate,
            Type = @event.Type.ToString(),
            Status = @event.Status.ToString(),
            Info = @event.Info.ToString(),
        };
    }
}