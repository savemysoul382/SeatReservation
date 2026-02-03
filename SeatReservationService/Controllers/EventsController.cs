// SeatReservationService

using Microsoft.AspNetCore.Mvc;
using SeatReservation.Application.EventsFolder;
using SeatReservation.Application.EventsFolder.Queries;
using SeatReservation.Contracts.Events;
using SeatReservation.Infrastructure.Postgres.Repositories;

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
    public async Task<ActionResult<GetEventDto>> GetById([FromRoute] Guid eventId, [FromServices] GetEventByIdHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new GetByIdRequest(eventId), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("/{eventId:guid}/dapper")]
    public async Task<ActionResult<GetEventDto>> GetByIdDapper([FromRoute] Guid eventId, [FromServices] GetByIdHandlerDapper handler, CancellationToken ct)
    {
        var result = await handler.Handle(new GetByIdRequest(eventId), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<GetEventsDto>> GetById(
        [FromQuery] GetEventsRequest request,
        [FromServices] GetEventsHandler handler,
        CancellationToken ct)
    {
        var result = await handler.Handle(request, ct);
        return Ok(result);
    }

    [HttpGet("dapper")]
    public async Task<ActionResult<GetEventsDto>> GetDapper(
        [FromQuery] GetEventsRequest request,
        [FromServices] GetEventsHandlerDapper handler,
        CancellationToken ct)
    {
        var result = await handler.Handle(request, ct);
        return Ok(result);
    }
}