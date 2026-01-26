// SeatReservationService

using Microsoft.AspNetCore.Mvc;
using SeatReservation.Application.Venues;
using SeatReservation.Contracts;
using CreateVenueRequest = SeatReservation.Application.Venues.CreateVenueRequest;

namespace SeatReservationService.Controllers;

[ApiController]
[Route("api/venues")]
public class VenuesController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromServices] CreateVenueHandler createVenueHandler, [FromBody] CreateVenueRequest request, CancellationToken ct)
    {
        var result = await createVenueHandler.Handle(request: request, ct: ct);
        return result.IsFailure ? result.Error.ToResponse() : Ok(value: result.Value);
    }

    [HttpPatch("/name")]
    public async Task<IActionResult> UpdateVenueName(
        [FromServices] UpdateVenueNameHandler updateVenueNameHandler,
        [FromBody] UpdateVenueNameRequest request,
        CancellationToken ct)
    {
        var result = await updateVenueNameHandler.Handle(request: request, ct: ct);
        return result.IsFailure ? result.Error.ToResponse() : Ok(value: result.Value);
    }

    [HttpPatch("/name/by-prefix")]
    public async Task<IActionResult> UpdateVenueNameByPrefix(
        [FromServices] UpdateVenueNameByPrefixHandler updateVenueNameByPrefixHandler,
        [FromBody] UpdateVenueNameByPrefixRequest request,
        CancellationToken ct)
    {
        var result = await updateVenueNameByPrefixHandler.Handle(request: request, ct: ct);
        return result.IsFailure ? result.Error.ToResponse() : Ok();
    }
}