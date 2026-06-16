using MediatR;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Appointments.Commands.Reject;

public class RejectAppointmentCommand : IRequest<Result>
{
    public Guid AppointmentId { get; set; }
    public string LawyerId { get; set; } = string.Empty;
}
