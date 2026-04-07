using MediatR;
using Wakiliy.Application.Features.Appointments.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Appointments.Queries.GetByLawyer;

public class GetAppointmentSlotsByLawyerQuery : IRequest<Result<List<AppointmentSlotDto>>>
{
    public string LawyerId { get; set; } = string.Empty;
}