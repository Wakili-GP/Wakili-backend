using MediatR;
using Wakiliy.Application.Features.Appointments.DTOs;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Appointments.Queries.GetApprovedAppointments;

public class GetApprovedAppointmentsQuery : IRequest<Result<List<ApprovedAppointmentDto>>>
{
    public string LawyerId { get; set; } = string.Empty;
    public CalendarViewType ViewType { get; set; }
}
