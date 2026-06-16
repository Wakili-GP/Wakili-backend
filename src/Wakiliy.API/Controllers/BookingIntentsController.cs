using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wakiliy.API.Extensions;
using Wakiliy.Application.Features.BookingIntents.Commands.Create;

namespace Wakiliy.API.Controllers;

[Route("api/booking-intents")]
[ApiController]
[Authorize]
public class BookingIntentsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateBookingIntent([FromBody] CreateBookingIntentCommand command)
    {
        command.ClientId = User.GetUserId();
        var result = await mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(new { PaymentKey = result.Value });
        }

        return BadRequest(result.Error);
    }
}