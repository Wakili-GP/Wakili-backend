using MediatR;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.AppointmentSlots.DTOs;

public class CreateAppointmentSlotDto
{
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}