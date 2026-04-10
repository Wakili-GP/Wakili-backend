using MediatR;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Appointments.Commands.Complete;

public class CompleteAppointmentCommand : IRequest<Result>
{
    public Guid AppointmentId { get; set; }
    public string LawyerId { get; set; } = string.Empty;
}
