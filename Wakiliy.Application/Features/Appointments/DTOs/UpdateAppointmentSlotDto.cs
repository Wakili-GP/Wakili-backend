using MediatR;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Appointments.DTOs;

public class UpdateAppointmentSlotDto
{
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}