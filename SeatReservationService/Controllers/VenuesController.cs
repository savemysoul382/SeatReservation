// SeatReservationService

using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using SeatReservation.Application;
using Shared;

namespace SeatReservationService.Controllers;

[ApiController]
[Route("api/venues")]
public class VenuesController : ControllerBase
{
    [HttpPost]
    public async Task<Result<Guid, Error>> Create([FromServices] CreateVenueHandler createVenueHandler, [FromBody] CreateVenueRequest request, CancellationToken ct)
    {
        var result = await createVenueHandler.Handle(request: request, ct: ct);
        return result;
    }
}