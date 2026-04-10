using MediatR;
using Wakiliy.Application.Features.AppointmentSlots.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.AppointmentSlots.Queries.GetByLawyer;

public class GetAppointmentSlotsByLawyerQuery : IRequest<Result<List<AppointmentSlotDto>>>
{
    public string LawyerId { get; set; } = string.Empty;
    public DateOnly? Date { get; set; }
}