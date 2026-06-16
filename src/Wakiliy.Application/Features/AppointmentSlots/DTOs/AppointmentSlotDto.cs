using Wakiliy.Domain.Entities;
using Wakiliy.Domain.Enums;

namespace Wakiliy.Application.Features.AppointmentSlots.DTOs;

public class AppointmentSlotDto
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public SessionType SessionType { get; set; }
}