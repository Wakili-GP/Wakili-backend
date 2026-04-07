using Wakiliy.Domain.Entities;

namespace Wakiliy.Application.Features.Appointments.DTOs;

public class AppointmentSlotDto
{
    public int Id { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}