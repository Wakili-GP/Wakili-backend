using MediatR;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.BookingIntents.Commands.Create;

public class CreateBookingIntentCommand : IRequest<Result<string>>
{
    public int SlotId { get; set; }
    public string ClientId { get; set; } = string.Empty;
    public string LawyerId { get; set; } = string.Empty;
}