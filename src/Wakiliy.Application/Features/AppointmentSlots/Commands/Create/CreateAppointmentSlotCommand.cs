using MediatR;
using Wakiliy.Application.Features.AppointmentSlots.DTOs;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.AppointmentSlots.Commands.Create;

public class CreateAppointmentSlotCommand : IRequest<Result<AppointmentSlotDto>>
{
    public string LawyerId { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public SessionType SessionType { get; set; }
}