using MediatR;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Appointments.Commands.Update;

public class UpdateAppointmentSlotCommand : IRequest<Result>
{
    public int Id { get; set; }
    public string LawyerId { get; set; } = string.Empty;
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}