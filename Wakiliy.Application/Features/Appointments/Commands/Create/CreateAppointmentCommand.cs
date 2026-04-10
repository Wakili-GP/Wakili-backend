using MediatR;
using Wakiliy.Application.Features.Appointments.DTOs;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Appointments.Commands.Create;

public class CreateAppointmentCommand : IRequest<Result<AppointmentDto>>
{
    public string ClientId { get; set; } = string.Empty;
    public string LawyerId { get; set; } = string.Empty;
    public int SlotId { get; set; }
    public SessionType SessionType { get; set; }
}
