using MediatR;
using Wakiliy.Application.Common.Models;
using Wakiliy.Application.Features.Appointments.DTOs;
using Wakiliy.Domain.Enums;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Appointments.Queries.GetByLawyer;

public class GetAppointmentsByLawyerQuery : IRequest<Result<PaginatedResult<LawyerReceivedAppointmentDto>>>
{
    public string LawyerId { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public AppointmentStatus? Status { get; set; }
    public bool SortDescending { get; set; } = true;
}
