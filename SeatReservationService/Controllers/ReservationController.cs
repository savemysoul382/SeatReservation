using Microsoft.AspNetCore.Mvc;
using SeatReservation.Application.Reservations;
using SeatReservation.Contracts;

namespace SeatReservationService.Controllers
{
    [ApiController]
    [Route("api/reservations")]
    public class ReservationController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Reserve([FromBody] ReserveRequest request, [FromServices] ReserveHandler handler, CancellationToken ct)
        {
            var result = await handler.Handle(request, ct);
            return result.IsSuccess ? Ok(result.Value) : result.Error.ToResponse();
        }
    }
}
