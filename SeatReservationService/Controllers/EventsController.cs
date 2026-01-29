// SeatReservationService

using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using SeatReservation.Application.EventsFolder;
using SeatReservation.Contracts.Events;
using SeatReservation.Infrastructure.Postgres.Repositories;
using EventId = SeatReservation.Domain.Events.EventId;

namespace SeatReservationService.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController : ControllerBase
{
    private readonly IEventsRepository _eventsRepository;

    public EventsController(IEventsRepository eventsRepository)
    {
        _eventsRepository = eventsRepository;
    }

    [HttpGet("/{eventId:guid}")]
    public async Task<ActionResult<GetEventDto>> GetById([FromRoute] Guid eventId, [FromServices] GetByIdHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new GetByIdRequest(eventId), ct);
        return result is null ? NotFound() : Ok(result);
    }
}