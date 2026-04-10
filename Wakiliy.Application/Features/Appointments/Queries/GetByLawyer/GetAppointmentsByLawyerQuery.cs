using MediatR;
using Wakiliy.Application.Features.Appointments.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Appointments.Queries.GetByLawyer;

public class GetAppointmentsByLawyerQuery : IRequest<Result<List<LawyerReceivedAppointmentDto>>>
{
    public string LawyerId { get; set; } = string.Empty;
}
