using MediatR;
using Wakiliy.Application.Features.AppointmentSlots.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.AppointmentSlots.Commands.Create;

public class CreateAppointmentSlotCommand : IRequest<Result<AppointmentSlotDto>>
{
    public string LawyerId { get; set; } = string.Empty;
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}