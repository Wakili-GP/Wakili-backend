using Wakiliy.Domain.Enums;

namespace Wakiliy.Application.Features.Appointments.DTOs;

public class CreateAppointmentDto
{
    public int SlotId { get; set; }
    public string LawyerId { get; set; } = string.Empty;
    public SessionType SessionType { get; set; }
}
